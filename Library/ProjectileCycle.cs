using System.Collections.Generic;
using UnityEngine;

namespace TGCore.Library
{
    public class ProjectileCycle : MonoBehaviour
    {
        private void Start()
        {
            rangeWeapon = GetComponent<RangeWeapon>();
            launcher = GetComponent<ProjectileLauncher>();
            projectiles = projectilesToCycle;
        }
        
        public void Switch()
        {
            switch (cyclingMode)
            {
                case CycleMode.Random:
                {
                    var chosenProjectile = projectiles[Random.Range(0, projectiles.Count)];
                    if (rangeWeapon) rangeWeapon.ObjectToSpawn = chosenProjectile;
                    if (launcher) launcher.objectToSpawn = chosenProjectile;
                    
                    break;
                }
                case CycleMode.BiasedRandom:
                {
                    var chosenIndex = Random.Range(0, projectiles.Count);
                    if (rangeWeapon) rangeWeapon.ObjectToSpawn = projectiles[chosenIndex];
                    if (launcher) launcher.objectToSpawn = projectiles[chosenIndex];

                    var lastRemovedProjectile = projectilesToCycle[lastRemovedIndex];
                    
                    if (!projectiles.Contains(lastRemovedProjectile)) projectiles.Add(lastRemovedProjectile);
                    lastRemovedIndex = chosenIndex;
                    projectiles.Remove(projectilesToCycle[chosenIndex]);
                    
                    break;
                }
                case CycleMode.Cycle:
                {
                    if (rangeWeapon) rangeWeapon.ObjectToSpawn = projectiles[currentIndex];
                    if (launcher) launcher.objectToSpawn = projectiles[currentIndex];
                    
                    currentIndex++;
                    if (currentIndex == projectiles.Count) currentIndex = 0;
                    
                    break;
                }
            }
        }

        private int currentIndex;
        private int lastRemovedIndex;
        private RangeWeapon rangeWeapon;
        private ProjectileLauncher launcher;
        private List<GameObject> projectiles;

        public List<GameObject> projectilesToCycle;

        public enum CycleMode
        {
            Random,
            BiasedRandom,
            Cycle
        }
        public CycleMode cyclingMode;
    }
}