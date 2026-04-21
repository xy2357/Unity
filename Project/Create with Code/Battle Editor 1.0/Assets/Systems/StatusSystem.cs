using System;
using System.Collections.Generic;
using UnityEngine;
using BattleEditor.Data;

namespace BattleEditor.Systems
{
    [Serializable]
    public class ActiveStatus
    {
        public StatusDef Def;
        public int Stacks = 1;
        public double Remaining;
        public double CooldownRemaining; // for onFatal cooldown
        public bool HasOnFatal => Def != null && Def.onFatal != null && Def.onFatal.type == "PreventDeath";
    }

    [Serializable]
    public class StatusContainer
    {
        List<ActiveStatus> _list = new List<ActiveStatus>();

        public void Tick(double dt)
        {
            for (int i = _list.Count - 1; i >= 0; --i)
            {
                var a = _list[i];
                if (a.Def.duration > 0)
                {
                    a.Remaining -= dt;
                    if (a.Remaining <= 0) _list.RemoveAt(i);
                }
                if (a.CooldownRemaining > 0) a.CooldownRemaining -= dt;
            }
        }

        public void Apply(StatusDef def, int stacks = 1, double? overrideDuration = null)
        {
            var exist = _list.Find(s => s.Def.id == def.id);
            if (exist == null)
            {
                exist = new ActiveStatus { Def = def, Stacks = 0, Remaining = overrideDuration ?? def.duration };
                _list.Add(exist);
            }
            exist.Stacks = Mathf.Clamp(exist.Stacks + stacks, 1, Mathf.Max(1, def.maxStacks));
            if (overrideDuration.HasValue) exist.Remaining = overrideDuration.Value;
            else if (def.duration > 0) exist.Remaining = def.duration;
        }

        public bool Has(string id) => _list.Exists(s => s.Def.id == id);

        public bool TryPreventDeath(Unit owner)
        {
            for (int i = 0; i < _list.Count; ++i)
            {
                var a = _list[i];
                if (a.HasOnFatal && a.CooldownRemaining <= 0)
                {
                    a.CooldownRemaining = a.Def.onFatal.cooldown;
                    return true;
                }
            }
            return false;
        }
    }
}
