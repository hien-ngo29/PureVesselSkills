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
        private bool focusCompleted = false;

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
            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Focus Heal", 16, SpawnBlast);
        }

        private void AddFocusBlastAttackToFSM()
        {
            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Focus Start", 0, SpawnFocusBlast);
            spellControl.InsertMethod("Focus Start", () => focusCancelled = false, 0);

            spellControl.InsertMethod("Set HP Amount", () => focusCompleted = true, 0);
            spellControl.InsertMethod("Focus Start", () => { focusCompleted = false; }, 0);
            spellControl.InsertMethod("Focus Cancel", () =>
            {
                if (!focusCompleted)
                {
                    FocusBlast.DestroySelf();
                }
            }, 0);
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
                BlastBubble blastScript = blast.AddComponent<BlastBubble>();
                blastScript.spawnUp = (i % 2 != 0);
                blastScript.blastNumber = i;
                blastScript.sourcePos = hcPos;
                blastScript.blastSound = blastSound;
                blast.SetActive(true);
            }
        }
    }
}
