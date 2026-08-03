using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TGCore.Library;

public class ChangeLayerOfChildren : MonoBehaviour
{
    private RigidbodyHolder RigHolder;
    private List<Rigidbody> Rigs = new List<Rigidbody>();
    private List<int> Layers = new List<int>();
    private List<int> ColliderLayers = new List<int>();
    private List<Collider> Colliders = new List<Collider>();
    private bool ChangedLayer;
    
    public bool includeWeapons = true;

    public void Start()
    {
        Rigs.Clear();
        Colliders.Clear();
        ColliderLayers.Clear();
        RigHolder = transform.root.GetComponentInChildren<RigidbodyHolder>();
        Rigs.AddRange(RigHolder.AllRigs);
        Colliders.AddRange(transform.root.GetComponentInChildren<DataHandler>().transform.GetComponentsInChildren<Collider>());
        if (includeWeapons)
        {
            var holdingHandler = transform.root.GetComponentInChildren<HoldingHandler>();
            var holdingHandlerMulti = transform.root.GetComponentInChildren<HoldingHandlerMulti>();
            if (holdingHandler)
            {
                if (holdingHandler.rightObject)
                {
                    Rigs.Add(holdingHandler.rightObject.rig);
                    Colliders.AddRange(holdingHandler.rightObject.GetComponentsInChildren<Collider>());
                }
                if (holdingHandler.leftObject)
                {
                    Rigs.Add(holdingHandler.leftObject.rig);
                    Colliders.AddRange(holdingHandler.leftObject.GetComponentsInChildren<Collider>());
                }
            }
            else if (holdingHandlerMulti)
            {
                foreach (var weapon in holdingHandlerMulti.spawnedWeapons)
                {
                    Rigs.Add(weapon.GetComponent<Rigidbody>());
                    Colliders.AddRange(weapon.GetComponentsInChildren<Collider>());
                }
            }
        }
    }

    public void ChangeLayer()
    {
        foreach (var rig in Rigs.Where(rig => rig))
        {
            Layers.Add(rig.gameObject.layer);
            rig.gameObject.layer = 20;
        }

        foreach (var collider in Colliders.Where(collider => collider))
        {
            ColliderLayers.Add(collider.gameObject.layer);
            collider.gameObject.layer = 20;
        }

        ChangedLayer = true;
    }

    public void ResetLayer()
    {
        if (!ChangedLayer) return;
        
        for (var i = 0; i < Rigs.Count; i++)
        {
            if ((bool)Rigs[i])
            {
                Rigs[i].gameObject.layer = Layers[i];
            }
        }
        for (var j = 0; j < Colliders.Count; j++)
        {
            if ((bool)Colliders[j])
            {
                Colliders[j].gameObject.layer = ColliderLayers[j];
            }
        }

        ChangedLayer = false;
    }
}
