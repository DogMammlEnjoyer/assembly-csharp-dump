using System;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class GtCameraModeComparator : MonoBehaviour
	{
		private void OnEnable()
		{
			this._gtSelectorsGroup.onCameraModeChanged.AddListener(new UnityAction<CameraMode>(this.EvaluateTargetModeSelection));
		}

		private void OnDisable()
		{
			this._gtSelectorsGroup.onCameraModeChanged.RemoveListener(new UnityAction<CameraMode>(this.EvaluateTargetModeSelection));
		}

		private void EvaluateTargetModeSelection(CameraMode mode)
		{
			bool arg = mode == this._targetMode;
			this.onTargetModeSelected.Invoke(arg);
		}

		[SerializeField]
		private GtSelectorsGroup _gtSelectorsGroup;

		[SerializeField]
		[Tooltip("Compares the target mode with the current one")]
		private CameraMode _targetMode;

		public UnityEvent<bool> onTargetModeSelected;
	}
}
