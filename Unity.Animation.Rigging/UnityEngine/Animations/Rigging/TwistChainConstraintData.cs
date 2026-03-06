using System;

namespace UnityEngine.Animations.Rigging
{
	[Serializable]
	public struct TwistChainConstraintData : IAnimationJobData, ITwistChainConstraintData
	{
		public Transform root
		{
			get
			{
				return this.m_Root;
			}
			set
			{
				this.m_Root = value;
			}
		}

		public Transform tip
		{
			get
			{
				return this.m_Tip;
			}
			set
			{
				this.m_Tip = value;
			}
		}

		public Transform rootTarget
		{
			get
			{
				return this.m_RootTarget;
			}
			set
			{
				this.m_RootTarget = value;
			}
		}

		public Transform tipTarget
		{
			get
			{
				return this.m_TipTarget;
			}
			set
			{
				this.m_TipTarget = value;
			}
		}

		public AnimationCurve curve
		{
			get
			{
				return this.m_Curve;
			}
			set
			{
				this.m_Curve = value;
			}
		}

		bool IAnimationJobData.IsValid()
		{
			return !(this.root == null) && !(this.tip == null) && this.tip.IsChildOf(this.root) && !(this.rootTarget == null) && !(this.tipTarget == null) && this.curve != null;
		}

		void IAnimationJobData.SetDefaultValues()
		{
			this.root = (this.tip = (this.rootTarget = (this.tipTarget = null)));
			this.curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		}

		[SerializeField]
		private Transform m_Root;

		[SerializeField]
		private Transform m_Tip;

		[SyncSceneToStream]
		[SerializeField]
		private Transform m_RootTarget;

		[SyncSceneToStream]
		[SerializeField]
		private Transform m_TipTarget;

		[SerializeField]
		private AnimationCurve m_Curve;
	}
}
