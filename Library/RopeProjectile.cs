using UnityEngine;

namespace TGCore.Library;

public class RopeProjectile : MonoBehaviour
{
    private float Counter;
    private TeamHolder Team;
    
    public Rope rope;
    public ProjectileStick stick;

    public float timeToDetach = 4f;
    public float projectilePosOffset = -0.2f;
    public float basePosOffset = 0.3f;
    
    private void Start()
    {
        Team = GetComponent<TeamHolder>();
    }

    private void Update()
    {
        var spawnerWeapon = Team && Team.spawnerWeapon ? Team.spawnerWeapon.transform : null;
        if (rope && spawnerWeapon)
        {
            rope.position1 = transform.position + transform.forward * projectilePosOffset;
            rope.Position2 = spawnerWeapon.position + spawnerWeapon.forward * basePosOffset;
            rope.middleVelocity += Vector3.up * (Mathf.Clamp(transform.forward.y, 0f, 1f) * Time.deltaTime * 250f);
        }
        if (stick.stuck)
        {
            Counter += Time.deltaTime;
            if (Counter > timeToDetach)
            {
                rope.done = true;
            }
        }
    }
}