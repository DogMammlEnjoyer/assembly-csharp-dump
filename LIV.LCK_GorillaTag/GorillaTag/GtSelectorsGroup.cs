using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class GtSelectorsGroup : MonoBehaviour
	{
		public CameraMode CurrentMode
		{
			get
			{
				return this._currentMode;
			}
			set
			{
				this._currentMode = value;
				this.onCameraModeChanged.Invoke(this._currentMode);
			}
		}

		public void Select(CameraMode mode)
		{
			this._currentMode = mode;
			this.onCameraModeChanged.Invoke(this._currentMode);
		}

		private void Awake()
		{
			foreach (GtSelector gtSelector in this._selectors)
			{
				gtSelector.onCameraModeUpdate.AddListener(new UnityAction<CameraMode>(this.UpdateCurrentMode));
			}
		}

		private void Start()
		{
			foreach (GtSelector @object in this._selectors)
			{
				this.onCameraModeChanged.AddListener(new UnityAction<CameraMode>(@object.ListenToCameraModeChanged));
			}
			this.onCameraModeChanged.Invoke(this._currentMode);
		}

		private void OnDestroy()
		{
			foreach (GtSelector @object in this._selectors)
			{
				this.onCameraModeChanged.RemoveListener(new UnityAction<CameraMode>(@object.ListenToCameraModeChanged));
			}
		}

		private void UpdateCurrentMode(CameraMode mode)
		{
			this.CurrentMode = mode;
		}

		[SerializeField]
		private List<GtSelector> _selectors;

		public UnityEvent<CameraMode> onCameraModeChanged;

		private CameraMode _currentMode;
	}
}
