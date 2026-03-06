using System;
using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.XR.MultiplayerBlocks.Fusion
{
	[NetworkBehaviourWeaved(65)]
	public class PlayerNameTagFusion : NetworkBehaviour
	{
		[Networked]
		[OnChangedRender("OnPlayerNameChange")]
		[NetworkedWeaved(0, 65)]
		public unsafe NetworkString<_64> OculusName
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing PlayerNameTagFusion.OculusName. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(NetworkString<_64>*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing PlayerNameTagFusion.OculusName. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(NetworkString<_64>*)(this.Ptr + 0) = value;
			}
		}

		private void Start()
		{
			this.nameTagGO.SetActive(!base.Object.HasStateAuthority);
			this.OnPlayerNameChange();
			if (OVRManager.instance)
			{
				this._centerEye = OVRManager.instance.GetComponentInChildren<OVRCameraRig>().centerEyeAnchor;
			}
		}

		private IEnumerator UpdateNameUI(string playerName)
		{
			this.nameTag.text = playerName;
			yield return new WaitForFixedUpdate();
			VerticalLayoutGroup component = this.nameTagPanel.GetComponent<VerticalLayoutGroup>();
			component.enabled = false;
			component.enabled = true;
			yield break;
		}

		private void OnPlayerNameChange()
		{
			base.StartCoroutine(this.UpdateNameUI(this.OculusName.ToString()));
		}

		public override void FixedUpdateNetwork()
		{
			if (!base.Object.HasStateAuthority)
			{
				return;
			}
			Vector3 vector = base.transform.position;
			this.nameTagContainer.localPosition = new Vector3(vector.x, Mathf.Sin(Time.time * 2f), vector.z) * 0.005f;
			Vector3 position = this._centerEye.transform.position;
			position.y += this.heightOffset;
			vector = Vector3.Lerp(vector, position, 0.1f);
			base.transform.position = vector;
		}

		private void Update()
		{
			if (base.Object.HasStateAuthority)
			{
				return;
			}
			if (this._centerEye != null)
			{
				this.nameTagGO.transform.LookAt(this._centerEye.position, Vector3.up);
			}
		}

		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			this.OculusName = this._OculusName;
		}

		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			this._OculusName = this.OculusName;
		}

		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("OculusName", 0, 65)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private NetworkString<_64> _OculusName;

		[SerializeField]
		private Text nameTag;

		[SerializeField]
		private GameObject nameTagGO;

		[SerializeField]
		private GameObject nameTagPanel;

		[SerializeField]
		private Transform nameTagContainer;

		[SerializeField]
		private float heightOffset = 0.3f;

		private Transform _centerEye;
	}
}
