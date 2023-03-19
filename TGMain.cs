using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using DM;
using HarmonyLib;
using Landfall.TABS.Workshop;
using TGCore.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TGCore 
{
	[BepInPlugin("teamgrad.core", "Team Grad Core", "1.0.1")]
	public class TGMain : BaseUnityPlugin 
	{
		private void Awake()
		{
			instance = this;
			Debug.Log("LOADING TGCORE...");
			
			AssetBundle.LoadFromMemory(Properties.Resources.tgcore);

			new Harmony("TGCore").PatchAll();

			StartCoroutine(InitializeMods());
		}
		
		private IEnumerator InitializeMods()
		{
			yield return new WaitUntil(() => FindObjectOfType<ServiceLocator>() != null);

			ServiceLocator.GetService<CustomContentLoaderModIO>().QuickRefresh(WorkshopContentType.Unit, null);

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

		public static TGMain instance;
		public static List<TGMod> modList = new List<TGMod>();
		
		public static ContentDatabase db => ContentDatabase.Instance();
		public static LandfallContentDatabase landfallDb => ContentDatabase.Instance().LandfallContentDatabase;
		
		public static List<SoundBank> newSounds = new List<SoundBank>(); 
	}
}
