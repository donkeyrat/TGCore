using UnityEngine;

namespace TGCore.Library;

public class PointRigUp : MonoBehaviour
{
    private bool Pointing;
    private Quaternion StartRotation;
    private float Counter;

    public float lerpSpeed = 1f;
    
    public void DoPoint()
    {
        Pointing = true;
        StartRotation = transform.rotation;
    }
    
    private void Update()
    {
        if (!Pointing) return;
        Counter += Time.deltaTime * lerpSpeed;

        transform.rotation = Quaternion.Lerp(StartRotation, Quaternion.LookRotation(Vector3.up), Counter);

        if (Counter >= 1f) Pointing = false;
    }
}