using System;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class ProjectileMissile : MonoBehaviour, GameObjectPooling.IPoolable
{
    private Unit Target;
    private MoveTransform Move;
    private float Rot;
    private float Counter;
    private Vector3 OriginalVelocity;
    private TeamHolder TeamHolder;

    public Transform upTarget;

    public float minRot;
    public float maxRot;
    public float drag;
    public float force;
    public float upForce;

    public AnimationCurve upForceOverRange;

    public AnimationCurve upForceOverTimes;

    public AnimationCurve dragOverTime;

    public AnimationCurve forceOverTime;

    public float prediction;

    public bool randomTarget = true;
    public float targetingRange = 30f;

    public bool IsManagedByPool { get; set; }
    public Action ReleaseSelf { get; set; }

    private void Start()
    {
        TeamHolder = GetComponent<TeamHolder>();
        Move = GetComponent<MoveTransform>();
        OriginalVelocity = Move.velocity;
        if (!IsManagedByPool)
        {
            InitializeOnSpawn();
        }
        
        SetTarget();
    }

    private void Update()
    {
        Counter += Time.deltaTime;
        if (Target)
        {
            var time = Vector3.Distance(transform.position, Target.data.mainRig.position);
            Move.velocity += force * forceOverTime.Evaluate(Counter) * Time.deltaTime * (Target.data.mainRig.position + Target.data.mainRig.velocity * prediction - transform.position).normalized;
            Move.velocity += Time.deltaTime * upForce * upForceOverRange.Evaluate(time) * upForceOverTimes.Evaluate(Counter) * upTarget.forward;
            Move.velocity -= drag * dragOverTime.Evaluate(Counter) * Time.deltaTime * Move.velocity;
        }
        else
        {
            SetTarget();
        }
        upTarget.transform.Rotate(Rot * Time.deltaTime * transform.forward, Space.World);
    }

    public void Initialize()
    {
        InitializeOnSpawn();
    }

    public void Reset()
    {
    }

    public void Release()
    {
        Counter = 0f;
        Move.velocity = OriginalVelocity;
    }

    private void InitializeOnSpawn()
    {
        Rot = UnityEngine.Random.Range(minRot, maxRot);
        upTarget.transform.Rotate(transform.forward * UnityEngine.Random.Range(0f, 360f), Space.World);
        if (UnityEngine.Random.value < 0.5f)
        {
            Rot *= -1f;
        }
    }
    
    private void SetTarget() 
    {
        var hits = Physics.SphereCastAll(transform.position, targetingRange, Vector3.up, 0.1f, LayerMask.GetMask(new string[] { "MainRig" }));
        var foundUnits = hits
            .Select(hit => hit.transform.root.GetComponent<Unit>())
            .Where(x => x && !x.data.Dead && (!TeamHolder || x.Team != TeamHolder.team))
            .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
            .Distinct()
            .ToArray();

        if (foundUnits.Length > 0)
        {
            var index = randomTarget ? UnityEngine.Random.Range(0, foundUnits.Length) : 0;
            Target = foundUnits[index];
        }
    }
}