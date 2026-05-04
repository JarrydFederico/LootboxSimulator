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
    public Lootbox currentLootbox { get; private set; }

    [Header("To Set")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image currentLootboxImage;
    [SerializeField] private LootboxDetailDisplay lootboxDetailDisplay;
    [SerializeField] private GameObject buttonOpen;
    [SerializeField] private OpenBoxDisplay openBoxDisplay;
    [SerializeField] private LootboxListSection lootboxListSection;

    public IReadOnlyList<Lootbox> CurrentLootboxes =>
        LootboxManager.Instance.CurrentLootboxes;


    public void ShowGameScreen()
    {
        if (!canvas) Debug.LogError("Canvas is not assigned in ", this);
        if (!lootboxDetailDisplay) Debug.LogError("LootboxDetailDisplay is not assigned in ", this);
        if (!openBoxDisplay) Debug.LogError("OpenBoxDisplay is not assigned in ", this);
        if (!lootboxListSection) Debug.LogError("LootboxListSection is not assigned in ", this);

        openBoxDisplay.gameObject.SetActive(false);

        lootboxListSection.Show();

        SelectLootbox(CurrentLootboxes.Count > 0 ? CurrentLootboxes[0] : null);
 
        canvas.SetActive(true);

        GameManager.SetCanInteract(true);
    }

    public void SelectLootbox(Lootbox lootbox)
    {
        currentLootbox = lootbox;
        bool isLootBox = lootbox != null;

        currentLootboxImage.gameObject.SetActive(isLootBox);

        if (isLootBox)
        {
            currentLootboxImage.sprite = lootbox.Sprite;
        }

        LootboxStateChanged();
    }


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
        lootboxListSection.OpenLootboxFinished();
        LootboxStateChanged();
        openBoxDisplay.gameObject.SetActive(false);
        GameManager.SetCanInteract(true);
    }

    #endregion

    public void LootboxStateChanged()
    {
        lootboxListSection.UpdateDisplay();
        lootboxDetailDisplay.UpdateDisplay(currentLootbox);
    }


}
