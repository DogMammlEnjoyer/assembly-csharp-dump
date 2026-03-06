using System;

namespace UnityEngine.VFX
{
	internal class SetSpawnTime : VFXSpawnerCallbacks
	{
		public sealed override void OnPlay(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
		}

		public sealed override void OnUpdate(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
			state.vfxEventAttribute.SetFloat(SetSpawnTime.spawnTimeID, state.totalTime);
		}

		public sealed override void OnStop(VFXSpawnerState state, VFXExpressionValues vfxValues, VisualEffect vfxComponent)
		{
		}

		private static readonly int spawnTimeID = Shader.PropertyToID("spawnTime");
	}
}
