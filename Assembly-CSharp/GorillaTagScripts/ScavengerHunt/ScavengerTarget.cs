using System;
using System.Collections;
using GorillaLocomotion.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.ScavengerHunt
{
	public class ScavengerTarget : MonoBehaviour, IGorillaGrabable
	{
		private void Awake()
		{
			base.StartCoroutine(this.ConnectToScavengerManager());
		}

		private IEnumerator ConnectToScavengerManager()
		{
			int num;
			for (int i = 0; i < 30; i = num)
			{
				if (!(ScavengerManager.Instance == null))
				{
					ScavengerManager.Instance.RegisterTarget(this);
					this._manager = ScavengerManager.Instance;
					yield break;
				}
				yield return null;
				num = i + 1;
			}
			Object.Destroy(this);
			throw new Exception(string.Format("No ScavengerManager found within {0} frames of attempts.", 30));
			yield break;
		}

		public void Collect()
		{
			this._manager.Collect(this);
		}

		public bool MomentaryGrabOnly()
		{
			return true;
		}

		public bool CanBeGrabbed(GorillaGrabber grabber)
		{
			return !this._manager.IsCollected(this);
		}

		public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
		{
			this.Collect();
			grabbedTransform = base.transform;
			localGrabbedPosition = base.transform.InverseTransformPoint(grabber.transform.position);
		}

		public void OnGrabReleased(GorillaGrabber grabber)
		{
		}

		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		public string HuntName;

		public string TargetName;

		public UnityEvent[] TargetCollected;

		public UnityEvent<ScavengerTarget>[] TargetCollectedArg;

		private ScavengerManager _manager;
	}
}
