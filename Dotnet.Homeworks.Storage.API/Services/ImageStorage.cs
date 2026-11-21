using Dotnet.Homeworks.Shared.Dto;
using Dotnet.Homeworks.Storage.API.Constants;
using Dotnet.Homeworks.Storage.API.Dto.Internal;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Dotnet.Homeworks.Storage.API.Services;

public class ImageStorage : IStorage<Image>
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public ImageStorage(IMinioClient minioClient, string bucketName)
    {
        _minioClient = minioClient;
        _bucketName = bucketName;
    }

    public async Task<Result> PutItemAsync(Image item, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await ExistsAsync(Buckets.Pending, item.FileName, cancellationToken) ||
                await ExistsAsync(_bucketName, item.FileName, cancellationToken))
                return new Result(false, $"Item '{item.FileName}' already exists");

            var metadata = new Dictionary<string, string>(item.Metadata);
            // объект всегда попадает сначала в Pending, а куда его переложить - хранится в метаданных
            if (_bucketName != Buckets.Pending)
                metadata[MetadataKeys.Destination] = _bucketName;

            if (item.Content.CanSeek)
                item.Content.Position = 0;

            var args = new PutObjectArgs()
                .WithBucket(Buckets.Pending)
                .WithObject(item.FileName)
                .WithStreamData(item.Content)
                .WithObjectSize(item.Content.Length)
                .WithContentType(item.ContentType)
                .WithHeaders(metadata);

            await _minioClient.PutObjectAsync(args, cancellationToken);
            return new Result(true);
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            return new Result(false, e.Message);
        }
    }

    public async Task<Image?> GetItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        try
        {
            var stat = await _minioClient.StatObjectAsync(
                new StatObjectArgs().WithBucket(_bucketName).WithObject(itemName), cancellationToken);

            var content = new MemoryStream();
            await _minioClient.GetObjectAsync(new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(itemName)
                .WithCallbackStream(stream => stream.CopyTo(content)), cancellationToken);
            content.Position = 0;

            var metadata = stat.MetaData is null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(stat.MetaData);

            return new Image(content, itemName, stat.ContentType, metadata);
        }
        catch (ObjectNotFoundException)
        {
            return null;
        }
    }

    public async Task<Result> RemoveItemAsync(string itemName, CancellationToken cancellationToken = default)
    {
        try
        {
            await _minioClient.RemoveObjectAsync(
                new RemoveObjectArgs().WithBucket(_bucketName).WithObject(itemName), cancellationToken);
            return new Result(true);
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            return new Result(false, e.Message);
        }
    }

    public async Task<IEnumerable<string>> EnumerateItemNamesAsync(CancellationToken cancellationToken = default)
    {
        var names = new List<string>();
        var args = new ListObjectsArgs().WithBucket(_bucketName).WithRecursive(true);

        await foreach (var item in _minioClient.ListObjectsEnumAsync(args, cancellationToken))
            names.Add(item.Key);

        return names;
    }

    public async Task<Result> CopyItemToBucketAsync(string itemName, string destinationBucketName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketExists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(destinationBucketName), cancellationToken);
            if (!bucketExists)
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(destinationBucketName), cancellationToken);

            var source = new CopySourceObjectArgs().WithBucket(_bucketName).WithObject(itemName);
            var args = new CopyObjectArgs()
                .WithBucket(destinationBucketName)
                .WithObject(itemName)
                .WithCopyObjectSource(source);

            await _minioClient.CopyObjectAsync(args, cancellationToken);
            return new Result(true);
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            return new Result(false, e.Message);
        }
    }

    private async Task<bool> ExistsAsync(string bucket, string objectName, CancellationToken cancellationToken)
    {
        try
        {
            await _minioClient.StatObjectAsync(
                new StatObjectArgs().WithBucket(bucket).WithObject(objectName), cancellationToken);
            return true;
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
        catch (BucketNotFoundException)
        {
            return false;
        }
    }
}
