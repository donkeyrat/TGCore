using UnityEngine;
using UnityEngine.Events;
using Landfall.TABS;

namespace TGCore.Library
{
    public class TeamColorEvent : MonoBehaviour 
    {
        private void Start()
        {
            if (goOnStart) Go();
        }

        public void Go()
        {
            var teamHolder = GetComponentInChildren<TeamHolder>();
            var parentTeamHolder = GetComponentInParent<TeamHolder>();
            var rootUnit = transform.root.GetComponent<Unit>();

            if (teamHolder)
            {
                if (teamHolder.team == Team.Red) redTeamEvent.Invoke();
                else blueTeamEvent.Invoke();
            }
            else if (parentTeamHolder)
            {
                if (parentTeamHolder.team == Team.Red) redTeamEvent.Invoke();
                else blueTeamEvent.Invoke();
            }
            else if (rootUnit)
            {
                if (rootUnit.Team == Team.Red) redTeamEvent.Invoke();
                else blueTeamEvent.Invoke();
            }
        }

        public bool goOnStart;

        public UnityEvent redTeamEvent = new UnityEvent();

        public UnityEvent blueTeamEvent = new UnityEvent();
    }
}