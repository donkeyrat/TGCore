using BepInEx;
using TGCore.Localization;
using UnityEngine.SceneManagement;

namespace TGCore
{
    public class TGMod : BaseUnityPlugin
    {
        private void Awake()
        {
            EarlyLaunch();
        }
        
        public virtual void EarlyLaunch()
        {
        }
        
        public virtual void Launch()
        {
        }
        
        public virtual void LateLaunch()
        {
        }

        public virtual void AddSettings()
        {
        }

        public virtual void SceneManager(Scene scene, LoadSceneMode loadSceneMode)
        {
        }
        
        public virtual void Localize(LocalizationHolder languageHolder)
        {
        }
    }
}