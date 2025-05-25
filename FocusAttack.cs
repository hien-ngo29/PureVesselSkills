using System;
using System.Collections.Generic;
using UnityEngine;
using SFCore.Utils;
using Random = UnityEngine.Random;
using System.Collections;
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
            spellControl = hc.spellControl;
            AddAttackToFSM();
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
            spellControl.ChangeTransition("Start Slug Anim", "FINISHED", "Focus Blast");

            spellControl.ChangeTransition("Focus Blast", "FINISHED", "Set Focus Speed");
            spellControl.ChangeTransition("Focus Blast", "BUTTON UP", "Focus Cancel");
            spellControl.ChangeTransition("Focus Blast", "LEFT GROUND", "Focus Cancel");

            spellControl.InsertMethod("Focus Cancel", DestroyHeroFocusBlast, 14);
        }

        private IEnumerator SpawnFocusBlast()
        {
            yield return new WaitForSeconds(0.25f);

            GameObject focusBlast = Instantiate(PureVesselSkills.preloadedGO["FocusBlast"], hc.transform.position, Quaternion.identity);
            focusBlast.name = "Hero Focus Blast";
            focusBlast.SetActive(true);

            yield return new WaitForSeconds(2.5f);
            DestroyHeroFocusBlast();
        }

        private void DestroyHeroFocusBlast()
        {
            Destroy(GameObject.Find("Hero Focus Blast"));
        }

        private IEnumerator SpawnBlast()
        {
            Vector3 hcPos = hc.transform.position;
            for (int i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(Random.Range(0f, 0.45f));

                GameObject blast = Instantiate(PureVesselSkills.preloadedGO["Blast"]);
                Blast blastScript = blast.AddComponent<Blast>();
                blastScript.spawnUp = (i % 2 != 0);
                blastScript.blastNumber = i;
                blastScript.sourcePos = hcPos;
                blast.SetActive(true);
            }
        }
    }
}
