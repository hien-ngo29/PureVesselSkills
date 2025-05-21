using System;
using System.Collections.Generic;
using UnityEngine;
using SFCore.Utils;
using Random = UnityEngine.Random;
using System.Collections;
using HutongGames.PlayMaker.Actions;


namespace PureVesselSkills
{
    class SpikeShootingAttack : MonoBehaviour
    {
        private HeroController hc => HeroController.instance;
        private PlayMakerFSM spellControl;

        public static void Init()
        {
            GameObject obj = new GameObject("SpikeShootingAttack");
            DontDestroyOnLoad(obj);
            obj.AddComponent<SpikeShootingAttack>();
        }

        void Awake()
        {
            if (!PureVesselSkills.preloadedGO["Spike"])
            {
                spellControl = hc.spellControl;
                PlayMakerFSM pvControl = Instantiate(PureVesselSkills.preloadedGO["PV"].LocateMyFSM("Control"), hc.transform);

                GameObject spike = Instantiate(pvControl.GetAction<FlingObjectsFromGlobalPoolTime>("SmallShot LowHigh", 2).gameObject.Value);
                spike.SetActive(false);
                spike.layer = (int)GlobalEnums.PhysLayers.HERO_ATTACK;
                spike.tag = "Hero Spell";
                Destroy(spike.GetComponent<DamageHero>());
                // Destroy(spike.LocateMyFSM("Control"));
                // spike.FindGameObjectInChildren("Dribble L").layer = 9;
                // spike.FindGameObjectInChildren("Glow").layer = 9;
                // spike.FindGameObjectInChildren("Beam").layer = 9;
                DontDestroyOnLoad(spike);
                PureVesselSkills.preloadedGO["Spike"] = spike;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ShootSpikes();
            }
        }

        private void ShootSpikes()
        {
            // TODO: Implement spike shooting logic
        }
    }
}
