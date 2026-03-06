using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class HmdRef : MonoBehaviour, IHmd
	{
		public event Action WhenUpdated
		{
			add
			{
				this.Hmd.WhenUpdated += value;
			}
			remove
			{
				this.Hmd.WhenUpdated -= value;
			}
		}

		protected virtual void Awake()
		{
			this.Hmd = (this._hmd as IHmd);
		}

		protected virtual void Start()
		{
		}

		public bool TryGetRootPose(out Pose pose)
		{
			return this.Hmd.TryGetRootPose(out pose);
		}

		public void InjectAllHmdRef(IHmd hmd)
		{
			this.InjectHmd(hmd);
		}

		public void InjectHmd(IHmd hmd)
		{
			this._hmd = (hmd as Object);
			this.Hmd = hmd;
		}

		[SerializeField]
		[Interface(typeof(IHmd), new Type[]
		{

		})]
		private Object _hmd;

		private IHmd Hmd;
	}
}
