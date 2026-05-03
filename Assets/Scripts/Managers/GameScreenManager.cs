using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
        GameManager.Instance.ProcessOpenLootbox(currentLootbox);
        openBoxDisplay.ShowOpenLootbox(currentLootbox, OpenLootboxFinished);
    }

    public void OpenLootboxFinished()
    {
        openBoxDisplay.gameObject.SetActive(false);
        GameManager.SetCanInteract(true);
    }

    #endregion
}
