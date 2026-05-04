using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Generates a random lootbox
/// 
/// Duplicates in lootboxes is intended
/// </summary>

public class LootboxGenerator : MonoBehaviour
{
    private IReadOnlyList<ItemInfo> ItemInfos => ItemDataManager.Instance.ItemInfos;

    [SerializeField] private int itemMin = 1;
    [SerializeField] private int itemMax = 3;
    [SerializeField] private int tagMin = 1;
    [SerializeField] private int tagMax = 5;

    [Header("Rarity weights")]
    [SerializeField] private float epicChance = 0.05f;
    [SerializeField] private float rareChance = 0.15f;
    [SerializeField] private float uncommonChance = 0.3f;
    [SerializeField] private float commonChance = 0.5f;
    [SerializeField] private float limitedChance = 0.5f; //Chance of items appearing that are limited

    private List<ItemInfo> genericPool; //A list of all items that aren't in 'limited' category

    public Lootbox GenerateLootbox()
    {
        if (genericPool == null)
            GenerateGenericPool();

        if (ItemInfos == null || ItemInfos.Count == 0)
        {
            Debug.LogError("Cannot generate lootbox: no ItemInfos are available.");
            return new Lootbox { };
        }

        int itemCount = Random.Range(itemMin, itemMax + 1);

        List<string> tags = GetRandomTags();

        Sprite sprite = GetRandomSprite();

        //To do later
        string displayName = "Lootbox_" + Random.Range(0, 100000);

        int counter = Random.Range(1, 4);

        Lootbox newLootbox = new Lootbox();

        newLootbox.SetSprite(sprite);
        newLootbox.SetDisplayName(displayName);
        newLootbox.SetTags(tags);
        newLootbox.SetExpiryCounter(counter);
        newLootbox.SetItemCount(itemCount);

        return newLootbox;
    }

    public void GenerateLootboxItems(Lootbox lootbox)
    {
        List<ItemInfo> itemPool = Random.value < limitedChance ? GetItemPoolFromTags(lootbox.Tags) : genericPool;

        List<ItemInfo> itemList = new();

        while (itemList.Count < lootbox.ItemCount)
        {
            if (itemPool.Count == 0)
                break;

            Rarity rarity = GetRandomRarity();

            ItemInfo chosenItem = GetItemFromPool(itemPool, rarity);

            if (chosenItem != null)
                itemList.Add(chosenItem);
        }

        lootbox.SetItems(itemList);
    }

    private List<string> GetRandomTags()
    {
        int numberOfTags = Random.Range(tagMin, tagMax + 1);
        List<string> possibleTags = ItemDataManager.Instance.AllTags.ToList();
        List<string> chosenTags = GeneralUtils.ChooseRandomAndRemove(possibleTags, numberOfTags);
        return chosenTags;
    }

    private Rarity GetRandomRarity()
    {
        float totalWeight = epicChance + rareChance + uncommonChance + commonChance;

        if (totalWeight <= 0f)
        {
            Debug.LogError("Total rarity weight must be greater than 0");
            return Rarity.Common;
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        cumulative += epicChance;
        if (roll < cumulative) return Rarity.Epic;

        cumulative += rareChance;
        if (roll < cumulative) return Rarity.Rare;

        cumulative += uncommonChance;
        if (roll < cumulative) return Rarity.Uncommon;

        return Rarity.Common;
    }

    private Rarity DowngradeRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Epic: return Rarity.Rare;
            case Rarity.Rare: return Rarity.Uncommon;
            case Rarity.Uncommon: return Rarity.Common;
            default: return Rarity.None;
        }
    }

    private ItemInfo GetItemFromPool(List<ItemInfo> pool, Rarity rarity)
    {
        ItemInfo item = TryGetItemFromPool(pool, rarity);

        if (item != null)
            return item;

        return TryGetItemFromPool(genericPool, rarity);
    }

    private ItemInfo TryGetItemFromPool(List<ItemInfo> fullPool, Rarity rarity)
    {
        while (rarity != Rarity.None)
        {
            var adjustedPool = fullPool.Where(x => x.rarity == rarity).ToList();

            if (adjustedPool.Count > 0)
                return adjustedPool[Random.Range(0, adjustedPool.Count)];

            rarity = DowngradeRarity(rarity);
        }

        return null;
    }

    private void GenerateGenericPool()
    {
        //Generic items can be found in any lootbox, as opposed to 'limited' items that require a tag
        genericPool = ItemInfos.Where(x => !x.limited).ToList();
    }

    private List<ItemInfo> GetItemPoolFromTags(IReadOnlyList<string> tags)
    {
        HashSet<string> tagSet = new(tags);

        return ItemInfos
            .Where(x => HasAnyTag(x.tags, tagSet))
            .ToList();
    }

    private bool HasAnyTag(IEnumerable<string> itemTags, HashSet<string> selectedTags)
    {
        foreach (var tag in itemTags)
        {
            if (selectedTags.Contains(tag))
                return true;
        }

        return false;
    }

    private Sprite GetRandomSprite()
    {
        Sprite[] spriteList = LootboxManager.Instance.lootboxSprites;
        return spriteList[Random.Range(0, spriteList.Length)];
    }

    private void OnValidate()
    {
        itemMin = Mathf.Max(1, itemMin);
        itemMax = Mathf.Max(itemMin, itemMax);

        tagMin = Mathf.Max(0, tagMin);
        tagMax = Mathf.Max(tagMin, tagMax);

        limitedChance = Mathf.Clamp01(limitedChance);
    }

}
