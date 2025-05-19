using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;
using HutongGames.PlayMaker;

namespace PureVesselSkills
{
    public class PureVesselSkills : Mod
    {
        public static Dictionary<string, GameObject> preloadedGO = new();

        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("GG_Hollow_Knight", "Battle Scene/HK Prime"),
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            preloadedGO["PV"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime"];

            ModHooks.AfterSavegameLoadHook += (SaveGameData data) =>
            {
                HeroController.instance.gameObject.AddComponent<GroundSlamAttack>();
            };
        }
    }
}
