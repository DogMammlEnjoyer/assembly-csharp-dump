using System;
using UnityEngine.EventSystems;

namespace UnityEngine.InputSystem.UI
{
	internal struct NavigationModel
	{
		public void Reset()
		{
			this.move = Vector2.zero;
		}

		public Vector2 move;

		public int consecutiveMoveCount;

		public MoveDirection lastMoveDirection;

		public float lastMoveTime;

		public AxisEventData eventData;

		public InputDevice device;
	}
}
