using System;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Interactions
{
	[Preserve]
	public class SectorInteraction : IInputInteraction<Vector2>, IInputInteraction
	{
		internal float pressPointOrDefault
		{
			get
			{
				if (this.pressPoint < 0f)
				{
					return SectorInteraction.defaultPressPoint;
				}
				return this.pressPoint;
			}
		}

		public static float defaultPressPoint { get; set; } = 0.5f;

		public void Process(ref InputInteractionContext context)
		{
			if (!context.ControlIsActuated(this.pressPointOrDefault))
			{
				SectorInteraction.State state = this.m_State;
				if (state == SectorInteraction.State.Centered)
				{
					return;
				}
				if (state - SectorInteraction.State.StartedValidDirection > 1)
				{
					return;
				}
				this.m_State = SectorInteraction.State.Centered;
				context.Canceled();
				return;
			}
			else
			{
				bool flag = this.IsValidDirection(ref context);
				if (this.m_State == SectorInteraction.State.Centered)
				{
					this.m_State = (flag ? SectorInteraction.State.StartedValidDirection : SectorInteraction.State.StartedInvalidDirection);
					if (flag)
					{
						context.PerformedAndStayPerformed();
					}
					this.m_WasValidDirection = flag;
					return;
				}
				switch (this.sweepBehavior)
				{
				case SectorInteraction.SweepBehavior.AllowReentry:
					if (this.m_WasValidDirection && !flag && this.m_State == SectorInteraction.State.StartedValidDirection)
					{
						context.Canceled();
					}
					else if (!this.m_WasValidDirection && flag && this.m_State == SectorInteraction.State.StartedValidDirection)
					{
						context.PerformedAndStayPerformed();
					}
					break;
				case SectorInteraction.SweepBehavior.DisallowReentry:
					if (this.m_WasValidDirection && !flag && this.m_State == SectorInteraction.State.StartedValidDirection)
					{
						context.Canceled();
					}
					break;
				case SectorInteraction.SweepBehavior.HistoryIndependent:
					if (this.m_WasValidDirection && !flag)
					{
						context.Canceled();
					}
					else if (!this.m_WasValidDirection && flag)
					{
						context.PerformedAndStayPerformed();
					}
					break;
				}
				this.m_WasValidDirection = flag;
				return;
			}
		}

		private bool IsValidDirection(ref InputInteractionContext context)
		{
			return (SectorInteraction.GetNearestDirection(CardinalUtility.GetNearestCardinal(context.ReadValue<Vector2>())) & this.directions) > SectorInteraction.Directions.None;
		}

		private static SectorInteraction.Directions GetNearestDirection(Cardinal value)
		{
			switch (value)
			{
			case Cardinal.North:
				return SectorInteraction.Directions.North;
			case Cardinal.South:
				return SectorInteraction.Directions.South;
			case Cardinal.East:
				return SectorInteraction.Directions.East;
			case Cardinal.West:
				return SectorInteraction.Directions.West;
			default:
				return SectorInteraction.Directions.None;
			}
		}

		public void Reset()
		{
		}

		[Preserve]
		static SectorInteraction()
		{
			InputSystem.RegisterInteraction<SectorInteraction>(null);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		[Preserve]
		private static void Initialize()
		{
		}

		public SectorInteraction.Directions directions;

		public SectorInteraction.SweepBehavior sweepBehavior;

		public float pressPoint = -1f;

		private SectorInteraction.State m_State;

		private bool m_WasValidDirection;

		[Flags]
		public enum Directions
		{
			None = 0,
			North = 1,
			South = 2,
			East = 4,
			West = 8
		}

		public enum SweepBehavior
		{
			Locked,
			[InspectorName("Allow Reentry")]
			AllowReentry,
			[InspectorName("Disallow Reentry")]
			DisallowReentry,
			[InspectorName("History Independent")]
			HistoryIndependent
		}

		private enum State
		{
			Centered,
			StartedValidDirection,
			StartedInvalidDirection
		}
	}
}
