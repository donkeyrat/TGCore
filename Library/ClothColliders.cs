using UnityEngine;

namespace TGCore.Library;

public class ClothColliders : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Cloth>().capsuleColliders = transform.root.GetComponentsInChildren<CapsuleCollider>();
    }
}