using System.Linq;
using UnityEngine;

namespace TGCore.Library;

public class ForceDismount : MonoBehaviour
{
    private RiderHolder RiderHolder;
    private Mount Mount;
    
    private void Start()
    {
        RiderHolder = transform.root.GetComponent<RiderHolder>();
        Mount = transform.root.GetComponent<Mount>();
    }

    public void Dismount()
    {
        if (!RiderHolder && !Mount) Start();
        
        if (RiderHolder && RiderHolder.riders.Count > 0)
        {
            foreach (var rider in RiderHolder.riders.Where(x => x && x.GetComponent<Mount>())
                         .Select(x => x.GetComponent<Mount>()))
            {
                rider.InvokeMethod("Fall");
            }
        }

        if (Mount)
        {
            Mount.InvokeMethod("Fall");
        }
    }
}