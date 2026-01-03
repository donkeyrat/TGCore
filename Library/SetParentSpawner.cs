using UnityEngine;

namespace TGCore.Library
{
    public class SetParentSpawner : MonoBehaviour
    {
        private TeamHolder TeamHolder;
        public bool setParentOnStart = true;
        private void Start()
        {
            TeamHolder = GetComponent<TeamHolder>();
            if (setParentOnStart) Go();
        }
    
        public void Go()
        {
            if (!TeamHolder) return;
            transform.SetParent(TeamHolder.spawner.transform.root);
        }
    }
}