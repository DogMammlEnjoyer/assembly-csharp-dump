using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Oculus.Interaction.Locomotion
{
	public class LocomotionTurnSliderSetting : MonoBehaviour
	{
		protected void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._slider.onValueChanged.AddListener(new UnityAction<float>(this.HandleValueChanged));
				this._snapTurnToggle.onValueChanged.AddListener(new UnityAction<bool>(this.HandleSnapTurnChanged));
				this._smoothTurnToggle.onValueChanged.AddListener(new UnityAction<bool>(this.HandleSmoothTurnChanged));
				this.HandleValueChanged(this._slider.value);
				this.HandleSnapTurnChanged(this._snapTurnToggle.isOn);
				this.HandleSmoothTurnChanged(this._smoothTurnToggle.isOn);
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._slider.onValueChanged.RemoveListener(new UnityAction<float>(this.HandleValueChanged));
				this._snapTurnToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.HandleSnapTurnChanged));
				this._smoothTurnToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.HandleSmoothTurnChanged));
			}
		}

		private void HandleValueChanged(float arg0)
		{
			int num = Mathf.RoundToInt(arg0);
			float snapTurnDegrees = this._snapTurnSteps[num];
			AnimationCurve smoothTurnCurve = this._smoothTurnSteps[num];
			foreach (TurnerEventBroadcaster turnerEventBroadcaster in this._controllerTurners)
			{
				turnerEventBroadcaster.SnapTurnDegrees = snapTurnDegrees;
				turnerEventBroadcaster.SmoothTurnCurve = smoothTurnCurve;
			}
			foreach (TurnerEventBroadcaster turnerEventBroadcaster2 in this._handTurners)
			{
				turnerEventBroadcaster2.SnapTurnDegrees = snapTurnDegrees;
				turnerEventBroadcaster2.SmoothTurnCurve = smoothTurnCurve;
			}
			foreach (TurnLocomotionBroadcaster turnLocomotionBroadcaster in this._locomotionTurners)
			{
				turnLocomotionBroadcaster.SnapTurnDegrees = snapTurnDegrees;
				turnLocomotionBroadcaster.SmoothTurnCurve = smoothTurnCurve;
			}
		}

		private void HandleSnapTurnChanged(bool snapTurn)
		{
			if (!snapTurn)
			{
				return;
			}
			TurnerEventBroadcaster[] controllerTurners = this._controllerTurners;
			for (int i = 0; i < controllerTurners.Length; i++)
			{
				controllerTurners[i].TurnMethod = TurnerEventBroadcaster.TurnMode.Snap;
			}
		}

		private void HandleSmoothTurnChanged(bool smoothTurn)
		{
			if (!smoothTurn)
			{
				return;
			}
			TurnerEventBroadcaster[] controllerTurners = this._controllerTurners;
			for (int i = 0; i < controllerTurners.Length; i++)
			{
				controllerTurners[i].TurnMethod = TurnerEventBroadcaster.TurnMode.Smooth;
			}
		}

		[SerializeField]
		private Slider _slider;

		[SerializeField]
		private Toggle _snapTurnToggle;

		[SerializeField]
		private Toggle _smoothTurnToggle;

		[SerializeField]
		private float[] _snapTurnSteps = new float[]
		{
			30f,
			45f,
			90f
		};

		[SerializeField]
		private AnimationCurve[] _smoothTurnSteps;

		[SerializeField]
		private TurnerEventBroadcaster[] _controllerTurners;

		[SerializeField]
		private TurnerEventBroadcaster[] _handTurners;

		[SerializeField]
		private TurnLocomotionBroadcaster[] _locomotionTurners;

		protected bool _started;
	}
}
