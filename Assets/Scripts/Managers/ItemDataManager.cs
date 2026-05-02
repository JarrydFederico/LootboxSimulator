using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles and stores all data for items collectable in lootboxes
/// Links with the ItemDataProcessor to extract the ItemInfo
/// </summary>

public class ItemDataManager : Manager<ItemDataManager>
{
    [SerializeField] private List<ItemInfo> itemInfos = new();
    private Dictionary<string, ItemInfo> itemDictionary = new();
    private HashSet<string> allTags = new HashSet<string>();
    public IReadOnlyList<ItemInfo> ItemInfos => itemInfos;
    public IReadOnlyCollection<string> AllTags => allTags;

    [SerializeField] private ItemDataProcessor itemDataProcessor;

    public override void Setup()
    {
        base.Setup();

        if (itemDataProcessor == null)
        {
            Debug.LogError("ItemDataProcessor missing on ItemDataManager");
            return;
        }
        ProcessItemInfos();
    }

    public void ProcessItemInfos()
    {
        var result = itemDataProcessor.ProcessItemInfo();
        itemInfos = result.itemInfos;
        allTags = result.tags;
        BuildItemDictionaries();
    }

    private void BuildItemDictionaries()
    {
        itemDictionary = new Dictionary<string, ItemInfo>();

        foreach (var item in itemInfos)
        {
            if (!itemDictionary.TryAdd(item.id, item))
            {
                Debug.LogWarning($"Duplicate item index found: {item.id}");
                continue;
            }
        }
    }


    public bool GetItemInfo(string index, out ItemInfo item)
    {
        return itemDictionary.TryGetValue(index, out item);
    }

}

[System.Serializable]
public class ItemInfo
{
    public string id;
    public string displayName;
    public Rarity rarity;
    public Sprite sprite;
    public bool limited;
    public string[] tags;
}

public enum Rarity
{
    None,
    Common,
    Uncommon,
    Rare,
    Epic
}