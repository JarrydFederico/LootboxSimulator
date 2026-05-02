using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Extracts item data from the ItemInfo ItemDirectory CSV
/// to be stored on the ItemDataManager
/// 
/// There are few weaknesses of this system:
/// Typos in tags are not accounted for and would be registered twice
/// Duplicate IDs are not accounted for
/// </summary>


public class ItemDataProcessor : MonoBehaviour
{
    [SerializeField] private TextAsset itemDirectory; //A CSV with all info about items

    [SerializeField] private int idColumn = 0;
    [SerializeField] private int displayNameColumn = 1;
    [SerializeField] private int rarityColumn = 2;
    [SerializeField] private int tagsColumnStart = 5;

    public (List<ItemInfo> itemInfos, HashSet<string> tags) ProcessItemInfo()
    {
        if (itemDirectory == null)
        {
            Debug.LogError("CSV file not assigned on ItemDataProcessor");
            return (new List<ItemInfo>(), new HashSet<string>());
        }

        HashSet<string> extractedTags = new HashSet<string>();

        List<ItemInfo> itemInfoList = new List<ItemInfo>();
        string[] lines = itemDirectory.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) //i starts at 1 to skip header
        {
            string line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] values = line.Split(','); //Note: quoted commas in cells are not supported

            if (!HasColumn(values, idColumn) || string.IsNullOrWhiteSpace(values[idColumn]))
                continue;

            Rarity rarity = Rarity.Common;

            if (HasColumn(values, rarityColumn))
            {
                string rarityText = values[rarityColumn].Trim();

                if (!System.Enum.TryParse(rarityText, true, out rarity))
                {
                    Debug.LogWarning($"Could not parse rarity value: '{rarityText}', defaulting to Common.");
                    rarity = Rarity.Common;
                }
            }

            string[] tags = ExtractTags(values, tagsColumnStart);

            foreach (var tag in tags)
            {
                extractedTags.Add(tag);
            }

            ItemInfo itemInfo = new ItemInfo
            {
                id = values[idColumn].Trim(),
                displayName = HasColumn(values, displayNameColumn) ? values[displayNameColumn].Trim() : "",
                rarity = rarity,
                tags = tags,
            };

            itemInfoList.Add(itemInfo);
            
        }

        return (itemInfoList, extractedTags);
    }

    private bool HasColumn(string[] values, int columnIndex)
    {
        return columnIndex >= 0 && columnIndex < values.Length;
    }

    private string[] ExtractTags(string[] values, int startIndex)
    {
        List<string> tags = new List<string>();

        for (int i = startIndex; i < values.Length; i++)
        {
            string tag = values[i].Trim();

            if (!string.IsNullOrWhiteSpace(tag))
            {
                tags.Add(tag);
            }
        }

        return tags.ToArray();
    }

}
