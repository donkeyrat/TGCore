using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TGCore.Library;

public class HideRenderers : MonoBehaviour
{
    private RendererHandler Rendering;
    
    private void Start()
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
        private Renderer[] BodyRenderers;
        private Renderer[] WeaponRenderers;
        private bool ShowBodyRenderers = true;
        private bool ShowWeaponRenderers = true;
        
        public Dictionary<HideRenderers, bool> shouldHideBodyDict = new Dictionary<HideRenderers, bool>();
        
        public Dictionary<HideRenderers, bool> shouldHideWeaponsDict = new Dictionary<HideRenderers, bool>();
    
        private void Start()
        {
            BodyRenderers = GetComponentsInChildren<Renderer>()
                .Where(x => x.enabled && !x.GetComponentInParent<Weapon>() && !(x is ParticleSystemRenderer)).ToArray();
            WeaponRenderers = GetComponentsInChildren<Renderer>()
                .Where(x => x.enabled && x.GetComponentInParent<Weapon>() && !(x is ParticleSystemRenderer)).ToArray();
        }

        public void UpdateHidden()
        {
            var showBody = true;
            foreach (var item in shouldHideBodyDict.Where(item => !item.Value))
            {
                Debug.Log("Wow this body is hidden....");
                showBody = false;
            }
            if (showBody != ShowBodyRenderers) IsHidingBody(showBody);
            
            var showWeapon = true;
            foreach (var item in shouldHideWeaponsDict.Where(item => !item.Value))
            {
                Debug.Log("Wow this weapon is hidden....");
                showWeapon = false;
            }
            if (showWeapon != ShowWeaponRenderers) IsHidingWeapons(showWeapon);
        }

        public void IsHidingBody(bool value)
        {
            Debug.Log("Updating body hiding...: " + value);
            ShowBodyRenderers = value;
            foreach (var render in BodyRenderers)
                if (render)
                {
                    render.enabled = value;
                }
        }
        
        public void IsHidingWeapons(bool value)
        {
            Debug.Log("Updating weapon hiding...: " + value);
            ShowWeaponRenderers = value;
            foreach (var render in WeaponRenderers)
                if (render)
                {
                    render.enabled = value;
                }
        }
    }
}