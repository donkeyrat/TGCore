using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class SendRootUnitToHell : MonoBehaviour
    {
        public void Banish()
        {
            var unit = transform.root.GetComponent<Unit>();
            if (!unit) return;

            unit.data.healthHandler.Die();
            
            foreach (var joint in unit.GetComponentsInChildren<ConfigurableJoint>())
            {
                Destroy(joint);
            }
            
            unit.transform.position = Vector3.down * 1000f;
        }
    }
}