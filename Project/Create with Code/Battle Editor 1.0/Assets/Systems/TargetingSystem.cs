using System.Collections.Generic;
using UnityEngine;
using BattleEditor.Data;
using System.Linq;

namespace BattleEditor.Systems
{
    public static class TargetingSystem
    {
        public static List<Unit> Resolve(Unit caster, List<Unit> allUnits, TargetingDef def)
        {
            var list = new List<Unit>();
            // Choose domain
            foreach (var u in allUnits)
            {
                if (!u.IsAlive) continue;
                if (def.faction == "self")
                {
                    if (u == caster) list.Add(u);
                }
                else if (def.faction == "ally")
                {
                    if (u.Faction == caster.Faction) list.Add(u);
                }
                else // enemy
                {
                    if (u.Faction != caster.Faction) list.Add(u);
                }
            }

            // Geometry filter
            if (def.shape != null)
            {
                if (def.shape.type == "single")
                {
                    // Keep all; we'll sort below and take first
                }
                else if (def.shape.type == "circle")
                {
                    list = list.Where(u => Vector3.Distance(u.transform.position, caster.transform.position) <= (float)def.shape.radius).ToList();
                }
                else if (def.shape.type == "chain")
                {
                    // chain healing: pick lowest hp% ally in radius, then nearest next etc.
                    int jumps = Mathf.Max(1, def.shape.jumps);
                    float radius = (float)def.shape.radius;
                    var chosen = new List<Unit>();
                    Unit current = caster;
                    for (int j = 0; j < jumps; j++)
                    {
                        var candidates = list.Where(u => !chosen.Contains(u) && Vector3.Distance(current.transform.position, u.transform.position) <= radius).ToList();
                        if (candidates.Count == 0) break;
                        Unit next = candidates.OrderBy(u => u.Stats.HpPct).FirstOrDefault();
                        if (next == null) break;
                        chosen.Add(next);
                        current = next;
                    }
                    return chosen;
                }
            }

            // Sorting
            if (def.sort == "lowestHpPercent")
                list = list.OrderBy(u => u.Stats.HpPct).ToList();
            else if (def.sort == "nearest")
                list = list.OrderBy(u => Vector3.Distance(u.transform.position, caster.transform.position)).ToList();

            // Count
            if (def.shape != null && def.shape.type == "single")
                return list.Count > 0 ? new List<Unit> { list[0] } : new List<Unit>();
            if (def.count > 0) return list.Take(def.count).ToList();
            return list;
        }
    }
}
