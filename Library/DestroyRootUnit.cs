using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class DestroyRootUnit : MonoBehaviour
    {
        private void Start()
        {
            Unit = transform.root.GetComponent<Unit>();
        }

        public void DoDestruction()
        {
            Unit?.DestroyUnit();
        }

        private Unit Unit;
    }
}