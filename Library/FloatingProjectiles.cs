using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class FloatingProjectiles : MonoBehaviour
{
	private ShootPosition[] SwordPoints;
	private List<SpookySword> Swords = new List<SpookySword>();
	private DataHandler Data;
	private bool Done;
	private int AttackID;
	
	public UnityEvent shootEvent;

	public GameObject sourceSword;
	public float throwRange = 10f;
	public float throwSpeed = 100f;
	public AnimationCurve throwCurve;

	public float timeBetweenShots = 0.1f;

	private void Awake()
	{
		SwordPoints = GetComponentsInChildren<ShootPosition>();
	}

	private void Start()
	{
		Data = transform.root.GetComponentInChildren<DataHandler>();
		var list = new List<Renderer>();
		foreach (var point in SwordPoints)
		{
			var spookySword = CreateNewSword(point.transform.position + Vector3.up * 2f, point.transform.rotation);
			var componentsInChildren = spookySword.gameObject.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null && componentsInChildren.Length != 0)
			{
				list.AddRange(componentsInChildren);
			}
			Swords.Add(spookySword);
		}
		Data.unit.AddRenderersToShowHide(list.ToArray(), Data.unit.IsSpawnedInBlindPlacement);
		for (var k = 0; k < Swords.Count; k++)
		{
			if (Swords[k].gameObject)
			{
				Swords[k].gameObject.transform.position = SwordPoints[k].transform.position;
				Swords[k].gameObject.transform.rotation = SwordPoints[k].transform.rotation;
			}
		}
	}

	private void Update()
	{
		if (Done)
		{
			return;
		}
		if (Data && Data.Dead)
		{
			Done = true;
			for (var i = 0; i < Swords.Count; i++)
			{
				AttackID = i;
				Attack(Data.mainRig, AttackID);
			}
		}
		
		for (var k = 0; k < Swords.Count; k++)
		{
			if (Swords[k].gameObject)
			{
				Swords[k].sinceSpawn += Time.deltaTime;
				Swords[k].gameObject.transform.position = SwordPoints[k].transform.position;
				Swords[k].gameObject.transform.rotation = SwordPoints[k].transform.rotation;
			}
		}
	}
	
	public void Attack(Rigidbody target, int useAttackID)
	{
		var spookySword = Swords[useAttackID];
		if (spookySword != null)
		{
			StartCoroutine(DoAttack(spookySword, target));
			shootEvent.Invoke();
			if (!Done)
			{
				Swords[useAttackID] = CreateNewSword(SwordPoints[useAttackID].gameObject.transform.position, SwordPoints[useAttackID].gameObject.transform.rotation, true);
			}
			else
			{
				spookySword.gameObject.GetComponent<ProjectileHit>().ignoreTeamMates = false;
			}
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		foreach (var sword in Swords)
		{
			Destroy(sword.gameObject);
		}
	}

	private IEnumerator DoAttack(SpookySword attackSword, Rigidbody targ)
	{
		if (!attackSword.gameObject)
		{
			yield break;
		}
		
		attackSword.gameObject.GetComponent<DelayEvent>().Go();
		
		var counter2 = 0f;
		var throwTime = throwCurve.keys[throwCurve.keys.Length - 1].time;
		var stick = attackSword.gameObject.GetComponent<ProjectileStick>();
		attackSword.gameObject.GetComponent<RaycastTrail>().enabled = true;
		
		var removeAfterSeconds = attackSword.gameObject.AddComponent<RemoveAfterSeconds>();
		removeAfterSeconds.seconds = 6f;
		removeAfterSeconds.shrink = true;
		
		while (counter2 < throwTime && (!stick || !stick.stuck))
		{
			if (!attackSword.gameObject || !targ)
			{
				yield break;
			}
			counter2 += Time.deltaTime;
			attackSword.move.velocity = Vector3.Lerp(attackSword.move.velocity, (targ.position - attackSword.gameObject.transform.position).normalized * 
				(throwSpeed * throwCurve.Evaluate(counter2)), Time.deltaTime * 8f);
			attackSword.gameObject.transform.rotation = Quaternion.Lerp(attackSword.gameObject.transform.rotation, 
				Quaternion.LookRotation(targ.position - attackSword.gameObject.transform.position, Vector3.up), Time.deltaTime * 7f);
			yield return null;
		}

		var moveTransform = attackSword.gameObject.GetComponent<MoveTransform>();
		moveTransform.rotationFollowVelocity = true;
		moveTransform.gravity = 10f;
	}

	private SpookySword CreateNewSword(Vector3 pos, Quaternion rot, bool playAnimation = false)
	{
		var obj = new SpookySword
		{
			gameObject = Instantiate(sourceSword, pos, rot, transform.root)
		};
		obj.move = obj.gameObject.GetComponent<MoveTransform>();
		var teamHolder = obj.gameObject.FetchComponent<TeamHolder>();
		teamHolder.team = Data.team;
		teamHolder.spawner = Data.unit.gameObject;
		if (playAnimation)
		{
			var codeAnimation = obj.gameObject.GetComponentInChildren<CodeAnimation>();
			codeAnimation.PlayIn();	
		}
		return obj;
	}
	
	public void ThrowSwords()
	{
		StartCoroutine(DoAttacks());
	}

	private IEnumerator DoAttacks()
	{
		for (var i = 0; i < Swords.Count; i++)
		{
			AttackID = i;
			if (Data.targetMainRig && Data.distanceToTarget < throwRange)
			{
				Attack(Data.targetMainRig, AttackID);
			}
			
			yield return new WaitForSeconds(timeBetweenShots);
		}
	}
}