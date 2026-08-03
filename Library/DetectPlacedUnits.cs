using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using Landfall.TABS.GameMode;
using Landfall.TABS.GameState;
using Unity.Entities;
using UnityEngine;

namespace TGCore.Library;

public class DetectPlacedUnits : GameStateListener
{
    public float raycastCubeOffset = -10f;
    
    private List<Unit> ExtraUnits = new List<Unit>();
    private Unit OwnUnit;
    private BaseGameMode CurrentGameMode;

    private void Start()
    {
        OwnUnit = transform.root.GetComponent<Unit>();
        CurrentGameMode = ServiceLocator.GetService<GameModeService>().CurrentGameMode;

        var raycastCube = transform.root.FindChildRecursive("RaycastCube");
        if (raycastCube)
        {
            raycastCube.localPosition = new Vector3(raycastCube.localPosition.x, raycastCubeOffset, raycastCube.localPosition.z);
        }
    }

    protected override void OnDestroy()
    {
        if (CurrentGameMode.BattleBudget.GetBudget(OwnUnit.Team) <= 0) return;
        foreach (var unit in ExtraUnits.Where(x => x && !x.IsRider))
        {
            CurrentGameMode.Brush.BrushBehaviour.RemoveUnit(unit);
        }
    }
    
    private void OnTriggerEnter(Collider col)
    {
        var collidedUnit = col.transform.root.GetComponent<Unit>();
        if (!collidedUnit || collidedUnit == OwnUnit || collidedUnit.Team != OwnUnit.Team) return;

        if (!ExtraUnits.Contains(collidedUnit))
        {
            ExtraUnits.Add(collidedUnit);
        }
    }

    public override void OnEnterPlacementState()
    {
    }

    public override void OnEnterBattleState()
    {
        ExtraUnits.Clear();
        Destroy(this);
    }
}