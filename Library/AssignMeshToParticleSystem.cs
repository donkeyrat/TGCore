using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
	public class AssignMeshToParticleSystem : MonoBehaviour
	{

		private void Start()
		{
			part = GetComponent<ParticleSystem>();

			switch (meshType)
			{
				case MeshType.MeshRenderer:
				{
					allMeshRenderers = transform.root.GetComponentsInChildren<MeshRenderer>();
					foreach (var renderer in allMeshRenderers)
					{
						if (renderer.CompareTag("UnitMesh") && !meshAssigned)
						{
							meshRenderer = renderer;
							meshAssigned = true;
						}
						if (disableMesh) renderer.enabled = false;
					}
					if (meshRenderer)
					{
						var shape = part.shape;
						shape.meshRenderer = meshRenderer;
					
						findEvent.Invoke();
						if (play) part.Play();
					}
				
					if (!disableParticles) return;
				
					particleSystems = transform.root.GetComponentsInChildren<ParticleSystem>();
					foreach (var particle in particleSystems)
					{
						if (!particle.CompareTag("DontRemove")) particle.Stop();
					}

					break;
				}
				case MeshType.SkinnedMeshRenderer:
				{
					allSkinnedMeshRenderers = transform.root.GetComponentsInChildren<SkinnedMeshRenderer>();
					foreach (var renderer in allSkinnedMeshRenderers)
					{
						if (renderer.CompareTag("UnitMesh"))
						{
							skinnedMeshRend = renderer;
							break;
						}
					}
					if (skinnedMeshRend)
					{
						var shape = GetComponent<ParticleSystem>().shape;
						shape.skinnedMeshRenderer = skinnedMeshRend;
					
						findEvent.Invoke();
						if (play) part.Play();
					}

					break;
				}
				case MeshType.NonUnitMeshRenderer:
				default:
				{
					allMeshRenderers = transform.root.GetComponentsInChildren<MeshRenderer>();
					foreach (var renderer in allMeshRenderers)
					{
						if (!meshAssigned)
						{
							meshRenderer = renderer;
							meshAssigned = true;
						}
						if (disableMesh) renderer.enabled = false;
					}
					if (meshRenderer)
					{
						var shape = part.shape;
						shape.meshRenderer = meshRenderer;
					
						findEvent.Invoke();
						if (play) part.Play();
					}

					break;
				}
			}
		}
		
		private MeshRenderer[] allMeshRenderers;
		private MeshRenderer meshRenderer;
		private SkinnedMeshRenderer[] allSkinnedMeshRenderers;
		private SkinnedMeshRenderer skinnedMeshRend;
		private ParticleSystem[] particleSystems;
		private ParticleSystem part;
		private bool meshAssigned;
		
		public enum MeshType
		{
			MeshRenderer,
			SkinnedMeshRenderer,
			NonUnitMeshRenderer
		}

		public MeshType meshType;
		
		public UnityEvent findEvent;
		
		public bool disableMesh;
		public bool disableParticles;
		public bool play = true;
	}
}
