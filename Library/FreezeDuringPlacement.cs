using Landfall.TABS.GameState;
using UnityEngine;

namespace TGCore.Library
{
    public class FreezeDuringPlacement : MonoBehaviour
    {
        private void Start()
        {
            GameStateManager = ServiceLocator.GetService<GameStateManager>();
            Rig = GetComponent<Rigidbody>();
        }
        
        private void Update()
        {
            if (!Rig.isKinematic && GameStateManager.GameState == GameState.PlacementState)
            {
                Rig.isKinematic = true;
            }
            else if (GameStateManager.GameState != GameState.PlacementState)
            {
                Rig.isKinematic = false;
                Destroy(this);
            }
        }

        private GameStateManager GameStateManager;
        private Rigidbody Rig;
    }
}