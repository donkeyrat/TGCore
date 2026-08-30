using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class MeleeWeaponGrab : CollisionWeaponEffect
{
	private Rigidbody Rig;
	private StickPosition StickPosition;
	private Unit OwnUnit;
	private GeneralInput Input;
	private Holdable Holdable;
	public List<IsStuck> hitList = new List<IsStuck>();
	
	public bool hardStick = true;

	public float fixPositionAmount;

	public float breakForce = 20000f;

	[HideInInspector] 
	public List<ConfigurableJoint> joints = new List<ConfigurableJoint>();

	public bool walkBackwardsWhenStuck;

	public float downwardsForceOnStuckRig;

	public float time = 3f;

	public UnityEvent stickEvent;
	public int hitLimit = 5;

	private void Start()
	{
		Rig = GetComponent<Rigidbody>();
		OwnUnit = transform.root.GetComponent<Unit>();

		StickPosition = GetComponentInChildren<StickPosition>();
		Input = transform.root.GetComponentInChildren<GeneralInput>();
		Holdable = GetComponent<Holdable>();
		if (Holdable)
		{
			Holdable.AddWasGrabbedAction(Grab);
		}
	}

	public void Grab()
	{
		OwnUnit = Holdable.holderData.GetComponentInParent<Unit>();
	}

	private void FixedUpdate()
	{
		foreach (var joint in joints.Where(x => x))
		{
			if (joint.connectedBody && (!OwnUnit || !OwnUnit.data.Dead))
			{
				joint.connectedBody.AddForce(Vector3.down * downwardsForceOnStuckRig, ForceMode.Force);
			}
		}

		if (joints.Where(x => x).ToArray().Length > 0 && walkBackwardsWhenStuck && Input)
		{
			Input.inputDirection = Vector3.forward * -1f;
		}
	}

	public override void DoEffect(Transform hitTransform, Collision collision)
	{
		var collisionUnit = collision.transform.GetComponentInParent<Unit>();
		if (!collisionUnit || !collision.rigidbody || hitList.Count >= hitLimit)
		{
			return;
		}
		
		if (collisionUnit.GetComponent<IsStuck>() || OwnUnit.Team == collisionUnit.Team)
		{
			return;
		}

		var sqrMagnitude = (StickPosition.transform.position - collision.GetContact(0).point).sqrMagnitude;
		if (sqrMagnitude < StickPosition.radius * StickPosition.radius)
		{
			var joint = AttachJoint(Rig, collision.rigidbody, collision.GetContact(0).point, fixPositionAmount,
				StickPosition.transform, hardStick);
			joint.breakForce = breakForce;
			stickEvent.Invoke();
			var stuck = collisionUnit.gameObject.AddComponent<IsStuck>();
			stuck.joint = joint;
			stuck.stickParent = this;
			stuck.removeTime = Random.Range(time - 1f, time + 1f);
		}
	}

	private static ConfigurableJoint AttachJoint(Rigidbody myRig, Rigidbody otherRig, Vector3 hitPos, float fix, Transform stickPos, bool hardStick)
	{
		if (fix != 0f)
		{
			otherRig.position = otherRig.transform.position + (stickPos.position - otherRig.position).normalized * fix;
		}

		var configurableJoint = myRig.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.anchor = myRig.transform.InverseTransformPoint(hitPos);
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Locked;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		if (hardStick)
		{
			configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
			configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
			configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
		}
		else
		{
			configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
			configurableJoint.angularYMotion = ConfigurableJointMotion.Free;
			configurableJoint.angularZMotion = ConfigurableJointMotion.Free;
		}

		if ((bool)otherRig)
		{
			configurableJoint.connectedBody = otherRig;
		}

		configurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;
		var angularXDrive = configurableJoint.angularXDrive;
		angularXDrive.positionSpring = 10f;
		angularXDrive.positionDamper = 2f;
		configurableJoint.angularXDrive = angularXDrive;
		configurableJoint.angularYZDrive = angularXDrive;
		configurableJoint.enablePreprocessing = false;
		return configurableJoint;
	}

	public class IsStuck : MonoBehaviour
	{
		private float Counter;
		public float removeTime;
		public ConfigurableJoint joint;
		public MeleeWeaponGrab stickParent;
        
		private void Update()
		{
			Counter += Time.deltaTime;
			if (Counter >= removeTime || !joint)
			{
				Break();
			}
		}

		public void Break()
		{
			if (joint) Destroy(joint);
			if (stickParent) stickParent.hitList.Remove(this);
			Destroy(this);
		}
	}
}