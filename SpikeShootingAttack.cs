using System;
using System.Collections.Generic;
using UnityEngine;
using SFCore.Utils;
using Random = UnityEngine.Random;
using System.Collections;
using HutongGames.PlayMaker.Actions;
using FrogCore;

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

        public static void DestroySelf()
        {
            Destroy(GameObject.Find("SpikeShootingAttack"));
        }

        void Awake()
        {
            spellControl = hc.spellControl;

            if (!PureVesselSkills.preloadedGO.ContainsKey("Spike"))
            {
                PlayMakerFSM pvControl = Instantiate(PureVesselSkills.preloadedGO["PV"].LocateMyFSM("Control"), hc.transform);

                GameObject spike = Instantiate(pvControl.GetAction<FlingObjectsFromGlobalPoolTime>("SmallShot LowHigh", 2).gameObject.Value);
                spike.SetActive(false);
                spike.layer = (int)GlobalEnums.PhysLayers.HERO_ATTACK;
                spike.tag = "Hero Spell";
                Destroy(spike.GetComponent<DamageHero>());
                spike.FindGameObjectInChildren("Dribble L").layer = 9;
                spike.FindGameObjectInChildren("Glow").layer = 9;
                spike.FindGameObjectInChildren("Beam").layer = 9;
                DontDestroyOnLoad(spike);
                PureVesselSkills.preloadedGO["Spike"] = spike;
            }

            ChangeFireballCastToSpikeShooting();
        }

        private void ChangeFireballCastToSpikeShooting()
        {
            spellControl.CopyState("Fireball 2", "Spike Attack");

            FrogCore.Fsm.FsmUtil.RemoveAction(spellControl, "Spike Attack", 3);
            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Spike Attack", 1, ShootSpikes);

            spellControl.ChangeTransition("Level Check", "LEVEL 1", "Spike Attack");
            spellControl.ChangeTransition("Level Check", "LEVEL 2", "Spike Attack");
            spellControl.ChangeTransition("Spike Attack", "FINISHED", "Fireball Recoil");
        }

        private IEnumerator ShootSpikes()
        {
            Vector2 pos = HeroController.instance.transform.position;
            float direction = HeroController.instance.gameObject.transform.localScale.x;

            for (float i = 150; i >= 10; i = i - 20)
            {
                GameObject spike = Instantiate(PureVesselSkills.preloadedGO["Spike"]);
                spike.transform.position = new Vector2(pos.x, pos.y - 0.7f);

                spike.SetActive(true);

                Spike spikeComponent = spike.AddComponent<Spike>();
                spikeComponent.angle = i * direction;

                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
