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
            if (GameObject.Find("FocusAttack"))
                return;

            GameObject obj = new GameObject("FocusAttack");
            DontDestroyOnLoad(obj);
            obj.AddComponent<FocusAttack>();
        }

        private void Awake()
        {
            spellControl = hc.spellControl;

            AddAttackToFSM();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SpawnFocusBlast();
            }
        }

        private void AddAttackToFSM()
        {
            FrogCore.Fsm.FsmUtil.InsertCoroutine(spellControl, "Focus Heal", 16, SpawnBlast);
        }

        private void SpawnFocusBlast()
        {
            GameObject focusBlast = Instantiate(PureVesselSkills.preloadedGO["FocusBlast"], hc.transform.position, Quaternion.identity);

            focusBlast.SetActive(true);
        }

        private IEnumerator SpawnBlast()
        {
            for (int i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(Random.Range(0f, 0.45f));

                GameObject blast = Instantiate(PureVesselSkills.preloadedGO["Blast"]);
                Blast blastScript = blast.AddComponent<Blast>();
                blastScript.spawnUp = (i % 2 != 0);
                blastScript.blastNumber = i;
                blast.SetActive(true);
            }
        }
    }
}
