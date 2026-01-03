using System.Reflection;
using Landfall.TABS.GameMode;
using UnityEngine;
using HarmonyLib;
using TFBGames;
using TGCore;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(UnitColorHandler), "SetMaterial")]
    internal class UnitColorHandlerPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(UnitColorHandler __instance, ref Material material, ref Renderer[] ___rends, ref bool ___initiated)
        {
            if (!___initiated)
            {
                //Init();
                __instance.InvokeMethod("Init");
            }

            __instance.SetField("isDone", true);
            
            //isDone = true;
            for (int i = 0; i < ___rends.Length; i++)
            {
                if (___rends[i] == null) continue;
			
                Material[] materialsNonAlloc = ___rends[i].GetMaterialsNonAlloc();
                for (int j = 0; j < materialsNonAlloc.Length; j++)
                {
                    Material material2 = materialsNonAlloc[j];
                    if (material2.HasProperty("_BumpMap"))
                    {
                        Texture texture = material2.GetTexture("_BumpMap");
                        if ((bool)texture)
                        {
                            Material material3 = new Material(material);
                            material3.SetTexture("_BumpMap", texture);
                            material3.EnableKeyword("_NORMALMAP");
                            materialsNonAlloc[j] = material3;
                            continue;
                        }
                    }
                    materialsNonAlloc[j] = material;
                }
                ___rends[i].materials = materialsNonAlloc;
            }

            __instance.SetField("rends", ___rends);
            return false;
        }
    }
}