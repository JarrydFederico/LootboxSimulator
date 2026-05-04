using UnityEngine;

public class EffectManager : Manager<EffectManager>
{
    [SerializeField] private TweenCurves tweenCurves;
    public TweenCurves TweenCurves => tweenCurves;

}
