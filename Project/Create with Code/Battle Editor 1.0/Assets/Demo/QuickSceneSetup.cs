using UnityEngine;
using BattleEditor.Data;
using BattleEditor.Systems;

namespace BattleEditor.Demo
{
    /// <summary> One-click scene setup: adds units and wires DemoController. Use from empty scene via the menu. </summary>
    public class QuickSceneSetup : MonoBehaviour
    {
        [ContextMenu("Setup Demo Scene")]
        void Setup()
        {
            var controller = new GameObject("DemoController").AddComponent<DemoController>();
            // Allies
            controller.AllyA = UnitFactory.Create("AllyA", Faction.Ally, new Vector3(-3,0,0));
            controller.AllyB = UnitFactory.Create("AllyB", Faction.Ally, new Vector3(-2,0,2));
            controller.AllyC = UnitFactory.Create("AllyC", Faction.Ally, new Vector3(-1,0,-2));
            // Enemies
            controller.EnemyA = UnitFactory.Create("EnemyA", Faction.Enemy, new Vector3(3,0,0));
            controller.EnemyB = UnitFactory.Create("EnemyB", Faction.Enemy, new Vector3(2,0,2));
            controller.Boss   = UnitFactory.Create("Boss",   Faction.Enemy, new Vector3(1,0,-2));
            // Unity will automatically call Awake() on the next frame
        }
    }
}
