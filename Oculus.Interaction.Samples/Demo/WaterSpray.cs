using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.Demo
{
	public class WaterSpray : MonoBehaviour, IHandGrabUseDelegate
	{
		private void SprayWater()
		{
			WaterSpray.NozzleMode nozzleMode = this.GetNozzleMode();
			if (nozzleMode != WaterSpray.NozzleMode.Spray)
			{
				if (nozzleMode == WaterSpray.NozzleMode.Stream)
				{
					this.Stream();
					UnityEvent whenStream = this.WhenStream;
					if (whenStream == null)
					{
						return;
					}
					whenStream.Invoke();
				}
				return;
			}
			this.Spray();
			UnityEvent whenSpray = this.WhenSpray;
			if (whenSpray == null)
			{
				return;
			}
			whenSpray.Invoke();
		}

		private void UpdateTriggerRotation(float progress)
		{
			float num = this._triggerRotationCurve.Evaluate(progress);
			Vector3 localEulerAngles = this._trigger.localEulerAngles;
			if ((this._axis & SnapAxis.X) != SnapAxis.None)
			{
				localEulerAngles.x = num;
			}
			if ((this._axis & SnapAxis.Y) != SnapAxis.None)
			{
				localEulerAngles.y = num;
			}
			if ((this._axis & SnapAxis.Z) != SnapAxis.None)
			{
				localEulerAngles.z = num;
			}
			this._trigger.localEulerAngles = localEulerAngles;
		}

		private WaterSpray.NozzleMode GetNozzleMode()
		{
			if (((int)this._nozzle.localEulerAngles.z + 45) / 90 % 2 == 0)
			{
				return WaterSpray.NozzleMode.Spray;
			}
			return WaterSpray.NozzleMode.Stream;
		}

		private void Spray()
		{
			base.StartCoroutine(this.StampRoutine(this._sprayHits, this._sprayRandomness, this._spraySpreadAngle, this._sprayStrength));
		}

		private void Stream()
		{
			base.StartCoroutine(this.StampRoutine(this._sprayHits, 0f, this._streamSpreadAngle, this._sprayStrength));
		}

		private IEnumerator StampRoutine(int stampCount, float randomness, float spread, float strength)
		{
			this.StartStamping();
			Pose originalPose = this._nozzle.GetPose(Space.World);
			int num;
			for (int i = 0; i < stampCount; i = num + 1)
			{
				yield return WaterSpray.WAIT_TIME;
				Pose pose = originalPose;
				pose.rotation *= Quaternion.Euler(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), 0f);
				this.Stamp(pose, this._maxDistance, spread, strength);
				num = i;
			}
			this.StartDrying();
			yield break;
		}

		private void StartStamping()
		{
			this._sprayStampMaterial.SetFloat(WaterSpray.SUBTRACT_PROPERTY, 0f);
		}

		private void StartDrying()
		{
			this._sprayStampMaterial.SetMatrix(WaterSpray.STAMP_MATRIX_PROPERTY, Matrix4x4.zero);
			this._sprayStampMaterial.SetFloat(WaterSpray.SUBTRACT_PROPERTY, this._dryingSpeed);
		}

		private void Stamp(Pose pose, float maxDistance, float angle, float strength)
		{
			this._sprayStampMaterial.SetMatrix(WaterSpray.STAMP_MATRIX_PROPERTY, this.CreateStampMatrix(pose, angle));
			this._sprayStampMaterial.SetFloat(WaterSpray.STAMP_MULTIPLIER_PROPERTY, strength);
			float num = Mathf.Tan(0.017453292f * angle / 2f) * maxDistance;
			Vector3 point = pose.position + pose.forward * num;
			Vector3 point2 = pose.position + pose.forward * maxDistance;
			HashSet<Transform> rootsFromOverlapResults = WaterSpray.NonAlloc.GetRootsFromOverlapResults(Physics.OverlapCapsuleNonAlloc(point, point2, num, WaterSpray.NonAlloc._overlapResults, this._raycastLayerMask.value, QueryTriggerInteraction.Ignore));
			foreach (Transform rootObject in rootsFromOverlapResults)
			{
				this.RenderSplash(rootObject);
			}
			rootsFromOverlapResults.Clear();
		}

		private void RenderSplash(Transform rootObject)
		{
			List<MeshFilter> meshFiltersInChildren = WaterSpray.NonAlloc.GetMeshFiltersInChildren(rootObject);
			for (int i = 0; i < meshFiltersInChildren.Count; i++)
			{
				int instanceID = meshFiltersInChildren[i].GetInstanceID();
				if (!WaterSpray.NonAlloc._blits.ContainsKey(instanceID))
				{
					WaterSpray.NonAlloc._blits[instanceID] = this.CreateMeshBlit(meshFiltersInChildren[i]);
				}
				WaterSpray.NonAlloc._blits[instanceID].Blit();
			}
		}

		private MeshBlit CreateMeshBlit(MeshFilter meshFilter)
		{
			MeshBlit meshBlit = meshFilter.gameObject.AddComponent<MeshBlit>();
			meshBlit.material = this._sprayStampMaterial;
			meshBlit.renderTexture = new RenderTexture(512, 512, 0, RenderTextureFormat.RHalf);
			meshBlit.BlitsPerSecond = 30f;
			Renderer renderer;
			if (meshFilter.TryGetComponent<Renderer>(out renderer))
			{
				renderer.GetPropertyBlock(WaterSpray.NonAlloc.PropertyBlock);
				WaterSpray.NonAlloc.PropertyBlock.SetTexture(WaterSpray.WET_MAP_PROPERTY, meshBlit.renderTexture);
				if (this._waterBumpOverride)
				{
					WaterSpray.NonAlloc.PropertyBlock.SetTexture(WaterSpray.WET_BUMPMAP_PROPERTY, this._waterBumpOverride);
				}
				renderer.SetPropertyBlock(WaterSpray.NonAlloc.PropertyBlock);
			}
			return meshBlit;
		}

		private Matrix4x4 CreateStampMatrix(Pose pose, float angle)
		{
			Matrix4x4 inverse = Matrix4x4.TRS(pose.position, pose.rotation, Vector3.one).inverse;
			inverse.m20 *= -1f;
			inverse.m21 *= -1f;
			inverse.m22 *= -1f;
			inverse.m23 *= -1f;
			return GL.GetGPUProjectionMatrix(Matrix4x4.Perspective(angle, 1f, 0f, this._maxDistance), true) * inverse;
		}

		private void OnDestroy()
		{
			WaterSpray.NonAlloc.CleanUpDestroyedBlits();
		}

		public void BeginUse()
		{
			this._dampedUseStrength = 0f;
			this._lastUseTime = Time.realtimeSinceStartup;
		}

		public void EndUse()
		{
		}

		public float ComputeUseStrength(float strength)
		{
			float num = Time.realtimeSinceStartup - this._lastUseTime;
			this._lastUseTime = Time.realtimeSinceStartup;
			if (strength > this._dampedUseStrength)
			{
				this._dampedUseStrength = Mathf.Lerp(this._dampedUseStrength, strength, this._triggerSpeed * num);
			}
			else
			{
				this._dampedUseStrength = strength;
			}
			float num2 = this._strengthCurve.Evaluate(this._dampedUseStrength);
			this.UpdateTriggerProgress(num2);
			return num2;
		}

		private void UpdateTriggerProgress(float progress)
		{
			this.UpdateTriggerRotation(progress);
			if (progress >= this._fireThresold && !this._wasFired)
			{
				this._wasFired = true;
				this.SprayWater();
				return;
			}
			if (progress <= this._releaseThresold)
			{
				this._wasFired = false;
			}
		}

		[Header("Input")]
		[SerializeField]
		private Transform _trigger;

		[SerializeField]
		private Transform _nozzle;

		[SerializeField]
		private AnimationCurve _triggerRotationCurve;

		[SerializeField]
		private SnapAxis _axis = SnapAxis.X;

		[SerializeField]
		[Range(0f, 1f)]
		private float _releaseThresold = 0.3f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _fireThresold = 0.9f;

		[SerializeField]
		private float _triggerSpeed = 3f;

		[SerializeField]
		private AnimationCurve _strengthCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[Header("Output")]
		[SerializeField]
		[Tooltip("Masks the Raycast used to find objects to make wet")]
		private LayerMask _raycastLayerMask = -1;

		[SerializeField]
		[Tooltip("The spread angle when spraying, larger values will make a larger area wet")]
		private float _spraySpreadAngle = 40f;

		[SerializeField]
		[Tooltip("The spread angle when using stream, larger values will make a larger area wet")]
		private float _streamSpreadAngle = 4f;

		[SerializeField]
		private float _sprayStrength = 1.5f;

		[SerializeField]
		private int _sprayHits = 6;

		[SerializeField]
		private float _sprayRandomness = 6f;

		[SerializeField]
		[Tooltip("The max distance of the spray, controls the raycast and shader")]
		private float _maxDistance = 2f;

		[SerializeField]
		private float _dryingSpeed = 0.1f;

		[SerializeField]
		[Tooltip("Material for applying a stamp, should using the MeshBlitStamp shader or similar")]
		private Material _sprayStampMaterial;

		[SerializeField]
		[Tooltip("When not null, will be set as the '_WetBumpMap' property on wet renderers")]
		private Texture _waterBumpOverride;

		[SerializeField]
		private UnityEvent WhenSpray;

		[SerializeField]
		private UnityEvent WhenStream;

		private static readonly int WET_MAP_PROPERTY = Shader.PropertyToID("_WetMap");

		private static readonly int STAMP_MULTIPLIER_PROPERTY = Shader.PropertyToID("_StampMultipler");

		private static readonly int SUBTRACT_PROPERTY = Shader.PropertyToID("_Subtract");

		private static readonly int WET_BUMPMAP_PROPERTY = Shader.PropertyToID("_WetBumpMap");

		private static readonly int STAMP_MATRIX_PROPERTY = Shader.PropertyToID("_StampMatrix");

		private static readonly WaitForSeconds WAIT_TIME = new WaitForSeconds(0.1f);

		private bool _wasFired;

		private float _dampedUseStrength;

		private float _lastUseTime;

		public enum NozzleMode
		{
			Spray,
			Stream
		}

		private static class NonAlloc
		{
			public static MaterialPropertyBlock PropertyBlock
			{
				get
				{
					if (WaterSpray.NonAlloc._block == null)
					{
						return WaterSpray.NonAlloc._block = new MaterialPropertyBlock();
					}
					return WaterSpray.NonAlloc._block;
				}
			}

			public static List<MeshFilter> GetMeshFiltersInChildren(Transform root)
			{
				root.GetComponentsInChildren<MeshFilter>(WaterSpray.NonAlloc._meshFilters);
				return WaterSpray.NonAlloc._meshFilters;
			}

			public static HashSet<Transform> GetRootsFromOverlapResults(int hitCount)
			{
				WaterSpray.NonAlloc._roots.Clear();
				for (int i = 0; i < hitCount; i++)
				{
					Transform root = WaterSpray.NonAlloc.GetRoot(WaterSpray.NonAlloc._overlapResults[i]);
					WaterSpray.NonAlloc._roots.Add(root);
				}
				return WaterSpray.NonAlloc._roots;
			}

			private static Transform GetRoot(Collider hit)
			{
				if (hit.attachedRigidbody)
				{
					return hit.attachedRigidbody.transform;
				}
				if (!hit.transform.parent)
				{
					return hit.transform;
				}
				return hit.transform.parent;
			}

			public static void CleanUpDestroyedBlits()
			{
				if (!WaterSpray.NonAlloc._blits.ContainsValue(null))
				{
					return;
				}
				foreach (int key in new List<int>(WaterSpray.NonAlloc._blits.Keys))
				{
					if (WaterSpray.NonAlloc._blits[key] == null)
					{
						WaterSpray.NonAlloc._blits.Remove(key);
					}
				}
			}

			public static readonly Collider[] _overlapResults = new Collider[12];

			public static readonly Dictionary<int, MeshBlit> _blits = new Dictionary<int, MeshBlit>();

			private static readonly List<MeshFilter> _meshFilters = new List<MeshFilter>();

			private static readonly HashSet<Transform> _roots = new HashSet<Transform>();

			private static MaterialPropertyBlock _block;
		}
	}
}
