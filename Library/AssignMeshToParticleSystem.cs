using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
	public class AssignMeshToParticleSystem : MonoBehaviour
	{

		private void Start()
		{
			Part = GetComponent<ParticleSystem>();

			switch (meshType)
			{
				case MeshType.MeshRenderer:
				{
					AllMeshRenderers = transform.root.GetComponentsInChildren<MeshRenderer>();
					foreach (var renderer in AllMeshRenderers)
					{
						if (renderer.CompareTag("UnitMesh") && !MeshAssigned)
						{
							MeshRenderer = renderer;
							MeshAssigned = true;
						}
						if (disableMesh) renderer.enabled = false;
					}
					if (MeshRenderer)
					{
						var shape = Part.shape;
						shape.meshRenderer = MeshRenderer;
					
						findEvent.Invoke();
						if (play) Part.Play();
					}
				
					if (!disableParticles) return;
				
					ParticleSystems = transform.root.GetComponentsInChildren<ParticleSystem>();
					foreach (var particle in ParticleSystems)
					{
						if (!particle.CompareTag("DontRemove")) particle.Stop();
					}

					break;
				}
				case MeshType.SkinnedMeshRenderer:
				{
					AllSkinnedMeshRenderers = transform.root.GetComponentsInChildren<SkinnedMeshRenderer>();
					foreach (var renderer in AllSkinnedMeshRenderers)
					{
						if (renderer.CompareTag("UnitMesh"))
						{
							SkinnedMeshRend = renderer;
							break;
						}
					}
					if (SkinnedMeshRend)
					{
						var shape = GetComponent<ParticleSystem>().shape;
						shape.skinnedMeshRenderer = SkinnedMeshRend;
					
						findEvent.Invoke();
						if (play) Part.Play();
					}

					break;
				}
				case MeshType.NonUnitMeshRenderer:
				default:
				{
					AllMeshRenderers = transform.root.GetComponentsInChildren<MeshRenderer>();
					foreach (var renderer in AllMeshRenderers)
					{
						if (!MeshAssigned)
						{
							MeshRenderer = renderer;
							MeshAssigned = true;
						}
						if (disableMesh) renderer.enabled = false;
					}
					if (MeshRenderer)
					{
						var shape = Part.shape;
						shape.meshRenderer = MeshRenderer;
					
						findEvent.Invoke();
						if (play) Part.Play();
					}

					break;
				}
			}
		}
		
		private MeshRenderer[] AllMeshRenderers;
		private MeshRenderer MeshRenderer;
		private SkinnedMeshRenderer[] AllSkinnedMeshRenderers;
		private SkinnedMeshRenderer SkinnedMeshRend;
		private ParticleSystem[] ParticleSystems;
		private ParticleSystem Part;
		private bool MeshAssigned;
		
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
