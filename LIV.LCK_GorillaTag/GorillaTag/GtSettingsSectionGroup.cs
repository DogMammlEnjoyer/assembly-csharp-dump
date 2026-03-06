using System;
using System.Collections.Generic;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtSettingsSectionGroup : MonoBehaviour
	{
		public void EvaluateMode(CameraMode mode)
		{
			foreach (SettingsSectionController settingsSectionController in this._sections)
			{
				settingsSectionController.EvaluateMode(mode);
			}
		}

		[SerializeField]
		private List<SettingsSectionController> _sections;
	}
}
