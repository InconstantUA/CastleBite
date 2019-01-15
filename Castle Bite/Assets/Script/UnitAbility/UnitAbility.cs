using UnityEngine;

public abstract class UnitAbility : ScriptableObject
{
    public GameEvent gameEvent;
    public TextAnimation textAnimation;
    public abstract void Apply(PartyUnit activeUnit, PartyUnit destinationUnit);
}