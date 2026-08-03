using System.Collections.Generic;
using EzECS.Barriers;
using Landfall.TABS.AI.Components;
using Landfall.TABS.AI.Components.Pathfinding;
using Pathfinding;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TGCore.Library
{
	[UpdateAfter(typeof(PreUpdateBarrier))]
	[UpdateBefore(typeof(UpdateBarrier))]
	public class GoToPointPathSystem : ComponentSystem
	{
		private struct Filter
		{
			public EntityArray Entities;

			public ComponentDataArray<PointPathSettings> PathSettings;

			[ReadOnly]
			public ComponentDataArray<GroundPosition> GroundPositions;

			[ReadOnly]
			public ComponentDataArray<PointTag> PointTags;

			[ReadOnly]
			public ComponentDataArray<Navmesh> NavmeshTypes;

			[ReadOnly]
			public SubtractiveComponent<IsInPool> IsInPool;

			public readonly int Length;
		}

		[Inject]
		private Filter m_filter;

		[Inject]
		private UpdateBarrier m_barrier;

		private Dictionary<int, Entity> m_pathIDs = new Dictionary<int, Entity>();

		private Seeker m_seeker;

		private AstarPath m_pathObject;

		private bool m_hasLargeUnitNavmesh;

		protected override void OnStartRunning()
		{
			m_seeker = Object.FindObjectOfType<Seeker>();
			m_pathObject = Object.FindObjectOfType<AstarPath>();
			base.OnCreateManager();
		}

		protected override void OnUpdate()
		{
			if (m_pathObject != null)
			{
				m_hasLargeUnitNavmesh = ((AstarData.active.data.graphs.Length > 1) ? true : false);
				for (var i = 0; i < m_filter.Length; i++)
				{
					var entity = m_filter.Entities[i];
					var componentData = m_filter.PathSettings[i];
					var navmesh = m_filter.NavmeshTypes[i];
					componentData.CurrentRate -= Time.deltaTime;
					if (componentData.CurrentRate <= 0f)
					{
						var groundPosition = m_filter.GroundPositions[i];
						var pointTag = m_filter.PointTags[i];
						componentData.CurrentRate = componentData.RepathRate;
						var aBPath = ABPath.Construct(groundPosition.Value, pointTag.Value, OnPathComplete);
						var none = NNConstraint.None;
						var num = (int)navmesh.Value;
						if (!m_hasLargeUnitNavmesh)
						{
							num = 0;
						}
						var num2 = 1 << num;
						none.graphMask = num2;
						aBPath.nnConstraint = none;
						m_pathIDs.Add(aBPath.pathID, entity);
						AstarPath.StartPath(aBPath);
					}
					EntityManager.SetComponentData(entity, componentData);
				}
				return;
			}
			for (var j = 0; j < m_filter.Length; j++)
			{
				var entity2 = m_filter.Entities[j];
				var componentData3 = EntityManager.GetComponentData<HipPosition>(entity2);
				var pointTag = EntityManager.GetComponentData<PointTag>(entity2);
				var buffer = EntityManager.GetBuffer<PathPoint>(entity2);
				buffer.Clear();
				buffer.Add(new PathPoint
				{
					Value = componentData3.Value
				});
				buffer.Add(new PathPoint
				{
					Value = pointTag.Value
				});
				var x = pointTag.Value - componentData3.Value;
				EntityManager.SetComponentData(entity2, new PathDistance
				{
					Distance = math.length(x)
				});
				EntityManager.SetComponentData(entity2, new CurrentWaypoint
				{
					Value = 0
				});
			}
		}

		private void OnPathComplete(Path path)
		{
			m_seeker.RunModifiers(Seeker.ModifierPass.PostProcess, path);
			int pathID = path.pathID;
			var entity = m_pathIDs[pathID];
			m_pathIDs.Remove(pathID);
			if (!EntityManager.Exists(entity))
			{
				return;
			}
			if (path.CompleteState == PathCompleteState.Complete || path.CompleteState == PathCompleteState.Partial)
			{
				var num = 0f;
				var buffer = EntityManager.GetBuffer<PathPoint>(entity);
				buffer.Clear();
				var vectorPath = path.vectorPath;
				for (var i = 0; i < vectorPath.Count; i++)
				{
					buffer.Add(new PathPoint
					{
						Value = vectorPath[i]
					});
					if (i > 0)
					{
						var vector = vectorPath[i - 1];
						var magnitude = (vectorPath[i] - vector).magnitude;
						num += magnitude;
					}
				}
				EntityManager.SetComponentData(entity, new CurrentWaypoint
				{
					Value = 0
				});
				EntityManager.SetComponentData(entity, new PathDistance
				{
					Distance = num
				});
			}
			else
			{
				var hipPosition = EntityManager.GetComponentData<HipPosition>(entity);
				var pointTag = EntityManager.GetComponentData<PointTag>(entity);
				var pathPointBuffer = EntityManager.GetBuffer<PathPoint>(entity);
				pathPointBuffer.Clear();
				pathPointBuffer.Add(new PathPoint
				{
					Value = hipPosition.Value
				});
				pathPointBuffer.Add(new PathPoint
				{
					Value = pointTag.Value
				});
				var x = pointTag.Value - hipPosition.Value;
				EntityManager.SetComponentData(entity, new PathDistance
				{
					Distance = math.length(x)
				});
				EntityManager.SetComponentData(entity, new CurrentWaypoint
				{
					Value = 0
				});
			}
		}
	}
}
