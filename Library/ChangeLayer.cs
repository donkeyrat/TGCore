using UnityEngine;

namespace TGCore.Library;

public class ChangeLayer : MonoBehaviour
{
    public void DoChange(int newLayer)
    {
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.gameObject.layer = newLayer;
        }
    }
}