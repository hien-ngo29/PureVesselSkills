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
        HeroController hc = HeroController.instance;
        PlayMakerFSM spellControl;

        void Awake()
        {
            spellControl = hc.spellControl;

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

            ModifySpellFSM();
        }

        void ModifySpellFSM()
        {
            if (enabled)
            {
                spellControl.ChangeTransition("Level Check 3", "LEVEL 1", "Scream Antic1 Blasts");
                spellControl.ChangeTransition("Level Check 3", "LEVEL 2", "Scream Antic2 Blasts");

                spellControl.ChangeTransition("Quake1 Down", "HERO LANDED", "Q1 Land Plumes");
                spellControl.ChangeTransition("Quake2 Down", "HERO LANDED", "Q2 Land Plumes");

                if (!PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_11)))
                {
                    spellControl.ChangeTransition("Level Check", "LEVEL 1", "Fireball 1 SmallShots");
                    spellControl.ChangeTransition("Level Check", "LEVEL 2", "Fireball 2 SmallShots");
                }
            }
            else
            {
                spellControl.ChangeTransition("Level Check 3", "LEVEL 1", "Scream Antic1");
                spellControl.ChangeTransition("Level Check 3", "LEVEL 2", "Scream Antic2");

                spellControl.ChangeTransition("Quake1 Down", "HERO LANDED", "Quake1 Land");
                spellControl.ChangeTransition("Quake2 Down", "HERO LANDED", "Q2 Land");

                spellControl.ChangeTransition("Level Check", "LEVEL 1", "Fireball 1");
                spellControl.ChangeTransition("Level Check", "LEVEL 2", "Fireball 2");
            }
        }

        void Update()
        {

        }

        public void CastPlumes(bool upgraded)
        {
            for (float x = 2; x <= 14; x += 3)
            {
                Vector2 pos = HeroController.instance.transform.position;
                float plumeY = pos.y - 2.2f;

                GameObject plumeL = Instantiate(PureVesselSkills.preloadedGO["Plume"], new Vector2(pos.x - x, plumeY), Quaternion.identity);
                plumeL.SetActive(true);
                plumeL.AddComponent<Plume>().upgraded = upgraded;

                GameObject plumeR = Instantiate(PureVesselSkills.preloadedGO["Plume"], new Vector2(pos.x + x, plumeY), Quaternion.identity);
                plumeR.SetActive(true);
                plumeR.AddComponent<Plume>().upgraded = upgraded;
            }
        }
    }
}
