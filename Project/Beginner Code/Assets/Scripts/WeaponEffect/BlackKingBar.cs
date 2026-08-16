using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class BlackKingBar : Weapon.WeaponAttackEffect
{

    public StatSystem.StatModifier Modifier = new StatSystem.StatModifier();
    public float DamageDuration = 5f;
    public int DamagePerTick = 1;
    public float TickInterval = 1f;

    public override void OnAttack(CharacterData target, CharacterData user, ref Weapon.AttackData attackData)
    {
        Modifier.ModifierMode = StatSystem.StatModifier.Mode.Absolute;
        Modifier.Stats.strength = 1;

        user.Stats.AddModifier(Modifier);
        Debug.Log($"{user.Stats.stats.strength}");

        ElementalEffect effect = new ElementalEffect(
            DamageDuration,
            StatSystem.DamageType.Fire,
            DamagePerTick,
            TickInterval
            );
        target.Stats.AddElementalEffect( effect );

    }
    
    public override void OnPostAttack(CharacterData target, CharacterData user, Weapon.AttackData data)
    {
        int actualDamage = data.GetDamage(StatSystem.DamageType.Physical);

        int healAmount = actualDamage;

        user.Stats.ChangeHealth(healAmount);
    }
}
