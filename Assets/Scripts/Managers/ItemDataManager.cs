using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles and stores all data for items collectable in lootboxes
/// Links with the ItemDataProcessor to extract the ItemInfo
/// </summary>

public class ItemDataManager : Manager<ItemDataManager>
{
    //[SerializeField] public ItemDataManager

    [SerializeField] private List<ItemInfo> itemInfos = new();
    private Dictionary<string, ItemInfo> itemDictionary = new();
    private HashSet<string> allTags = new HashSet<string>();
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
        BuildItemDictionary();
    }

    private void BuildItemDictionary()
    {
        itemDictionary = new Dictionary<string, ItemInfo>();

        foreach (var item in itemInfos)
        {
            if (!itemDictionary.TryAdd(item.id, item))
            {
                Debug.LogWarning($"Duplicate item index found: {item.id}");
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