using System.Collections.Generic;
using UnityEngine;
using BattleEditor.Data;

namespace BattleEditor.Systems
{
    public class ThreatTable
    {
        Dictionary<Unit, float> _map = new Dictionary<Unit, float>();
        public void Add(Unit source, float amount)
        {
            if (source == null) return;
            _map.TryGetValue(source, out float v);
            _map[source] = v + amount;
        }

        public Unit Highest()
        {
            float max = float.MinValue; Unit best = null;
            foreach (var kv in _map)
            {
                if (kv.Value > max) { max = kv.Value; best = kv.Key; }
            }
            return best;
        }

        public void Decay(float dt, float rate)
        {
            var keys = new List<Unit>(_map.Keys);
            foreach (var k in keys) { _map[k] = Mathf.Max(0, _map[k] - rate * dt); }
        }
    }
}
