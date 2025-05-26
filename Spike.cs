using System.Collections;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using UnityEngine;
using SFCore;
using FrogCore.Fsm;

namespace PureVesselSkills
{
    public class Spike : MonoBehaviour
    {
        public float angle { get; set; }

        private float dx;
        private float dy;

        private Rigidbody2D rb;
        private const int SpikeDamage = 40;
        private const float MovingSpeed = 1350f;

        private void Awake()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            rb.isKinematic = true;

            DamageEnemies damageEnemies = gameObject.AddComponent<DamageEnemies>();
            damageEnemies.ignoreInvuln = false;
            damageEnemies.direction = 4;
            damageEnemies.attackType = AttackTypes.Spell;
            damageEnemies.damageDealt = SpikeDamage;

            StartCoroutine(WaitAndDestroy());
        }

        private void Update()
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        void FixedUpdate()
        {
            float radians = (angle + 90) * Mathf.Deg2Rad;
            dx = Mathf.Cos(radians) * MovingSpeed * Time.deltaTime;
            dy = Mathf.Sin(radians) * MovingSpeed * Time.deltaTime;
            rb.velocity = new Vector2(dx, dy);
        }

        private IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
    }
}
