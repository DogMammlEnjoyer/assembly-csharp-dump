using System;
using UnityEngine;

namespace Liv.Lck
{
	[RequireComponent(typeof(Camera))]
	public class LckTargetEyeSetter : MonoBehaviour
	{
		private void OnValidate()
		{
			base.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.None;
		}
	}
}
