using System;
using System.Linq;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Feature(Feature.Interaction)]
	public class SetDisplayRefresh : MonoBehaviour
	{
		public void SetDesiredDisplayFrequency(float desiredDisplayFrequency)
		{
			if (OVRPlugin.systemDisplayFrequenciesAvailable.Contains(this._desiredDisplayFrequency))
			{
				Debug.Log("[Oculus.Interaction] Setting desired display frequency to " + this._desiredDisplayFrequency.ToString());
				OVRPlugin.systemDisplayFrequency = this._desiredDisplayFrequency;
			}
		}

		protected virtual void Awake()
		{
			this.SetDesiredDisplayFrequency(this._desiredDisplayFrequency);
		}

		[SerializeField]
		private float _desiredDisplayFrequency = 90f;
	}
}
