using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenEffect : MonoBehaviour
{
    private Transform _transform;
    private CanvasGroup _canvasGroup;

    [HideInInspector] public float targetSize = 1f;
    private TweenType loadedTween;
    private bool destroyAtEnd = false;
    private System.Action onCompleteCallback;

    private EffectManager effectManager => EffectManager.Instance;

    public void PlayTween(TweenType tweenToPlay, float delay = 0, bool _destroyAtEnd = false, System.Action onComplete = null)
    {
        gameObject.SetActive(true);
        if (!gameObject.activeInHierarchy) return;

        _transform = transform;
        _canvasGroup = GetComponent<CanvasGroup>();

        destroyAtEnd = _destroyAtEnd;
        loadedTween = tweenToPlay;
        onCompleteCallback = onComplete;

        switch (loadedTween)
        {
            case TweenType.ItemEntry:
                if (delay > 0)
                    transform.localScale = Vector3.zero;
                break;
            case TweenType.FadeIn:
                if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                _canvasGroup.alpha = 0f;
                break;
            case TweenType.FadeOut:
                if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                _canvasGroup.alpha = 1f;
                break;
        }

        if (delay > 0)
        {
            Invoke(nameof(StartTween), delay);
        }
        else
        {
            StartTween();
        }
    }

    private void StartTween()
    {
        TweenCallback onCompleteAction = () =>
        {
            if (destroyAtEnd) Destroy(this);
            //else gameObject.SetActive(false);

            onCompleteCallback?.Invoke();
        };

        switch (loadedTween)
        {
            case TweenType.ItemEntry:
                _transform.localScale = Vector3.zero;
                ItemEntry(onCompleteAction);
                break;
            case TweenType.ItemClose:
                ItemClose(onCompleteAction);
                break;
            case TweenType.Pulse:
                Pulse(onCompleteAction);
                break;
            case TweenType.PulseLarge:
                PulseLarge(onCompleteAction);
                break;
            case TweenType.PulseSlow:
                PulseSlow(onCompleteAction);
                break;
            case TweenType.PulseStart:
                PulseStart(onCompleteAction);
                break;
            case TweenType.PulseEnd:
                PulseEnd(onCompleteAction);
                break;
            case TweenType.PulseDouble:
                PulseDouble(onCompleteAction);
                break;
            case TweenType.FadeIn:
                FadeIn(onCompleteAction);
                break;
            case TweenType.FadeOut:
                FadeOut(onCompleteAction);
                break;
        }
    }

    public void ItemEntry(TweenCallback onComplete = null)
        => PlayScaleCurve(effectManager.itemEntryCurve, effectManager.itemEntryDuration, onComplete);

    public void ItemClose(TweenCallback onComplete = null)
        => PlayScaleCurve(effectManager.itemCloseCurve, effectManager.itemCloseDuration, onComplete);

    public void Pulse(TweenCallback onComplete = null)
        => PlayScaleCurve(effectManager.pulseCurve, effectManager.pulseDuration, onComplete);

    public void PulseSlow(TweenCallback onComplete = null)
        => PlayScaleCurve(effectManager.pulseSlowCurve, effectManager.pulseSlowDuration, onComplete);

    public void PulseLarge(TweenCallback onComplete = null)
        => PlayScaleCurve(effectManager.pulseLargeCurve, effectManager.pulseDuration, onComplete);

    public void PulseStart(TweenCallback onComplete = null)
        => PlayScaleCurve(effectManager.pulseStartCurve, effectManager.pulseStartDuration, onComplete);

    public void PulseEnd(TweenCallback onComplete = null)
        => PlayScaleCurve(effectManager.pulseEndCurve, effectManager.pulseEndDuration, onComplete);

    public void PulseDouble(TweenCallback onComplete = null)
        => PlayScaleCurve(effectManager.pulseDoubleCurve, effectManager.pulseDoubleDuration, onComplete);

    public void FadeIn(TweenCallback onComplete = null)
        => PlayAlphaCurve(effectManager.fadeInCurve, effectManager.fadeInDuration, 0f, 1f, onComplete);

    public void FadeOut(TweenCallback onComplete = null)
        => PlayAlphaCurve(effectManager.fadeOutCurve, effectManager.fadeOutDuration, 1f, 0f, onComplete);

    Tween PlayScaleCurve(AnimationCurve curve, float duration, TweenCallback onComplete = null)
    {
        var t = DOVirtual.Float(0f, 1f, duration, v =>
        {
            float s = curve.Evaluate(v);
            transform.localScale = new Vector3(s, s, s);
        })
        .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        if (onComplete != null) t.OnComplete(onComplete);
        return t.Play();
    }

    Tween PlayAlphaCurve(AnimationCurve curve, float duration, float from, float to, TweenCallback onComplete = null)
    {
        // Ensure starting alpha
        _canvasGroup.alpha = from;

        var t = DOVirtual.Float(0f, 1f, duration, v =>
        {
            // Evaluate curve and remap to [from,to]
            float k = curve.Evaluate(v);
            _canvasGroup.alpha = Mathf.LerpUnclamped(from, to, k);
        })
        .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        if (onComplete != null) t.OnComplete(onComplete);
        return t.Play();
    }
}

public enum TweenType
{
    ItemEntry,
    ItemClose,
    Pulse,
    PulseLarge,
    PulseSlow,
    PulseStart,
    PulseEnd,
    PulseDouble,
    FadeIn,
    FadeOut
}
