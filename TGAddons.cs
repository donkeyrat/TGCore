using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DM;
using Landfall.TABS;
using Landfall.TABS.UnitEditor;
using Landfall.TABS.Workshop;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TGCore
{
    public static class TGAddons
    {
		public static object InvokeMethod<T>(this T obj, string methodName, params object[] args)
		{
			var type = typeof(T);
			var method = type.GetTypeInfo().GetDeclaredMethod(methodName);
			return method.Invoke(obj, args);
		}

		public static T SetField<T>(this T self, string name, object value) where T : class
		{
			FieldInfo field = typeof(T).GetField(name, (BindingFlags)(-1));
			if (field != null)
			{
				field.SetValue(self, value);
			}
			return self;
		}

		public static object GetField<T>(this T self, string name) where T : class
		{
			FieldInfo field = typeof(T).GetField(name, (BindingFlags)(-1));
			if (field != null)
			{
				return field.GetValue(self);
			}
			return null;
		}
		
		public static string DeepString(this GameObject self)
		{
			string final = "\nGameObject '" + self.name + "':\n{\n\tComponents:\n\t{\n";
			final += String.Concat(from Component component in self.GetComponents<Component>() select ("\t\t" + component.GetType().Name + "\n"));
			final += "\t}\n";
			if (self.transform.childCount > 0)
			{
				final += "\tChildren:\n\t{\n";
				final += String.Concat(from Transform child in self.transform select (child.gameObject.DeepString().Replace("\n", "\n\t\t")));
				final += "\n\t}\n";
			}
			final += "}\n";
			return final;
		}

		public static T DeepCopyOf<T>(this T self, T from) where T : class
		{
			foreach (FieldInfo fieldInfo in typeof(T).GetFields((BindingFlags)(-1)))
			{
				try
				{
					fieldInfo.SetValue(self, fieldInfo.GetValue(from));
				}
				catch
				{
				}
			}
			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties((BindingFlags)(-1)))
			{
				if (propertyInfo.CanWrite && propertyInfo.CanRead)
				{
					try
					{
						propertyInfo.SetValue(self, propertyInfo.GetValue(from));
					}
					catch
					{
					}
				}
			}
			return self;
		}
		
		public static SettingsInstance CreateSetting(SettingsInstance.SettingsType settingsType, string settingName, string toolTip, string settingListToAddTo, float defaultValue, float currentValue, string[] options = null, float min = 0f, float max = 1f) 
        {
            var setting = new SettingsInstance
            {
                settingName = settingName,
                toolTip = toolTip,
                m_settingsKey = settingName,
                settingsType = settingsType,
                options = options,
                min = min,
                max = max,
                defaultValue = (int)defaultValue,
                currentValue = (int)currentValue,
                defaultSliderValue = defaultValue,
                currentSliderValue = currentValue
            };

            var global = ServiceLocator.GetService<GlobalSettingsHandler>();
            SettingsInstance[] listToAdd;
            switch (settingListToAddTo)
            {
	            case "BUG":
		            listToAdd = global.BugsSettings;
		            break;
	            case "VIDEO":
		            listToAdd = global.VideoSettings;
		            break;
	            case "AUDIO":
		            listToAdd = global.AudioSettings;
		            break;
	            case "CONTROLS":
		            listToAdd = global.ControlSettings;
		            break;
	            default:
		            listToAdd = global.GameplaySettings;
		            break;
            }

            var list = listToAdd.ToList();
            list.Add(setting);

            switch (settingListToAddTo)
            {
                case "BUG":
                    typeof(GlobalSettingsHandler).GetField("m_bugsSettings", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(global, list.ToArray());
                    break;
                case "VIDEO":
                    typeof(GlobalSettingsHandler).GetField("m_videoSettings", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(global, list.ToArray());
                    break;
                case "AUDIO":
                    typeof(GlobalSettingsHandler).GetField("m_audioSettings", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(global, list.ToArray());
                    break;
                case "CONTROLS":
                    typeof(GlobalSettingsHandler).GetField("m_controlSettings", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(global, list.ToArray());
                    break;
                default:
                    typeof(GlobalSettingsHandler).GetField("m_gameplaySettings", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(global, list.ToArray());
                    break;
            }

            return setting;
        }
		
		public static void AddItems(IEnumerable<UnitBlueprint> newUnits, IEnumerable<Faction> newFactions, IEnumerable<TABSCampaignAsset> newCampaigns, IEnumerable<TABSCampaignLevelAsset> newCampaignLevels, IEnumerable<VoiceBundle> newVoiceBundles, IEnumerable<FactionIcon> newFactionIcons, IEnumerable<Unit> newUnitBases, IEnumerable<PropItem> newProps, IEnumerable<SpecialAbility> newAbilities, IEnumerable<WeaponItem> newWeapons, IEnumerable<ProjectileEntity> newProjectiles)
		{
			var db = ContentDatabase.Instance().LandfallContentDatabase;
			
			var nonStreamableAssets = (Dictionary<DatabaseID, Object>)typeof(AssetLoader).GetField("m_nonStreamableAssets", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(ContentDatabase.Instance().AssetLoader);

			var units = (Dictionary<DatabaseID, UnitBlueprint>)typeof(LandfallContentDatabase).GetField("m_unitBlueprints", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(db);
            foreach (var unit in newUnits.Where(unit => unit && units != null && !units.ContainsKey(unit.Entity.GUID)))
            {
	            units.Add(unit.Entity.GUID, unit);
	            nonStreamableAssets.Add(unit.Entity.GUID, unit);
            }
            typeof(LandfallContentDatabase).GetField("m_unitBlueprints", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(db, units);
            
            var factions = (Dictionary<DatabaseID, Faction>)typeof(LandfallContentDatabase).GetField("m_factions", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(db);
            var defaultHotbarFactions = (List<DatabaseID>)typeof(LandfallContentDatabase).GetField("m_defaultHotbarFactionIds", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(db);
            foreach (var faction in newFactions.Where(faction => faction && factions != null && !factions.ContainsKey(faction.Entity.GUID)))
            {
	            factions.Add(faction.Entity.GUID, faction);
	            nonStreamableAssets.Add(faction.Entity.GUID, faction);
	            defaultHotbarFactions.Add(faction.Entity.GUID);
            }
            typeof(LandfallContentDatabase).GetField("m_factions", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(db, factions);
            typeof(LandfallContentDatabase).GetField("m_defaultHotbarFactionIds", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(db, defaultHotbarFactions.OrderBy(x => factions[x].index).ToList());

            var campaigns = (Dictionary<DatabaseID, TABSCampaignAsset>)typeof(LandfallContentDatabase).GetField("m_campaigns", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(db);
            foreach (var campaign in newCampaigns.Where(campaign => campaign && campaigns != null && !campaigns.ContainsKey(campaign.Entity.GUID)))
            {
	            campaigns.Add(campaign.Entity.GUID, campaign);
	            nonStreamableAssets.Add(campaign.Entity.GUID, campaign);
            }
            typeof(LandfallContentDatabase).GetField("m_campaigns", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(db, campaigns);
            
            var campaignLevels = (Dictionary<DatabaseID, TABSCampaignLevelAsset>)typeof(LandfallContentDatabase).GetField("m_campaignLevels", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var campaignLevel in newCampaignLevels.Where(campaignLevel => campaignLevel && campaignLevels != null && !campaignLevels.ContainsKey(campaignLevel.Entity.GUID)))
            {
	            campaignLevels.Add(campaignLevel.Entity.GUID, campaignLevel);
	            nonStreamableAssets.Add(campaignLevel.Entity.GUID, campaignLevel);
            }
            typeof(LandfallContentDatabase).GetField("m_campaignLevels", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, campaignLevels);
            
            var voiceBundles = (Dictionary<DatabaseID, VoiceBundle>)typeof(LandfallContentDatabase).GetField("m_voiceBundles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var voiceBundle in newVoiceBundles.Where(voiceBundle => voiceBundle && voiceBundles != null && !voiceBundles.ContainsKey(voiceBundle.Entity.GUID)))
            {
	            voiceBundles.Add(voiceBundle.Entity.GUID, voiceBundle);
	            nonStreamableAssets.Add(voiceBundle.Entity.GUID, voiceBundle);
            }
            typeof(LandfallContentDatabase).GetField("m_voiceBundles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, voiceBundles);
            
            var factionIcons = (List<DatabaseID>)typeof(LandfallContentDatabase).GetField("m_factionIconIds", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var factionIcon in newFactionIcons.Where(factionIcon => factionIcon && factionIcons != null && !factionIcons.Contains(factionIcon.Entity.GUID)))
            {
	            factionIcons.Add(factionIcon.Entity.GUID);
	            nonStreamableAssets.Add(factionIcon.Entity.GUID, factionIcon);
            }
            typeof(LandfallContentDatabase).GetField("m_factionIconIds", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, factionIcons);
            
            var unitBases = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_unitBases", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(db);
            foreach (var unitBase in newUnitBases.Where(unitBase => unitBase && unitBases != null && !unitBases.ContainsKey(unitBase.Entity.GUID)))
            {
	            unitBases.Add(unitBase.Entity.GUID, unitBase.gameObject);
	            nonStreamableAssets.Add(unitBase.Entity.GUID, unitBase.gameObject);
            }
            typeof(LandfallContentDatabase).GetField("m_unitBases", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, unitBases);
            
            var props = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_characterProps", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var prop in newProps.Where(prop => prop && props != null && !props.ContainsKey(prop.Entity.GUID)))
            {
	            props.Add(prop.Entity.GUID, prop.gameObject);
	            nonStreamableAssets.Add(prop.Entity.GUID, prop.gameObject);
            }
            typeof(LandfallContentDatabase).GetField("m_characterProps", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, props);
            
            var abilities = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_combatMoves", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var ability in newAbilities.Where(ability => ability && abilities != null && !abilities.ContainsKey(ability.Entity.GUID)))
            {
	            abilities.Add(ability.Entity.GUID, ability.gameObject);
	            nonStreamableAssets.Add(ability.Entity.GUID, ability.gameObject);
            }
            typeof(LandfallContentDatabase).GetField("m_combatMoves", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, abilities);
            
            var weapons = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_weapons", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var weapon in newWeapons.Where(weapon => weapon && weapons != null && !weapons.ContainsKey(weapon.Entity.GUID)))
            {
	            weapons.Add(weapon.Entity.GUID, weapon.gameObject);
	            nonStreamableAssets.Add(weapon.Entity.GUID, weapon.gameObject);
            }
            typeof(LandfallContentDatabase).GetField("m_weapons", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, weapons);
            
            var projectiles = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_projectiles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var proj in newProjectiles.Where(proj => proj && projectiles != null && !projectiles.ContainsKey(proj.Entity.GUID)))
            {
	            projectiles.Add(proj.Entity.GUID, proj.gameObject);
	            nonStreamableAssets.Add(proj.Entity.GUID, proj.gameObject);
            }
            typeof(LandfallContentDatabase).GetField("m_projectiles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, projectiles);
		}
	}
}
