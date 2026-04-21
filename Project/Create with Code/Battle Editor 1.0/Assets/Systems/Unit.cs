using System.Collections.Generic;
using UnityEngine;
using BattleEditor.Data;
using BattleEditor.Core;

namespace BattleEditor.Systems
{
    public class UnitStats
    {
        public float MaxHP = 100;
        public float HP = 100;
        public float Atk = 10;
        public float Mag = 10;
        public float CritRate = 0.1f;
        public float Haste = 0f;

        public float HpPct => Mathf.Clamp01(HP / Mathf.Max(1, MaxHP));
    }

    public class Unit : MonoBehaviour
    {
        public string UnitId = "u_dummy";
        public Faction Faction = Faction.Ally;
        public UnitStats Stats = new UnitStats();

        public StatusContainer Statuses = new StatusContainer();
        public ThreatTable Threat = new ThreatTable();

        public bool IsAlive => Stats.HP > 0.01f;

        public void Init(Faction faction, float hp = 100f, float atk = 10f, float mag = 10f)
        {
            Faction = faction; Stats.MaxHP = hp; Stats.HP = hp; Stats.Atk = atk; Stats.Mag = mag;
        }

        public void ApplyDamage(float amount, object source)
        {
            if (amount <= 0) return;
            float before = Stats.HP;
            float after = before - amount;
            // OnFatal check (immortal window / prevent death)
            if (after <= 0 && Statuses.TryPreventDeath(this))
            {
                after = 1;
                EventBus.ImmortalSaved(this, source);
            }
            Stats.HP = Mathf.Max(0, after);
            EventBus.HPChanged(this, Stats.HP - before, source);
        }

        public void ApplyHeal(float amount, object source)
        {
            if (amount <= 0) return;
            float before = Stats.HP;
            Stats.HP = Mathf.Min(Stats.MaxHP, Stats.HP + amount);
            EventBus.HPChanged(this, Stats.HP - before, source);
        }

        public void Revive(float hpRatio, object source)
        {
            if (IsAlive) return;
            Stats.HP = Mathf.Max(1, Stats.MaxHP * Mathf.Clamp01(hpRatio));
            EventBus.Revived(this, source);
        }

        public override string ToString() => $"{UnitId}[{Faction}] HP:{Stats.HP}/{Stats.MaxHP}";
    }
}
