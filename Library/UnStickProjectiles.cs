using UnityEngine;

namespace TGCore.Library;

public class UnStickProjectiles : MonoBehaviour
{
    public void UnStick()
    {
        var stuckProjectiles = transform.root.GetComponent<StuckProjectilesList>();
        if (stuckProjectiles) stuckProjectiles.UnStickAll();
    }
}