using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Obsolete]
	[AddComponentMenu("")]
	public class CinemachineTouchInputMapper : MonoBehaviour
	{
		private void Start()
		{
			CinemachineCore.GetInputAxis = new CinemachineCore.AxisInputDelegate(this.GetInputAxis);
		}

		private float GetInputAxis(string axisName)
		{
			if (Input.touchCount > 0)
			{
				if (axisName == this.TouchXInputMapTo)
				{
					return Input.touches[0].deltaPosition.x / this.TouchSensitivityX;
				}
				if (axisName == this.TouchYInputMapTo)
				{
					return Input.touches[0].deltaPosition.y / this.TouchSensitivityY;
				}
			}
			return Input.GetAxis(axisName);
		}

		[Tooltip("Sensitivity multiplier for x-axis")]
		public float TouchSensitivityX = 10f;

		[Tooltip("Sensitivity multiplier for y-axis")]
		public float TouchSensitivityY = 10f;

		[Tooltip("Input channel to spoof for X axis")]
		public string TouchXInputMapTo = "Mouse X";

		[Tooltip("Input channel to spoof for Y axis")]
		public string TouchYInputMapTo = "Mouse Y";
	}
}
