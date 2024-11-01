using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library 
{
    public class ProjectileLauncherShowProjectile : MonoBehaviour 
    {
        private void Start()
        {
            RangeWeapon = GetComponent<RangeWeapon>();
            
            Spawned = Instantiate(GetComponent<ProjectileLauncher>().objectToSpawn, pivot.position, pivot.rotation, pivot);

            var idle = Spawned.GetComponentInChildren<ArrowIdlePosition>();
            if (idle) Spawned.transform.localPosition = idle.transform.localPosition;
            Spawned.transform.localPosition += offset;
            
            if (RangeWeapon && RangeWeapon.connectedData != null)
            {
                var componentsInChildren = Spawned.GetComponentsInChildren<Renderer>(); 
                RangeWeapon.connectedData.unit.AddRenderersToShowHide(componentsInChildren, false);
            }

            foreach (var rig in Spawned.GetComponentsInChildren<Rigidbody>())
            {
                rig.isKinematic = true;
                if (rig.GetComponent<Joint>()) Destroy(rig.GetComponent<Joint>());
                Destroy(rig);
            }
            
            foreach (var mono in Spawned.GetComponentsInChildren<MonoBehaviour>()) 
            {
                if (!(mono is SetTeamColorOnStart) && !(mono is TeamColor)) Destroy(mono);
            }
            
            Spawned.transform.localScale = Vector3.one;
            
            foreach (var particle in Spawned.GetComponentsInChildren<ParticleSystem>()) 
            {
                var idleParticle = particle.GetComponent<IdleBowParticle>();
                if (idleParticle) particle.Play();
                else Destroy(particle);
            }
        }

        private GameObject Spawned;

        private RangeWeapon RangeWeapon;

        public Transform pivot;

        public Vector3 offset;
    }
}