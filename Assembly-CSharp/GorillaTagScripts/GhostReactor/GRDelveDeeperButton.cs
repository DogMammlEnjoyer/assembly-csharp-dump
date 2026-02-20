using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	[RequireComponent(typeof(GorillaPressableButton))]
	public sealed class GRDelveDeeperButton : MonoBehaviour, IGorillaSliceableSimple
	{
		private void Awake()
		{
			this.CountMonkes();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawCube(this._drillCollider.bounds.center, this._drillCollider.bounds.size);
		}

		private void CountMonkes()
		{
			int num = Physics.OverlapBoxNonAlloc(this._drillCollider.bounds.center, this._drillCollider.bounds.extents, this._overlapBoxResults, this._drillCollider.transform.rotation, 2048);
			this._numGorillasInDrill = 0;
			for (int i = 0; i < num; i++)
			{
				if (this._overlapBoxResults[i].GetComponent<VRRig>() != null && this._drillCollider.bounds.Contains(this._overlapBoxResults[i].transform.position))
				{
					this._numGorillasInDrill++;
				}
			}
		}

		private void OnEnable()
		{
			if (this._shiftManager == null)
			{
				throw new Exception("_shiftManager unset for GREndShiftButton.");
			}
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this._button = base.GetComponent<GorillaPressableButton>();
			this.UpdateButton();
		}

		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		public void SliceUpdate()
		{
			this.CountMonkes();
			this.UpdateButton();
		}

		private void UpdateButton()
		{
			if (this._shiftManager.authorizedToDelveDeeper && this._numGorillasInDrill == this._shiftManager.reactor.NumActivePlayers)
			{
				this._button.enabled = true;
				this._text.text = "DELVE\nNOW";
				return;
			}
			this._button.enabled = false;
			this._text.text = "DISABLED";
		}

		public void DelveDeeper()
		{
			this._shiftManager.EndShift();
		}

		[SerializeField]
		private BoxCollider _drillCollider;

		[SerializeField]
		private GhostReactorShiftManager _shiftManager;

		[SerializeField]
		private TextMeshPro _text;

		private GorillaPressableButton _button;

		private int _numGorillasInDrill;

		private readonly Collider[] _overlapBoxResults = new Collider[200];
	}
}
