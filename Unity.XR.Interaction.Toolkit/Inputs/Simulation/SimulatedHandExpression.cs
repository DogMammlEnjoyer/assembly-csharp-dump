using System;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
	[Serializable]
	public class SimulatedHandExpression : ISerializationCallbackReceiver
	{
		public string name
		{
			get
			{
				return this.m_ExpressionName.ToString();
			}
		}

		public XRInputButtonReader toggleInput
		{
			get
			{
				return this.m_ToggleInput;
			}
			set
			{
				this.m_ToggleInput = value;
			}
		}

		internal HandExpressionCapture capture
		{
			get
			{
				return this.m_Capture;
			}
			set
			{
				this.m_Capture = value;
			}
		}

		public bool isQuickAction
		{
			get
			{
				return this.m_IsQuickAction;
			}
			set
			{
				this.m_IsQuickAction = value;
			}
		}

		internal HandExpressionName expressionName
		{
			get
			{
				return this.m_ExpressionName;
			}
			set
			{
				this.m_ExpressionName = value;
			}
		}

		public Sprite icon
		{
			get
			{
				return this.m_Capture.icon;
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.m_Name = this.m_ExpressionName.ToString();
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.m_ExpressionName = new HandExpressionName(this.m_Name);
		}

		[SerializeField]
		[Tooltip("The unique name for the hand expression.")]
		[Delayed]
		private string m_Name;

		[SerializeField]
		[Tooltip("The input to trigger the simulated hand expression.")]
		private XRInputButtonReader m_ToggleInput;

		[SerializeField]
		[Tooltip("The captured hand expression to simulate when the input action is performed.")]
		private HandExpressionCapture m_Capture;

		[SerializeField]
		[Tooltip("Whether or not this expression appears in the quick action list in the simulator.")]
		private bool m_IsQuickAction;

		private HandExpressionName m_ExpressionName;
	}
}
