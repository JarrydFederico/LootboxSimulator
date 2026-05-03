using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Handles all displays in the main game screen
/// and manages player interactions with it
/// </summary>

public class GameScreenManager : Manager<GameScreenManager>
{
    [SerializeField] private Lootbox currentLootbox;

    [Header("To Set")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image currentLootboxImage;
    [SerializeField] private LootboxDetailDisplay lootboxDetailDisplay;
    [SerializeField] private GameObject buttonOpen;
    [SerializeField] private Transform lootboxListItemHolder;
    [SerializeField] private LootboxListItem lootboxListItemPrefab;
    [SerializeField] private OpenBoxDisplay openBoxDisplay;

    public IReadOnlyList<Lootbox> CurrentLootboxes =>
        LootboxManager.Instance.CurrentLootboxes;

    private List<LootboxListItem> currentLootboxListItems = new();

    public void ShowGameScreen()
    {
        if (!canvas) Debug.LogError("Canvas is not assigned in ", this);
        if (!lootboxListItemHolder) Debug.LogError("LootboxListItemHolder is not assigned in ", this);
        if (!lootboxListItemPrefab) Debug.LogError("LootboxListItemPrefab is not assigned in ", this);
        if (!lootboxDetailDisplay) Debug.LogError("LootboxDetailDisplay is not assigned in ", this);
        if (!openBoxDisplay) Debug.LogError("OpenBoxDisplay is not assigned in ", this);

        openBoxDisplay.gameObject.SetActive(false);

        LoadLootboxListItems();

        SelectLootbox(CurrentLootboxes.Count > 0 ? CurrentLootboxes[0] : null);
 
        canvas.SetActive(true);

        GameManager.SetCanInteract(true);
    }

    public void SelectLootbox(Lootbox lootbox)
    {
        currentLootbox = lootbox;
        bool isLootBox = lootbox != null;

        lootboxDetailDisplay.UpdateDisplay(currentLootbox);
        currentLootboxImage.gameObject.SetActive(isLootBox);

        if (isLootBox)
        {
            currentLootboxImage.sprite = lootbox.Sprite;
        }
    }

    #region LootboxList
    private void LoadLootboxListItems()
    {
        ClearLootboxListItems();

        foreach(var lb in CurrentLootboxes)
        {
            SpawnLootboxListItem(lb);
        }
    }

    private void SpawnLootboxListItem(Lootbox lootbox)
    {
        var newLootboxListItem = Instantiate(lootboxListItemPrefab, lootboxListItemHolder);
        newLootboxListItem.Show(lootbox, PressedLootboxListItem);
        currentLootboxListItems.Add(newLootboxListItem);
    }

    private void ClearLootboxListItems()
    {
        foreach (Transform t in lootboxListItemHolder)
            Destroy(t.gameObject);
        currentLootboxListItems = new();
    }
    #endregion

    #region Interaction
    public void PressedLootboxListItem(Lootbox lootbox, LootboxListItem pressedItem)
    {
        SelectLootbox(lootbox);
    }

    public void PressedOpenButton()
    {
        if (!GameManager.CanInteract) return;
        if (currentLootbox == null) return;
        LootboxManager.Instance.ProcessOpenLootbox(currentLootbox);
        GameManager.Instance.LootBoxWasOpened(currentLootbox);
        openBoxDisplay.ShowOpenLootbox(currentLootbox, OpenLootboxFinished);
    }

    public void OpenLootboxFinished()
    {
        List<LootboxListItem> lootboxListItemsToRemove =
            currentLootboxListItems.Where(x => x.Lootbox.CheckIsExpired())
            .ToList();

        foreach(var lootboxListItem in lootboxListItemsToRemove)
        {
            if (currentLootbox == lootboxListItem.Lootbox)
                SelectLootbox(null);
            currentLootboxListItems.Remove(lootboxListItem);
            Destroy(lootboxListItem.gameObject);
        }

        foreach(var lootbox in LootboxManager.Instance.CurrentLootboxes)
        {
            if (GetLootboxListItem(lootbox) == null)
                SpawnLootboxListItem(lootbox);
        }

        LootboxStateChanged();
        openBoxDisplay.gameObject.SetActive(false);
        GameManager.SetCanInteract(true);
    }

    #endregion

    public void LootboxStateChanged()
    {
        foreach (var lootboxListItem in currentLootboxListItems)
            lootboxListItem.UpdateDisplay();
        lootboxDetailDisplay.UpdateDisplay(currentLootbox);
    }

    public LootboxListItem GetLootboxListItem(Lootbox lootbox)
    {
        return currentLootboxListItems.FirstOrDefault
            (x => x.Lootbox == lootbox);
    }
}
