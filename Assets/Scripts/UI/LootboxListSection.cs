using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Displays the current selectable lootboxes
/// 
/// This handles the spawning, positioning
/// and animations of LootboxListItems
/// </summary>

public class LootboxListSection : MonoBehaviour
{

    [Header("To set")]
    [SerializeField] private Transform itemHolder;
    [SerializeField] private LootboxListItem itemPrefab;
    [SerializeField] private Transform positionHolder;
    [SerializeField] private Transform spawnPosition;

    private List<Transform> positions;
    private IReadOnlyList<Lootbox> currentLootboxes => gameScreenManager.CurrentLootboxes;
    private List<LootboxListItem> currentItems = new();
    private Lootbox currentLootbox => gameScreenManager.currentLootbox;
    private GameScreenManager gameScreenManager => GameScreenManager.Instance;

    public void Show()
    {
        if (!itemHolder) Debug.LogError("itemHolder is not assigned in ", this);
        if (!itemPrefab) Debug.LogError("itemPrefab is not assigned in ", this);
        if (!positionHolder) Debug.LogError("positionHolder is not assigned in ", this);
        if (!spawnPosition) Debug.LogError("spawnPosition is not assigned in ", this);
        if (positions == null)
            SetupPositions();

        LoadLootboxListItems();
    }

    public void UpdateDisplay()
    {
        foreach (var lootboxListItem in currentItems)
            lootboxListItem.UpdateDisplay(gameScreenManager.currentLootbox == lootboxListItem.Lootbox);
    }

    private void LoadLootboxListItems()
    {
        ClearLootboxListItems();

        int spawnOrder = 0;
        foreach (var lb in currentLootboxes)
        {
            SpawnItem(lb, spawnOrder);
            spawnOrder++;
        }

        ItemsWereChanged();
    }

    private void SpawnItem(Lootbox lootbox, int spawnOrder)
    {
        //Items are spawned at the spawn point with an offset based on
        //the order they are spawned

        var spawnPos = spawnPosition.position;
        float spawnXOffset = 10f;
        spawnPos.x += spawnOrder * spawnXOffset;

        var newLootboxListItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity, itemHolder);
        
        newLootboxListItem.transform.SetAsFirstSibling();

        newLootboxListItem.Spawn(lootbox, gameScreenManager.PressedLootboxListItem, spawnOrder);
        currentItems.Add(newLootboxListItem);
    }

    public void RemoveItem(LootboxListItem item)
    {
        currentItems.Remove(item);
        item.OnRemove();
    }

    private void ClearLootboxListItems()
    {
        foreach (Transform t in itemHolder)
            Destroy(t.gameObject);
        currentItems = new();
    }

    public void OpenLootboxFinished()
    {
        List<LootboxListItem> lootboxListItemsToRemove =
            currentItems.Where(
                x => x.Lootbox != null
                && x.Lootbox.CheckIsExpired())
            .ToList();

        foreach (var lootboxListItem in lootboxListItemsToRemove)
        {
            if (currentLootbox == lootboxListItem.Lootbox)
                gameScreenManager.SelectLootbox(null);
            RemoveItem(lootboxListItem);
        }

        int spawnOrder = 0;
        foreach (var lootbox in LootboxManager.Instance.CurrentLootboxes)
        {
            if (GetLootboxListItem(lootbox) == null)
            {
                SpawnItem(lootbox, spawnOrder);
                spawnOrder++;
            }
        }

        ItemsWereChanged();
    }

    public LootboxListItem GetLootboxListItem(Lootbox lootbox)
    {
        return currentItems.FirstOrDefault(x => x.Lootbox == lootbox);
    }

    private void SetupPositions()
    {
        positions = GeneralUtils.GetComponentsFromChildren<Transform>(positionHolder);
    }

    private void ItemsWereChanged()
    {
        for(int i = 0; i < currentItems.Count; i++)
        {
            bool showLootbox = i < positions.Count;
            currentItems[i].gameObject.SetActive(showLootbox);

            if (!showLootbox)
                continue;

            currentItems[i].SetTargetPosition(positions[i]);
        }
    }
}
