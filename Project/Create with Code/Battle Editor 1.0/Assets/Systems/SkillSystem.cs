using System;
using System.Collections.Generic;
using UnityEngine;
using BattleEditor.Data;
using BattleEditor.Core;
using BattleEditor.Util;

namespace BattleEditor.Systems
{
    public class SkillLibrary
    {
        public Dictionary<string, SkillData> Skills = new Dictionary<string, SkillData>();
        public Dictionary<string, StatusDef> Statuses = new Dictionary<string, StatusDef>();
        public ThreatRules ThreatRules = new ThreatRules();
    }

    public class SkillExecutor : MonoBehaviour
    {
        public List<Unit> AllUnits = new List<Unit>();
        public SkillLibrary Library = new SkillLibrary();

        void Update()
        {
            // Tick all statuses
            double dt = Time.deltaTime;
            foreach (var u in AllUnits) u.Statuses.Tick(dt);
            // Decay threat
            foreach (var u in AllUnits) u.Threat.Decay(Time.deltaTime, (float)Library.ThreatRules.baseDecayPerSec);
        }

        public void RegisterUnit(Unit u)
        {
            if (!AllUnits.Contains(u)) AllUnits.Add(u);
        }

        public void LoadSkillJson(string json)
        {
            var data = DataLoader.LoadSkillFromJson(json);
            Library.Skills[data.id] = data;
        }
        public void LoadStatusJson(string json)
        {
            var data = DataLoader.LoadStatusFromJson(json);
            Library.Statuses[data.id] = data;
        }

        public bool Cast(Unit caster, string skillId)
        {
            if (!Library.Skills.TryGetValue(skillId, out var skill)) { Debug.LogWarning($"Skill {skillId} not found"); return false; }
            EventBus.CardPlayed(skillId, caster);
            EventBus.SkillStarted(skillId, caster);

            foreach (var phase in skill.phases)
            {
                if (!CheckConditions(caster, phase.conditions)) return false;
                var targets = TargetingSystem.Resolve(caster, AllUnits, phase.targeting);
                EventBus.TargetsResolved(skillId, caster, targets);
                if (targets.Count == 0)
                {
                    if (phase.targeting != null && phase.targeting.onEmpty == "self") targets = new List<Unit> { caster };
                    else if (phase.targeting != null && phase.targeting.onEmpty == "skip") { continue; }
                    else { return false; }
                }
                foreach (var eff in phase.effects) ApplyEffect(skillId, caster, targets, eff);
            }
            return true;
        }

        bool CheckConditions(Unit caster, List<ConditionDef> conds)
        {
            foreach (var c in conds)
            {
                if (c.type == "TargetHpPct")
                {
                    // For simplicity: evaluate on caster's current target (highest threat) or nearest enemy
                    Unit target = FindPrimaryEnemy(caster);
                    if (target == null) return false;
                    float hpPct = target.Stats.HpPct;
                    bool ok = c.op == "<" ? hpPct < (float)c.value : hpPct > (float)c.value;
                    if (!ok) return false;
                }
            }
            return true;
        }

        Unit FindPrimaryEnemy(Unit caster)
        {
            Unit best = null; float bestDist = float.MaxValue;
            foreach (var u in AllUnits)
            {
                if (!u.IsAlive) continue;
                if (u.Faction == caster.Faction) continue;
                float d = Vector3.Distance(caster.transform.position, u.transform.position);
                if (d < bestDist) { best = u; bestDist = d; }
            }
            return best;
        }

        void ApplyEffect(string skillId, Unit caster, List<Unit> targets, EffectDef eff)
        {
            foreach (var t in targets)
            {
                switch (eff.type)
                {
                    case "Damage":
                        {
                            float amount = (float)EvalAmount(eff.amount, caster, t);
                            t.ApplyDamage(amount, this);
                            // Threat from damage increases toward caster
                            t.Threat.Add(caster, amount);
                            EventBus.EffectApplied(skillId, caster, t, "Damage", amount);
                        }
                        break;
                    case "Heal":
                        {
                            float amount = (float)EvalAmount(eff.amount, caster, t);
                            t.ApplyHeal(amount, this);
                            // Healing increases threat toward healer for enemies
                            foreach (var e in AllUnits)
                            {
                                if (e.Faction != caster.Faction)
                                    e.Threat.Add(caster, amount * (float)Library.ThreatRules.healingToThreatRatio);
                            }
                            EventBus.EffectApplied(skillId, caster, t, "Heal", amount);
                        }
                        break;
                    case "ApplyStatus":
                        {
                            if (Library.Statuses.TryGetValue(eff.statusId, out var sdef))
                            {
                                t.Statuses.Apply(sdef, eff.stacks, eff.duration > 0 ? (double?)eff.duration : null);
                                EventBus.EffectApplied(skillId, caster, t, "ApplyStatus", eff.stacks);
                            }
                        }
                        break;
                    case "ThreatMultiplier":
                        {
                            // Simplified: add a big flat threat to enforce taunt
                            foreach (var e in AllUnits)
                            {
                                if (e.Faction != caster.Faction)
                                    e.Threat.Add(caster, 1000f);
                            }
                            EventBus.EffectApplied(skillId, caster, t, "ThreatMultiplier", (float)eff.value);
                        }
                        break;
                }
            }
        }

        double EvalAmount(string expr, Unit caster, Unit target)
        {
            return SimpleExpression.Eval(expr, name =>
            {
                switch (name)
                {
                    case "Atk": return caster.Stats.Atk;
                    case "Mag": return caster.Stats.Mag;
                    case "Caster.Atk": return caster.Stats.Atk;
                    case "Caster.Mag": return caster.Stats.Mag;
                    case "Target.HpPct": return target.Stats.HpPct;
                    default: return 0;
                }
            });
        }
    }
}
