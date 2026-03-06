using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TouchShadowHand
	{
		public ShadowHand ShadowHand
		{
			get
			{
				return this._shadowHand;
			}
		}

		public int TotalIterations
		{
			get
			{
				return this._totalIterations;
			}
			set
			{
				this._totalIterations = ((this._totalIterations > 0) ? this._totalIterations : 1);
			}
		}

		public int PushoutIterations
		{
			get
			{
				return this._pushoutIterations;
			}
			set
			{
				this._pushoutIterations = ((this._pushoutIterations > 0) ? this._pushoutIterations : 1);
			}
		}

		public TouchShadowHand(IHandSphereMap map, Handedness handedness, int iterations = 10)
		{
			this._handSphereMap = map;
			this._handedness = handedness;
			this.Iterations = iterations;
			this.TotalIterations = iterations;
			this.PushoutIterations = iterations;
		}

		public void SetShadowRootFromHand(ShadowHand hand)
		{
			Pose root = hand.GetRoot();
			this._shadowHand.SetRoot(root);
			this._shadowHand.SetRootScale(hand.GetRootScale());
		}

		public void SetShadowRootFromHands(ShadowHand from, ShadowHand to, float t)
		{
			Pose root = from.GetRoot();
			Pose root2 = to.GetRoot();
			ref root.Lerp(root2, t);
			this._shadowHand.SetRoot(root);
			this._shadowHand.SetRootScale(from.GetRootScale());
		}

		public void SetShadowFingerFrom(int fingerIdx, ShadowHand from)
		{
			foreach (HandJointId handJointId in FingersMetadata.FINGER_TO_JOINTS[fingerIdx])
			{
				Pose localPose = from.GetLocalPose(handJointId);
				this._shadowHand.SetLocalPose(handJointId, localPose);
			}
		}

		private void SetShadowFingerFromLerp(int fingerIdx, ShadowHand from, ShadowHand to, float t)
		{
			foreach (HandJointId handJointId in FingersMetadata.FINGER_TO_JOINTS[fingerIdx])
			{
				Pose localPose = from.GetLocalPose(handJointId);
				Pose localPose2 = to.GetLocalPose(handJointId);
				ref localPose.Lerp(localPose2, t);
				this._shadowHand.SetLocalPose(handJointId, localPose);
			}
		}

		private void SetShadowFingerFromLerps(int fingerIdx, ShadowHand from, ShadowHand to, float[] t)
		{
			HandJointId[] array = FingersMetadata.FINGER_TO_JOINTS[fingerIdx];
			for (int i = 0; i < array.Length; i++)
			{
				HandJointId handJointId = array[i];
				Pose localPose = from.GetLocalPose(handJointId);
				Pose localPose2 = to.GetLocalPose(handJointId);
				ref localPose.Lerp(localPose2, t[i]);
				this._shadowHand.SetLocalPose(handJointId, localPose);
			}
		}

		private void SetShadowFromLerpHands(ShadowHand from, ShadowHand to, float t)
		{
			Pose root = from.GetRoot();
			Pose root2 = to.GetRoot();
			ref root.Lerp(root2, t);
			this._shadowHand.SetRoot(root);
			this._shadowHand.SetRootScale(from.GetRootScale());
			for (int i = 0; i < 26; i++)
			{
				Pose localPose = from.GetLocalPose((HandJointId)i);
				Pose localPose2 = to.GetLocalPose((HandJointId)i);
				ref localPose.Lerp(localPose2, t);
				this._shadowHand.SetLocalPose((HandJointId)i, localPose);
			}
		}

		private void LoadSpheresForFingerFromShadow(int fingerIdx, int jointIdx = 0)
		{
			HandJointId[] array = FingersMetadata.FINGER_TO_JOINTS[fingerIdx];
			this._spheres.Clear();
			for (int i = jointIdx; i < array.Length; i++)
			{
				HandJointId handJointId = array[i];
				this._handSphereMap.GetSpheres(this._handedness, handJointId, this._shadowHand.GetWorldPose(handJointId), this._shadowHand.GetRootScale(), this._spheres);
			}
		}

		private void LoadSpheresForHandFromShadow()
		{
			this._spheres.Clear();
			for (int i = 0; i < 26; i++)
			{
				HandJointId handJointId = (HandJointId)i;
				this._handSphereMap.GetSpheres(this._handedness, handJointId, this._shadowHand.GetWorldPose(handJointId), this._shadowHand.GetRootScale(), this._spheres);
			}
		}

		private bool CheckSphereCollision(ColliderGroup colliderGroup, Vector3 offset, List<int> sphereHit = null, List<int> sphereIndices = null)
		{
			bool result = false;
			if (sphereHit != null)
			{
				sphereHit.Clear();
			}
			for (int i = 0; i < ((sphereIndices == null) ? this._spheres.Count : sphereIndices.Count); i++)
			{
				int num = (sphereIndices == null) ? i : sphereIndices[i];
				HandSphere handSphere = this._spheres[num];
				if (Collisions.IsSphereWithinCollider(handSphere.Position - offset, handSphere.Radius, colliderGroup.Bounds))
				{
					int j = 0;
					while (j < colliderGroup.Colliders.Count)
					{
						if (Collisions.IsSphereWithinCollider(handSphere.Position - offset, handSphere.Radius, colliderGroup.Colliders[j]))
						{
							result = true;
							if (sphereHit == null)
							{
								return true;
							}
							sphereHit.Add(num);
							break;
						}
						else
						{
							j++;
						}
					}
				}
			}
			return result;
		}

		public bool CheckFingerTouch(int fingerIdx, int jointIdx, ColliderGroup colliderGroup, Vector3 offset, List<int> sphereHit = null)
		{
			this.LoadSpheresForFingerFromShadow(fingerIdx, jointIdx);
			return this.CheckSphereCollision(colliderGroup, offset, sphereHit, null);
		}

		public void CheckTouchFingers(ShadowHand hand, ColliderGroup colliderGroup, TouchShadowHand.GrabTouchInfo result)
		{
			this._shadowHand.Copy(hand);
			for (int i = 0; i < 5; i++)
			{
				this.LoadSpheresForFingerFromShadow(i, 0);
				this._sphereHit.Clear();
				if (this.CheckFingerTouch(i, 0, colliderGroup, Vector3.zero, this._sphereHit))
				{
					result.grabbingFingers[i] = true;
				}
			}
		}

		public bool GrabReleaseFinger(int fingerIdx, ShadowHand fromHand, ShadowHand toHand, ColliderGroup colliderGroup, Vector3 offset)
		{
			float num = 1f / (float)this.TotalIterations;
			float num2 = 0f;
			for (;;)
			{
				num2 = Mathf.Clamp01(num2);
				this.SetShadowFingerFromLerp(fingerIdx, fromHand, toHand, num2);
				this.LoadSpheresForFingerFromShadow(fingerIdx, 0);
				if (!this.CheckFingerTouch(fingerIdx, 0, colliderGroup, offset, null))
				{
					break;
				}
				if (num2 == 1f)
				{
					return false;
				}
				num2 += num;
			}
			return true;
		}

		public bool GrabConformFinger(int fingerIdx, ShadowHand fromHand, ShadowHand toHand, ColliderGroup colliderGroup, Vector3 offset)
		{
			float num = 1f / (float)this.TotalIterations;
			float[] array = new float[FingersMetadata.FINGER_TO_JOINT_INDEX.Length];
			bool[] array2 = new bool[FingersMetadata.FINGER_TO_JOINT_INDEX.Length];
			bool result = false;
			bool flag = false;
			bool flag2 = false;
			int num2 = 0;
			do
			{
				this.SetShadowFingerFromLerps(fingerIdx, fromHand, toHand, array);
				this.LoadSpheresForFingerFromShadow(fingerIdx, 0);
				this._sphereHit.Clear();
				if (this.CheckFingerTouch(fingerIdx, num2 + 1, colliderGroup, offset, this._sphereHit))
				{
					for (int i = 0; i < this._sphereHit.Count; i++)
					{
						HandJointId joint = this._spheres[this._sphereHit[i]].Joint;
						int num3 = FingersMetadata.JOINT_TO_FINGER_INDEX[(int)joint];
						for (int j = num3; j >= 0; j--)
						{
							if (!array2[j])
							{
								array2[j] = true;
								result = true;
								if (num2 < num3)
								{
									num2 = num3;
								}
							}
						}
					}
				}
				for (int k = 0; k < array.Length; k++)
				{
					if (!array2[k])
					{
						flag2 = true;
						array[k] += num;
						if (array[k] > 1f)
						{
							array[k] = 1f;
							flag = true;
						}
					}
				}
			}
			while (flag2 && !flag);
			this.SetShadowFingerFromLerps(fingerIdx, fromHand, toHand, array);
			return result;
		}

		public void GrabConformFingers(ShadowHand fromHand, ShadowHand toHand, ColliderGroup colliderGroup, Vector3 offset)
		{
			for (int i = 0; i < 5; i++)
			{
				this.GrabConformFinger(i, fromHand, toHand, colliderGroup, offset);
			}
		}

		public bool PushoutFinger(int fingerIdx, ShadowHand from, ShadowHand to, ColliderGroup colliderGroup, Vector3 offset)
		{
			float num = 1f / (float)this.TotalIterations;
			float num2 = 0f;
			for (;;)
			{
				if (num2 > 1f)
				{
					num2 = Mathf.Clamp01(num2);
				}
				this.SetShadowFingerFromLerp(fingerIdx, from, to, num2);
				this.LoadSpheresForFingerFromShadow(fingerIdx, 0);
				if (!this.CheckFingerTouch(fingerIdx, 0, colliderGroup, offset, null))
				{
					break;
				}
				if (num2 == 1f)
				{
					return false;
				}
				num2 += num;
			}
			return true;
		}

		public void GrabTouchStep(ShadowHand from, ShadowHand to, ColliderGroup colliderGroup, int iteration, Vector3 colliderOffset, bool pushout, TouchShadowHand.GrabTouchInfo result)
		{
			if (iteration > this.TotalIterations)
			{
				return;
			}
			float num = 1f / (float)this.TotalIterations;
			float num2 = Mathf.Clamp01((float)iteration * num);
			result.offset = colliderOffset;
			Pose root = from.GetRoot();
			Vector3 b = (to.GetRoot().position - root.position) * num;
			this.SetShadowFromLerpHands(from, to, num2);
			this.LoadSpheresForHandFromShadow();
			this._sphereHit.Clear();
			for (int i = 0; i < 5; i++)
			{
				result.grabbingFingers[i] = false;
			}
			if (this.CheckSphereCollision(colliderGroup, result.offset, this._sphereHit, null))
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				for (int j = 0; j < this._sphereHit.Count; j++)
				{
					HandJointId joint = this._spheres[this._sphereHit[j]].Joint;
					int num3 = (int)HandJointUtils.JointToFingerList[(int)joint];
					if (num3 >= 0)
					{
						result.grabbingFingers[num3] = true;
					}
					if (num3 > 0)
					{
						flag = true;
					}
					else if (num3 == 0)
					{
						flag2 = true;
					}
					else
					{
						flag3 = true;
					}
				}
				if (flag && (flag2 || flag3))
				{
					result.grabbing = true;
					result.grabT = num2;
					return;
				}
				if (!pushout)
				{
					return;
				}
				Vector3 a = default(Vector3);
				this.SetShadowFromLerpHands(from, to, Mathf.Clamp01(num2 + num));
				this.LoadSpheresForHandFromShadow();
				for (int k = 0; k < this._spheres.Count; k++)
				{
					a += this._spheres[k].Position / (float)this._spheres.Count;
				}
				this.SetShadowFromLerpHands(from, to, Mathf.Clamp01(num2 - num));
				this.LoadSpheresForHandFromShadow();
				for (int l = 0; l < this._spheres.Count; l++)
				{
					a -= this._spheres[l].Position / (float)this._spheres.Count;
				}
				float num4 = 0f;
				for (int m = 0; m < this._sphereHit.Count; m++)
				{
					num4 += this._spheres[this._sphereHit[m]].Radius / (float)this._sphereHit.Count;
				}
				Vector3 b2 = num4 * (a - b).normalized;
				this.SetShadowFromLerpHands(from, to, num2);
				this.LoadSpheresForHandFromShadow();
				bool flag4 = false;
				for (int n = 0; n < this.PushoutIterations; n++)
				{
					result.offset += b2;
					if (!this.CheckSphereCollision(colliderGroup, result.offset, null, this._sphereHit))
					{
						flag4 = true;
						break;
					}
				}
				if (!flag4)
				{
					result.offset = Vector3.zero;
					result.grabbing = false;
					this.SetShadowFromLerpHands(from, to, 1f);
				}
			}
		}

		public void GrabTouch(ShadowHand fromHand, ShadowHand toHand, ColliderGroup colliderGroup, bool pushout, TouchShadowHand.GrabTouchInfo result)
		{
			result.grabbing = false;
			result.offset = Vector3.zero;
			for (int i = 0; i <= this.Iterations; i++)
			{
				this.GrabTouchStep(fromHand, toHand, colliderGroup, i, result.offset, pushout, result);
				if (result.grabbing)
				{
					break;
				}
			}
		}

		public void GetJointsFromShadow(HandJointId[] jointIds, Pose[] outJoints, bool local)
		{
			for (int i = 0; i < jointIds.Length; i++)
			{
				outJoints[i] = (local ? this._shadowHand.GetLocalPose(jointIds[i]) : this._shadowHand.GetWorldPose(jointIds[i]));
			}
		}

		private readonly ShadowHand _shadowHand = new ShadowHand();

		private readonly IHandSphereMap _handSphereMap;

		private readonly Handedness _handedness;

		private readonly List<HandSphere> _spheres = new List<HandSphere>();

		private int _totalIterations = 10;

		private int _pushoutIterations = 10;

		public int Iterations;

		private List<int> _sphereHit = new List<int>();

		public class GrabTouchInfo
		{
			public Vector3 offset;

			public bool grabbing;

			public bool[] grabbingFingers = new bool[5];

			public float grabT;
		}
	}
}
