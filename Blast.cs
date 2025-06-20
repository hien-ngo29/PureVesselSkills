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
        public Vector3 sourcePos { get; set; }

        public AudioClip blastSound;
        private AudioSource audioSource;

        private const int Damage = 80;
        private Animator anim;

        private void Awake()
        {
            anim = gameObject.GetComponent<Animator>();
            anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            audioSource = gameObject.AddComponent<AudioSource>();

            SetBlastPosition();
            Destroy(gameObject.FindGameObjectInChildren("hero_damager"));
            AddDamageEnemiesComponentToBlast();

            StartCoroutine(WaitAndAddHitboxDamage());
            StartCoroutine(WaitAndDestroy());
        }

        private IEnumerator WaitAndAddHitboxDamage()
        {
            yield return new WaitForSeconds(0.858f);

            PlayBlastSound();

            CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.offset = Vector2.down;
            col.radius = 4f;

            for (int i = 0; i < 10; i++)
                yield return null;

            Destroy(col);
        }

        private void PlayBlastSound()
        {
            audioSource.pitch = Random.Range(1f, 1.25f);
            audioSource.PlayOneShot(blastSound, 1f);
        }

        private void SetBlastPosition()
        {
            Vector3 pos = gameObject.transform.position;
            pos.x = (sourcePos.x - 20) + (8 * blastNumber) + Random.Range(-1.5f, 1.5f);
            pos.y = sourcePos.y + (spawnUp ? Random.Range(11.88f, 14.08f) : Random.Range(7.88f, 10.08f)) - 7f;

            blastNumber++;
            gameObject.transform.position = pos;
        }

        private void AddDamageEnemiesComponentToBlast()
        {
            DamageEnemies damageEnemies = gameObject.AddComponent<DamageEnemies>();
            damageEnemies.ignoreInvuln = false;
            damageEnemies.attackType = AttackTypes.Spell;
            damageEnemies.direction = 2;
            damageEnemies.damageDealt = Damage;
        }

        private IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(4f);
            Destroy(gameObject);
        }
    }
}
