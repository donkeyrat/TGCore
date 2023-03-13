using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TGCore.Localization
{
    public class LocalizationHolder : MonoBehaviour
    {
        public void ReadLocalization()
		{
			Debug.Log("LOCALIZING...");
			
			var locField = typeof(Localizer).GetField("m_localization", BindingFlags.Static | BindingFlags.NonPublic);
            var localizer = (Dictionary<Localizer.Language, Dictionary<string, string>>)locField.GetValue(null);

            try
            {
                foreach (var lang in languages.Where(x => x.key.Count > 0 && x.value.Count > 0))
                {
                    for (var i = 0; i < lang.key.Count; i++) localizer[lang.langage].Add(lang.key[i], lang.value[i]);
                }
            }
            catch (Exception exception)
            {
	            Debug.LogError("LOCALIZATION HAS FAILED:");
                Debug.LogError(exception);
            }
		}

        public List<LocalizationEntry> languages;
    }
}