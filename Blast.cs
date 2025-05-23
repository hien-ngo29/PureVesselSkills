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
    public class Blast : MonoBehaviour
    {
        public bool spawnUp = false;
        public int blastNumber = 0;
        private HeroController hc => HeroController.instance;
        private const int Damage = 80;

        private void Awake()
        {
            SetBlastPosition();
            Destroy(gameObject.FindGameObjectInChildren("hero_damager"));
            MakeBlastDamageableToEnemy();
        }

        private void SetBlastPosition()
        {
            Vector3 pos = gameObject.transform.position;
            pos.x = (hc.transform.position.x - 16) + (8 * blastNumber) + Random.Range(-1.5f, 1.5f);
            pos.y = hc.transform.position.y + (spawnUp ? Random.Range(11.88f, 14.08f) : Random.Range(7.88f, 10.08f)) - 7f;

            blastNumber++;
            gameObject.transform.position = pos;
        }

        private void MakeBlastDamageableToEnemy()
        {
            DamageEnemies damageEnemies = gameObject.AddComponent<DamageEnemies>();
            damageEnemies.ignoreInvuln = false;
            damageEnemies.attackType = AttackTypes.Spell;
            damageEnemies.direction = 2;
            damageEnemies.damageDealt = Damage;
        }
    }
}
