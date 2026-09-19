using System.Collections.Concurrent;
using System.Text.Json;
using System.Xml.Linq;

namespace ObjectOrientedTest;

public enum RepositoryItemType
{
    Json = 1,
    Xml = 2
}

public sealed record RepositoryItem(string Content, RepositoryItemType Type);

public interface IRepositoryStorage
{
    bool TryAdd(string itemName, RepositoryItem item);

    bool TryGet(string itemName, out RepositoryItem item);

    bool TryRemove(string itemName);
}

public interface IItemContentValidator
{
    RepositoryItemType ItemType { get; }

    bool IsValid(string itemContent);
}

public sealed class InMemoryRepositoryStorage : IRepositoryStorage
{
    private readonly ConcurrentDictionary<string, RepositoryItem> _items = new(StringComparer.Ordinal);

    public bool TryAdd(string itemName, RepositoryItem item) => _items.TryAdd(itemName, item);

    public bool TryGet(string itemName, out RepositoryItem item)
    {
        if (_items.TryGetValue(itemName, out var storedItem))
        {
            item = storedItem;
            return true;
        }

        item = null!;
        return false;
    }

    public bool TryRemove(string itemName) => _items.TryRemove(itemName, out _);
}

public sealed class RepositoryManager
{
    private readonly IRepositoryStorage _storage;
    private readonly IReadOnlyDictionary<RepositoryItemType, IItemContentValidator> _validators;

    public RepositoryManager()
        : this(
            new InMemoryRepositoryStorage(),
            new IItemContentValidator[]
            {
                new JsonContentValidator(),
                new XmlContentValidator()
            })
    {
    }

    public RepositoryManager(
        IRepositoryStorage storage,
        IEnumerable<IItemContentValidator> validators)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        ArgumentNullException.ThrowIfNull(validators);

        var validatorList = validators.ToArray();
        if (validatorList.Any(validator => validator is null))
        {
            throw new ArgumentException("Validators cannot contain null values.", nameof(validators));
        }

        _validators = validatorList.ToDictionary(validator => validator.ItemType);
    }

    // The repository is ready after construction; this method remains for the required API.
    public void Initialize()
    {
    }

    public void Register(string itemName, string itemContent, int itemType)
    {
        ValidateItemName(itemName);
        ArgumentNullException.ThrowIfNull(itemContent);

        if (!Enum.IsDefined((RepositoryItemType)itemType)
            || !_validators.TryGetValue((RepositoryItemType)itemType, out var validator))
        {
            throw new ArgumentOutOfRangeException(nameof(itemType), itemType, "The item type is not supported.");
        }

        if (!validator.IsValid(itemContent))
        {
            throw new ArgumentException("The item content is invalid for the supplied item type.", nameof(itemContent));
        }

        var item = new RepositoryItem(itemContent, (RepositoryItemType)itemType);
        if (!_storage.TryAdd(itemName, item))
        {
            throw new InvalidOperationException($"An item named '{itemName}' is already registered.");
        }
    }

    public string Retrieve(string itemName)
    {
        ValidateItemName(itemName);
        return TryGetItem(itemName).Content;
    }

    public int GetType(string itemName)
    {
        ValidateItemName(itemName);
        return (int)TryGetItem(itemName).Type;
    }

    public void Deregister(string itemName)
    {
        ValidateItemName(itemName);
        if (!_storage.TryRemove(itemName))
        {
            throw new KeyNotFoundException($"No item named '{itemName}' is registered.");
        }
    }

    private RepositoryItem TryGetItem(string itemName)
    {
        if (_storage.TryGet(itemName, out var item))
        {
            return item;
        }

        throw new KeyNotFoundException($"No item named '{itemName}' is registered.");
    }

    private static void ValidateItemName(string itemName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemName);
    }
}

internal sealed class JsonContentValidator : IItemContentValidator
{
    public RepositoryItemType ItemType => RepositoryItemType.Json;

    public bool IsValid(string itemContent)
    {
        try
        {
            using var document = JsonDocument.Parse(itemContent);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}

internal sealed class XmlContentValidator : IItemContentValidator
{
    public RepositoryItemType ItemType => RepositoryItemType.Xml;

    public bool IsValid(string itemContent)
    {
        try
        {
            XDocument.Parse(itemContent);
            return true;
        }
        catch (System.Xml.XmlException)
        {
            return false;
        }
    }
}
