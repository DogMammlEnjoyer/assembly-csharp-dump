using System;
using Oculus.Interaction.Input;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class PoseUseSample : MonoBehaviour
	{
		private IHmd Hmd { get; set; }

		protected virtual void Awake()
		{
			this.Hmd = (this._hmd as IHmd);
		}

		protected virtual void Start()
		{
			this._poseActiveVisuals = new GameObject[this._poses.Length];
			for (int i = 0; i < this._poses.Length; i++)
			{
				this._poseActiveVisuals[i] = Object.Instantiate<GameObject>(this._poseActiveVisualPrefab);
				this._poseActiveVisuals[i].GetComponentInChildren<TextMeshPro>().text = this._poses[i].name;
				this._poseActiveVisuals[i].GetComponentInChildren<ParticleSystemRenderer>().material = this._onSelectIcons[i];
				this._poseActiveVisuals[i].SetActive(false);
				int poseNumber = i;
				this._poses[i].WhenSelected += delegate()
				{
					this.ShowVisuals(poseNumber);
				};
				this._poses[i].WhenUnselected += delegate()
				{
					this.HideVisuals(poseNumber);
				};
			}
		}

		private void ShowVisuals(int poseNumber)
		{
			Pose pose;
			if (!this.Hmd.TryGetRootPose(out pose))
			{
				return;
			}
			Vector3 position = pose.position + pose.forward;
			this._poseActiveVisuals[poseNumber].transform.position = position;
			this._poseActiveVisuals[poseNumber].transform.LookAt(2f * this._poseActiveVisuals[poseNumber].transform.position - pose.position);
			HandRef[] components = this._poses[poseNumber].GetComponents<HandRef>();
			Vector3 a = Vector3.zero;
			foreach (HandRef handRef in components)
			{
				Pose pose2;
				handRef.GetRootPose(out pose2);
				Vector3 a2 = (handRef.Handedness == Handedness.Left) ? pose2.right : (-pose2.right);
				a += pose2.position + a2 * 0.15f + Vector3.up * 0.02f;
			}
			this._poseActiveVisuals[poseNumber].transform.position = a / (float)components.Length;
			this._poseActiveVisuals[poseNumber].gameObject.SetActive(true);
		}

		private void HideVisuals(int poseNumber)
		{
			this._poseActiveVisuals[poseNumber].gameObject.SetActive(false);
		}

		[SerializeField]
		[Interface(typeof(IHmd), new Type[]
		{

		})]
		private Object _hmd;

		[SerializeField]
		private ActiveStateSelector[] _poses;

		[SerializeField]
		private Material[] _onSelectIcons;

		[SerializeField]
		private GameObject _poseActiveVisualPrefab;

		private GameObject[] _poseActiveVisuals;
	}
}
