using UnityEngine;

namespace TGCore.Localization
{
    public class LocalizationHolder : MonoBehaviour
    {
        public LocalizationHolder()
        {
            english = gameObject.AddComponent<LocalizationEntry>();
            russian = gameObject.AddComponent<LocalizationEntry>();
            chinese = gameObject.AddComponent<LocalizationEntry>();
            french = gameObject.AddComponent<LocalizationEntry>();
            spanish = gameObject.AddComponent<LocalizationEntry>();
            japanese = gameObject.AddComponent<LocalizationEntry>();
            deutsch = gameObject.AddComponent<LocalizationEntry>();
            italian = gameObject.AddComponent<LocalizationEntry>();
            portugeuse = gameObject.AddComponent<LocalizationEntry>();
        }
        
        public LocalizationEntry english;
        
        public LocalizationEntry russian;
        
        public LocalizationEntry chinese;
        
        public LocalizationEntry french;
        
        public LocalizationEntry spanish;
        
        public LocalizationEntry japanese;
        
        public LocalizationEntry deutsch;
        
        public LocalizationEntry italian;
        
        public LocalizationEntry portugeuse;
    }
}