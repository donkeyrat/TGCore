using UnityEngine;
using Landfall.TABS;

namespace TGCore.Library
{
    public class KillSelf : MonoBehaviour
    {
        public void KillYourself()
        {
            if (transform.root.GetComponent<Unit>()) transform.root.GetComponent<Unit>().data.healthHandler.Die();
            if (destroy) transform.root.GetComponent<Unit>().DestroyUnit();
        }

        public bool destroy;
    }
}