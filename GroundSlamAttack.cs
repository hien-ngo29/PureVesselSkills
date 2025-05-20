using System;
using System.Collections.Generic;
using UnityEngine;
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

        void Start()
        {
            spellControl = hc.gameObject.LocateMyFSM("Spell Control");

            PlayMakerFSM pvControl = PureVesselSkills.preloadedGO["PV"].LocateMyFSM("Control");

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

            ModifySpellFSM(true);
            InsertSpellEffectsInFsm();
        }

        void Update()
        {
            // Testing if the spike floor works
            if (Input.GetKeyDown(KeyCode.P))
                CastPlumes();
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
                spellControl.ChangeTransition("Quake1 Down", "HERO LANDED", "Quake1 Land");
                spellControl.ChangeTransition("Quake2 Down", "HERO LANDED", "Q2 Land");
            }
        }

        private void InsertSpellEffectsInFsm()
        {
            spellControl.InsertMethod("Q1 Land Plumes", () => hc.GetComponent<GroundSlamAttack>().CastPlumes(), 0);
            spellControl.InsertMethod("Q2 Land Plumes", () => hc.GetComponent<GroundSlamAttack>().CastPlumes(), 0);
        }

        private void CastPlumes()
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
        }
    }
}
