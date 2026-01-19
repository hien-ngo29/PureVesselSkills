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

        private ChainedBlastsSpawner chainedBlastsSpawner;
        private FocusBlast focusBlast;

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
            chainedBlastsSpawner = new();
            focusBlast = new();

            On.HeroController.TakeDamage += OnHeroTakeDamage;
            spellControl = hc.spellControl;

            AddAttackToFSM();
        }

        private void OnHeroTakeDamage(On.HeroController.orig_TakeDamage orig, HeroController self, GameObject go, CollisionSide damageSide, int damageAmount, int hazardType)
        {
            orig(self, go, damageSide, damageAmount, hazardType);

            focusCancelled = false;
        }

        private void AddAttackToFSM()
        {
            AddAfterBlastAttackToFSM();
            AddFocusBlastAttackToFSM();
        }

        private void AddAfterBlastAttackToFSM()
        {
            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Focus Heal", 16, chainedBlastsSpawner.SpawnChainedBlasts);
        }

        private void AddFocusBlastAttackToFSM()
        {
            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Focus", 0, SpawnFocusBlast);
            spellControl.InsertMethod("Focus Start", () => focusCancelled = false, 0);

            spellControl.InsertMethod("Set HP Amount", () => focusCompleted = true, 0);
            spellControl.InsertMethod("Focus Start", () => { focusCompleted = false; }, 0);
            spellControl.InsertMethod("Focus Cancel", () =>
            {
                if (!focusCompleted)
                {
                    
                }
            }, 0);
        }

        private IEnumerator SpawnFocusBlast()
        {
            if (focusCancelled)
            {
                focusCancelled = false;
                yield break;
            }

            focusBlast = new();
            focusBlast.SetDelayTimeBeforeShowingUp(0.891f);
            focusBlast.Activate();
        }
    }
}
