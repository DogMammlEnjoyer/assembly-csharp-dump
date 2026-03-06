using System;
using UnityEngine.Serialization;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public class RigLayer : IRigLayer
	{
		public Rig rig
		{
			get
			{
				return this.m_Rig;
			}
			private set
			{
				this.m_Rig = value;
			}
		}

		public bool active
		{
			get
			{
				return this.m_Active;
			}
			set
			{
				this.m_Active = value;
			}
		}

		public string name
		{
			get
			{
				if (!(this.rig != null))
				{
					return "no-name";
				}
				return this.rig.gameObject.name;
			}
		}

		public IRigConstraint[] constraints
		{
			get
			{
				if (!this.isInitialized)
				{
					return null;
				}
				return this.m_Constraints;
			}
		}

		public IAnimationJob[] jobs
		{
			get
			{
				if (!this.isInitialized)
				{
					return null;
				}
				return this.m_Jobs;
			}
		}

		public bool isInitialized { get; private set; }

		public RigLayer(Rig rig, bool active = true)
		{
			this.rig = rig;
			this.active = active;
		}

		public bool Initialize(Animator animator)
		{
			if (this.isInitialized)
			{
				return true;
			}
			if (!(this.rig != null))
			{
				return false;
			}
			this.m_Constraints = RigUtils.GetConstraints(this.rig);
			if (this.m_Constraints == null || this.m_Constraints.Length == 0)
			{
				return false;
			}
			this.m_Jobs = RigUtils.CreateAnimationJobs(animator, this.m_Constraints);
			return this.isInitialized = true;
		}

		public void Update()
		{
			if (!this.isInitialized)
			{
				return;
			}
			int i = 0;
			int num = this.m_Constraints.Length;
			while (i < num)
			{
				this.m_Constraints[i].UpdateJob(this.m_Jobs[i]);
				i++;
			}
		}

		public void Reset()
		{
			if (!this.isInitialized)
			{
				return;
			}
			RigUtils.DestroyAnimationJobs(this.m_Constraints, this.m_Jobs);
			this.m_Constraints = null;
			this.m_Jobs = null;
			this.isInitialized = false;
		}

		public bool IsValid()
		{
			return this.rig != null && this.isInitialized;
		}

		[SerializeField]
		[FormerlySerializedAs("rig")]
		private Rig m_Rig;

		[SerializeField]
		[FormerlySerializedAs("active")]
		private bool m_Active = true;

		private IRigConstraint[] m_Constraints;

		private IAnimationJob[] m_Jobs;
	}
}
