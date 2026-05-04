using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// The display/button that the player can select
/// to change the current active lootbox
/// </summary>

public class LootboxListItem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveLerpSpeed = 5.0f;
    [SerializeField] private float snapThreshold = 0.1f;
    [SerializeField] private float delayPause = 0.2f;
    [SerializeField] private float delayStep = 0.2f;
    private float delayTimer;

    [Header("To set")]
    [SerializeField] private Image lootboxImage;
    [SerializeField] private TextMeshProUGUI stateTMP;
    [SerializeField] private TweenEffect tweenEffect;
    [SerializeField] private Image buttonImage;

    [Header("Sprites")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite selectedSprite;

    public Lootbox Lootbox { get; private set; }
    private Transform target;
    private bool isMoving;
    private System.Action<Lootbox, LootboxListItem> onPressed;

    public void Spawn(Lootbox lootbox, System.Action<Lootbox, LootboxListItem> onPressed, int spawnOrder)
    {
        this.Lootbox = lootbox;
        this.onPressed = onPressed;

        delayTimer = delayPause + spawnOrder * delayStep;

        tweenEffect.PlayTween(TweenType.ItemEntry);

        UpdateDisplay();
    }

    public void SetTargetPosition(Transform target)
    {
        this.target = target;
        isMoving = target != null;
    }

    private void Update()
    {
        if (!isMoving) return;

        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;

            if (delayTimer <= 0f)
                gameObject.SetActive(true);

            return;
        }

        isMoving = MoveTowardsTarget();
    }

    private bool MoveTowardsTarget()
    {
        if (target == null) return false;

        Vector2 current = transform.position;
        Vector2 targetPos = target.position;
        if (Vector2.Distance(current, targetPos) <= snapThreshold)
        {
            transform.position = targetPos;
            return false;
        }

        transform.position = Vector2.Lerp(
            current,
            targetPos,
            moveLerpSpeed * Time.deltaTime
        );
        return true;
    }

    public void UpdateDisplay(bool selected = false)
    {
        if (Lootbox == null)
        {
            gameObject.SetActive(false);
            return;
        }

        buttonImage.sprite = selected ? selectedSprite : defaultSprite;

        lootboxImage.sprite = Lootbox.Sprite;

        if (Lootbox.ExpiryCounter <= 1)
            ShowStateText("EXPIRING");
        else
            ShowStateText("");

    }

    public void OnRemove()
    {
        tweenEffect.PlayTween(TweenType.ItemClose);
        StartCoroutine(DestroyAfterDelay(1.5f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
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
