using System.Collections;
using System.Linq;
using Landfall.TABS.GameState;
using UnityEngine;

namespace TGCore.Library.UnitFaker
{
	public class FakeConditionalEvent : MonoBehaviour
	{
		private void Start()
		{
			Man = ServiceLocator.GetService<GameStateManager>();
			OwnUnit = transform.root.GetComponent<FakeUnit>();
			
			conditionalEvent.moves = GetComponents<Move>();
			foreach (var eventCondition in conditionalEvent.conditions)
			{
				if (!eventCondition.startOnCD) eventCondition.counter = float.MaxValue;
			}
		}

		public void Update()
		{
			IncrementCounters();
			CheckConditionsUpdate();
			
			if (IsStunnedFor > 0f) IsStunnedFor -= Time.deltaTime;
		}

		public void CheckConditionsUpdate()
		{
			if (conditionalEvent.checkAutomatically)
			{
				CheckConditions(conditionalEvent);
			}
		}

		private void CheckConditions(ConditionalEventInstance eventToCheck)
		{
			if (gameObject == null || eventToCheck == null) return;
			
			if (eventToCheck.conditions.Any(eventCondition => !CheckCondition(eventCondition)))
			{
				if (eventToCheck.isOn)
				{
					if (gameObject.activeInHierarchy)
					{
						StartCoroutine(DelayTurnOffEvent(eventToCheck));
					}
					eventToCheck.isOn = false;
				}

				return;
			}
			if (!eventToCheck.isOn)
			{
				if (gameObject.activeInHierarchy)
				{
					StartCoroutine(DelayTurnOnEvent(eventToCheck));
				}
				eventToCheck.isOn = true;
			}
			foreach (var eventCondition in eventToCheck.conditions)
			{
				eventCondition.counter = Random.Range(0f, 0f - eventCondition.extraRandomCooldown);
			}
			if (gameObject.activeInHierarchy)
			{
				StartCoroutine(DelayMovesAndContinuousEvent(eventToCheck));
			}
		}

		private static IEnumerator DelayTurnOffEvent(ConditionalEventInstance eventToCheck)
		{
			yield return new WaitForSeconds(eventToCheck.delay);
			eventToCheck.turnOffEvent?.Invoke();
		}

		private static IEnumerator DelayTurnOnEvent(ConditionalEventInstance eventToCheck)
		{
			yield return new WaitForSeconds(eventToCheck.delay);
			eventToCheck.turnOnEvent?.Invoke();
		}

		private IEnumerator DelayMovesAndContinuousEvent(ConditionalEventInstance eventToCheck)
		{
			yield return new WaitForSeconds(eventToCheck.delay);
			
			eventToCheck.continuousEvent?.Invoke();
			
			foreach (var move in eventToCheck.moves)
			{
				move.DoMove(cachedEnemyWeapon, OwnUnit.target ? OwnUnit.target.data.mainRig : null,
					OwnUnit.target ? OwnUnit.target.data : null);
			}
		}

		private bool CheckCondition(EventCondition condition)
		{
			if (!OwnUnit || IsStunnedFor > 0f || Man.GameState != GameState.BattleState) return false;
			
			switch (condition.conditionType)
			{
				case EventCondition.ConditionType.UnitDistanceToTarget:
					switch (condition.valueType)
					{
						case EventCondition.ValueType.Max when OwnUnit.distanceToTarget > condition.value +
							(condition.rangeType == EventCondition.RangeType.RangePlusUnitRange
								? OwnUnit.distanceToKeep + 0.5f
								: 0f):
							return false;
						case EventCondition.ValueType.Min when OwnUnit.distanceToTarget < condition.value +
							(condition.rangeType == EventCondition.RangeType.RangePlusUnitRange
								? OwnUnit.distanceToKeep + 0.5f
								: 0f):
							return false;
					}
					
					break;
				case EventCondition.ConditionType.UnitTargetHP:
					switch (condition.valueType)
					{
						case EventCondition.ValueType.Max when !OwnUnit.target || OwnUnit.target.data.maxHealth > condition.value:
							return false;
						case EventCondition.ValueType.Min when !OwnUnit.target || OwnUnit.target.data.maxHealth < condition.value:
							return false;
					}

					break;
				case EventCondition.ConditionType.UnitAngleToTarget:
					switch (condition.valueType)
					{
						case EventCondition.ValueType.Min when OwnUnit.angleToTarget < condition.value:
							return false;
						case EventCondition.ValueType.Max when OwnUnit.angleToTarget > condition.value:
							return false;
					}

					break;
				case EventCondition.ConditionType.Cooldown when condition.counter <= condition.value:
					return false;
				case EventCondition.ConditionType.Cooldown:
				{
					if (condition.alwaysResetCounter)
					{
						condition.counter = Random.Range(0f, 0f - condition.extraRandomCooldown);
					}

					break;
				}
				case EventCondition.ConditionType.Chance when condition.value < Random.value:
					return false;
				case EventCondition.ConditionType.ChancePerSecond
					when condition.value * Mathf.Clamp(Time.deltaTime, 0f, 0.02f) < Random.value:
					return false;
			}
			return true;
		}

		private void IncrementCounters()
		{
			foreach (var eventCondition in conditionalEvent.conditions)
			{
				if (eventCondition.conditionType == EventCondition.ConditionType.Cooldown)
				{
					if (!eventCondition.onlyCountWhenUnitInRange)
					{
						eventCondition.counter += Time.deltaTime;
					}
					else if (eventCondition.whichRange == EventCondition.WhichRange.UnitRange && OwnUnit.distanceToTarget <= OwnUnit.distanceToKeep + 0.5f)
					{
						eventCondition.counter += Time.deltaTime;
					}
					else if (eventCondition.whichRange == EventCondition.WhichRange.Specified && OwnUnit.distanceToTarget < eventCondition.cooldownRange)
					{
						eventCondition.counter += Time.deltaTime;
					}
				}
			}
		}

		public void AddRangeToAllConditions(float range)
		{
			foreach (var condition in conditionalEvent.conditions)
			{
				if (condition.conditionType == EventCondition.ConditionType.UnitDistanceToTarget && condition.valueType == EventCondition.ValueType.Max)
				{
					condition.value += range;
				}
			}
		}

		public void StunAllOfMyMovesFor(float seconds)
		{
			IsStunnedFor = seconds;
		}
		
		private FakeUnit OwnUnit;
		private GameStateManager Man;

		public ConditionalEventInstance conditionalEvent;

		[HideInInspector]
		public Rigidbody cachedEnemyWeapon;

		private float IsStunnedFor;
	}
}
