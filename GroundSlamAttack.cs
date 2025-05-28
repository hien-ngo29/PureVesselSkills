using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SFCore.Utils;
using Random = UnityEngine.Random;
using System.Collections;
using HutongGames.PlayMaker.Actions;

namespace PureVesselSkills
{
    class GroundSlamAttack : MonoBehaviour
    {
        private HeroController hc => HeroController.instance;
        private PlayMakerFSM spellControl;
        private PlayMakerFSM pvControl;

        private AudioSource audioSource;
        private AudioClip plumeUpSound;

        public static void Init()
        {
            GameObject obj = new GameObject("GroundSlamAttack");
            DontDestroyOnLoad(obj);
            obj.AddComponent<GroundSlamAttack>();
        }

        public static void DestroySelf()
        {
            Destroy(GameObject.Find("GroundSlamAttack"));
        }

        void Awake()
        {
            spellControl = hc.spellControl;
            pvControl = Instantiate(PureVesselSkills.preloadedGO["PV"].LocateMyFSM("Control"), hc.transform);

            if (!PureVesselSkills.preloadedGO.ContainsKey("Plume"))
            {
                GameObject plume = Instantiate(pvControl.GetAction<SpawnObjectFromGlobalPool>("Plume Gen", 0).gameObject.Value);
                plume.SetActive(false);
                plume.layer = (int)GlobalEnums.PhysLayers.HERO_ATTACK;
                plume.tag = "Hero Spell";
                Destroy(plume.GetComponent<DamageHero>());
                DontDestroyOnLoad(plume);
                PureVesselSkills.preloadedGO["Plume"] = plume;
            }

            PreloadAudio();
            InsertSpellEffectsInFsm();
            ModifySpellFSM(true);
            ChangeStandGroundSlamHeight();
        }

        private void PreloadAudio()
        {
            audioSource = GameObject.Find("PureVesselAudioSource").GetComponent<AudioSource>();
            plumeUpSound = (AudioClip)pvControl.GetAction<AudioPlayerOneShotSingle>("Plume Up", 1).audioClip.Value;
        }

        private void InsertSpellEffectsInFsm()
        {
            spellControl.CopyState("Quake1 Land", "Q1 Land Plumes");
            spellControl.CopyState("Q2 Land", "Q2 Land Plumes");

            spellControl.ChangeTransition("Q2 Land Plumes", "FINISHED", "Quake Finish");

            spellControl.InsertMethod("Q1 Land Plumes", () => CastPlumes(), 0);
            spellControl.InsertMethod("Q2 Land Plumes", () => CastPlumes(), 0);

            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Q1 Land Plumes", 0, CastPlumes);
            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Q2 Land Plumes", 0, CastPlumes);
        }

        private void ModifySpellFSM(bool enabled)
        {
            if (enabled)
            {
                spellControl.ChangeTransition("Quake1 Down", "HERO LANDED", "Q1 Land Plumes");
                spellControl.ChangeTransition("Quake2 Down", "HERO LANDED", "Q2 Land Plumes");
            }
            else
            {
                spellControl.ChangeTransition("Q2 Land", "FINISHED", "Q2 Pillar");
            }
        }

        private void ChangeStandGroundSlamHeight()
        {
            spellControl.GetAction<SetFloatValue>("Q On Ground", 0).floatValue = 25f;
        }

        private IEnumerator CastPlumes()
        {
            for (float x = 2; x <= 14; x += 3)
            {
                Vector2 pos = HeroController.instance.transform.position;
                float plumeY = pos.y - 2.2f;

                GameObject plumeL = Instantiate(PureVesselSkills.preloadedGO["Plume"], new Vector2(pos.x - x, plumeY), Quaternion.identity);
                plumeL.SetActive(true);
                plumeL.AddComponent<Plume>();

                GameObject plumeR = Instantiate(PureVesselSkills.preloadedGO["Plume"], new Vector2(pos.x + x, plumeY), Quaternion.identity);
                plumeR.SetActive(true);
                plumeR.AddComponent<Plume>();
            }

            yield return new WaitForSeconds(0.25f);
            audioSource.PlayOneShot(plumeUpSound, GameManager.instance.GetImplicitCinematicVolume());
        }
    }
}
