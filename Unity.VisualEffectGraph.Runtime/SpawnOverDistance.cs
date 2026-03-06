using System;

namespace UnityEngine.VFX
{
	internal class SpawnOverDistance : VFXSpawnerCallbacks
	{
		public sealed override void OnPlay(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
			this.m_OldPosition = vfxValues.GetVector3(SpawnOverDistance.positionPropertyId);
		}

		public sealed override void OnUpdate(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
			if (!state.playing || state.deltaTime == 0f)
			{
				return;
			}
			float @float = vfxValues.GetFloat(SpawnOverDistance.velocityThresholdPropertyId);
			Vector3 vector = vfxValues.GetVector3(SpawnOverDistance.positionPropertyId);
			float num = Vector3.Magnitude(this.m_OldPosition - vector);
			if (@float <= 0f || num < @float * state.deltaTime)
			{
				float num2 = num * vfxValues.GetFloat(SpawnOverDistance.ratePerUnitPropertyId);
				if (vfxValues.GetBool(SpawnOverDistance.clampToOnePropertyId))
				{
					num2 = Mathf.Min(num2, 1f);
				}
				state.spawnCount += num2;
				state.vfxEventAttribute.SetVector3(SpawnOverDistance.oldPositionAttributeId, this.m_OldPosition);
				state.vfxEventAttribute.SetVector3(SpawnOverDistance.positionAttributeId, vector);
			}
			this.m_OldPosition = vector;
		}

		public sealed override void OnStop(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
		}

		private Vector3 m_OldPosition;

		private static readonly int positionPropertyId = Shader.PropertyToID("Position");

		private static readonly int ratePerUnitPropertyId = Shader.PropertyToID("RatePerUnit");

		private static readonly int velocityThresholdPropertyId = Shader.PropertyToID("VelocityThreshold");

		private static readonly int clampToOnePropertyId = Shader.PropertyToID("ClampToOne");

		private static readonly int positionAttributeId = Shader.PropertyToID("position");

		private static readonly int oldPositionAttributeId = Shader.PropertyToID("oldPosition");

		public class InputProperties
		{
			public Vector3 Position = Vector3.zero;

			public float RatePerUnit = 10f;

			public float VelocityThreshold = 50f;

			public bool ClampToOne;
		}
	}
}
