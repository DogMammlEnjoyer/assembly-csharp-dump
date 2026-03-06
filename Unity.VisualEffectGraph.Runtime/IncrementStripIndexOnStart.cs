using System;

namespace UnityEngine.VFX
{
	internal class IncrementStripIndexOnStart : VFXSpawnerCallbacks
	{
		public override void OnPlay(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
			this.m_Index = (this.m_Index + 1U) % Math.Max(1U, vfxValues.GetUInt(IncrementStripIndexOnStart.stripMaxCountID));
			state.vfxEventAttribute.SetUint(IncrementStripIndexOnStart.stripIndexID, this.m_Index);
		}

		public override void OnStop(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
			this.m_Index = 0U;
		}

		public override void OnUpdate(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
		}

		private static readonly int stripMaxCountID = Shader.PropertyToID("StripMaxCount");

		private static readonly int stripIndexID = Shader.PropertyToID("stripIndex");

		private uint m_Index;

		public class InputProperties
		{
			[Tooltip("Maximum Strip Count (Used to cycle indices)")]
			public uint StripMaxCount = 8U;
		}
	}
}
