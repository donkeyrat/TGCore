using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using DM;
using HarmonyLib;
using Landfall.TABS;
using Landfall.TABS.Workshop;
using LevelCreator;
using TFBGames;
using TGCore.Library;
using TGCore.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TGCore 
{
	[BepInPlugin("teamgrad.core", "Team Grad Core", "1.1.0")]
	public class TGMain : BaseUnityPlugin
	{
		private void Awake()
		{
			instance = this;
			Debug.Log("LOADING TGCORE...");

			new Harmony("TGCore").PatchAll();

			StartCoroutine(InitializeMods());
			SceneManager.sceneLoaded += OnSceneLoad;
		}
		
		private IEnumerator InitializeMods()
		{
			yield return new WaitUntil(() => FindObjectOfType<ServiceLocator>() != null);

			ServiceLocator.GetService<CustomContentLoaderModIO>().QuickRefresh(WorkshopContentType.Unit, null);
			
			TGAddons.AddTeamColors(tgcore.LoadAsset<UnitEditorColorPalette>("UCColorPalette"));

			var languageHolder = gameObject.AddComponent<LocalizationHolder>();

			modList = Chainloader.ManagerObject.GetComponents<TGMod>().ToList();
			var failedModList = new List<TGMod>();
			
			foreach (var mod in modList)
			{
				try
				{
					Debug.Log("LOADING MOD...");
					
					mod.Launch();
					mod.AddSettings();
					mod.Localize(languageHolder);
					SceneManager.sceneLoaded += mod.SceneManager;
				}
				catch (Exception exc)
				{
					failedModList.Add(mod);
					Debug.LogError("A MOD HAS FAILED TO LAUNCH:");
					Debug.LogError(exc);
				}
			}
			modList.RemoveAll(x => failedModList.Contains(x));
			
			UpdateSoundBank();
			languageHolder.ReadLocalization();
			
			foreach (var mod in modList)
			{
				try
				{
					mod.LateLaunch();
				}
				catch (Exception exc)
				{
					Debug.LogError("A MOD HAS FAILED TO LAUNCH:");
					Debug.LogError(exc);
				}
			}
			modList.RemoveAll(x => failedModList.Contains(x));
		}

		public void UpdateSoundBank()
		{
			Debug.Log("UPDATING SOUNDS...");
			
			foreach (var sb in newSounds) 
			{
				try
				{
					if (sb.name.Contains("Sound")) 
					{
						var vsb = ServiceLocator.GetService<SoundPlayer>().soundBank;
						foreach (var sound in sb.Categories) sound.categoryMixerGroup = vsb.Categories[0].categoryMixerGroup;
                    
						var cat = vsb.Categories.ToList();
						cat.AddRange(sb.Categories);
						vsb.Categories = cat.ToArray();
					}
					else if (sb.name.Contains("Music")) 
					{
						var vsb = ServiceLocator.GetService<MusicHandler>().bank;
						var cat = vsb.Categories.ToList();
						cat.AddRange(sb.Categories);
						foreach (var category in sb.Categories) 
						{
							foreach (var sound in category.soundEffects) 
							{
								var song = new SongInstance
								{
									clip = sound.clipTypes[0].clips[0],
									soundEffectInstance = sound,
									songRef = category.categoryName + "/" + sound.soundRef
								};

								ServiceLocator.GetService<MusicHandler>().m_songs.Add(song.songRef, song);
							}
						}
						vsb.Categories = cat.ToArray();
					}
				}
				catch (Exception exception)
				{
					Debug.LogError("UPDATING SOUNDS HAS FAILED:");
					Debug.LogError(exception);
				}
			}
		}
		
		public void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (scene.name.Contains("GameScene"))
			{
				foreach (var obj in scene.GetRootGameObjects())
				{
					var hpBar = obj.transform.FindChildRecursive("HealthBar");
					if (hpBar)
					{
						var fill = hpBar.FindChildRecursive("Fill").gameObject;
						var barrierFill = Instantiate(fill, fill.transform.position, fill.transform.rotation,
							hpBar);
						var rect = barrierFill.GetComponent<RectTransform>();
						rect.SetTop(-10f);
						rect.SetBottom(15f);
						var image = barrierFill.GetComponent<Image>();
						image.color = Color.yellow;
						image.fillAmount = 0f;
						barrierFill.AddComponent<UpdateBarByBarrier>().image = image;
					}
				}
			}
			else if (scene.name.Contains("Editor Scene"))
			{
				var vanillaObjectTable = scene.GetRootGameObjects().Where(x => x.GetComponent<DMEditor>())
					.Select(x => x.GetComponent<DMEditor>().editorObjectTable).ToArray()[0];
				foreach (var objectTable in objectTables)
				{
					var rowValues = objectTable.GetRowValues();
					for (var i = 0; i < rowValues.Length; i++)
					{
						vanillaObjectTable.AddRow(objectTable.GetKeys()[i], rowValues[i]);
					}
				}
			}
			else if (scene.name.Contains("LevelScene"))
			{
				var vanillaObjectTable = scene.GetRootGameObjects().Where(x => x.GetComponent<SpawnLevel>())
					.Select(x => (DMEditorObjectTable)x.GetComponent<SpawnLevel>().GetField("editorObjectTable")).ToArray()[0];
				foreach (var objectTable in objectTables)
				{
					var rowValues = objectTable.GetRowValues();
					for (var i = 0; i < rowValues.Length; i++)
					{
						vanillaObjectTable.AddRow(objectTable.GetKeys()[i], rowValues[i]);
					}
				}
			}
			else if (scene.name.Contains("UnitCreator"))
			{
				foreach (var obj in scene.GetRootGameObjects())
				{
					var unitSpawner = obj.GetComponent<UnitEditorSpawnTestEnemies>();
					if (unitSpawner)
					{
						var factionList = unitSpawner.factionsToSpawn.ToList();
						factionList.AddRange(NewFactions);
						unitSpawner.factionsToSpawn = factionList.ToArray();
					}
				}
			}
		}


		public static TGMain instance;
		public static AssetBundle tgcore = AssetBundle.LoadFromMemory(Properties.Resources.tgcore);
		public static List<TGMod> modList = new List<TGMod>();
		
		public static ContentDatabase DB => ContentDatabase.Instance();
		public static LandfallContentDatabase landfallDb => ContentDatabase.Instance().LandfallContentDatabase;
		
		public static List<SoundBank> newSounds = new List<SoundBank>();
		public static List<DMEditorObjectTable> objectTables = new();
		public static List<Faction> NewFactions = new List<Faction>();
	}
}
