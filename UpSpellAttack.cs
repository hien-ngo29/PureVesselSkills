using System.Collections;
using System.Runtime.CompilerServices;
using IL.tk2dRuntime;
using Satchel;
using UnityEngine;

namespace PureVesselSkills
{
    public class UpSpellAttack : MonoBehaviour
    {
        const float BLASTWIDTH = 3f;
        const float ANIMATION_SPEED = 2f;

        private PlayMakerFSM spellControl;

        public static void Init()
        {
            GameObject obj = new GameObject("UpSpellAttack");
            DontDestroyOnLoad(obj);
            obj.AddComponent<UpSpellAttack>();
        }

        public static void DestroySelf()
        {
            Destroy(GameObject.Find("UpSpellAttack"));
        }

        private void Awake()
        {
            spellControl = HeroController.instance.spellControl;
            RemoveOrigUpSpell();
            AddPVUpSpell();
        }

        private void RemoveOrigUpSpell()
        {
            spellControl.RemoveAction("Scream Burst 2", 5);
            // spellControl.RemoveAction("Scream Burst 3", 7);
        }

        private void AddPVUpSpell()
        {
            
        }

        void Update()
        {
        }

        private IEnumerator SpawnBlastsSeries()
        {
            SpawnBlastInFirstRow();
            yield return new WaitForSeconds(0.05f);
            SpawnBlastInSecondRow();
            yield return new WaitForSeconds(0.05f);
            SpawnBlastInThirdRow();
        }

        private void SpawnBlastInFirstRow()
        {
            var hcPos = HeroController.instance.gameObject.transform.position;
            BlastBubble blast = new();
            var pos = hcPos;
            pos.x = hcPos.x;
            pos.y = hcPos.y + BLASTWIDTH;
            blast.SetPosition(pos);
            blast.SetAnimationSpeedScale(ANIMATION_SPEED);
            blast.Activate();
        }

        private void SpawnBlastInSecondRow()
        {
            var hcPos = HeroController.instance.gameObject.transform.position;
            var pos = hcPos;
            for (int i = 0; i < 2; i++)
            {
                BlastBubble blast = new();
                pos.x = hcPos.x + BLASTWIDTH * (i - 0.5f);
                pos.y = hcPos.y + (BLASTWIDTH * 2);
                blast.SetPosition(pos);
                blast.SetAnimationSpeedScale(ANIMATION_SPEED);
                blast.Activate();
            }
        }

        private void SpawnBlastInThirdRow()
        {
            var hcPos = HeroController.instance.gameObject.transform.position;
            var pos = hcPos;
            for (int i = 0; i < 3; i++)
            {
                BlastBubble blast = new();
                pos.x = hcPos.x + BLASTWIDTH * (i - 1);
                pos.y = hcPos.y + (BLASTWIDTH * 3);
                blast.SetPosition(pos);
                blast.SetAnimationSpeedScale(ANIMATION_SPEED);
                blast.Activate();
            }
        }
    }
}