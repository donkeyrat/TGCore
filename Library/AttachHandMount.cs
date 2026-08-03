using UnityEngine;

namespace TGCore.Library;

public class AttachHandMount : MonoBehaviour
{
    public Rigidbody rigToHold;
    public Transform pivot;

    public bool attachLeftHand;
    public bool attachRightHand;
}