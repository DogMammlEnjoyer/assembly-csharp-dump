using System;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace UnityEngine.VFX.Utility
{
	[AddComponentMenu("VFX/Property Binders/Input Mouse Binder")]
	[VFXBinder("Input/Mouse")]
	internal class VFXInputMouseBinder : VFXBinderBase
	{
		public string MouseLeftClickProperty
		{
			get
			{
				return (string)this.m_MouseLeftClickProperty;
			}
			set
			{
				this.m_MouseLeftClickProperty = value;
			}
		}

		public string MouseRightClickProperty
		{
			get
			{
				return (string)this.m_MouseRightClickProperty;
			}
			set
			{
				this.m_MouseRightClickProperty = value;
			}
		}

		public string PositionProperty
		{
			get
			{
				return (string)this.m_PositionProperty;
			}
			set
			{
				this.m_PositionProperty = value;
			}
		}

		public string VelocityProperty
		{
			get
			{
				return (string)this.m_VelocityProperty;
			}
			set
			{
				this.m_VelocityProperty = value;
			}
		}

		public override bool IsValid(VisualEffect component)
		{
			return component.HasVector3(this.m_PositionProperty) && (!this.CheckLeftClick || component.HasBool(this.m_MouseLeftClickProperty)) && (!this.CheckRightClick || component.HasBool(this.m_MouseRightClickProperty)) && (!this.SetVelocity || component.HasVector3(this.m_VelocityProperty));
		}

		public override void UpdateBinding(VisualEffect component)
		{
			Vector3 vector = Vector3.zero;
			if (this.CheckLeftClick)
			{
				component.SetBool(this.MouseLeftClickProperty, this.IsLeftClickPressed());
			}
			if (this.CheckRightClick)
			{
				component.SetBool(this.MouseRightClickProperty, this.IsRightClickPressed());
			}
			if (this.Target != null)
			{
				Vector3 position = this.GetMousePosition();
				position.z = this.Distance;
				vector = this.Target.ScreenToWorldPoint(position);
			}
			else
			{
				vector = this.GetMousePosition();
			}
			component.SetVector3(this.m_PositionProperty, vector);
			if (this.SetVelocity)
			{
				component.SetVector3(this.m_VelocityProperty, (vector - this.m_PreviousPosition) / Time.deltaTime);
			}
			this.m_PreviousPosition = vector;
		}

		private bool IsRightClickPressed()
		{
			return Mouse.current != null && Mouse.current.rightButton.isPressed;
		}

		private bool IsLeftClickPressed()
		{
			return Mouse.current != null && Mouse.current.leftButton.isPressed;
		}

		private Vector2 GetMousePosition()
		{
			return Pointer.current.position.ReadValue();
		}

		public override string ToString()
		{
			return string.Format("Mouse: '{0}' -> {1}", this.m_PositionProperty, (this.Target == null) ? "(null)" : this.Target.name);
		}

		[VFXPropertyBinding(new string[]
		{
			"System.Boolean"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_MouseLeftClickParameter")]
		protected ExposedProperty m_MouseLeftClickProperty = "LeftClick";

		[VFXPropertyBinding(new string[]
		{
			"System.Boolean"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_MouseRightClickParameter")]
		protected ExposedProperty m_MouseRightClickProperty = "RightClick";

		[VFXPropertyBinding(new string[]
		{
			"UnityEditor.VFX.Position",
			"UnityEngine.Vector3"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_PositionParameter")]
		protected ExposedProperty m_PositionProperty = "Position";

		[VFXPropertyBinding(new string[]
		{
			"UnityEngine.Vector3"
		})]
		[SerializeField]
		[FormerlySerializedAs("m_VelocityParameter")]
		protected ExposedProperty m_VelocityProperty = "Velocity";

		public Camera Target;

		public float Distance = 10f;

		public bool SetVelocity;

		public bool CheckLeftClick = true;

		public bool CheckRightClick;

		private Vector3 m_PreviousPosition;
	}
}
