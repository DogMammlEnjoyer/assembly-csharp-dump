using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Gravity
{
	public class BasicGravityZone : MonoBehaviour, ICallbackUnique, ICallBack
	{
		private IReadOnlyList<MonkeGravityController> GravityTargets
		{
			get
			{
				return this.m_gravityTargets.GetReadonlyList();
			}
		}

		bool ICallbackUnique.Registered { get; set; }

		protected virtual void Awake()
		{
			this.m_gravityDirection = base.gameObject.transform.up;
			this.invertRotationDirection = ((this.gravityStrength > 0f && !this.invertRotationDirection) || (this.gravityStrength <= 0f && this.invertRotationDirection));
		}

		protected virtual void OnEnable()
		{
			this.m_gravityTargets.ItemProcessor = new InAction<MonkeGravityController>(this.ProcessGravityTargets);
		}

		protected virtual void OnDisable()
		{
			this.m_gravityTargets.ItemProcessor = new InAction<MonkeGravityController>(this.ProcessRemoveTargets);
			this.m_gravityTargets.ProcessList();
			this.m_gravityTargets.Clear();
			this.m_targetGravityInfos.Clear();
			MonkeGravityManager.RemoveGravityCallback(this);
		}

		public virtual void CallBack()
		{
			this.m_gravityTargets.ProcessList();
		}

		private void ProcessRemoveTargets(in MonkeGravityController target)
		{
			target.OnLeftGravityZone(this);
		}

		private void ProcessGravityTargets(in MonkeGravityController targetController)
		{
			if (!this.PassesScaleFilter(targetController))
			{
				this.OnTargetFilteredOut(targetController);
				return;
			}
			GravityInfo gravityInfo = default(GravityInfo);
			Vector3 worldPoint = targetController.GetWorldPoint();
			Vector3 gravityVectorAtPoint = this.GetGravityVectorAtPoint(worldPoint, targetController);
			Vector3 normalized = gravityVectorAtPoint.normalized;
			gravityInfo.gravityUpDirection = normalized;
			gravityInfo.rotationDirection = this.GetRotationDirection(normalized);
			gravityInfo.gravityStrength = this.GetGravityStrength(gravityVectorAtPoint);
			gravityInfo.rotationSpeed = this.GetRotationSpeed(gravityVectorAtPoint);
			gravityInfo.rotate = this.GetRotationIntent(gravityVectorAtPoint);
			this.m_targetGravityInfos[targetController] = gravityInfo;
			if (gravityInfo.gravityStrength != 0f)
			{
				MonkeGravityController monkeGravityController = targetController;
				Vector3 vector = normalized * gravityInfo.gravityStrength;
				monkeGravityController.ApplyGravityForce(vector, ForceMode.Acceleration);
			}
		}

		protected virtual Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return this.m_gravityDirection;
		}

		protected virtual float GetGravityStrength(in Vector3 offsetFromGravity)
		{
			return this.gravityStrength;
		}

		protected virtual bool GetRotationIntent(in Vector3 offsetFromGravity)
		{
			return this.rotateTarget;
		}

		protected virtual Vector3 GetRotationDirection(in Vector3 gravityDirection)
		{
			if (this.invertRotationDirection)
			{
				return -gravityDirection;
			}
			return gravityDirection;
		}

		protected virtual float GetRotationSpeed(in Vector3 offsetFromGravity)
		{
			return this.rotationSpeed;
		}

		public bool GetGravityInfo(MonkeGravityController target, out GravityInfo info)
		{
			return this.m_targetGravityInfos.TryGetValue(target, out info);
		}

		public void RemoveTarget(MonkeGravityController target)
		{
			if (!target.Register || !this.m_gravityTargets.Remove(target))
			{
				return;
			}
			this.m_targetGravityInfos.Remove(target);
			target.OnLeftGravityZone(this);
			this.OnTargetExited(target);
			if (this.m_gravityTargets.Count < 1)
			{
				MonkeGravityManager.RemoveGravityCallback(this);
			}
		}

		public void AddTarget(MonkeGravityController target)
		{
			if (!target.Register || this.m_gravityTargets.Contains(target))
			{
				return;
			}
			this.m_gravityTargets.Add(target);
			target.OnEnteredGravityZone(this);
			MonkeGravityManager.AddGravityCallback(this);
		}

		protected virtual void OnTargetExited(MonkeGravityController target)
		{
		}

		protected virtual void OnTargetFilteredOut(MonkeGravityController target)
		{
		}

		private bool PassesScaleFilter(MonkeGravityController target)
		{
			if (this.scaleFilter == GravityZoneScaleFilter.Anyone)
			{
				return true;
			}
			bool flag = target.TargetTransform.localScale.x < 1f;
			if (this.scaleFilter != GravityZoneScaleFilter.SmallOnly)
			{
				return !flag;
			}
			return flag;
		}

		private void OnTriggerEnter(Collider other)
		{
			ValueTuple<bool, MonkeGravityController> monkeGravityController = MonkeGravityManager.GetMonkeGravityController(other);
			if (!monkeGravityController.Item1)
			{
				return;
			}
			this.AddTarget(monkeGravityController.Item2);
		}

		private void OnTriggerExit(Collider other)
		{
			ValueTuple<bool, MonkeGravityController> monkeGravityController = MonkeGravityManager.GetMonkeGravityController(other);
			if (!monkeGravityController.Item1)
			{
				return;
			}
			this.RemoveTarget(monkeGravityController.Item2);
		}

		[Header("Gravity Settings")]
		[Tooltip("negative number pulls, positive number expels")]
		public float gravityStrength = -9.81f;

		[Tooltip("Filter which players are affected based on scale. Small = scale < 1")]
		[SerializeField]
		private GravityZoneScaleFilter scaleFilter;

		[Header("Rotation Settings")]
		[Tooltip("If enabled, rotates the target away from gravity direction to be upside down")]
		[SerializeField]
		protected bool invertRotationDirection;

		[SerializeField]
		protected bool rotateTarget = true;

		[SerializeField]
		protected float rotationSpeed = 10f;

		protected Vector3 m_gravityDirection;

		private ListProcessor<MonkeGravityController> m_gravityTargets = new ListProcessor<MonkeGravityController>(5, null);

		private Dictionary<MonkeGravityController, GravityInfo> m_targetGravityInfos = new Dictionary<MonkeGravityController, GravityInfo>(5);
	}
}
