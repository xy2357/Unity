using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BattleEditor.Util;

namespace BattleEditor.Data
{
    /// <summary> Loads JSON files from project folders into strongly typed data objects using FastJson. </summary>
    public static class DataLoader
    {
        public static SkillData LoadSkillFromJson(string json)
        {
            var root = FastJson.Parse(json) as Dictionary<string, object>;
            var s = new SkillData();
            s.id = FastJson.GetString(root, "id");
            s.name = FastJson.GetString(root, "name");
            var cd = FastJson.GetDict(root, "cooldown");
            if (cd != null) s.cooldown = new CooldownDef { time = FastJson.GetNumber(cd, "time"), startsAt = FastJson.GetString(cd, "startsAt", "OnCast") };
            var cost = FastJson.GetDict(root, "cost");
            if (cost != null) { s.cost = new Dictionary<string, double>(); foreach (var kv in cost) if (kv.Value is double) s.cost[kv.Key] = (double)kv.Value; }
            var phases = FastJson.GetList(root, "phases");
            if (phases != null)
            {
                foreach (var p in phases)
                {
                    var pd = ParsePhase(p as Dictionary<string, object>);
                    s.phases.Add(pd);
                }
            }
            return s;
        }

        static PhaseData ParsePhase(Dictionary<string, object> d)
        {
            var p = new PhaseData();
            var trg = FastJson.GetDict(d, "trigger");
            if (trg != null) p.trigger = new TriggerDef { type = FastJson.GetString(trg, "type") };

            var conds = FastJson.GetList(d, "conditions");
            if (conds != null) foreach (var c in conds) p.conditions.Add(ParseCondition(c as Dictionary<string, object>));

            var tgt = FastJson.GetDict(d, "targeting");
            if (tgt != null) p.targeting = ParseTargeting(tgt);

            var effs = FastJson.GetList(d, "effects");
            if (effs != null) foreach (var e in effs) p.effects.Add(ParseEffect(e as Dictionary<string, object>));

            var fx = FastJson.GetDict(d, "fx");
            if (fx != null) p.fx = new FxDef { vfx = FastJson.GetString(fx, "vfx"), sfx = FastJson.GetString(fx, "sfx") };
            return p;
        }

        static ConditionDef ParseCondition(Dictionary<string, object> d)
        {
            return new ConditionDef
            {
                type = FastJson.GetString(d, "type"),
                op = FastJson.GetString(d, "op"),
                value = FastJson.GetNumber(d, "value"),
                statusId = FastJson.GetString(d, "statusId")
            };
        }

        static TargetingDef ParseTargeting(Dictionary<string, object> d)
        {
            var t = new TargetingDef();
            t.faction = FastJson.GetString(d, "faction", "enemy");
            t.sort = FastJson.GetString(d, "sort", "nearest");
            t.lockMode = FastJson.GetString(d, "lock", "onFire");
            t.onEmpty = FastJson.GetString(d, "onEmpty", "cancel");
            t.count = (int)FastJson.GetNumber(d, "count", 0);
            var shape = FastJson.GetDict(d, "shape");
            if (shape != null) t.shape = new ShapeDef { type = FastJson.GetString(shape, "type"), radius = FastJson.GetNumber(shape, "radius"), jumps = (int)FastJson.GetNumber(shape, "jumps", 0) };
            return t;
        }

        static EffectDef ParseEffect(Dictionary<string, object> d)
        {
            return new EffectDef
            {
                type = FastJson.GetString(d, "type"),
                amount = FastJson.GetString(d, "amount"),
                statusId = FastJson.GetString(d, "statusId"),
                stacks = (int)FastJson.GetNumber(d, "stacks", 1),
                duration = FastJson.GetNumber(d, "duration", 0),
                value = FastJson.GetNumber(d, "value", 0)
            };
        }

        public static StatusDef LoadStatusFromJson(string json)
        {
            var d = FastJson.Parse(json) as Dictionary<string, object>;
            var s = new StatusDef();
            s.id = FastJson.GetString(d, "id");
            s.name = FastJson.GetString(d, "name");
            s.maxStacks = (int)FastJson.GetNumber(d, "maxStacks", 1);
            s.duration = FastJson.GetNumber(d, "duration", 0);
            s.dispellable = FastJson.GetBool(d, "dispellable", true);
            s.conflictGroup = FastJson.GetString(d, "conflictGroup");

            var fatal = FastJson.GetDict(d, "onFatal");
            if (fatal != null)
            {
                s.onFatal = new OnFatalDef { type = FastJson.GetString(fatal, "type"), hpLeft = (int)FastJson.GetNumber(fatal, "hpLeft", 1), cooldown = FastJson.GetNumber(fatal, "cooldown", 0) };
            }
            return s;
        }
    }
}
