using System;
using UnityEngine.Events;

namespace UnityEngine.Localization.PropertyVariants.TrackedObjects
{
	[CustomTrackedObject(typeof(MonoBehaviour), true)]
	[Serializable]
	public class TrackedMonoBehaviourObject : JsonSerializerTrackedObject
	{
		public UnityEvent Changed
		{
			get
			{
				return this.m_Changed;
			}
		}

		protected override void PostApplyTrackedProperties()
		{
			base.PostApplyTrackedProperties();
			this.m_Changed.Invoke();
		}

		[SerializeField]
		private UnityEvent m_Changed = new UnityEvent();
	}
}
