using System.Collections;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library 
{

    public class SpikeBehavior : MonoBehaviour 
    {
        private void Start() 
        {
            if (trigger == RiseType.OnStart) DoSpike();
            
            if (GetComponent<TeamHolder>()) Team = GetComponent<TeamHolder>().team;
            else if (GetComponentInParent<TeamHolder>()) Team = GetComponentInParent<TeamHolder>().team;
            else if (GetComponentInChildren<TeamHolder>()) Team = GetComponentInChildren<TeamHolder>().team;
            else if (transform.root.GetComponent<Unit>()) Team = transform.root.GetComponent<Unit>().Team == Team.Red ? Team.Blue : Team.Red;
        }
    
        public void DoSpike() 
        {
            Rising = true;
        }
        
        void Update() 
        {
            if (Joint) 
            {
                Target.data.healthHandler.TakeDamage(stickDamage * Time.deltaTime, Vector3.zero);
               
                if (AdjustCounter < 1f) 
                {
                    AdjustCounter += Time.deltaTime * adjustTime;
                    Joint.connectedAnchor = Vector3.Lerp(Joint.connectedAnchor, Vector3.zero, AdjustCounter);
                }
            }
    
            if (!Target) 
            {
                if (trigger == RiseType.WhenTargetNear) SetTarget();
                else if ((trigger == RiseType.OnStart || trigger == RiseType.Trigger) && transform.root.GetComponent<Unit>())
                {
                    Target = transform.root.GetComponent<Unit>(); 
                    DoSpike(); 
                    foreach (var rig in Target.GetComponentsInChildren<Rigidbody>()) rig.velocity *= 0f;
                }
            }
            
            if (Rising)
            {
                var riseValue = Vector3.up * (riseSpeed * Time.deltaTime);

                if (impaleType == SpikeType.Scale) transform.localScale += riseValue;
                else tip.localPosition += riseValue;
                
                if (Target && Vector3.Distance(tip.position, Target.data.mainRig.position) < stickDistance && !Joint) {
                    StartCoroutine(DelayEnd());
                    StartCoroutine(Stick());
                }
                else if (Target && Vector3.Distance(tip.position, Target.data.mainRig.position) > 6f) { Destroy(this); }
            }
        }
    
        private IEnumerator Stick() 
        {
            Joint = tip.gameObject.AddComponent<FixedJoint>();
            Joint.connectedBody = Target.data.mainRig;
            Joint.autoConfigureConnectedAnchor = false;
            
            Target.data.healthHandler.TakeDamage(damage, Vector3.zero);
            
            yield return new WaitForSeconds(stickTime);
            Destroy(Joint);
        }
    
        private IEnumerator DelayEnd() 
        {
            yield return new WaitForSeconds(stopSpikeDelay);
            Rising = false;
        }
    
        public void SetTarget() 
        {
            var hits = Physics.SphereCastAll(tip.position, targetDistance, Vector3.up, 0.1f, LayerMask.GetMask(new string[] { "MainRig" }));
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => x && !x.data.Dead && x.Team != Team)
                .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
                .Distinct()
                .ToArray();

            if (foundUnits.Length > 0)
            {
                Target = foundUnits[0]; 
                DoSpike(); 
                foreach (var rig in Target.GetComponentsInChildren<Rigidbody>()) rig.velocity *= 0f;
            }
        }
    
        public enum SpikeType
        {
            Scale,
            MoveTip
        }
    
        public enum RiseType
        {
            OnStart,
            Trigger,
            WhenTargetNear
        }
    
        private Unit Target;
    
        private float AdjustCounter;
    
        private FixedJoint Joint;
    
        private Team Team;
    
        private bool Rising;
    
        public SpikeType impaleType;
    
        public RiseType trigger;
    
        public Transform tip;
    
        public float damage = 100f;
    
        public float stickDamage;
    
        public float stickTime = 1f;
    
        public float riseSpeed = 1f;
    
        public float stopSpikeDelay;
    
        public float stickDistance = 0.5f;
    
        public float adjustTime = 1f;
    
        public float targetDistance = 1f;
    }
}