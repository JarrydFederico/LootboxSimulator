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
    public Lootbox Lootbox { get; private set; }
    private System.Action<Lootbox, LootboxListItem> onPressed;

    public void Show(Lootbox lootbox, System.Action<Lootbox, LootboxListItem> onPressed)
    {
        this.Lootbox = lootbox;
        this.onPressed = onPressed;

        lootboxImage.sprite = lootbox.Sprite;
    }

    public void Pressed()
    {
        if (!GameManager.CanInteract) return;
        onPressed?.Invoke(Lootbox, this);
    }

}
