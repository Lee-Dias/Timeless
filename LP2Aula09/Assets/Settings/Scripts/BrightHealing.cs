using UnityEngine;

public class BrightHealing : Superpower
{
    [SerializeField] private float healAmount = 5;
    public override void Activate()
    {
        LightMyFire();
        Heal(healAmount);
    }
}