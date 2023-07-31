using UnityEngine;

namespace TGCore.Library
{
    public class LoadScene : MonoBehaviour
    {
        private void Start()
        {
            if (loadOnStart) Go(sceneToLoad);
        }
        
        public void Go(string scene)
        {
            TABSSceneManager.LoadScene(scene, forceInstantLoad);
        }

        public bool loadOnStart = true;
        
        public string sceneToLoad;
        public bool forceInstantLoad = true;
    }
}