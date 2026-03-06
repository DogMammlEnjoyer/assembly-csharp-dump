using System;
using System.Collections.Generic;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct MultiReferentialConstraintData : IAnimationJobData, IMultiReferentialConstraintData
	{
		public int driver
		{
			get
			{
				return this.m_Driver;
			}
			set
			{
				this.m_Driver = Mathf.Clamp(value, 0, this.m_SourceObjects.Count - 1);
			}
		}

		public List<Transform> sourceObjects
		{
			get
			{
				if (this.m_SourceObjects == null)
				{
					this.m_SourceObjects = new List<Transform>();
				}
				return this.m_SourceObjects;
			}
			set
			{
				this.m_SourceObjects = value;
				this.m_Driver = Mathf.Clamp(this.m_Driver, 0, this.m_SourceObjects.Count - 1);
			}
		}

		Transform[] IMultiReferentialConstraintData.sourceObjects
		{
			get
			{
				return this.m_SourceObjects.ToArray();
			}
		}

		int IMultiReferentialConstraintData.driverValue
		{
			get
			{
				return this.m_Driver;
			}
		}

		string IMultiReferentialConstraintData.driverIntProperty
		{
			get
			{
				return ConstraintsUtils.ConstructConstraintDataPropertyName("m_Driver");
			}
		}

		bool IAnimationJobData.IsValid()
		{
			if (this.m_SourceObjects.Count < 2)
			{
				return false;
			}
			using (List<Transform>.Enumerator enumerator = this.m_SourceObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == null)
					{
						return false;
					}
				}
			}
			return true;
		}

		void IAnimationJobData.SetDefaultValues()
		{
			this.m_Driver = 0;
			this.m_SourceObjects = new List<Transform>();
		}

		public void UpdateDriver()
		{
			this.m_Driver = Mathf.Clamp(this.m_Driver, 0, (this.m_SourceObjects != null) ? (this.m_SourceObjects.Count - 1) : 0);
		}

		[SyncSceneToStream]
		[SerializeField]
		private int m_Driver;

		[SyncSceneToStream]
		[SerializeField]
		private List<Transform> m_SourceObjects;
	}
}
