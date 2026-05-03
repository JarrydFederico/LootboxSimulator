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

    public override void Setup()
    {
        base.Setup();

        //Initialise the player's progress
        //To be replaced by an improved system later
        LootboxProgress = new LootboxProgress();
    }
}

public class LootboxProgress
{
    private readonly List<Lootbox> currentLootboxes = new();
    public IReadOnlyList<Lootbox> CurrentLootboxes => currentLootboxes;

    public void AddLootbox(Lootbox lootbox)
    {
        if (lootbox == null) return;
        currentLootboxes.Add(lootbox);
    }

    public void RemoveLootbox(Lootbox lootbox)
    {
        currentLootboxes.Remove(lootbox);
    }
}

public class ItemProgress
{

}
