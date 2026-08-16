using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;

public class BoomEquipment : EquipmentItem.EquippedEffect
{
    public StatSystem.StatModifier Modifier = new StatSystem.StatModifier();

     public override void Equipped(CharacterData user)
     {
        Modifier.ModifierMode = StatSystem.StatModifier.Mode.Absolute;
        Modifier.Stats.strength = 5;

        user.Stats.AddModifier(Modifier);
     }
     
     public override void Removed(CharacterData user)
     {
        user.Stats.RemoveModifier(Modifier);
     }
}
