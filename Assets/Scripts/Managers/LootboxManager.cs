using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the creation of lootboxes in conjuction
/// with the LootboxGenerator
/// 
/// At the start of the run, the player has lootboxes
/// spawned. When they expire, they are re-generated here
/// </summary>

public class LootboxManager : Manager<LootboxManager>
{
    [Header("Settings")]
    [SerializeField] private int startLootboxCount;

    public Sprite[] lootboxSprites; //List of possible sprites, this system to be replaced

    [SerializeField] private LootboxGenerator lootboxGenerator;

    public IReadOnlyList<Lootbox> CurrentLootboxes =>
        ProgressManager.Instance.LootboxProgress.CurrentLootboxes;


    public override void OnStartRun()
    {
        GenerateStartLootboxes();
    }

    public void GenerateStartLootboxes()
    {
        while(CurrentLootboxes.Count < startLootboxCount)
        {
            AddLootbox(GenerateLootbox());
        }
    }

    public void AddLootbox(Lootbox lootbox)
    {
        ProgressManager.Instance.LootboxProgress.AddLootbox(lootbox);
    }

    public void RemoveLootbox(Lootbox lootbox)
    {
        ProgressManager.Instance.LootboxProgress.RemoveLootbox(lootbox);
    }

    public Lootbox GenerateLootbox()
    {
        Lootbox lootbox = lootboxGenerator.GenerateLootbox();
        return lootbox;
    }

}

public class Lootbox
{
    public string DisplayName { get; private set; }
    public Sprite Sprite { get; private set; }

    private readonly List<ItemInfo> items = new();
    public IReadOnlyList<ItemInfo> Items => items;

    private readonly List<string> tags = new();
    public IReadOnlyList<string> Tags => tags;

    public int ItemAmount => items.Count;

    public void SetItems(List<ItemInfo> newItems)
    {
        items.Clear();

        if (newItems == null)
            return;

        items.AddRange(newItems);
    }

    public void AddItem(ItemInfo newItem)
    {
        if (newItem == null)
            return;

        items.Add(newItem);
    }

    public void SetTags(List<string> newTags)
    {
        tags.Clear();

        if (newTags == null)
            return;

        tags.AddRange(newTags);
    }

    public void SetSprite(Sprite sprite)
    {
        this.Sprite = sprite;
    }

    public void SetDisplayName(string displayName)
    {
        this.DisplayName = displayName;
    }
}