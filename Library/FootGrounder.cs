using UnityEngine;

namespace TGCore.Library;

public class FootGrounder : MonoBehaviour
{
    private bool Lifted;
    private Vector3 GroundNormal;
    private float TimeSinceStep;

    public Transform foot;
    public float lerpSpeed = 5f;
    public float maxAngle = 100f;
    
    public void Lift()
    {
        Lifted = true;
    }
    
    public void Ground()
    {
        Lifted = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collide(collision);
    }
    
    private void OnCollisionStay(Collision collision)
    {
        Collide(collision);
    }

    private void Collide(Collision collision)
    {
        var contact = collision.GetContact(0);
        var angle = Vector3.Angle(Vector3.up, contact.normal);
        
        if (collision.rigidbody || angle > maxAngle || collision.transform.root == transform.root)
        {
            return;
        }
        
        TimeSinceStep = 0f;
        GroundNormal = contact.normal;
    }

    private void Update()
    {
        TimeSinceStep += Time.deltaTime;
        if (Lifted || TimeSinceStep > 0.3f)
        {
            foot.up = Vector3.Lerp(foot.up, Vector3.up, Time.deltaTime * lerpSpeed);
        }
        else if (!Lifted)
        {
            foot.up = Vector3.Lerp(foot.up, GroundNormal, Time.deltaTime * lerpSpeed);
        }
    }
}