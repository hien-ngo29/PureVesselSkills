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

        void Awake()
        {
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
