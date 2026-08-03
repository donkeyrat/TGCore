using System.Linq;
using Landfall.TABS;
using Landfall.TABS.AI.Components.Tags;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TGCore.Library;

public class SetPathPoints : MonoBehaviour
{
    private Unit OwnUnit;
    private GameObjectEntity Entity;
    private Vector3 CurrentPoint;
    private float Counter;

    public LayerMask mask;
    public LayerMask avoidMask;
    public LayerMask unitMask;
    public float randomRange = 8f;
    public float timeTillChangePoint = 5f;
    public float retargetThreshold = 3f;
    public float avoidUnitRadius = 5f;
    public float avoidMapRadius = 1f;
    public float yCheckOffset = 4f;
    public Transform currentPointFollower;
    
    private void Start()
    {
        OwnUnit = GetComponent<Unit>();
        Entity = GetComponent<GameObjectEntity>();
        if (!Entity) Entity = gameObject.AddComponent<GameObjectEntity>();
        
        GetNewWaypoint();
        Entity.EntityManager.AddComponentData(Entity.Entity, new PointTag
        {
            Value = CurrentPoint
        });
        Entity.EntityManager.AddComponentData(Entity.Entity, new PointPathSettings
        {
            RepathRate = 0.2f,
            CurrentRate = 0f
        });
    }
    
    private void Update()
    {
        Counter += Time.deltaTime;
        
        if (currentPointFollower) currentPointFollower.position = CurrentPoint;
        
        var pointTag = Entity.EntityManager.GetComponentData<PointTag>(Entity.Entity);
        if (CurrentPoint == Vector3.zero || 
            Vector3.Distance(OwnUnit.data.mainRig.position, CurrentPoint) < retargetThreshold || 
            Counter > timeTillChangePoint)
        {
            Counter = 0f;
            GetNewWaypoint();
            pointTag.Value = CurrentPoint;
            Entity.EntityManager.SetComponentData(Entity.Entity,  pointTag);
        }
    }

    private void GetNewWaypoint()
    {
        var target = OwnUnit.data.targetMainRig;
        if (!target)
        {
            return;
        }

        var tryCount = 0;
        while (tryCount < 20)
        {
            var randomPoint = target.transform.localPosition;
            randomPoint.x += Random.Range(-randomRange, randomRange);
            randomPoint.z += Random.Range(-randomRange/2, randomRange/2);
            randomPoint = target.transform.TransformPoint(randomPoint);
            randomPoint.y = target.position.y;
            
            var unitColliders = new Collider[10];
            Physics.OverlapSphereNonAlloc(randomPoint, avoidUnitRadius, unitColliders, unitMask);
            if (unitColliders.Where(x => x && x.transform.root != transform.root).ToArray().Length > 0 && tryCount < 15)
            {
                tryCount++;
                continue;
            }
            
            var mapColliders = new Collider[20];
            Physics.OverlapSphereNonAlloc(randomPoint, avoidMapRadius, mapColliders, avoidMask);
            if (mapColliders.Where(x => x).ToArray().Length > 0)
            {
                tryCount++;
                continue;
            }

            randomPoint.y += yCheckOffset;

            var groundPoint = Physics.Raycast(randomPoint, Vector3.down, out var hit, 15f, mask);
            if (groundPoint)
            {
                CurrentPoint = hit.point;
                return;
            }

            tryCount++;
        }
    }
}