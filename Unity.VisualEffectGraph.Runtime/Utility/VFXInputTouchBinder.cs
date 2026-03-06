using System;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Input Touch Binder")]
	[VFXBinder("Input/Touch")]
	internal class VFXInputTouchBinder : VFXBinderBase
	{
		public string TouchEnabledProperty
		{
			get
			{
				return (string)this.m_TouchEnabledProperty;
			}
			set
			{
				this.m_TouchEnabledProperty = value;
			}
		}

		public string Parameter
		{
			get
			{
				return (string)this.m_Parameter;
			}
			set
			{
				this.m_Parameter = value;
			}
		}

		public string VelocityParameter
		{
			get
			{
				return (string)this.m_VelocityParameter;
			}
			set
			{
				this.m_VelocityParameter = value;
			}
		}

		public override bool IsValid(VisualEffect component)
		{
			return this.Target != null && component.HasVector3(this.m_Parameter) && component.HasBool(this.m_TouchEnabledProperty) && (!this.SetVelocity || component.HasVector3(this.m_VelocityParameter));
		}

		public override void UpdateBinding(VisualEffect component)
		{
			Vector3 vector = Vector3.zero;
			bool previousTouch;
			if (this.GetTouchCount() > this.TouchIndex)
			{
				Vector2 touchPosition = this.GetTouchPosition(this.TouchIndex);
				previousTouch = true;
				Vector3 position = touchPosition;
				position.z = this.Distance;
				vector = this.Target.ScreenToWorldPoint(position);
				component.SetBool(this.m_TouchEnabledProperty, true);
				component.SetVector3(this.m_Parameter, vector);
			}
			else
			{
				previousTouch = false;
				component.SetBool(this.m_TouchEnabledProperty, false);
				component.SetVector3(this.m_Parameter, Vector3.zero);
			}
			if (this.SetVelocity)
			{
				if (this.m_PreviousTouch)
				{
					component.SetVector3(this.m_VelocityParameter, (vector - this.m_PreviousPosition) / Time.deltaTime);
				}
				else
				{
					component.SetVector3(this.m_VelocityParameter, Vector3.zero);
				}
			}
			this.m_PreviousTouch = previousTouch;
			this.m_PreviousPosition = vector;
		}

		private int GetTouchCount()
		{
			if (Touchscreen.current == null)
			{
				return 0;
			}
			return Touchscreen.current.touches.Count((TouchControl t) => t.IsPressed(0f));
		}

		private Vector2 GetTouchPosition(int touchIndex)
		{
			if (Touchscreen.current == null || touchIndex >= Touchscreen.current.touches.Count || touchIndex < 0)
			{
				return Vector2.zero;
			}
			return Touchscreen.current.touches[touchIndex].ReadValue().position;
		}

		public override string ToString()
		{
			return string.Format("Touch #{2} : '{0}' -> {1}", this.m_Parameter, (this.Target == null) ? "(null)" : this.Target.name, this.TouchIndex);
		}

		[VFXPropertyBinding(new string[]
		{
			"System.Boolean"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_TouchEnabledParameter")]
		protected ExposedProperty m_TouchEnabledProperty = "TouchEnabled";

		[VFXPropertyBinding(new string[]
		{
			"UnityEditor.VFX.Position",
			"UnityEngine.Vector3"
		})]
		[SerializeField]
		protected ExposedProperty m_Parameter = "Position";

		[VFXPropertyBinding(new string[]
		{
			"UnityEngine.Vector3"
		})]
		[SerializeField]
		protected ExposedProperty m_VelocityParameter = "Velocity";

		public int TouchIndex;

		public Camera Target;

		public float Distance = 10f;

		public bool SetVelocity;

		private Vector3 m_PreviousPosition;

		private bool m_PreviousTouch;
	}
}
