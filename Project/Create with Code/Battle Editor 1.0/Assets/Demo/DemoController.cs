using UnityEngine;
using System.IO;
using BattleEditor.Systems;

namespace BattleEditor.Demo
{
    /// <summary> Minimal scene bootstrapper. Drag this onto an empty GameObject in a new scene. </summary>
    public class DemoController : MonoBehaviour
    {
        public SkillExecutor Executor;
        public Unit AllyA, AllyB, AllyC;
        public Unit EnemyA, EnemyB, Boss;

        void Awake()
        {
            if (Executor == null) Executor = gameObject.AddComponent<SkillExecutor>();
            RegisterUnits();
            LoadSampleJson();
        }

        void RegisterUnits()
        {
            var all = new Unit[] { AllyA, AllyB, AllyC, EnemyA, EnemyB, Boss };
            foreach (var u in all)
            {
                if (u == null) continue;
                Executor.RegisterUnit(u);
            }
        }

        void LoadSampleJson()
        {
            // Editor-only friendly: read from Assets/BattleEditor/Data/Samples/...
            string basePath = Path.Combine(Application.dataPath, "BattleEditor/Data/Samples");
            string skills = Path.Combine(basePath, "Skills");
            string statuses = Path.Combine(basePath, "Statuses");

            string ReadFile(string path) => File.Exists(path) ? File.ReadAllText(path) : null;

            string[] sfiles = { "sk_chain_heal.json", "sk_execute_strike.json", "sk_taunt_shout.json" };
            foreach (var f in sfiles)
            {
                var json = ReadFile(Path.Combine(skills, f));
                if (!string.IsNullOrEmpty(json)) Executor.LoadSkillJson(json);
                else Debug.LogWarning("Missing skill file: " + f);
            }

            string[] stfiles = { "st_revive_mark.json", "st_immortal_window.json", "st_taunted.json" };
            foreach (var f in stfiles)
            {
                var json = ReadFile(Path.Combine(statuses, f));
                if (!string.IsNullOrEmpty(json)) Executor.LoadStatusJson(json);
                else Debug.LogWarning("Missing status file: " + f);
            }
        }

        void Update()
        {
            // Controls:
            // 1: AllyA casts chain heal
            // 2: EnemyA uses execute strike on lowest HP ally (if <20%)
            // 3: AllyA taunts nearby enemies
            if (Input.GetKeyDown(KeyCode.Alpha1)) Executor.Cast(AllyA, "sk_chain_heal");
            if (Input.GetKeyDown(KeyCode.Alpha2)) Executor.Cast(EnemyA, "sk_execute_strike");
            if (Input.GetKeyDown(KeyCode.Alpha3)) Executor.Cast(AllyA, "sk_taunt_shout");
        }
    }
}
