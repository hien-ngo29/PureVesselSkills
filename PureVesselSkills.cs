using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using static AbilityChanger.AbilityChanger;
using UnityEngine;
using UObject = UnityEngine.Object;
using HutongGames.PlayMaker;
using AbilityChanger;

namespace PureVesselSkills
{
    public class PureVesselSkills : Mod
    {
        public static Dictionary<string, GameObject> preloadedGO = new();
        public static AudioSource audioSource;

        List<Ability> abilities;

        public override string GetVersion() => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("GG_Hollow_Knight", "Battle Scene/HK Prime"),
                ("GG_Hollow_Knight", "Battle Scene/HK Prime/Focus Blast"),
                ("GG_Hollow_Knight", "Battle Scene/Focus Blasts/HK Prime Blast/Blast"),
                ("GG_Hollow_Knight", "Battle Scene/Focus Blasts/HK Prime Blast")
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            preloadedGO["PV"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime"];
            preloadedGO["FocusBlast"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime/Focus Blast"];
            preloadedGO["Blast"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/Focus Blasts/HK Prime Blast/Blast"];
            preloadedGO["HKPrimeBlast"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/Focus Blasts/HK Prime Blast"];

            On.HeroController.Awake += (On.HeroController.orig_Awake orig, HeroController self) => { StartMod(); orig(self); };
        }

        private void StartMod()
        {
            // PureVesselAudioSource.DestroySelf();
            // GroundSlamAttack.DestroySelf();
            // SpikeShootingAttack.DestroySelf();
            // FocusAttack.DestroySelf();

            PureVesselAudioSource.Init();
            // GroundSlamAttack.Init();
            // SpikeShootingAttack.Init();
            // FocusAttack.Init();

            abilities = new() {
                new GroundSlamAttackAbility()
            };

            foreach (var ability in abilities)
            {
                RegisterAbility(ability);
            }

            audioSource = GameObject.Find("PureVesselAudioSource").GetComponent<AudioSource>();
        }
    }
}
