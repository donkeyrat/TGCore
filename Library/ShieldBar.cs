using System;
using Landfall.TABC;
using Landfall.TABS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TGCore.Library;

public class ShieldBar : MonoBehaviour
{
	private DataHandler DataHandler;
	private bool Done;
	private bool HasBeenInitiated;
	private float CurrSizeMultiplier = 1f;
	private float HealthSizeFactor;
	private Vector3 StartScale;
	private SettingsInstance HealthBarSizeOption;
	private Effect_Shield Shield;
	private AchillesArmor.UnitIsArmored Armor;
	
	public Image healthBar;
	public Image whiteHealthBar;
	public Landfall.TABC.CodeAnimation codeAnimation;
	public float upMultiplier = 1.5f;
	public float sizeHeightInfluence;

	public void Init(Unit unit, Effect_Shield shield = null, AchillesArmor.UnitIsArmored armor = null)
	{
		unit.gameObject.AddComponent<Shielded>().bar = this;
		DataHandler = unit.data;
		
		
		var populate = GetComponentInChildren<Populate>();
		populate.times = Mathf.RoundToInt(Mathf.Clamp((unit.data.health - 50f) / 50f, 0f, 10f));
		populate.DoPopulate();
		
		HealthBarSizeOption = ServiceLocator.GetService<GlobalSettingsHandler>().GetSettingsInstance("GAMEPLAY_HEALTHBAR_SIZE");
		
		var animations = codeAnimation.animations;
		foreach (var anim in animations)
		{
			anim.multiplier = HealthBarSizeOption.currentSliderValue;
		}

		HealthSizeFactor = 1f + populate.times * 0.1f;
		codeAnimation.startScale *= HealthSizeFactor;
		StartScale = codeAnimation.startScale;

		Shield = shield;
		Armor = armor;
		
		HasBeenInitiated = true;
	}

	public void SetShield(Effect_Shield shield)
	{
		Shield = shield;
	}
	
	public void SetArmor(AchillesArmor.UnitIsArmored armor)
	{
		Armor = armor;
	}

	public void RemoveBar()
	{
		var shielded = DataHandler.unit.GetComponent<Shielded>();
		if (shielded) Destroy(shielded);
		Destroy(gameObject);
	}

	private void CheckForCorrectScaling()
	{
		if (HasBeenInitiated)
		{
			var currentSliderValue = HealthBarSizeOption.currentSliderValue;
			if (!Mathf.Approximately(CurrSizeMultiplier, currentSliderValue))
			{
				CurrSizeMultiplier = currentSliderValue;
				transform.localScale = StartScale * CurrSizeMultiplier;
			}
		}
	}

	private void LateUpdate()
	{
		var vanillaHealthBar = (TABCUnitUI)DataHandler.unit.GetField("m_unitUI");
		if (vanillaHealthBar && !vanillaHealthBar.gameObject.activeSelf)
		{
			transform.GetChild(0).gameObject.SetActive(false);
			return;
		}
		if (!transform.GetChild(0).gameObject.activeSelf)
		{
			transform.GetChild(0).gameObject.SetActive(true);
		}
		
		CheckForCorrectScaling();
		
		var num = 0f;
		if (Armor && Shield)
		{
			num = Mathf.Clamp((Shield.currentShield + Armor.armorHealth) / (Shield.maxShield + Armor.startingArmorHealth), 0f, 1f);
		}
		else if (Armor)
		{
			num = Mathf.Clamp(Armor.armorHealth / Armor.startingArmorHealth, 0f, 1f);
		}
		else if (Shield)
		{
			num = Mathf.Clamp(Shield.currentShield / Shield.maxShield, 0f, 1f);
		}
		
		if ((!DataHandler || DataHandler.Dead || num <= 0f) && !Done)
		{
			codeAnimation.PlayOut();
			Done = true;
		}
		if (DataHandler)
		{
			var position = DataHandler.head.transform.position + Vector3.up * (upMultiplier * (1f + HealthSizeFactor * CurrSizeMultiplier * sizeHeightInfluence));
			if (Landfall.TABC.MainCam.instance && Landfall.TABC.MainCam.instance.cam)
			{
				if (Landfall.TABC.MainCam.instance.cam.transform.InverseTransformPoint(position).z > 0f)
				{
					transform.SetPositionAndRotation(position, Quaternion.LookRotation(transform.position - Landfall.TABC.MainCam.instance.transform.position));
					healthBar.fillAmount = num;
					whiteHealthBar.fillAmount = Mathf.Lerp(whiteHealthBar.fillAmount, num, Time.deltaTime * 5f);
				}
				else
				{
					transform.position = Vector3.down * 1000f;
				}
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public class Shielded : MonoBehaviour
	{
		public ShieldBar bar;
	}
}