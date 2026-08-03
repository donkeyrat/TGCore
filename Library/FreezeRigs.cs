using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TGCore.Library;

public class FreezeRigs : MonoBehaviour
{
	private Rigidbody[] Rigs;
	
    public bool freezeOnStart;
	public bool freezePosition = true;
	public bool freezeRotation;

	public void Awake()
	{
		Rigs = transform.root.GetComponentInChildren<RigidbodyHolder>().AllRigs.Where(x => !x.isKinematic).ToArray();
		
		if (freezeOnStart)
		{
			Freeze();
		}
	}

	public void Freeze()
	{
		foreach (var rig in Rigs.Where(rig => rig))
		{
			if (freezePosition && freezeRotation)
			{
				rig.isKinematic = true;
			}
			else if (freezePosition)
			{
				rig.constraints = RigidbodyConstraints.FreezePosition;
			}
			else if (freezeRotation)
			{
				rig.constraints = RigidbodyConstraints.FreezeRotation;
			}
		}
	}

	public void UnFreeze()
	{
		foreach (var rig in Rigs.Where(rig => rig))
		{
			rig.constraints = RigidbodyConstraints.None;
			rig.isKinematic = false;
		}
	}
}