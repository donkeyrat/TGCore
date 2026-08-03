using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class RaycastSelf : MonoBehaviour, GameObjectPooling.IPoolable
{
    private NativeArray<RaycastCommand> RaycastCommands;
    private NativeArray<RaycastHit> RaycastHits;
    private JobHandle JobHandle;
    private bool NativeArraysDisposed = true;
    private Vector3 LastPos;
    private Vector3 DeltaPos;
    private bool Done;

    public UnityEvent hitMapEvent;
    public LayerMask mask;
    
    private void Start()
    {
        if (!IsManagedByPool)
        {
            Initialize();
        }
    }

    private void Update()
    {
        Check();
    }
    
    private void OnDestroy()
    {
        Release();
    }
    
    public void Initialize()
    {
        RaycastCommands = new NativeArray<RaycastCommand>(1, Allocator.Persistent);
        RaycastHits = new NativeArray<RaycastHit>(1, Allocator.Persistent);
        NativeArraysDisposed = false;
        LastPos = transform.position;
        Check();
    }

    public void Reset()
    {
        LastPos = transform.position;
    }

    public void Release()
    {
        if (!NativeArraysDisposed)
        {
            JobHandle.Complete();
            RaycastCommands.Dispose();
            RaycastHits.Dispose();
            NativeArraysDisposed = true;
        }
    }

    private void Check()
    {
        if (NativeArraysDisposed || Done) return;
        
        JobHandle.Complete();
        var sentHit = RaycastHits[0];

        if (sentHit.collider)
        {
            hitMapEvent.Invoke();
            SetDone(true);
        }
        
        DeltaPos = transform.position - LastPos;
        if (!NativeArraysDisposed)
        {
            RaycastCommands[0] = new RaycastCommand(LastPos, DeltaPos, Vector3.Distance(base.transform.position, LastPos), mask);
            JobHandle = RaycastCommand.ScheduleBatch(RaycastCommands, RaycastHits, 1);
        }
        LastPos = transform.position;
    }

    public void SetDone(bool value)
    {
        Done = value;
    }

    public bool IsManagedByPool { get; set; }
    public Action ReleaseSelf { get; set; }
}