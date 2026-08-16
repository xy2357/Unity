using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using Unity.VisualScripting;

public class Bomb : UsableItem.UsageEffect
{
    public float Radius = 5f;
    public int DamageAmount = 20;

    public override bool Use(CharacterData user)
    {
        Collider[] hits = Physics.OverlapSphere(
            user.transform.position,
            Radius,
            ~0,
            QueryTriggerInteraction.Ignore
            );

        HashSet<CharacterData> damageTargets = new HashSet<CharacterData>();

        foreach (Collider hit in hits)
        {
            CharacterData target = hit.GetComponent<CharacterData>();

            if (target == null ||
                target == user ||
                target.Stats.CurrentHealth == 0 ||
                !damageTargets.Add(target)) 
            {
                continue;
            }

            Weapon.AttackData attackData = new Weapon.AttackData(target, user);

            attackData.AddDamage(
                StatSystem.DamageType.Physical,
                DamageAmount
            );

            target.Damage(attackData);
        }

        return true;
    }
}
