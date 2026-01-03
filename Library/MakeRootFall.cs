using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class MakeRootFall : MonoBehaviour
    {
        private void Start()
        {
            OwnUnit = transform.root.GetComponent<Unit>();
        }
        
        public void Fall(float time)
        {
            if (!OwnUnit) OwnUnit = transform.root.GetComponent<Unit>();
            if (OwnUnit.data.immunityForSeconds > 0f) return;

            var output = time * healthDependentMultiplier / OwnUnit.data.health;

            if (!dependsOnHealth) output = time;
            
            OwnUnit.data.fallTime = Mathf.Clamp(baseTime + output, 0f, maxTime);
        }

        private Unit OwnUnit;

        public float baseTime = 1f;
        public float maxTime = 5f;
        public bool dependsOnHealth;
        public float healthDependentMultiplier = 100f;
    }
}