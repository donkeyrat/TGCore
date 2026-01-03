using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class CollisionStick : MonoBehaviour
{
    private void Start()
    {
        Rig = GetComponent<Rigidbody>();
        TeamHolder = GetComponent<TeamHolder>();
    }

    public void OnCollisionEnter(Collision col)
    {
        var enemy = col.transform.root.GetComponent<Unit>();
        if (Disabled || !col.rigidbody || !enemy || !TeamHolder || enemy.Team == TeamHolder.team 
            || hitList.Count >= hitLimit || HasBeenHitList.Contains(enemy) || enemy.GetComponent<IsStuck>()
            || (hitLayer & (1 << col.gameObject.layer)) != 0) return;
        
        if (Rig.mass > col.rigidbody.mass * thresholdMultiplier)
        {
            HasBeenHitList.Add(enemy);
            
            var joint = JointActions.AttachJoint(col.rigidbody, Rig, 25f, 15f, false,
                true, 15f);
            joint.breakForce = breakForce;
            joint.breakTorque = breakForce;
            
            var stuck = enemy.gameObject.AddComponent<IsStuck>();
            stuck.joint = joint;
            stuck.removeTime = removeJointAfterSeconds;
            stuck.stickParent = this;
            hitList.Add(stuck);
        }
    }

    public void DestroyJoints()
    {
        foreach (var enemy in hitList.Where(enemy => enemy))
        {
            enemy.Break();
        }
        hitList.Clear();
        
        Disabled = true;
    }

    private Rigidbody Rig;
    private TeamHolder TeamHolder;
    private bool Disabled;
    private List<Unit> HasBeenHitList = new List<Unit>();

    public List<IsStuck> hitList = new List<IsStuck>();

    public float breakForce = 30000f;
    public float thresholdMultiplier = 10f;
    public int hitLimit = 10;
    public float removeJointAfterSeconds = 10f;
    public LayerMask hitLayer;

    public class IsStuck : MonoBehaviour
    {
        private float Counter;
        public float removeTime;
        public ConfigurableJoint joint;
        public CollisionStick stickParent;
        
        private void Update()
        {
            Counter += Time.deltaTime;
            if (Counter >= removeTime || !joint)
            {
                Break();
            }
        }

        public void Break()
        {
            if (joint) Destroy(joint);
            if (stickParent) stickParent.hitList.Remove(this);
            Destroy(this);
        }
    }
}