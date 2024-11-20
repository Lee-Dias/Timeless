using UnityEngine;

public class HealingWing : Superpower
{
    [SerializeField] private float healAmount = 5;
    [SerializeField] private float speedBoostAmount = 5;

    public override void Activate()
    {
        BoostSpeed(speedBoostAmount);
        Heal(healAmount);
    }
}