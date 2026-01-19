using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using HutongGames.PlayMaker.Actions;

namespace PureVesselSkills
{
    // Handle the blasts that spawn after you healed
    public class ChainedBlastsSpawner
    {
        private int numberOfBlasts { get; set; }

        public ChainedBlastsSpawner()
        {
            numberOfBlasts = 5;
        }

        public IEnumerator SpawnChainedBlasts()
        {
            for (int i = 0; i < numberOfBlasts; i++)
            {
                yield return new WaitForSeconds(Random.Range(0f, 0.45f));
                SpawnBlastAtRandomYPlaceFromOrder(i);
            }
        }

        private void SpawnBlastAtRandomYPlaceFromOrder(int indexOrder)
        {
            Vector3 spawnPos = GetRandomYPositionFromOrder(indexOrder);

            BlastBubble blastBubble = new BlastBubble();
            blastBubble.SetPosition(spawnPos);
            blastBubble.Activate();
        }

        private Vector3 GetRandomYPositionFromOrder(int indexOrder)
        {
            Vector3 pos = new();
            Vector3 heroPos = HeroController.instance.gameObject.transform.position;
            pos.x = heroPos.x - 20 + (8 * indexOrder) + Random.Range(-1.5f, 1.5f);
            pos.y = heroPos.y + (indexOrder % 2 != 0 ? Random.Range(11.88f, 14.08f) : Random.Range(7.88f, 10.08f)) - 7f;

            return pos;
        }
    }
}