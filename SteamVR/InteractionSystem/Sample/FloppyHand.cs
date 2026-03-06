using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class FloppyHand : MonoBehaviour
	{
		private void Start()
		{
			for (int i = 0; i < this.fingers.Length; i++)
			{
				this.fingers[i].Init();
				this.fingers[i].flexAngle = this.fingerFlexAngle;
				this.fingers[i].squeezyAction = this.squeezyAction;
				this.fingers[i].inputSource = this.inputSource;
			}
		}

		private void Update()
		{
			for (int i = 0; i < this.fingers.Length; i++)
			{
				this.fingers[i].ApplyForce(this.constforce);
				this.fingers[i].UpdateFinger(Time.deltaTime);
				this.fingers[i].ApplyTransforms();
			}
		}

		protected float fingerFlexAngle = 140f;

		public SteamVR_Action_Single squeezyAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze", false);

		public SteamVR_Input_Sources inputSource;

		public FloppyHand.Finger[] fingers;

		public Vector3 constforce;

		[Serializable]
		public class Finger
		{
			public void ApplyForce(Vector3 worldForce)
			{
				for (int i = 0; i < this.startRot.Length; i++)
				{
					this.velocity[i] += worldForce / 50f;
				}
			}

			public void Init()
			{
				this.startRot = new Quaternion[this.bones.Length];
				this.rotation = new Vector3[this.bones.Length];
				this.velocity = new Vector3[this.bones.Length];
				this.oldTipPosition = new Vector3[this.bones.Length];
				this.oldTipDelta = new Vector3[this.bones.Length];
				this.boneTips = new Transform[this.bones.Length];
				this.inertiaSmoothing = new Vector3[this.bones.Length, this.inertiaSteps];
				for (int i = 0; i < this.bones.Length; i++)
				{
					this.startRot[i] = this.bones[i].localRotation;
					if (i < this.bones.Length - 1)
					{
						this.boneTips[i] = this.bones[i + 1];
					}
				}
			}

			public void UpdateFinger(float deltaTime)
			{
				if (deltaTime == 0f)
				{
					return;
				}
				float f = 0f;
				if (this.squeezyAction != null && this.squeezyAction.GetActive(this.inputSource))
				{
					f = this.squeezyAction.GetAxis(this.inputSource);
				}
				this.squeezySmooth = Mathf.Lerp(this.squeezySmooth, Mathf.Sqrt(f), deltaTime * 10f);
				if (this.renderer.sharedMesh.blendShapeCount > 0)
				{
					this.renderer.SetBlendShapeWeight(0, this.squeezySmooth * 100f);
				}
				float num = 0f;
				if (this.referenceAxis == FloppyHand.Finger.eulerAxis.X)
				{
					num = this.referenceBone.localEulerAngles.x;
				}
				if (this.referenceAxis == FloppyHand.Finger.eulerAxis.Y)
				{
					num = this.referenceBone.localEulerAngles.y;
				}
				if (this.referenceAxis == FloppyHand.Finger.eulerAxis.Z)
				{
					num = this.referenceBone.localEulerAngles.z;
				}
				num = this.FixAngle(num);
				this.pos = Mathf.InverseLerp(this.referenceAngles.x, this.referenceAngles.y, num);
				if (this.mass > 0f)
				{
					for (int i = 0; i < this.bones.Length; i++)
					{
						bool flag = this.boneTips[i] != null;
						if (flag)
						{
							Vector3 vector = (this.boneTips[i].localPosition - this.bones[i].InverseTransformPoint(this.oldTipPosition[i])) / deltaTime;
							Vector3 vector2 = (vector - this.oldTipDelta[i]) / deltaTime;
							this.oldTipDelta[i] = vector;
							Vector3 b = vector * -2f;
							vector2 *= -2f;
							for (int j = this.inertiaSteps - 1; j > 0; j--)
							{
								this.inertiaSmoothing[i, j] = this.inertiaSmoothing[i, j - 1];
							}
							this.inertiaSmoothing[i, 0] = vector2;
							Vector3 vector3 = Vector3.zero;
							for (int k = 0; k < this.inertiaSteps; k++)
							{
								vector3 += this.inertiaSmoothing[i, k];
							}
							vector3 /= (float)this.inertiaSteps;
							vector3 = this.PowVector(vector3 / 20f, 3f) * 20f;
							Vector3 fromDirection = this.forwardAxis;
							Vector3 toDirection = this.forwardAxis + b;
							Vector3 toDirection2 = this.forwardAxis + vector3;
							Quaternion quaternion = Quaternion.FromToRotation(fromDirection, toDirection);
							Quaternion quaternion2 = Quaternion.FromToRotation(fromDirection, toDirection2);
							this.velocity[i] += this.FixVector(quaternion.eulerAngles) * 2f * deltaTime;
							this.velocity[i] += this.FixVector(quaternion2.eulerAngles) * 50f * deltaTime;
							this.velocity[i] = Vector3.ClampMagnitude(this.velocity[i], 1000f);
						}
						Vector3 b2 = this.pos * Vector3.right * (this.flexAngle / (float)this.bones.Length);
						Vector3 a = -this.k * (this.rotation[i] - b2);
						Vector3 b3 = this.damping * this.velocity[i];
						Vector3 a2 = (a - b3) / this.mass;
						this.velocity[i] += a2 * deltaTime;
						this.rotation[i] += this.velocity[i] * Time.deltaTime;
						this.rotation[i] = Vector3.ClampMagnitude(this.rotation[i], 180f);
						if (flag)
						{
							this.oldTipPosition[i] = this.boneTips[i].position;
						}
					}
					return;
				}
				Debug.LogError("<b>[SteamVR Interaction]</b> finger mass is zero");
			}

			public void ApplyTransforms()
			{
				for (int i = 0; i < this.bones.Length; i++)
				{
					this.bones[i].localRotation = this.startRot[i];
					this.bones[i].Rotate(this.rotation[i], Space.Self);
				}
			}

			private Vector3 FixVector(Vector3 ang)
			{
				return new Vector3(this.FixAngle(ang.x), this.FixAngle(ang.y), this.FixAngle(ang.z));
			}

			private float FixAngle(float ang)
			{
				if (ang > 180f)
				{
					ang = -360f + ang;
				}
				return ang;
			}

			private Vector3 PowVector(Vector3 vector, float power)
			{
				Vector3 vector2 = new Vector3(Mathf.Sign(vector.x), Mathf.Sign(vector.y), Mathf.Sign(vector.z));
				vector.x = Mathf.Pow(Mathf.Abs(vector.x), power) * vector2.x;
				vector.y = Mathf.Pow(Mathf.Abs(vector.y), power) * vector2.y;
				vector.z = Mathf.Pow(Mathf.Abs(vector.z), power) * vector2.z;
				return vector;
			}

			public float mass;

			[Range(0f, 1f)]
			public float pos;

			public Vector3 forwardAxis;

			public SkinnedMeshRenderer renderer;

			[HideInInspector]
			public SteamVR_Action_Single squeezyAction;

			public SteamVR_Input_Sources inputSource;

			public Transform[] bones;

			public Transform referenceBone;

			public Vector2 referenceAngles;

			public FloppyHand.Finger.eulerAxis referenceAxis;

			[HideInInspector]
			public float flexAngle;

			private Vector3[] rotation;

			private Vector3[] velocity;

			private Transform[] boneTips;

			private Vector3[] oldTipPosition;

			private Vector3[] oldTipDelta;

			private Vector3[,] inertiaSmoothing;

			private float squeezySmooth;

			private int inertiaSteps = 10;

			private float k = 400f;

			private float damping = 8f;

			private Quaternion[] startRot;

			public enum eulerAxis
			{
				X,
				Y,
				Z
			}
		}
	}
}
