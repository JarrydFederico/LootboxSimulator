using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

/// <summary>
/// The UI that displays the details of a selected lootbox
/// such as rarity rates, amount to open, etc
/// </summary>

public class LootboxDetailDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI detailTMP;

    private readonly StringBuilder builder = new();

    public void UpdateDisplay(Lootbox lootbox)
    {
        if (lootbox == null)
        {
            detailTMP.text = "-";
            return;
        }

        builder.Clear();
        builder.AppendLine($"{lootbox.DisplayName}");
        builder.AppendLine();
        builder.AppendLine($"{lootbox.ItemCount} ITEM");
        builder.AppendLine();
        builder.AppendLine($"{lootbox.ExpiryCounter} UNTIL EXPIRY");
        builder.AppendLine();
        
        builder.AppendLine("Epic/Rare/Unc/Com");
        builder.AppendLine("5%/15%/30%/50%");
        builder.AppendLine();

        foreach (string tag in lootbox.Tags)
            builder.AppendLine(tag);

        detailTMP.text = builder.ToString();
    }

}
