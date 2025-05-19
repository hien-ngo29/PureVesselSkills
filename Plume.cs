using System.Collections;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using SFCore;
using FrogCore.Fsm;

namespace PureVesselSkills
{
    public class Plume : MonoBehaviour
    {
        // Used for raycast check
        private const float Extension = 0.01f;
        private const int CollisionMask = 1 << 8;

        private PlayMakerFSM _fsm;
        public bool upgraded;

        private const int PlumeDamage = 7;
        private const int PlumeDamageShaman = 11;
        private const float PlumeTime = 0.4f;
        private const float PlumeTimeUpgraded = 0.7f;

        private void Awake()
        {
            if (!IsGrounded()) Destroy(gameObject);

            _fsm = gameObject.LocateMyFSM("FSM");
            _fsm.GetAction<Wait>("Antic", 2).time.Value = 0.25f;
            _fsm.GetAction<Wait>("End", 0).time.Value = 0.5f;
            FsmUtil.InsertCoroutine(_fsm, "Plume 2", 0, AnimControl);
            _fsm.GetAction<FloatCompare>("Outside Arena?", 2).float2.Value = Mathf.Infinity;
            _fsm.GetAction<FloatCompare>("Outside Arena?", 3).float2.Value = -Mathf.Infinity;
        }

        private void Start()
        {
            DamageEnemies damageEnemies = gameObject.AddComponent<DamageEnemies>();
            damageEnemies.ignoreInvuln = false;
            damageEnemies.attackType = AttackTypes.Spell;
            damageEnemies.direction = 2;
            damageEnemies.damageDealt = PlayerData.instance.equippedCharm_19 ? PlumeDamageShaman : PlumeDamage;
        }

        private IEnumerator AnimControl()
        {
            yield return new WaitForSeconds(0.1f);
            gameObject.GetComponent<tk2dSpriteAnimator>().enabled = false;
            yield return new WaitForSeconds(upgraded ? PlumeTimeUpgraded : PlumeTime);
            gameObject.GetComponent<tk2dSpriteAnimator>().enabled = true;
        }

        private bool IsGrounded()
        {
            float rayLength = 1.0f + Extension;
            Vector2 pos = transform.position;
            return Physics2D.Raycast(pos + Vector2.up, Vector2.down, rayLength, CollisionMask);
        }
    }
}
