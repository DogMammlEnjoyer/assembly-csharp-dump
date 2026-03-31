using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag.Gravity
{
	public class MonkeGravityManager : MonoBehaviour
	{
		private void Awake()
		{
			Vector3 vector = -Physics.gravity.normalized;
			MonkeGravityManager.k_defaultGravityInfo.gravityUpDirection = vector;
			MonkeGravityManager.k_defaultGravityInfo.rotationDirection = vector;
			MonkeGravityManager.k_defaultGravityInfo.rotationSpeed = this.defaultRotationSpeed;
			MonkeGravityManager.k_defaultGravityInfo.gravityStrength = this.defaultGravityStrength;
		}

		private void FixedUpdate()
		{
			MonkeGravityManager.k_zones.RunCallbacks();
			MonkeGravityManager.k_controllers.RunCallbacks();
		}

		public static GravityInfo DefaultGravityInfo
		{
			get
			{
				return MonkeGravityManager.k_defaultGravityInfo;
			}
		}

		public static void AddMonkeGravityController(MonkeGravityController gravity)
		{
			Collider activatorCollider = gravity.ActivatorCollider;
			MonkeGravityManager.k_allowedColliders.TryAdd(activatorCollider, gravity);
			MonkeGravityManager.k_controllers.Add(gravity);
		}

		public static void RemoveMonkeGravityController(MonkeGravityController gravity)
		{
			MonkeGravityManager.k_allowedColliders.Remove(gravity.ActivatorCollider);
			MonkeGravityManager.k_controllers.Remove(gravity);
		}

		[return: TupleElementNames(new string[]
		{
			"found",
			"target"
		})]
		public static ValueTuple<bool, MonkeGravityController> GetMonkeGravityController(Collider collider)
		{
			MonkeGravityController item;
			return new ValueTuple<bool, MonkeGravityController>(MonkeGravityManager.k_allowedColliders.TryGetValue(collider, out item), item);
		}

		public static void AddGravityCallback(BasicGravityZone zone)
		{
			MonkeGravityManager.k_zones.Add(zone);
		}

		public static void RemoveGravityCallback(BasicGravityZone zone)
		{
			MonkeGravityManager.k_zones.Remove(zone);
		}

		[SerializeField]
		private float defaultRotationSpeed = 10f;

		[SerializeField]
		private float defaultGravityStrength = -9.3f;

		private static readonly CallbackContainerUnique<BasicGravityZone> k_zones = new CallbackContainerUnique<BasicGravityZone>(5);

		private static readonly Dictionary<Collider, MonkeGravityController> k_allowedColliders = new Dictionary<Collider, MonkeGravityController>(10);

		private static readonly CallbackContainerUnique<MonkeGravityController> k_controllers = new CallbackContainerUnique<MonkeGravityController>(10);

		private static GravityInfo k_defaultGravityInfo = new GravityInfo
		{
			gravityUpDirection = Vector3.up,
			rotationDirection = Vector3.up,
			rotationSpeed = 10f,
			gravityStrength = -9.3f,
			rotate = false
		};
	}
}
