using System;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	public struct InstantiationParameters
	{
		public Vector3 Position
		{
			get
			{
				return this.m_Position;
			}
		}

		public Quaternion Rotation
		{
			get
			{
				return this.m_Rotation;
			}
		}

		public Transform Parent
		{
			get
			{
				return this.m_Parent;
			}
		}

		public bool InstantiateInWorldPosition
		{
			get
			{
				return this.m_InstantiateInWorldPosition;
			}
		}

		public bool SetPositionRotation
		{
			get
			{
				return this.m_SetPositionRotation;
			}
		}

		public InstantiationParameters(Transform parent, bool instantiateInWorldSpace)
		{
			this.m_Position = Vector3.zero;
			this.m_Rotation = Quaternion.identity;
			this.m_Parent = parent;
			this.m_InstantiateInWorldPosition = instantiateInWorldSpace;
			this.m_SetPositionRotation = false;
		}

		public InstantiationParameters(Vector3 position, Quaternion rotation, Transform parent)
		{
			this.m_Position = position;
			this.m_Rotation = rotation;
			this.m_Parent = parent;
			this.m_InstantiateInWorldPosition = false;
			this.m_SetPositionRotation = true;
		}

		public TObject Instantiate<TObject>(TObject source) where TObject : Object
		{
			TObject result;
			if (this.m_Parent == null)
			{
				if (this.m_SetPositionRotation)
				{
					result = Object.Instantiate<TObject>(source, this.m_Position, this.m_Rotation);
				}
				else
				{
					result = Object.Instantiate<TObject>(source);
				}
			}
			else if (this.m_SetPositionRotation)
			{
				result = Object.Instantiate<TObject>(source, this.m_Position, this.m_Rotation, this.m_Parent);
			}
			else
			{
				result = Object.Instantiate<TObject>(source, this.m_Parent, this.m_InstantiateInWorldPosition);
			}
			return result;
		}

		private Vector3 m_Position;

		private Quaternion m_Rotation;

		private Transform m_Parent;

		private bool m_InstantiateInWorldPosition;

		private bool m_SetPositionRotation;
	}
}
