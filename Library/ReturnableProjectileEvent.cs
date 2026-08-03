using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class ReturnableProjectileEvent : MonoBehaviour
{
    public void Go()
    {
        returnEvent.Invoke();
    }
    
    public UnityEvent returnEvent = new UnityEvent();
}