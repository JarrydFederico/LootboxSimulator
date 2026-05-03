using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The display/button that the player can select
/// to change the current active lootbox
/// </summary>

public class LootboxListItem : MonoBehaviour
{
    [SerializeField] private Image lootboxImage;
    [SerializeField] private TextMeshProUGUI stateTMP;
    public Lootbox Lootbox { get; private set; }
    private System.Action<Lootbox, LootboxListItem> onPressed;

    public void Show(Lootbox lootbox, System.Action<Lootbox, LootboxListItem> onPressed)
    {
        this.Lootbox = lootbox;
        this.onPressed = onPressed;
        UpdateDisplay();
        ShowStateText("NEW");
    }

    public void UpdateDisplay()
    {
        lootboxImage.sprite = Lootbox.Sprite;
        if (Lootbox.ExpiryCounter <= 1)
            ShowStateText("EXPIRING");
        else
            ShowStateText("");
    }

    private void ShowStateText(string newText)
    {
        stateTMP.text = newText;
    }

    public void Pressed()
    {
        if (!GameManager.CanInteract) return;
        onPressed?.Invoke(Lootbox, this);
    }

}
