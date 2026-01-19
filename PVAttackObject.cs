using UnityEngine;

namespace PureVesselSkills
{
    public abstract class PVAttackObject
    {
        protected GameObject gameObject;

        protected PVAttackObject()
        {
            AddAttackCoreToGameObject();
        }

        protected abstract void AddAttackCoreToGameObject();

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void SetPosition(Vector3 pos)
        {
            gameObject.transform.position = pos;
        }

        public Vector3 GetPosition()
        {
            return gameObject.transform.position;
        }
    }
}