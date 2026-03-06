using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CustomMapSupport;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[NullableContext(2)]
	[Nullable(0)]
	public class GTObjectPlaceholder : MonoBehaviour
	{
		public WaterVolumeProperties GetWaterVolumeProperties()
		{
			return new WaterVolumeProperties
			{
				surfacePlane = this.surfacePlane,
				surfaceColliders = this.surfaceColliders,
				liquidType = this.liquidType
			};
		}

		public ForceVolumeProperties GetForceVolumeProperties()
		{
			return new ForceVolumeProperties
			{
				accel = this.accel_FV,
				maxSpeed = this.maxSpeed_FV,
				maxDepth = this.maxDepth_FV,
				disableGrip = this.disableGrip_FV,
				dampenLateralVelocity = this.dampenLatVel_FV,
				dampenXVel = this.dampenXVel_FV,
				dampenZVel = this.dampenZVel_FV,
				applyPullToCenterAcceleration = this.applyPull_FV,
				pullToCenterAccel = this.pullToCenterAccel_FV,
				pullToCenterMaxSpeed = this.pullToCenterMaxSpeed_FV,
				pullToCenterMinDistance = this.pullToCenterMinDist_FV,
				enterClip = this.enterClip,
				exitClip = this.exitClip,
				loopClip = this.loopClip,
				loopCrescendoClip = this.loopCrescendoClip
			};
		}

		public void SetForceVolumeProperties(ForceVolumeProperties props)
		{
			this.accel_FV = props.accel;
			this.maxSpeed_FV = props.maxSpeed;
			this.maxDepth_FV = props.maxDepth;
			this.disableGrip_FV = props.disableGrip;
			this.dampenLatVel_FV = props.dampenLateralVelocity;
			this.dampenXVel_FV = props.dampenXVel;
			this.dampenZVel_FV = props.dampenZVel;
			this.applyPull_FV = props.applyPullToCenterAcceleration;
			this.pullToCenterAccel_FV = props.pullToCenterAccel;
			this.pullToCenterMaxSpeed_FV = props.pullToCenterMaxSpeed;
			this.pullToCenterMinDist_FV = props.pullToCenterMinDistance;
			this.enterClip = props.enterClip;
			this.exitClip = props.exitClip;
			this.loopClip = props.loopClip;
			this.loopCrescendoClip = props.loopCrescendoClip;
		}

		public GTObject PlaceholderObject;

		public bool useDefaultPlaceholder = true;

		public bool useCustomMesh;

		public float maxDistanceBeforeRespawn = 180f;

		public float maxSpeed = 30f;

		public float maxAccel = 15f;

		[Nullable(1)]
		public AnimationCurve SpeedVSAccelCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		public Vector3 localWindDirection = Vector3.up;

		public bool useWaterMesh = true;

		public float scrollTextureX;

		public float scrollTextureY;

		public float scaleTexture = 20f;

		[Tooltip("Transform for your flat Water Surface Plane. Y-Axis should point towards the Top of the water")]
		public Transform surfacePlane;

		[Nullable(1)]
		[Tooltip("Put any mesh colliders here that are used for your Water Surface if they aren't flat and aligned with the surfacePlane Transform")]
		public List<MeshCollider> surfaceColliders = new List<MeshCollider>();

		[Tooltip("Type of liquid for this Water Volume. This will also determine the Splash Effects that are used.")]
		public CMSZoneShaderSettings.EZoneLiquidType liquidType = CMSZoneShaderSettings.EZoneLiquidType.Water;

		[Tooltip("How fast to accelerate to the max speed of the Force Volume.\n\nExample: An acceleration of 10 would get to a max speed of 50 over 5 seconds.")]
		[Range(0f, 120f)]
		public float accel_FV;

		[Tooltip("Max depth towards the center of the volume before forcing closing velocity to 0 (-1 to not use max depth)")]
		[Range(-1f, 100f)]
		public float maxDepth_FV = -1f;

		[Tooltip("Maximum speed, in meters per second, the player can move along the direction of the volume's Y-Axis.")]
		[Range(0f, 120f)]
		public float maxSpeed_FV;

		[Tooltip("If true, all surfaces become maximum slippery while in the force volume")]
		public bool disableGrip_FV;

		public bool dampenLatVel_FV = true;

		[Tooltip("Dampen current velocity on the X axis")]
		[Range(0f, 100f)]
		public float dampenXVel_FV;

		[Tooltip("Dampen current velocity on the Z axis")]
		[Range(0f, 100f)]
		public float dampenZVel_FV;

		[Tooltip("If true, pulls player to center of the volume (towards Y-Axis)")]
		public bool applyPull_FV = true;

		[Range(0f, 500f)]
		public float pullToCenterAccel_FV;

		[Range(0f, 500f)]
		public float pullToCenterMaxSpeed_FV;

		[Tooltip("The Minimum distance before the centering force is applied")]
		[Range(0.0001f, 0.5f)]
		public float pullToCenterMinDist_FV = 0.1f;

		public AudioClip enterClip;

		public AudioClip exitClip;

		public AudioClip loopClip;

		public AudioClip loopCrescendoClip;

		[Nullable(1)]
		[Tooltip("Creator Code that is pre-filled on this specific ATM")]
		public string defaultCreatorCode = "";

		[Range(3f, 31f)]
		public int ropeLength = 3;

		public GameObject ropeSwingSegmentPrefab;

		public float ropeSegmentGenerationOffset = 1f;

		[Nullable(1)]
		public List<RopeSwingSegment> ropeSwingSegments = new List<RopeSwingSegment>();

		public BezierSpline spline;

		public GameObject ziplineSegmentPrefab;

		public float ziplineSegmentGenerationOffset = 0.92f;

		[Nullable(1)]
		public List<ZiplineSegment> ziplineSegments = new List<ZiplineSegment>();

		public GTObjectPlaceholder.ECustomMapCosmeticItem CosmeticItem;

		[NullableContext(0)]
		public enum ECustomMapCosmeticItem
		{
			Item_A,
			Item_B,
			Item_C,
			Item_D,
			Item_E,
			Item_F,
			Item_G,
			Item_H,
			Item_I,
			Item_J,
			Item_K,
			Item_L
		}
	}
}
