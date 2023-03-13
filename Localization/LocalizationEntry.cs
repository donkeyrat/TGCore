using System.Collections.Generic;
using UnityEngine;

namespace TGCore.Localization 
{
    public class LocalizationEntry : MonoBehaviour
    {
        public Localizer.Language langage;
        
        public List<string> key;

        public List<string> value;
    }
}