using UnityEngine;
using UnityEngine.Events;

//public enum UnitPowerModifierID
//{
//    LifeLeech
//}

public abstract class UnitPowerModifier : ScriptableObject
{
    //public UnityEvent OnApply;
    public GameEvent gameEvent;
    public UnitPowerModifierConfig unitPowerModifierConfig;
    public TextAnimation textAnimation;
    public abstract void Apply(PartyUnit activeUnit, PartyUnit destinationUnit);
    //public void TriggerEvent(GameEvent gameEvent)
    //{
    //    gameEvent.Raise(this);
    //}
}