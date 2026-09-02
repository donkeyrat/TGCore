using System.Collections.Generic;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class AddRigsToAllRigs : MonoBehaviour
{
    private void Start()
    {
        var unit = transform.root.GetComponent<Unit>();

        var rigs = new List<Rigidbody>(unit.data.allRigs.AllRigs);
        var drags = new List<Vector2>(unit.data.allRigs.AllDrags);
        var defaultDrags = new List<Vector2>(unit.data.allRigs.defaultDrags);
        foreach (var child in GetComponentsInChildren<Rigidbody>())
        {
            rigs.Add(child);
            drags.Add(new Vector2(child.drag, child.angularDrag));
            defaultDrags.Add(new Vector2(child.drag, child.angularDrag));
        }
        unit.data.allRigs.SetField("allRigs", rigs.ToArray());
        unit.data.allRigs.SetField("allDrags", drags.ToArray());
        unit.data.allRigs.defaultDrags = defaultDrags.ToArray();
    }
}