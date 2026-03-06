using System;

namespace UnityEngine.InputSystem.XR
{
	public struct Bone
	{
		public uint parentBoneIndex
		{
			get
			{
				return this.m_ParentBoneIndex;
			}
			set
			{
				this.m_ParentBoneIndex = value;
			}
		}

		public Vector3 position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				this.m_Position = value;
			}
		}

		public Quaternion rotation
		{
			get
			{
				return this.m_Rotation;
			}
			set
			{
				this.m_Rotation = value;
			}
		}

		public uint m_ParentBoneIndex;

		public Vector3 m_Position;

		public Quaternion m_Rotation;
	}
}
