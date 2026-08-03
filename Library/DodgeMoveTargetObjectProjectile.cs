using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;
 
public class DodgeMoveTargetObjectProjectile : MonoBehaviour
{
    private GameObject CurrentObject;
    
    public ProjectileLauncher projectileLauncher;
    public GameObject objectToGive;
    public UnityEvent modifyProjectileEvent;
    public DodgeMove dodgeMove;
    
    private void Start()
    {
        projectileLauncher.SpawnedObject += ModifyProjectile;
    }

    public void ModifyProjectile(GameObject projectile)
    {
        var newObject = Instantiate(objectToGive, projectile.transform, false);
        newObject.transform.position = projectile.transform.position;
        CurrentObject = newObject;
        modifyProjectileEvent.Invoke();
    }

    public void DoMove()
    {
        if (CurrentObject) dodgeMove.DoMove(CurrentObject.transform);
    }
}