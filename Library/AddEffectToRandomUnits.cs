using System.Collections;
using System.Collections.Generic;
using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using Unity.Entities;
using UnityEngine;

namespace TGCore.Library 
{
	public class AddEffectToRandomUnits : MonoBehaviour 
	{
		private void Start() 
	    {
		    team = transform.root.GetComponent<Unit>().Team;
	        teamSystem = World.Active.GetOrCreateManager<TeamSystem>();
	    }
	
	    public void AddEffectToUnits() 
	    {
			ClearHitList();
			StartCoroutine(DoEffect(teamSystem.GetTeamUnits(team == Team.Red ? (enemyTeam ? Team.Blue : team) : (enemyTeam ? Team.Red : team))));
	    }
	
		private IEnumerator DoEffect(List<Unit> enemyUnits) 
		{
			if (selectionType == SelectionType.Percentage && enemyUnits.Count / value > 0) 
			{
				for (var i = 0; i < enemyUnits.Count / value; i++) 
				{
					if (!hitList.Contains(enemyUnits[i])) 
					{
						AddEffect(transform.root.GetComponent<Unit>(), enemyUnits[i].data);
						hitList.Add(enemyUnits[i]);
						
						yield return new WaitForSeconds(delay);
					}
				}
			}
			else if (selectionType == SelectionType.Specific) 
			{
				for (var i = 0; i < value; i++)
				{
					if (enemyUnits.Count <= 0) yield break;
					
					var chosenUnit = enemyUnits[Random.Range(0, enemyUnits.Count - 1)];
					if (!hitList.Contains(chosenUnit)) 
					{
						AddEffect(transform.root.GetComponent<Unit>(), chosenUnit.data);
						hitList.Add(chosenUnit);
						
						yield return new WaitForSeconds(delay);
					}
				}
			}
		}
	
		public void ClearHitList() 
		{
			hitList.Clear();
		}
	
		public void AddEffect(Unit attacker, DataHandler targetData)
		{
			if (attacker != null && (!targetData || !targetData.Dead) && targetData)
			{
				var unitEffectBase = UnitEffectBase.AddEffectToTarget(targetData.unit.transform.gameObject, effectToAdd);
				if (!unitEffectBase)
				{
					var spawnedEffect = Instantiate(effectToAdd.gameObject, targetData.unit.transform.root);
					spawnedEffect.transform.position = targetData.unit.transform.position;
					spawnedEffect.transform.rotation = Quaternion.LookRotation(targetData.mainRig.position - attacker.data.mainRig.position);
					unitEffectBase = spawnedEffect.GetComponent<UnitEffectBase>();
					TeamHolder.AddTeamHolder(spawnedEffect, transform.root.gameObject);
					unitEffectBase.DoEffect();
				}
				else if (!addEffectOnce)
				{
					unitEffectBase.transform.rotation = Quaternion.LookRotation(targetData.mainRig.position - attacker.data.mainRig.position);
					unitEffectBase.Ping();
				}
			}
		}
	
		public enum SelectionType 
		{
			Percentage,
			Specific
		}
	
		private Team team;
		private TeamSystem teamSystem;
		private List<Unit> hitList = new List<Unit>();
		
		[Header("Selection Settings")]
	
		public SelectionType selectionType;
		public int value;
		public float delay;
		public bool enemyTeam = true;
		
		[Header("Effect Settings")]
	
		public UnitEffectBase effectToAdd;
		public bool addEffectOnce;
	}
}