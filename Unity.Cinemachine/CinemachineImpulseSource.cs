using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[SaveDuringPlay]
	[AddComponentMenu("Cinemachine/Helpers/Cinemachine Impulse Source")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineImpulseSource.html")]
	public class CinemachineImpulseSource : MonoBehaviour
	{
		private void OnValidate()
		{
			this.ImpulseDefinition.OnValidate();
		}

		private void Reset()
		{
			this.ImpulseDefinition = new CinemachineImpulseDefinition
			{
				ImpulseChannel = 1,
				ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Bump,
				CustomImpulseShape = new AnimationCurve(),
				ImpulseDuration = 0.2f,
				ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform,
				DissipationDistance = 100f,
				DissipationRate = 0.25f,
				PropagationSpeed = 343f
			};
			this.DefaultVelocity = Vector3.down;
		}

		public void GenerateImpulseAtPositionWithVelocity(Vector3 position, Vector3 velocity)
		{
			if (this.ImpulseDefinition != null)
			{
				this.ImpulseDefinition.CreateEvent(position, velocity);
			}
		}

		public void GenerateImpulseWithVelocity(Vector3 velocity)
		{
			this.GenerateImpulseAtPositionWithVelocity(base.transform.position, velocity);
		}

		public void GenerateImpulseWithForce(float force)
		{
			this.GenerateImpulseAtPositionWithVelocity(base.transform.position, this.DefaultVelocity * force);
		}

		public void GenerateImpulse()
		{
			this.GenerateImpulseWithVelocity(this.DefaultVelocity);
		}

		public void GenerateImpulseAt(Vector3 position, Vector3 velocity)
		{
			this.GenerateImpulseAtPositionWithVelocity(position, velocity);
		}

		public void GenerateImpulse(Vector3 velocity)
		{
			this.GenerateImpulseWithVelocity(velocity);
		}

		public void GenerateImpulse(float force)
		{
			this.GenerateImpulseWithForce(force);
		}

		[FormerlySerializedAs("m_ImpulseDefinition")]
		public CinemachineImpulseDefinition ImpulseDefinition = new CinemachineImpulseDefinition();

		[Header("Default Invocation")]
		[Tooltip("The default direction and force of the Impulse Signal in the absense of any specified overrides.  Overrides can be specified by calling the appropriate GenerateImpulse method in the API.")]
		[FormerlySerializedAs("m_DefaultVelocity")]
		public Vector3 DefaultVelocity = Vector3.down;
	}
}
