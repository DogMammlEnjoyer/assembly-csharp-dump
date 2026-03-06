using System;
using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using UnityEngine;

namespace Oculus.Interaction
{
	public class JointDeltaProviderRef : MonoBehaviour, IJointDeltaProvider
	{
		public IJointDeltaProvider JointDeltaProvider { get; private set; }

		protected virtual void Awake()
		{
			this.JointDeltaProvider = (this._jointDeltaProvider as IJointDeltaProvider);
		}

		protected virtual void Start()
		{
		}

		public bool GetPositionDelta(HandJointId joint, out Vector3 delta)
		{
			return this.JointDeltaProvider.GetPositionDelta(joint, out delta);
		}

		public bool GetRotationDelta(HandJointId joint, out Quaternion delta)
		{
			return this.JointDeltaProvider.GetRotationDelta(joint, out delta);
		}

		public void RegisterConfig(JointDeltaConfig config)
		{
			this.JointDeltaProvider.RegisterConfig(config);
		}

		public void UnRegisterConfig(JointDeltaConfig config)
		{
			this.JointDeltaProvider.UnRegisterConfig(config);
		}

		public void InjectAllJointDeltaProviderRef(IJointDeltaProvider jointDeltaProvider)
		{
			this.InjectJointDeltaProvider(jointDeltaProvider);
		}

		public void InjectJointDeltaProvider(IJointDeltaProvider jointDeltaProvider)
		{
			this._jointDeltaProvider = (jointDeltaProvider as Object);
			this.JointDeltaProvider = jointDeltaProvider;
		}

		[SerializeField]
		[Interface(typeof(IJointDeltaProvider), new Type[]
		{

		})]
		private Object _jointDeltaProvider;
	}
}
