using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Photon.Pun.UtilityScripts
{
	public class OnClickRpc : MonoBehaviourPun, IPointerClickHandler, IEventSystemHandler
	{
		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (!PhotonNetwork.InRoom || (this.ModifierKey != KeyCode.None && !Input.GetKey(this.ModifierKey)) || eventData.button != this.Button)
			{
				return;
			}
			base.photonView.RPC("ClickRpc", this.Target, Array.Empty<object>());
		}

		[PunRPC]
		public void ClickRpc()
		{
			base.StartCoroutine(this.ClickFlash());
		}

		public IEnumerator ClickFlash()
		{
			if (this.isFlashing)
			{
				yield break;
			}
			this.isFlashing = true;
			this.originalMaterial = base.GetComponent<Renderer>().material;
			if (!this.originalMaterial.HasProperty("_EmissionColor"))
			{
				string str = "Doesn't have emission, can't flash ";
				GameObject gameObject = base.gameObject;
				Debug.LogWarning(str + ((gameObject != null) ? gameObject.ToString() : null));
				yield break;
			}
			bool wasEmissive = this.originalMaterial.IsKeywordEnabled("_EMISSION");
			this.originalMaterial.EnableKeyword("_EMISSION");
			this.originalColor = this.originalMaterial.GetColor("_EmissionColor");
			this.originalMaterial.SetColor("_EmissionColor", Color.white);
			for (float f = 0f; f <= 1f; f += 0.08f)
			{
				Color value = Color.Lerp(Color.white, this.originalColor, f);
				this.originalMaterial.SetColor("_EmissionColor", value);
				yield return null;
			}
			this.originalMaterial.SetColor("_EmissionColor", this.originalColor);
			if (!wasEmissive)
			{
				this.originalMaterial.DisableKeyword("_EMISSION");
			}
			this.isFlashing = false;
			yield break;
		}

		public PointerEventData.InputButton Button;

		public KeyCode ModifierKey;

		public RpcTarget Target;

		private Material originalMaterial;

		private Color originalColor;

		private bool isFlashing;
	}
}
