using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library 
{
    public class ProjectileLauncherShowProjectile : MonoBehaviour 
    {
        private void Start() 
        {
            rangeWeapon = GetComponent<RangeWeapon>();
            
            spawned = Instantiate(GetComponent<ProjectileLauncher>().objectToSpawn, pivot.position, pivot.rotation, pivot);

            var idle = spawned.GetComponentInChildren<ArrowIdlePosition>();
            if (idle) spawned.transform.localPosition = idle.transform.localPosition;
            spawned.transform.localPosition += offset;
            
            if (rangeWeapon && rangeWeapon.connectedData != null)
            {
                var componentsInChildren = spawned.GetComponentsInChildren<Renderer>(); 
                rangeWeapon.connectedData.unit.AddRenderersToShowHide(componentsInChildren, GetComponent<ShowProjectile>().IsInBlindGame);
            }

            foreach (var rig in spawned.GetComponentsInChildren<Rigidbody>())
            {
                rig.isKinematic = true;
                if (rig.GetComponent<Joint>()) Destroy(rig.GetComponent<Joint>());
                Destroy(rig);
            }
            
            foreach (var mono in spawned.GetComponentsInChildren<MonoBehaviour>()) 
            {
                if (!(mono is SetTeamColorOnStart) && !(mono is TeamColor)) Destroy(mono);
            }
            
            spawned.transform.localScale = Vector3.one;
            
            foreach (var particle in spawned.GetComponentsInChildren<ParticleSystem>()) 
            {
                var idleParticle = particle.GetComponent<IdleBowParticle>();
                if (idleParticle) particle.Play();
                else Destroy(particle);
            }
        }

        private GameObject spawned;

        private RangeWeapon rangeWeapon;

        public Transform pivot;

        public Vector3 offset;
    }
}