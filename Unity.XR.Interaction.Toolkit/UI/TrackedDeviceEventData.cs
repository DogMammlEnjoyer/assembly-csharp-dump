using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	public class TrackedDeviceEventData : PointerEventData
	{
		public TrackedDeviceEventData(EventSystem eventSystem) : base(eventSystem)
		{
		}

		public List<Vector3> rayPoints { get; set; }

		public int rayHitIndex { get; set; }

		public LayerMask layerMask { get; set; }

		public IUIInteractor interactor
		{
			get
			{
				XRUIInputModule xruiinputModule = base.currentInputModule as XRUIInputModule;
				if (xruiinputModule != null)
				{
					return xruiinputModule.GetInteractor(base.pointerId);
				}
				return null;
			}
		}

		internal Vector3 pressWorldPosition { get; set; }
	}
}
