using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Gizmo
{
	internal class GizmoRendererManager : MonoBehaviour
	{
		public void Setup(Type classType, MemberInfo memberInfo, DebugGizmoType gizmoType, Color gizmoColor, InstanceCache instanceCache)
		{
			this._classType = classType;
			this._memberInfo = memberInfo;
			this._isStatic = memberInfo.IsStatic();
			this._instanceCache = instanceCache;
			this._gizmoType = gizmoType;
			this._gizmoColor = gizmoColor;
		}

		private void Start()
		{
			this.AddGizmoRenderer();
		}

		private void Update()
		{
			if (this._isStatic && this._renderers.Count != 0)
			{
				this._renderers[0].UpdateDataSource(this._memberInfo.GetValue(null));
				this._renderers[0].enabled = this._enabledInstances.Contains(0);
				return;
			}
			List<InstanceHandle> cacheDataForClass = this._instanceCache.GetCacheDataForClass(this._classType);
			if (cacheDataForClass.Count == 0)
			{
				return;
			}
			while (this._renderers.Count < cacheDataForClass.Count)
			{
				this.AddGizmoRenderer();
			}
			int i;
			for (i = 0; i < cacheDataForClass.Count; i++)
			{
				InstanceHandle instanceHandle = cacheDataForClass[i];
				if (instanceHandle.Valid)
				{
					this._renderers[i].UpdateDataSource(this._memberInfo.GetValue(instanceHandle.Instance));
					this._renderers[i].enabled = this._enabledInstances.Contains(instanceHandle.InstanceId);
				}
				else
				{
					this._renderers[i].enabled = false;
				}
			}
			while (i < this._renderers.Count && this._renderers[i].enabled)
			{
				this._renderers[i].enabled = false;
				i++;
			}
		}

		private void AddGizmoRenderer()
		{
			GizmoRenderer gizmoRenderer = base.gameObject.AddComponent<GizmoRenderer>();
			gizmoRenderer.SetUpGizmo(this._gizmoType, this._gizmoColor);
			gizmoRenderer.enabled = false;
			this._renderers.Add(gizmoRenderer);
		}

		public bool GetState(Object instance)
		{
			int item = (instance != null) ? instance.GetInstanceID() : 0;
			return this._enabledInstances.Contains(item);
		}

		public void SetState(Object instance, bool state)
		{
			int item = (instance != null) ? instance.GetInstanceID() : 0;
			if (state)
			{
				this._enabledInstances.Add(item);
				return;
			}
			this._enabledInstances.Remove(item);
		}

		private Type _classType;

		private MemberInfo _memberInfo;

		private bool _isStatic;

		private InstanceCache _instanceCache;

		private DebugGizmoType _gizmoType;

		private Color _gizmoColor;

		private List<GizmoRenderer> _renderers = new List<GizmoRenderer>();

		private HashSet<int> _enabledInstances = new HashSet<int>();
	}
}
