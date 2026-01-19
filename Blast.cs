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
    public class BlastBubble: PVAttackObject
    {
        protected override void AddAttackCoreToGameObject()
        {
            gameObject = GameObject.Instantiate(PureVesselSkills.preloadedGO["Blast"]);
            gameObject.AddComponent<BlastBubbleCore>();
        }
    }

    public class BlastBubbleCore : MonoBehaviour
    {
        private AudioClip blastSound;
        private AudioSource audioSource;

        private const int Damage = 80;
        private Animator anim;

        private void Awake()
        {
            ConfigureAnimator();

            blastSound = SoundClips.blastAudio;
            audioSource = HeroController.instance.GetComponent<AudioSource>();

            Destroy(gameObject.FindGameObjectInChildren("hero_damager"));
            AddDamageEnemiesComponentToBlast();

            StartCoroutine(WaitAndAddHitboxDamage());
            StartCoroutine(WaitAndDestroy());
        }

        public void SetBlastActive()
        {
            gameObject.SetActive(true);
        }

        private void ConfigureAnimator()
        {
            anim = gameObject.GetComponent<Animator>();
            anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
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
