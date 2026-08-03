using Landfall.TABS.GameState;

namespace TGCore.Library;

public class SwapProjectilesOnWeapon : GameStateListener
{
    private RangeWeapon Ranged;
    private ProjectileLauncher ProjectileLauncher;
    
    private void Start()
    {
        Ranged = GetComponent<RangeWeapon>();
        ProjectileLauncher = GetComponent<ProjectileLauncher>();
    }
    
    public override void OnEnterPlacementState()
    {
    }

    public override void OnEnterBattleState()
    {
        if (Ranged && Ranged.ObjectToSpawn && ProjectileLauncher && ProjectileLauncher.objectToSpawn)
        {
            (ProjectileLauncher.objectToSpawn, Ranged.ObjectToSpawn) = (Ranged.ObjectToSpawn, ProjectileLauncher.objectToSpawn);
        }
    }
}