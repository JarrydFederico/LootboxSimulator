using UnityEngine;

public class EffectManager : Manager<EffectManager>
{
    [Header("Tween Curves")]
    public AnimationCurve itemEntryCurve = new AnimationCurve();
    public float itemEntryDuration;

    public AnimationCurve itemCloseCurve = new AnimationCurve();
    public float itemCloseDuration;

    public AnimationCurve pulseCurve = new AnimationCurve();
    public AnimationCurve pulseLargeCurve = new AnimationCurve();
    public float pulseDuration;


    public AnimationCurve pulseSlowCurve = new AnimationCurve();
    public float pulseSlowDuration;

    public AnimationCurve pulseStartCurve = new AnimationCurve();
    public float pulseStartDuration;

    public AnimationCurve pulseEndCurve = new AnimationCurve();
    public float pulseEndDuration;

    public AnimationCurve pulseDoubleCurve = new AnimationCurve();
    public float pulseDoubleDuration;

    public AnimationCurve fadeInCurve = new AnimationCurve();
    public float fadeInDuration;

    public AnimationCurve fadeOutCurve = new AnimationCurve();
    public float fadeOutDuration;

}
