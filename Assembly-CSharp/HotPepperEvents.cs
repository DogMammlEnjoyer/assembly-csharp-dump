using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

public class HotPepperEvents : MonoBehaviour
{
	private void OnEnable()
	{
		this._pepper.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	private void OnDisable()
	{
		this._pepper.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		if (nextState != 8)
		{
			return;
		}
		GameObject gameObject = rig.cosmeticReferences.Get(this.m_targetEffectID);
		if (gameObject.IsNull())
		{
			return;
		}
		HotPepperFace component = gameObject.GetComponent<HotPepperFace>();
		if (component.IsNull())
		{
			return;
		}
		component.PlayFX(1f);
	}

	[SerializeField]
	private EdibleHoldable _pepper;

	[SerializeField]
	private CosmeticRefID m_targetEffectID = CosmeticRefID.HotPepperFaceEffect;

	public enum EdibleState
	{
		A = 1,
		B,
		C = 4,
		D = 8
	}
}
