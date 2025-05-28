using System;
using System.Collections.Generic;
using UnityEngine;
using SFCore.Utils;
using Random = UnityEngine.Random;
using System.Collections;
using GlobalEnums;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using FrogCore;
using System.Linq;

namespace PureVesselSkills
{
    public class FocusAttack : MonoBehaviour
    {
        private HeroController hc => HeroController.instance;
        private PlayMakerFSM spellControl;
        private bool focusCancelled = false;

        private AudioClip blastSound;

        public static void Init()
        {
            GameObject obj = new GameObject("FocusAttack");
            DontDestroyOnLoad(obj);
            obj.AddComponent<FocusAttack>();
        }

        public static void DestroySelf()
        {
            Destroy(GameObject.Find("FocusAttack"));
        }

        private void Awake()
        {
            On.HeroController.TakeDamage += OnHeroTakeDamage;

            spellControl = hc.spellControl;

            PreloadAudio();
            AddAttackToFSM();
        }

        private void PreloadAudio()
        {
            PlayMakerFSM pvControl = PureVesselSkills.preloadedGO["PV"].LocateMyFSM("Control");
            PlayMakerFSM hkPrimeBlastControl = PureVesselSkills.preloadedGO["HKPrimeBlast"].LocateMyFSM("Control");
            blastSound = (AudioClip)hkPrimeBlastControl.GetAction<AudioPlayerOneShotSingle>("Sound", 1).audioClip.Value;
        }

        private void OnHeroTakeDamage(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
        {
            orig(self, go, damageSide, damageAmount, hazardType);
            focusCancelled = false;
            FocusBlast.DestroySelf();
        }

        private void AddAttackToFSM()
        {
            AddAfterBlastAttackToFSM();
            AddFocusBlastAttackToFSM();
        }

        private void AddAfterBlastAttackToFSM()
        {
            spellControl.CopyState("Focus Heal", "Focus Attack");
            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Focus Attack", 16, SpawnBlast);
            spellControl.ChangeTransition("Set HP Amount", "FINISHED", "Focus Attack");
            spellControl.ChangeTransition("Focus Attack", "WAIT", "Full HP?");
        }

        private void AddFocusBlastAttackToFSM()
        {
            spellControl.CopyState("Focus Start", "Focus Blast");

            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Focus Blast", 0, SpawnFocusBlast);
            spellControl.InsertMethod("Focus Blast", () => focusCancelled = false, 0);
            spellControl.ChangeTransition("Start Slug Anim", "FINISHED", "Focus Blast");

            spellControl.ChangeTransition("Focus Blast", "FINISHED", "Set Focus Speed");
            spellControl.ChangeTransition("Focus Blast", "BUTTON UP", "Focus Cancel");
            spellControl.ChangeTransition("Focus Blast", "LEFT GROUND", "Focus Cancel");

            spellControl.InsertMethod("Focus Cancel", () => { focusCancelled = true; FocusBlast.DestroySelf(); }, 14);
        }

        private IEnumerator SpawnFocusBlast()
        {
            yield return new WaitForSeconds(0.25f);

            if (focusCancelled)
            {
                focusCancelled = false;
                yield break;
            }

            GameObject focusBlast = Instantiate(PureVesselSkills.preloadedGO["FocusBlast"], hc.transform.position, Quaternion.identity);
            focusBlast.AddComponent<FocusBlast>();
            focusBlast.SetActive(true);
        }

        private IEnumerator SpawnBlast()
        {
            Vector3 hcPos = hc.transform.position;
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(Random.Range(0f, 0.45f));

                GameObject blast = Instantiate(PureVesselSkills.preloadedGO["Blast"]);
                Blast blastScript = blast.AddComponent<Blast>();
                blastScript.spawnUp = (i % 2 != 0);
                blastScript.blastNumber = i;
                blastScript.sourcePos = hcPos;
                blastScript.blastSound = blastSound;
                blast.SetActive(true);
            }
        }
    }
}
