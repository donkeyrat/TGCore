using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class HideRenderers : MonoBehaviour
{
    private RendererHandler Rendering;
    
    public void Start()
    {
        var getRendering = transform.root.GetComponent<RendererHandler>();
        if (!getRendering)
        {
            Rendering = transform.root.gameObject.AddComponent<RendererHandler>();
            Rendering.shouldHideBodyDict = new Dictionary<HideRenderers, bool>();
            Rendering.shouldHideWeaponsDict = new Dictionary<HideRenderers, bool>();
        }
        else Rendering = getRendering;
        
        Rendering.shouldHideBodyDict.Add(this, true);
        Rendering.shouldHideWeaponsDict.Add(this, true);
    }
    
    public void HideBody()
    {
        Rendering.shouldHideBodyDict[this] = false;
        Rendering.UpdateHidden();
    }
    
    
    public void HideWeapons()
    {
        Rendering.shouldHideWeaponsDict[this] = false;
        Rendering.UpdateHidden();
    }
    
    public void HideAll()
    {
        Rendering.shouldHideBodyDict[this] = false;
        Rendering.shouldHideWeaponsDict[this] = false;
        Rendering.UpdateHidden();
    }

    public void UnHideBody()
    {
        Rendering.shouldHideBodyDict[this] = true;
        Rendering.UpdateHidden();
    }
    
    
    public void UnHideWeapons()
    {
        Rendering.shouldHideWeaponsDict[this] = true;
        Rendering.UpdateHidden();
    }
    
    public void UnHideAll()
    {
        Rendering.shouldHideBodyDict[this] = true;
        Rendering.shouldHideWeaponsDict[this] = true;
        Rendering.UpdateHidden();
    }

    public class RendererHandler : MonoBehaviour
    {
        private Unit OwnUnit;
        public Renderer[] BodyRenderers;
        public Renderer[] WeaponRenderers;
        private Renderer[] SkinnedRenderers;
        private bool ShowBodyRenderers = true;
        private bool ShowWeaponRenderers = true;
        private bool DisableSkinnedMeshes;
        
        public Dictionary<HideRenderers, bool> shouldHideBodyDict = new Dictionary<HideRenderers, bool>();
        
        public Dictionary<HideRenderers, bool> shouldHideWeaponsDict = new Dictionary<HideRenderers, bool>();
    
        private void Start()
        {
            OwnUnit = GetComponent<Unit>();
            
            var bodyRenderers = GetComponentsInChildren<Renderer>(true)
                .Where(x => ((x.enabled && x.gameObject.activeInHierarchy && x.gameObject.activeSelf)|| x.GetComponentInParent<GooglyEye>()) && !x.GetComponentInParent<Holdable>() && !(x is ParticleSystemRenderer)).ToList();
            
            foreach (var renderer in (List<Renderer>)GetComponent<Unit>().GetField("cachedRenderers"))
            {
                if (!bodyRenderers.Contains(renderer))
                {
                    bodyRenderers.Add(renderer);
                }
            }

            BodyRenderers = bodyRenderers.ToArray();
            
            WeaponRenderers = GetComponentsInChildren<Renderer>(true)
                .Where(x => ((x.enabled && x.gameObject.activeInHierarchy && x.gameObject.activeSelf)|| x.GetComponentInParent<GooglyEye>()) && x.GetComponentInParent<Holdable>() && !(x is ParticleSystemRenderer)).ToArray();
            SkinnedRenderers = OwnUnit.data.GetComponentsInChildren<Renderer>()
                .Where(x => x.enabled && x is SkinnedMeshRenderer).ToArray();
            
            if (GetComponentInChildren<DisableAllSkinnedClothes>())
            {
                DisableSkinnedMeshes = true;
            }
        }

        public void UpdateHidden()
        {
            var showBody = true;
            foreach (var item in shouldHideBodyDict.Where(item => !item.Value))
            {
                showBody = false;
            }
            if (showBody != ShowBodyRenderers) IsHidingBody(showBody);
            
            var showWeapon = true;
            foreach (var item in shouldHideWeaponsDict.Where(item => !item.Value))
            {
                showWeapon = false;
            }
            if (showWeapon != ShowWeaponRenderers) IsHidingWeapons(showWeapon);
        }

        public void IsHidingBody(bool value)
        {
            ShowBodyRenderers = value;
            foreach (var render in BodyRenderers)
            {
                if (render && (!DisableSkinnedMeshes || (DisableSkinnedMeshes && !SkinnedRenderers.Contains(render)) || (DisableSkinnedMeshes && !value) || (DisableSkinnedMeshes && !OwnUnit.data.Dead) || (DisableSkinnedMeshes && OwnUnit.data.Dead && OwnUnit.data.healthHandler.willBeRewived)))
                {
                    render.enabled = value;
                }
            }
        }
        
        public void IsHidingWeapons(bool value)
        {
            ShowWeaponRenderers = value;
            foreach (var render in WeaponRenderers)
            {
                if (render && (!DisableSkinnedMeshes || (DisableSkinnedMeshes && !SkinnedRenderers.Contains(render)) || (DisableSkinnedMeshes && !value)))
                {
                    render.enabled = value;
                }
            }
        }
    }
}