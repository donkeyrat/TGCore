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
            if (GetComponent<TeamHolder>() && GetComponent<TeamHolder>().team == Team.Red) redTeamEvent.Invoke();
            else if (GetComponent<TeamHolder>() && GetComponent<TeamHolder>().team == Team.Blue) blueTeamEvent.Invoke();
            else if (GetComponentInParent<TeamHolder>() && GetComponentInParent<TeamHolder>().team == Team.Red) redTeamEvent.Invoke();
            else if (GetComponentInParent<TeamHolder>() && GetComponentInParent<TeamHolder>().team == Team.Blue) blueTeamEvent.Invoke();
            else if (transform.root.GetComponent<Unit>() && transform.root.GetComponent<Unit>().Team == Team.Red) redTeamEvent.Invoke();
            else if (transform.root.GetComponent<Unit>() && transform.root.GetComponent<Unit>().Team == Team.Blue) blueTeamEvent.Invoke();
        }

        public bool goOnStart;

        public UnityEvent redTeamEvent = new UnityEvent();

        public UnityEvent blueTeamEvent = new UnityEvent();
    }
}