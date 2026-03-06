using System;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	internal struct NavigationModel
	{
		public Vector2 move { readonly get; set; }

		public bool submitButtonDown
		{
			get
			{
				return this.m_SubmitButtonDown;
			}
			set
			{
				if (this.m_SubmitButtonDown != value)
				{
					this.submitButtonDelta = (value ? ButtonDeltaState.Pressed : ButtonDeltaState.Released);
					this.m_SubmitButtonDown = value;
				}
			}
		}

		internal ButtonDeltaState submitButtonDelta { readonly get; private set; }

		public bool cancelButtonDown
		{
			get
			{
				return this.m_CancelButtonDown;
			}
			set
			{
				if (this.m_CancelButtonDown != value)
				{
					this.cancelButtonDelta = (value ? ButtonDeltaState.Pressed : ButtonDeltaState.Released);
					this.m_CancelButtonDown = value;
				}
			}
		}

		internal ButtonDeltaState cancelButtonDelta { readonly get; private set; }

		internal NavigationModel.ImplementationData implementationData { readonly get; set; }

		public void Reset()
		{
			this.move = Vector2.zero;
			this.m_SubmitButtonDown = (this.m_CancelButtonDown = false);
			this.submitButtonDelta = (this.cancelButtonDelta = ButtonDeltaState.NoChange);
			this.implementationData.Reset();
		}

		public void OnFrameFinished()
		{
			this.submitButtonDelta = ButtonDeltaState.NoChange;
			this.cancelButtonDelta = ButtonDeltaState.NoChange;
		}

		private bool m_SubmitButtonDown;

		private bool m_CancelButtonDown;

		public struct ImplementationData
		{
			public int consecutiveMoveCount { readonly get; set; }

			public MoveDirection lastMoveDirection { readonly get; set; }

			public float lastMoveTime { readonly get; set; }

			public void Reset()
			{
				this.consecutiveMoveCount = 0;
				this.lastMoveTime = 0f;
				this.lastMoveDirection = MoveDirection.None;
			}
		}
	}
}
