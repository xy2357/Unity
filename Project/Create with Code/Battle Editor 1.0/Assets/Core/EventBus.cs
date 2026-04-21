using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleEditor.Core
{
    /// <summary> Simple static event bus used by runtime systems. </summary>
    public static class EventBus
    {
        public static Action<string, Systems.Unit> OnCardPlayed; // (skillId, caster)
        public static Action<string, Systems.Unit> OnSkillStarted;
        public static Action<string, Systems.Unit, List<Systems.Unit>> OnTargetsResolved;
        public static Action<string, Systems.Unit, Systems.Unit, string, float> OnEffectApplied; // (skillId, caster, target, effectType, amount)
        public static Action<Systems.Unit, float, object> OnHPChanged; // (unit, delta, source)
        public static Action<Systems.Unit, object> OnRevive;
        public static Action<Systems.Unit, object> OnImmortalSaved;

        public static void CardPlayed(string skillId, Systems.Unit caster) => OnCardPlayed?.Invoke(skillId, caster);
        public static void SkillStarted(string id, Systems.Unit caster) => OnSkillStarted?.Invoke(id, caster);
        public static void TargetsResolved(string id, Systems.Unit caster, List<Systems.Unit> targets) => OnTargetsResolved?.Invoke(id, caster, targets);
        public static void EffectApplied(string id, Systems.Unit caster, Systems.Unit target, string effect, float amount) => OnEffectApplied?.Invoke(id, caster, target, effect, amount);
        public static void HPChanged(Systems.Unit u, float d, object src) => OnHPChanged?.Invoke(u, d, src);
        public static void Revived(Systems.Unit u, object src) => OnRevive?.Invoke(u, src);
        public static void ImmortalSaved(Systems.Unit u, object src) => OnImmortalSaved?.Invoke(u, src);
    }
}
