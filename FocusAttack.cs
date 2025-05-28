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
            PlayerData playerData = PlayerData.instance;

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
            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Focus Heal", 16, SpawnBlast);
        }

        private void AddFocusBlastAttackToFSM()
        {
            // spellControl.CopyState("Focus Start", "Focus Blast");

            // FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Focus Blast", 0, SpawnFocusBlast);
            // spellControl.InsertMethod("Focus Blast", () => focusCancelled = false, 0);
            // spellControl.ChangeTransition("Start Slug Anim", "FINISHED", "Focus Blast");

            // spellControl.ChangeTransition("Focus Blast", "FINISHED", "Set Focus Speed");
            // spellControl.ChangeTransition("Focus Blast", "BUTTON UP", "Focus Cancel");
            // spellControl.ChangeTransition("Focus Blast", "LEFT GROUND", "Focus Cancel");
            AddBlastOnFocusToFSM();
            AddCancelBlastOnFocusStartupToFSM();
            AddCancelBlastWhileFocusingToFSM();
        }

        private void AddBlastOnFocusToFSM()
        {
            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Focus Start", 0, SpawnFocusBlast);
            spellControl.InsertMethod("Focus Start", () => focusCancelled = false, 0);
        }

        private void AddCancelBlastOnFocusStartupToFSM()
        {
            spellControl.AddState("Cancel Blast");
            spellControl.AddMethod("Cancel Blast", () => { MyLogger.Log("Hello?"); focusCancelled = true; FocusBlast.DestroySelf(); });
            spellControl.ChangeTransition("Focus Start", "BUTTON UP", "Cancel Blast");
            spellControl.ChangeTransition("Focus Start", "LEFT GROUND", "Cancel Blast");

            NextFrameEvent focusBlastNextAction = new NextFrameEvent();
            focusBlastNextAction.sendEvent = new FsmEvent("FINISHED");

            spellControl.AddAction("Cancel Blast", focusBlastNextAction);
            spellControl.AddTransition("Cancel Blast", "FINISHED", "Focus Cancel");
        }

        private void AddCancelBlastWhileFocusingToFSM()
        {
            spellControl.AddState("Cancel Blast 2");
            spellControl.AddMethod("Cancel Blast 2", () => { MyLogger.Log("Hello? 2"); focusCancelled = true; FocusBlast.DestroySelf(); });
            spellControl.ChangeTransition("Focus", "BUTTON UP", "Cancel Blast 2");
            spellControl.ChangeTransition("Focus", "LEFT GROUND", "Cancel Blast 2");

            NextFrameEvent focusBlastNextAction = new NextFrameEvent();
            focusBlastNextAction.sendEvent = new FsmEvent("FINISHED");

            spellControl.AddAction("Cancel Blast 2", focusBlastNextAction);
            spellControl.AddTransition("Cancel Blast 2", "FINISHED", "Grace Check");
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
