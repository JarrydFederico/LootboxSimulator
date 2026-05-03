using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This manages everything related to player progress
/// e.g., this they have unlocked, active lootboxes etc
/// 
/// </summary>

public class ProgressManager : Manager<ProgressManager>
{
    public LootboxProgress LootboxProgress { get; private set; }
    public ItemProgress ItemProgress { get; private set; }

    public override void OnLootboxOpened(Lootbox lootbox)
    {
        foreach (var item in lootbox.Items)
            ItemProgress.AddItem(item, 1);

        LootboxProgress.AddToHistory(lootbox);

        List<Lootbox> expiredLootboxes = new();
        foreach(var currentLootbox in LootboxProgress.CurrentLootboxes)
        {
            currentLootbox.ReduceCounter();
            if (currentLootbox.CheckIsExpired())
                expiredLootboxes.Add(currentLootbox);
        }

        int replacementCount = expiredLootboxes.Count;

        foreach (var expiredLootbox in expiredLootboxes)
            LootboxProgress.RemoveLootbox(expiredLootbox);

        //Note: I should consider moving this to LootboxManager
        while(replacementCount > 0)
        {
            Lootbox newLootBox = LootboxManager.Instance.GenerateLootbox();
            LootboxProgress.AddLootbox(newLootBox);
            replacementCount--;
        }
    }

    public override void Setup()
    {
        base.Setup();

        //Initialise the player's progress
        //To be replaced by an improved system later
        LootboxProgress = new LootboxProgress();
        ItemProgress = new ItemProgress();
    }
}

public class LootboxProgress
{
    private readonly List<Lootbox> currentLootboxes = new();
    private readonly List<Lootbox> openedLootboxes = new();
    public IReadOnlyList<Lootbox> CurrentLootboxes => currentLootboxes;
    public IReadOnlyList<Lootbox> OpenedLootboxes => openedLootboxes;

    public void AddLootbox(Lootbox lootbox)
    {
        if (lootbox == null) return;
        currentLootboxes.Add(lootbox);
    }

    public void RemoveLootbox(Lootbox lootbox)
    {
        currentLootboxes.Remove(lootbox);
    }

    public void AddToHistory(Lootbox lootbox)
    {
        openedLootboxes.Add(lootbox);
    }
}

public class ItemProgress
{
    private readonly Dictionary<string, int> ownedItemCounts = new();
    public IReadOnlyDictionary<string, int> OwnedItemCounts => ownedItemCounts;

    public void AddItem(ItemInfo item, int amount)
    {
        if (item == null)
            return;

        string itemId = item.id;

        if (!ownedItemCounts.TryAdd(itemId, amount))
            ownedItemCounts[itemId] += amount;
    }

    public int GetItemCount(ItemInfo item)
    {
        if (item == null)
            return 0;

        string itemId = item.id;

        return ownedItemCounts.TryGetValue(itemId, out int count)
            ? count : 0;
    }

    public bool OwnsItem(ItemInfo itemInfo)
    {
        return GetItemCount(itemInfo) > 0;
    }
}
