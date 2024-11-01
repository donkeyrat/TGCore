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
            if (OwnUnit.data.immunityForSeconds > 0f) return;

            var output = time * healthDependentMultiplier / OwnUnit.data.health;

            if (!dependsOnHealth) output = time;
            
            OwnUnit.data.fallTime = baseTime + output;
        }

        private Unit OwnUnit;

        public float baseTime = 1f;
        public bool dependsOnHealth;
        public float healthDependentMultiplier = 100f;
    }
}