using System.Collections.Generic;

namespace BattleEditor.Data
{
    public enum Faction { Ally, Enemy, Neutral }

    public class CooldownDef { public double time; public string startsAt; }

    public class SkillData
    {
        public string id;
        public string name;
        public CooldownDef cooldown;
        public Dictionary<string, double> cost;
        public List<PhaseData> phases = new List<PhaseData>();
    }

    public class PhaseData
    {
        public TriggerDef trigger;
        public List<ConditionDef> conditions = new List<ConditionDef>();
        public TargetingDef targeting;
        public List<EffectDef> effects = new List<EffectDef>();
        public FxDef fx;
    }

    public class TriggerDef { public string type; }

    public class ConditionDef { public string type; public string op; public double value; public string statusId; }

    public class TargetingDef
    {
        public string faction; // "ally" / "enemy" / "self"
        public ShapeDef shape;
        public string sort; // nearest / lowestHpPercent
        public string lockMode; // onFire / onCast
        public string onEmpty; // cancel/self/skip
        public int count; // optional
    }

    public class ShapeDef { public string type; public double radius; public int jumps; }

    public class EffectDef
    {
        public string type;              // Damage / Heal / ApplyStatus / ThreatMultiplier
        public string amount;            // formula string
        public string statusId;
        public int stacks = 1;
        public double duration;
        public double value;             // for ThreatMultiplier
    }

    public class FxDef { public string vfx; public string sfx; }

    public class StatusDef
    {
        public string id; public string name;
        public int maxStacks = 1;
        public double duration = 0;
        public bool dispellable = true;
        public string conflictGroup;
        public OnFatalDef onFatal; // optional
    }

    public class OnFatalDef { public string type; public int hpLeft = 1; public double cooldown; }

    public class BattleRules
    {
        public bool reviveAllowed = true;
        public int revivePoint = 1;
        public double timeScale = 1.0;
    }

    public class ThreatRules
    {
        public double baseDecayPerSec = 2;
        public double distanceWeight = 0.3;
        public double healingToThreatRatio = 0.8;
        public double tauntMultiplier = 5.0;
    }

    public class EncounterData
    {
        public string encounterId;
        public List<Wave> waves = new List<Wave>();
        public Dictionary<string, bool> win;
        public Dictionary<string, bool> lose;
    }

    public class Wave
    {
        public double time;
        public List<Spawn> spawns = new List<Spawn>();
    }

    public class Spawn { public string unitId; public double[] pos; public int count = 1; public List<string> statusOnSpawn; }
}
