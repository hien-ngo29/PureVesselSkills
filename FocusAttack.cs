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
            focusBlast.DestroySelf();
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
            spellControl.InsertMethod("Focus", SpawnFocusBlast, 0);
            spellControl.InsertMethod("Focus Cancel", () => { 
                focusCancelled = true; 
                focusBlast.DestroySelf(); 
            }, 0);
            spellControl.InsertMethod("Regain Control", () => focusCancelled = false, 0);
        }

        private void SpawnFocusBlast()
        {
            if (focusCancelled)
            {
                return;
            }

            focusBlast = new();
            focusBlast.SetDelayTimeBeforeShowingUp(GetHealingTime());
            focusBlast.Activate();
        }

        private float GetHealingTime()
        {
            var playerData = PlayerData.instance;
            return 0.891f * (playerData.equippedCharm_7 ? 0.66f : 1f) * (playerData.equippedCharm_34 ? 1.65f : 1f);
        }
    }
}
