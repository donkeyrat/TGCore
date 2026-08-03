using UnityEngine;
using Landfall.TABS;

namespace TGCore.Library
{
    public class KillSelf : MonoBehaviour
    {
        public void KillYourself()
        {
            var unit = transform.root.GetComponent<Unit>();
            if (unit)
            {
                unit.data.healthHandler.TakeDamage(1000000f, Vector3.zero);
                unit.data.healthHandler.Die();
                if (destroy) unit.DestroyUnit();
            }
        }

        public bool destroy;
    }
}