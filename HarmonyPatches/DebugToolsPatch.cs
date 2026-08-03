using System.Reflection;
using System;
using UnityEngine;
using HarmonyLib;
using TFBGames;
using System.Collections.Generic;
using DM;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(CameraSpawnObject), "Start")]
    internal class DebugToolsPatch 
    {
        [HarmonyPrefix]
        public static bool Prefix(CameraSpawnObject __instance)
        {
            var newObjectsToSpawn = new List<GameObject>();
            var newSounds = new List<string>();
            foreach (var proj in ContentDatabase.Instance().GetAllProjectiles()) { 
                if (proj != null && !proj.name.ToUpper().Contains("STORM")) { 
                    newObjectsToSpawn.Add(proj); 
                    newSounds.Add("Medieval Attacks/Bow"); 
                } 
            }
            __instance.soundToPlay = newSounds.ToArray();
            __instance.objectsToSpawn = newObjectsToSpawn.ToArray();

            return true;
        }
    }
}