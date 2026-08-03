using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TGCore.Library;

public class StuckProjectilesList : MonoBehaviour
{
    public List<ProjectileStick> sticks = new List<ProjectileStick>();

    [Button]
    public void UnStickAll()
    {
        foreach (var stick in sticks.Where(stick => stick && stick.targetRig && stick.targetRig.transform.root == transform))
        {
            stick.target = null;
            stick.targetRig = null;
        }
        sticks.Clear();
    }
}