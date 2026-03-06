using System;

namespace UnityEngine.InputSystem.LowLevel
{
	internal static class InputUpdate
	{
		internal static void OnBeforeUpdate(InputUpdateType type)
		{
			InputUpdate.s_LatestUpdateType = type;
			if (type - InputUpdateType.Dynamic <= 1 || type == InputUpdateType.Manual)
			{
				InputUpdate.s_PlayerUpdateStepCount.OnBeforeUpdate();
				InputUpdate.s_UpdateStepCount = InputUpdate.s_PlayerUpdateStepCount.value;
			}
		}

		internal static void OnUpdate(InputUpdateType type)
		{
			InputUpdate.s_LatestUpdateType = type;
			if (type - InputUpdateType.Dynamic <= 1 || type == InputUpdateType.Manual)
			{
				InputUpdate.s_PlayerUpdateStepCount.OnUpdate();
				InputUpdate.s_UpdateStepCount = InputUpdate.s_PlayerUpdateStepCount.value;
			}
		}

		public static InputUpdate.SerializedState Save()
		{
			return new InputUpdate.SerializedState
			{
				lastUpdateType = InputUpdate.s_LatestUpdateType,
				playerUpdateStepCount = InputUpdate.s_PlayerUpdateStepCount
			};
		}

		public static void Restore(InputUpdate.SerializedState state)
		{
			InputUpdate.s_LatestUpdateType = state.lastUpdateType;
			InputUpdate.s_PlayerUpdateStepCount = state.playerUpdateStepCount;
			InputUpdateType inputUpdateType = InputUpdate.s_LatestUpdateType;
			if (inputUpdateType - InputUpdateType.Dynamic <= 1 || inputUpdateType == InputUpdateType.Manual)
			{
				InputUpdate.s_UpdateStepCount = InputUpdate.s_PlayerUpdateStepCount.value;
				return;
			}
			InputUpdate.s_UpdateStepCount = 0U;
		}

		public static InputUpdateType GetUpdateTypeForPlayer(this InputUpdateType mask)
		{
			if ((mask & InputUpdateType.Manual) != InputUpdateType.None)
			{
				return InputUpdateType.Manual;
			}
			if ((mask & InputUpdateType.Dynamic) != InputUpdateType.None)
			{
				return InputUpdateType.Dynamic;
			}
			if ((mask & InputUpdateType.Fixed) != InputUpdateType.None)
			{
				return InputUpdateType.Fixed;
			}
			return InputUpdateType.None;
		}

		public static bool IsPlayerUpdate(this InputUpdateType updateType)
		{
			return updateType != InputUpdateType.Editor && updateType > InputUpdateType.None;
		}

		public static uint s_UpdateStepCount;

		public static InputUpdateType s_LatestUpdateType;

		public static InputUpdate.UpdateStepCount s_PlayerUpdateStepCount;

		[Serializable]
		public struct UpdateStepCount
		{
			public uint value { readonly get; private set; }

			public void OnBeforeUpdate()
			{
				this.m_WasUpdated = true;
				uint value = this.value;
				this.value = value + 1U;
			}

			public void OnUpdate()
			{
				if (!this.m_WasUpdated)
				{
					uint value = this.value;
					this.value = value + 1U;
				}
				this.m_WasUpdated = false;
			}

			private bool m_WasUpdated;
		}

		[Serializable]
		public struct SerializedState
		{
			public InputUpdateType lastUpdateType;

			public InputUpdate.UpdateStepCount playerUpdateStepCount;
		}
	}
}
