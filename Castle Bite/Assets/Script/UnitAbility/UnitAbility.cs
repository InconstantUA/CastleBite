using UnityEngine;

public abstract class UnitAbility : ScriptableObject
{
    public GameEvent gameEvent;
    public TextAnimation textAnimation;
    public Color abilityIsApplicableColor;
    public Color abilityNotApplicableColor;
    public abstract void Apply(PartyUnit activeUnit, PartyUnit destinationUnit);
}