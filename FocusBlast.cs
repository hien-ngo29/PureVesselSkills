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
    public class FocusBlast : MonoBehaviour
    {
        private AudioSource audioSource;
        private AudioClip ballUpSound;
        private AudioClip completeSound;

        private void Awake()
        {
            PreloadAudio();
            audioSource.PlayOneShot(ballUpSound, 1f);

            gameObject.name = "Hero Focus Blast";
            AddDamageEnemiesComponentToBlast();
            StartCoroutine(WaitAndAddHitboxDamage());
            StartCoroutine(WaitAndDestroy());
        }

        private void PreloadAudio()
        {
            PlayMakerFSM pvControl = PureVesselSkills.preloadedGO["PV"].LocateMyFSM("Control");
            ballUpSound = (AudioClip)pvControl.GetAction<AudioPlayerOneShotSingle>("Ball Up", 2).audioClip.Value;
            completeSound = (AudioClip)pvControl.GetAction<AudioPlayerOneShotSingle>("Focus Burst", 8).audioClip.Value;

            audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void AddDamageEnemiesComponentToBlast()
        {
            DamageEnemies damageEnemies = gameObject.AddComponent<DamageEnemies>();
            damageEnemies.ignoreInvuln = false;
            damageEnemies.attackType = AttackTypes.Spell;
            damageEnemies.direction = 2;
            damageEnemies.damageDealt = 60;
        }

        private IEnumerator WaitAndAddHitboxDamage()
        {
            yield return new WaitForSeconds(0.891f);

            audioSource.PlayOneShot(completeSound, 1f);

            CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.offset = Vector2.down;
            col.radius = 10f;

            for (int i = 0; i < 10; i++)
                yield return null;

            Destroy(col);
        }

        private IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(2.5f);
            Destroy(gameObject);
        }

        public static void DestroySelf()
        {
            Destroy(GameObject.Find("Hero Focus Blast"));
        }
    }
}
