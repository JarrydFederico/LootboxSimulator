using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This page is displayed when a lootbox is opened
/// and shows the items received inside
/// 
/// When the screen is pressed, it will show the next
/// item if there is one. It will close if there isn't.
/// </summary>

public class OpenBoxDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rarityTMP;
    [SerializeField] private Image itemImage;

    private List<ItemInfo> itemsToOpen = new();
    private System.Action finishAction;

    private Coroutine currentCoroutine;

    private ItemInfo currentItem;
    private ItemInfo NextItem =>
        itemsToOpen != null && itemsToOpen.Count > 0 
        ? itemsToOpen[0] : null;

    public void ShowOpenLootbox(Lootbox lootbox, System.Action finishAction)
    {
        this.finishAction = finishAction;

        if (lootbox == null)
        {
            Finish();
            return;
        }

        itemsToOpen.Clear();
        itemsToOpen.AddRange(lootbox.Items);

        gameObject.SetActive(true);

        StartOpenSequence();
    }

    private void StartOpenSequence()
    {
        if (NextItem == null)
        {
            Finish();
            return;
        }

        currentItem = NextItem;
        itemsToOpen.RemoveAt(0);

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(OpenItemSequence());
    }

    private IEnumerator OpenItemSequence()
    {
        GameManager.SetCanInteract(false);

        itemImage.sprite = currentItem.sprite;
        rarityTMP.text = currentItem.rarity.ToString();

        yield return new WaitForSeconds(0.1f);

        GameManager.SetCanInteract(true);
        currentCoroutine = null;
    }

    private void Finish()
    {
        finishAction?.Invoke();
    }

    public void Pressed()
    {
        if (!GameManager.CanInteract) return;

        if (NextItem != null)
        {
            StartOpenSequence();
        }
        else
        {
            Finish();
        }
    }
}
