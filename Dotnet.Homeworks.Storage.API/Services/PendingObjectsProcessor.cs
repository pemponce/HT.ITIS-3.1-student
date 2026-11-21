using Dotnet.Homeworks.Storage.API.Constants;

namespace Dotnet.Homeworks.Storage.API.Services;

public class PendingObjectsProcessor : BackgroundService
{
    private readonly IStorageFactory _storageFactory;
    private readonly ILogger<PendingObjectsProcessor>? _logger;

    public PendingObjectsProcessor(IStorageFactory storageFactory, ILogger<PendingObjectsProcessor>? logger = null)
    {
        _storageFactory = storageFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(PendingObjectProcessor.Period);

        try
        {
            do
            {
                try
                {
                    await ProcessPendingObjectsAsync(stoppingToken);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    _logger?.LogError(e, "Failed to process pending objects");
                }
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task ProcessPendingObjectsAsync(CancellationToken cancellationToken)
    {
        var pendingStorage = await _storageFactory.CreateImageStorageWithinBucketAsync(Buckets.Pending);
        var names = (await pendingStorage.EnumerateItemNamesAsync(cancellationToken)).ToList();

        foreach (var name in names)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var item = await pendingStorage.GetItemAsync(name, cancellationToken);
            if (item is null) continue;

            var destination = item.Metadata
                .FirstOrDefault(kv => string.Equals(kv.Key, MetadataKeys.Destination,
                    StringComparison.OrdinalIgnoreCase)).Value;
            await item.Content.DisposeAsync();

            // Pending должен очищаться полностью. Если переносить некуда
            // (нет назначения или назначение - сам Pending), объект просто удаляется
            if (!string.IsNullOrWhiteSpace(destination) && destination != Buckets.Pending)
            {
                var copyResult = await pendingStorage.CopyItemToBucketAsync(name, destination, cancellationToken);
                if (copyResult.IsFailure)
                {
                    _logger?.LogError("Failed to move '{Name}' to '{Destination}': {Error}",
                        name, destination, copyResult.Error);
                    continue;
                }
            }

            await pendingStorage.RemoveItemAsync(name, cancellationToken);
        }
    }
}
