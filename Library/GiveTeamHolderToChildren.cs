using UnityEngine;

namespace TGCore.Library;

public class GiveTeamHolderToChildren : MonoBehaviour
{
    private void Start()
    {
        var teamHolder = GetComponent<TeamHolder>();
        if (teamHolder)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var newTeamHolder = transform.GetChild(i).gameObject.AddComponent<TeamHolder>();
                newTeamHolder.spawner = teamHolder.spawner;
                newTeamHolder.team = teamHolder.team;
                newTeamHolder.spawnerWeapon = teamHolder.spawnerWeapon;
                newTeamHolder.setTeamcolors = teamHolder.setTeamcolors;
                newTeamHolder.target = teamHolder.target;
            }
        }
    }
}