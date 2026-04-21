using UnityEngine;
using BattleEditor.Systems;
using BattleEditor.Data;

namespace BattleEditor.Demo
{
    public static class UnitFactory
    {
        public static Unit Create(string name, Faction faction, Vector3 pos)
        {
            var go = new GameObject(name);
            go.transform.position = pos;
            var u = go.AddComponent<Unit>();
            u.Init(faction, 200, faction==Faction.Enemy?15:10, faction==Faction.Enemy?8:12);
            var col = go.AddComponent<SphereCollider>(); col.radius = 0.5f;
            var rb = go.AddComponent<Rigidbody>(); rb.useGravity = false; rb.isKinematic = true;
            return u;
        }
    }
}
