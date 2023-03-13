using Landfall.TABS.GameState;
using UnityEngine;

namespace TGCore.Library
{
    public class FreezeDuringPlacement : MonoBehaviour
    {
        private void Start()
        {
            GameStateManager = ServiceLocator.GetService<GameStateManager>();
            rig = GetComponent<Rigidbody>();
        }
        
        private void Update()
        {
            if (!rig.isKinematic && GameStateManager.GameState == GameState.PlacementState)
            {
                rig.isKinematic = true;
            }
            else if (GameStateManager.GameState != GameState.PlacementState)
            {
                rig.isKinematic = false;
                Destroy(this);
            }
        }

        private GameStateManager GameStateManager;
        private Rigidbody rig;
    }
}