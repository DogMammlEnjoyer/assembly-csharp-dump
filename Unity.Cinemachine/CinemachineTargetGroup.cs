using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Helpers/Cinemachine Target Group")]
	[SaveDuringPlay]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineTargetGroup.html")]
	public class CinemachineTargetGroup : MonoBehaviour, ICinemachineTargetGroup
	{
		private void OnValidate()
		{
			int count = this.Targets.Count;
			for (int i = 0; i < count; i++)
			{
				this.Targets[i].Weight = Mathf.Max(0f, this.Targets[i].Weight);
				this.Targets[i].Radius = Mathf.Max(0f, this.Targets[i].Radius);
			}
		}

		private void Reset()
		{
			this.PositionMode = CinemachineTargetGroup.PositionModes.GroupCenter;
			this.RotationMode = CinemachineTargetGroup.RotationModes.Manual;
			this.UpdateMethod = CinemachineTargetGroup.UpdateMethods.LateUpdate;
			this.Targets.Clear();
		}

		private void Awake()
		{
			if (this.m_LegacyTargets != null && this.m_LegacyTargets.Length != 0)
			{
				this.Targets.AddRange(this.m_LegacyTargets);
			}
			this.m_LegacyTargets = null;
		}

		[Obsolete("m_Targets is obsolete.  Please use Targets instead")]
		public CinemachineTargetGroup.Target[] m_Targets
		{
			get
			{
				return this.Targets.ToArray();
			}
			set
			{
				this.Targets.Clear();
				this.Targets.AddRange(value);
			}
		}

		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		public bool IsValid
		{
			get
			{
				return this != null;
			}
		}

		public Bounds BoundingBox
		{
			get
			{
				if (this.m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
				{
					this.DoUpdate();
				}
				return this.m_BoundingBox;
			}
			private set
			{
				this.m_BoundingBox = value;
			}
		}

		public BoundingSphere Sphere
		{
			get
			{
				if (this.m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
				{
					this.DoUpdate();
				}
				return this.m_BoundingSphere;
			}
			private set
			{
				this.m_BoundingSphere = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				if (this.m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
				{
					this.DoUpdate();
				}
				return this.m_ValidMembers.Count == 0;
			}
		}

		public void AddMember(Transform t, float weight, float radius)
		{
			this.Targets.Add(new CinemachineTargetGroup.Target
			{
				Object = t,
				Weight = weight,
				Radius = radius
			});
		}

		public void RemoveMember(Transform t)
		{
			int num = this.FindMember(t);
			if (num >= 0)
			{
				this.Targets.RemoveAt(num);
			}
		}

		public int FindMember(Transform t)
		{
			int count = this.Targets.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.Targets[i].Object == t)
				{
					return i;
				}
			}
			return -1;
		}

		public BoundingSphere GetWeightedBoundsForMember(int index)
		{
			if (this.m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
			{
				this.DoUpdate();
			}
			if (!this.IndexIsValid(index) || !this.m_MemberValidity[index])
			{
				return this.Sphere;
			}
			return CinemachineTargetGroup.WeightedMemberBoundsForValidMember(this.Targets[index], this.m_AveragePos, this.m_MaxWeight);
		}

		public Bounds GetViewSpaceBoundingBox(Matrix4x4 observer, bool includeBehind)
		{
			if (this.m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
			{
				this.DoUpdate();
			}
			Matrix4x4 matrix4x = observer;
			if (!Matrix4x4.Inverse3DAffine(observer, ref matrix4x))
			{
				matrix4x = observer.inverse;
			}
			Bounds result = new Bounds(matrix4x.MultiplyPoint3x4(this.m_AveragePos), Vector3.zero);
			if (this.CachedCountIsValid)
			{
				bool flag = false;
				Vector3 a = 2f * Vector3.one;
				int count = this.m_ValidMembers.Count;
				for (int i = 0; i < count; i++)
				{
					BoundingSphere boundingSphere = CinemachineTargetGroup.WeightedMemberBoundsForValidMember(this.Targets[this.m_ValidMembers[i]], this.m_AveragePos, this.m_MaxWeight);
					boundingSphere.position = matrix4x.MultiplyPoint3x4(boundingSphere.position);
					if (boundingSphere.position.z > 0f || includeBehind)
					{
						if (flag)
						{
							result.Encapsulate(new Bounds(boundingSphere.position, boundingSphere.radius * a));
						}
						else
						{
							result = new Bounds(boundingSphere.position, boundingSphere.radius * a);
						}
						flag = true;
					}
				}
			}
			return result;
		}

		private bool CachedCountIsValid
		{
			get
			{
				return this.m_MemberValidity.Count == this.Targets.Count;
			}
		}

		private bool IndexIsValid(int index)
		{
			return index >= 0 && index < this.Targets.Count && this.CachedCountIsValid;
		}

		private static BoundingSphere WeightedMemberBoundsForValidMember(CinemachineTargetGroup.Target t, Vector3 avgPos, float maxWeight)
		{
			Vector3 b = (t.Object == null) ? avgPos : TargetPositionCache.GetTargetPosition(t.Object);
			float num = Mathf.Max(0f, t.Weight);
			if (maxWeight > 0.0001f && num < maxWeight)
			{
				num /= maxWeight;
			}
			else
			{
				num = 1f;
			}
			return new BoundingSphere(Vector3.Lerp(avgPos, b, num), t.Radius * num);
		}

		public void DoUpdate()
		{
			this.m_LastUpdateFrame = CinemachineCore.CurrentUpdateFrame;
			this.UpdateMemberValidity();
			this.m_AveragePos = this.CalculateAveragePosition();
			this.BoundingBox = this.CalculateBoundingBox();
			this.m_BoundingSphere = this.CalculateBoundingSphere();
			CinemachineTargetGroup.PositionModes positionMode = this.PositionMode;
			if (positionMode != CinemachineTargetGroup.PositionModes.GroupCenter)
			{
				if (positionMode == CinemachineTargetGroup.PositionModes.GroupAverage)
				{
					base.transform.position = this.m_AveragePos;
				}
			}
			else
			{
				base.transform.position = this.Sphere.position;
			}
			CinemachineTargetGroup.RotationModes rotationMode = this.RotationMode;
			if (rotationMode != CinemachineTargetGroup.RotationModes.Manual && rotationMode == CinemachineTargetGroup.RotationModes.GroupAverage)
			{
				base.transform.rotation = this.CalculateAverageOrientation();
			}
		}

		private void UpdateMemberValidity()
		{
			if (this.Targets == null)
			{
				this.Targets = new List<CinemachineTargetGroup.Target>();
			}
			int count = this.Targets.Count;
			this.m_ValidMembers.Clear();
			this.m_ValidMembers.Capacity = Mathf.Max(this.m_ValidMembers.Capacity, count);
			this.m_MemberValidity.Clear();
			this.m_MemberValidity.Capacity = Mathf.Max(this.m_MemberValidity.Capacity, count);
			this.m_WeightSum = (this.m_MaxWeight = 0f);
			for (int i = 0; i < count; i++)
			{
				this.m_MemberValidity.Add(this.Targets[i].Object != null && this.Targets[i].Weight > 0.0001f && this.Targets[i].Object.gameObject.activeInHierarchy);
				if (this.m_MemberValidity[i])
				{
					this.m_ValidMembers.Add(i);
					this.m_MaxWeight = Mathf.Max(this.m_MaxWeight, this.Targets[i].Weight);
					this.m_WeightSum += this.Targets[i].Weight;
				}
			}
		}

		private Vector3 CalculateAveragePosition()
		{
			if (this.m_WeightSum < 0.0001f)
			{
				return base.transform.position;
			}
			Vector3 a = Vector3.zero;
			int count = this.m_ValidMembers.Count;
			for (int i = 0; i < count; i++)
			{
				int index = this.m_ValidMembers[i];
				float weight = this.Targets[index].Weight;
				a += TargetPositionCache.GetTargetPosition(this.Targets[index].Object) * weight;
			}
			return a / this.m_WeightSum;
		}

		private Bounds CalculateBoundingBox()
		{
			if (this.m_MaxWeight < 0.0001f)
			{
				return this.m_BoundingBox;
			}
			Bounds result = new Bounds(this.m_AveragePos, Vector3.zero);
			int count = this.m_ValidMembers.Count;
			for (int i = 0; i < count; i++)
			{
				BoundingSphere boundingSphere = CinemachineTargetGroup.WeightedMemberBoundsForValidMember(this.Targets[this.m_ValidMembers[i]], this.m_AveragePos, this.m_MaxWeight);
				result.Encapsulate(new Bounds(boundingSphere.position, boundingSphere.radius * 2f * Vector3.one));
			}
			return result;
		}

		private BoundingSphere CalculateBoundingSphere()
		{
			int count = this.m_ValidMembers.Count;
			if (count == 0 || this.m_MaxWeight < 0.0001f)
			{
				return this.m_BoundingSphere;
			}
			BoundingSphere boundingSphere = CinemachineTargetGroup.WeightedMemberBoundsForValidMember(this.Targets[this.m_ValidMembers[0]], this.m_AveragePos, this.m_MaxWeight);
			for (int i = 1; i < count; i++)
			{
				BoundingSphere boundingSphere2 = CinemachineTargetGroup.WeightedMemberBoundsForValidMember(this.Targets[this.m_ValidMembers[i]], this.m_AveragePos, this.m_MaxWeight);
				float num = (boundingSphere2.position - boundingSphere.position).magnitude + boundingSphere2.radius;
				if (num > boundingSphere.radius)
				{
					boundingSphere.radius = (boundingSphere.radius + num) * 0.5f;
					boundingSphere.position = (boundingSphere.radius * boundingSphere.position + (num - boundingSphere.radius) * boundingSphere2.position) / num;
				}
			}
			return boundingSphere;
		}

		private Quaternion CalculateAverageOrientation()
		{
			if (this.m_WeightSum > 0.001f)
			{
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				int count = this.m_ValidMembers.Count;
				for (int i = 0; i < count; i++)
				{
					int index = this.m_ValidMembers[i];
					float d = this.Targets[index].Weight / this.m_WeightSum;
					Quaternion targetRotation = TargetPositionCache.GetTargetRotation(this.Targets[index].Object);
					vector += targetRotation * Vector3.forward * d;
					vector2 += targetRotation * Vector3.up * d;
				}
				if (vector.sqrMagnitude > 0.0001f && vector2.sqrMagnitude > 0.0001f)
				{
					return Quaternion.LookRotation(vector, vector2);
				}
			}
			return base.transform.rotation;
		}

		private void FixedUpdate()
		{
			if (this.UpdateMethod == CinemachineTargetGroup.UpdateMethods.FixedUpdate)
			{
				this.DoUpdate();
			}
		}

		private void Update()
		{
			if (!Application.isPlaying || this.UpdateMethod == CinemachineTargetGroup.UpdateMethods.Update)
			{
				this.DoUpdate();
			}
		}

		private void LateUpdate()
		{
			if (this.UpdateMethod == CinemachineTargetGroup.UpdateMethods.LateUpdate)
			{
				this.DoUpdate();
			}
		}

		public void GetViewSpaceAngularBounds(Matrix4x4 observer, out Vector2 minAngles, out Vector2 maxAngles, out Vector2 zRange)
		{
			if (this.m_LastUpdateFrame != CinemachineCore.CurrentUpdateFrame)
			{
				this.DoUpdate();
			}
			Matrix4x4 matrix4x = observer;
			if (!Matrix4x4.Inverse3DAffine(observer, ref matrix4x))
			{
				matrix4x = observer.inverse;
			}
			float radius = this.m_BoundingSphere.radius;
			Bounds bounds = new Bounds
			{
				center = matrix4x.MultiplyPoint3x4(this.m_AveragePos),
				extents = new Vector3(radius, radius, radius)
			};
			zRange = new Vector2(bounds.center.z - radius, bounds.center.z + radius);
			if (this.CachedCountIsValid)
			{
				bool flag = false;
				int count = this.m_ValidMembers.Count;
				for (int i = 0; i < count; i++)
				{
					BoundingSphere boundingSphere = CinemachineTargetGroup.WeightedMemberBoundsForValidMember(this.Targets[this.m_ValidMembers[i]], this.m_AveragePos, this.m_MaxWeight);
					Vector3 vector = matrix4x.MultiplyPoint3x4(boundingSphere.position);
					if (vector.z >= 0.0001f)
					{
						float num = boundingSphere.radius / vector.z;
						Vector3 vector2 = new Vector3(num, num, 0f);
						Vector3 vector3 = vector / vector.z;
						if (!flag)
						{
							bounds.center = vector3;
							bounds.extents = vector2;
							zRange = new Vector2(vector.z, vector.z);
							flag = true;
						}
						else
						{
							bounds.Encapsulate(vector3 + vector2);
							bounds.Encapsulate(vector3 - vector2);
							zRange.x = Mathf.Min(zRange.x, vector.z);
							zRange.y = Mathf.Max(zRange.y, vector.z);
						}
					}
				}
			}
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			minAngles = new Vector2(Vector3.SignedAngle(Vector3.forward, new Vector3(0f, max.y, 1f), Vector3.right), Vector3.SignedAngle(Vector3.forward, new Vector3(min.x, 0f, 1f), Vector3.up));
			maxAngles = new Vector2(Vector3.SignedAngle(Vector3.forward, new Vector3(0f, min.y, 1f), Vector3.right), Vector3.SignedAngle(Vector3.forward, new Vector3(max.x, 0f, 1f), Vector3.up));
		}

		[Tooltip("How the group's position is calculated.  Select GroupCenter for the center of the bounding box, and GroupAverage for a weighted average of the positions of the members.")]
		[FormerlySerializedAs("m_PositionMode")]
		public CinemachineTargetGroup.PositionModes PositionMode;

		[Tooltip("How the group's rotation is calculated.  Select Manual to use the value in the group's transform, and GroupAverage for a weighted average of the orientations of the members.")]
		[FormerlySerializedAs("m_RotationMode")]
		public CinemachineTargetGroup.RotationModes RotationMode;

		[Tooltip("When to update the group's transform based on the position of the group members")]
		[FormerlySerializedAs("m_UpdateMethod")]
		public CinemachineTargetGroup.UpdateMethods UpdateMethod = CinemachineTargetGroup.UpdateMethods.LateUpdate;

		[NoSaveDuringPlay]
		[Tooltip("The target objects, together with their weights and radii, that will contribute to the group's average position, orientation, and size.")]
		public List<CinemachineTargetGroup.Target> Targets = new List<CinemachineTargetGroup.Target>();

		private float m_MaxWeight;

		private float m_WeightSum;

		private Vector3 m_AveragePos;

		private Bounds m_BoundingBox;

		private BoundingSphere m_BoundingSphere;

		private int m_LastUpdateFrame = -1;

		private List<int> m_ValidMembers = new List<int>();

		private List<bool> m_MemberValidity = new List<bool>();

		[HideInInspector]
		[SerializeField]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("m_Targets")]
		private CinemachineTargetGroup.Target[] m_LegacyTargets;

		[Serializable]
		public class Target
		{
			[Tooltip("The target object.  This object's position and rotation will contribute to the group's average position and rotation, in accordance with its weight")]
			[FormerlySerializedAs("target")]
			public Transform Object;

			[Tooltip("How much weight to give the target when averaging.  Cannot be negative")]
			[FormerlySerializedAs("weight")]
			public float Weight = 1f;

			[Tooltip("The radius of the target, used for calculating the bounding box.  Cannot be negative")]
			[FormerlySerializedAs("radius")]
			public float Radius = 0.5f;
		}

		public enum PositionModes
		{
			GroupCenter,
			GroupAverage
		}

		public enum RotationModes
		{
			Manual,
			GroupAverage
		}

		public enum UpdateMethods
		{
			Update,
			FixedUpdate,
			LateUpdate
		}
	}
}
