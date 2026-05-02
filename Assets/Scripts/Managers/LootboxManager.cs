using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>

public class LootboxManager : Manager<LootboxManager>
{
    [SerializeField] private LootboxGenerator lootboxGenerator;

    [SerializeField] private Lootbox debugLootbox;

    public void GenerateLootbox()
    {
        Lootbox lootBox = lootboxGenerator.GenerateLootbox();
        debugLootbox = lootBox;
    }

}

[System.Serializable]
public class Lootbox
{
    public string displayName;
    public List<ItemInfo> items = new ();
    public List<string> tags = new();
    public int itemAmount => items.Count;
}