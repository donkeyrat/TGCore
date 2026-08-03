using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class SendRootUnitToHell : MonoBehaviour
    {
        private CameraAbilityPossess Possess;
        private PossesionCamera PossessionCamera;
        
        private void Start()
        {
            Possess = MainCam.instance.GetComponentInParent<CameraAbilityPossess>();
            PossessionCamera = transform.root.GetComponentInChildren<PossesionCamera>();
        }
        
        public void Banish()
        {
            var unit = transform.root.GetComponent<Unit>();
            if (!unit) return;

            unit.data.healthHandler.Die();
            
            if (Possess && Possess.currentUnit == unit) Possess.ExitUnit();
            
            foreach (var joint in unit.GetComponentsInChildren<ConfigurableJoint>())
            {
                Destroy(joint);
            }
            
            unit.transform.position = Vector3.down * 1000f;
        }
        
        public void Ascend()
        {
            var unit = transform.root.GetComponent<Unit>();
            if (!unit) return;

            foreach (var rig in unit.GetComponentsInChildren<Rigidbody>())
            {
                rig.position += Vector3.up * 100f;
            }
            if (Possess && Possess.currentUnit == unit) Possess.ExitUnit();
            unit.SetHealthBarActive(false);
            if (PossessionCamera) PossessionCamera.gameObject.SetActive(false);
        }

        public void EnablePossession()
        {
            if (PossessionCamera) PossessionCamera.gameObject.SetActive(true);
            
            var unit = transform.root.GetComponent<Unit>();
            if (unit && (!unit.data.Dead || unit.data.healthHandler.willBeRewived)) unit.SetHealthBarActive(true);
        }
    }
}