using System;

namespace UnityEngine.InputSystem
{
	[Serializable]
	public struct InputActionProperty : IEquatable<InputActionProperty>, IEquatable<InputAction>, IEquatable<InputActionReference>
	{
		public InputAction action
		{
			get
			{
				if (!this.m_UseReference)
				{
					return this.m_Action;
				}
				if (!(this.m_Reference != null))
				{
					return null;
				}
				return this.m_Reference.action;
			}
		}

		public InputActionReference reference
		{
			get
			{
				if (!this.m_UseReference)
				{
					return null;
				}
				return this.m_Reference;
			}
		}

		internal InputAction serializedAction
		{
			get
			{
				return this.m_Action;
			}
		}

		internal InputActionReference serializedReference
		{
			get
			{
				return this.m_Reference;
			}
		}

		public InputActionProperty(InputAction action)
		{
			this.m_UseReference = false;
			this.m_Action = action;
			this.m_Reference = null;
		}

		public InputActionProperty(InputActionReference reference)
		{
			this.m_UseReference = true;
			this.m_Action = null;
			this.m_Reference = reference;
		}

		public bool Equals(InputActionProperty other)
		{
			return this.m_Reference == other.m_Reference && this.m_UseReference == other.m_UseReference && this.m_Action == other.m_Action;
		}

		public bool Equals(InputAction other)
		{
			return this.action == other;
		}

		public bool Equals(InputActionReference other)
		{
			return this.m_Reference == other;
		}

		public override bool Equals(object obj)
		{
			if (this.m_UseReference)
			{
				return this.Equals(obj as InputActionReference);
			}
			return this.Equals(obj as InputAction);
		}

		public override int GetHashCode()
		{
			if (this.m_UseReference)
			{
				if (!(this.m_Reference != null))
				{
					return 0;
				}
				return this.m_Reference.GetHashCode();
			}
			else
			{
				if (this.m_Action == null)
				{
					return 0;
				}
				return this.m_Action.GetHashCode();
			}
		}

		public static bool operator ==(InputActionProperty left, InputActionProperty right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(InputActionProperty left, InputActionProperty right)
		{
			return !left.Equals(right);
		}

		[SerializeField]
		private bool m_UseReference;

		[SerializeField]
		private InputAction m_Action;

		[SerializeField]
		private InputActionReference m_Reference;
	}
}
