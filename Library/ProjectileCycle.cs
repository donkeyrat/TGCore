using System.Collections.Generic;
using UnityEngine;

namespace TGCore.Library
{
    public class ProjectileCycle : MonoBehaviour
    {
        private void Start()
        {
            RangeWeapon = GetComponent<RangeWeapon>();
            Launcher = GetComponent<ProjectileLauncher>();
            Projectiles = projectilesToCycle;
        }
        
        public void Switch()
        {
            switch (cyclingMode)
            {
                case CycleMode.Random:
                {
                    var chosenProjectile = Projectiles[Random.Range(0, Projectiles.Count)];
                    if (RangeWeapon) RangeWeapon.ObjectToSpawn = chosenProjectile;
                    if (Launcher) Launcher.objectToSpawn = chosenProjectile;
                    
                    break;
                }
                case CycleMode.BiasedRandom:
                {
                    var chosenIndex = Random.Range(0, Projectiles.Count);
                    if (RangeWeapon) RangeWeapon.ObjectToSpawn = Projectiles[chosenIndex];
                    if (Launcher) Launcher.objectToSpawn = Projectiles[chosenIndex];

                    var lastRemovedProjectile = projectilesToCycle[LastRemovedIndex];
                    
                    if (!Projectiles.Contains(lastRemovedProjectile)) Projectiles.Add(lastRemovedProjectile);
                    LastRemovedIndex = chosenIndex;
                    Projectiles.Remove(projectilesToCycle[chosenIndex]);
                    
                    break;
                }
                case CycleMode.Cycle:
                {
                    if (RangeWeapon) RangeWeapon.ObjectToSpawn = Projectiles[CurrentIndex];
                    if (Launcher) Launcher.objectToSpawn = Projectiles[CurrentIndex];
                    
                    CurrentIndex++;
                    if (CurrentIndex == Projectiles.Count) CurrentIndex = 0;
                    
                    break;
                }
            }
        }

        private int CurrentIndex;
        private int LastRemovedIndex;
        private RangeWeapon RangeWeapon;
        private ProjectileLauncher Launcher;
        private List<GameObject> Projectiles;

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