using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class CoconutCamera : MonoBehaviour, IGtCameraVisuals
	{
		private void Awake()
		{
			this._propertyBlock = new MaterialPropertyBlock();
			this._isRecordingID = Shader.PropertyToID(this.IS_RECORDING);
		}

		public void SetVisualsActive(bool active)
		{
			if (this._isNetworkedVersion)
			{
				return;
			}
			this._visuals.SetActive(active);
			this.SetRecordingState(this._isRecording);
		}

		public void SetNetworkedVisualsActive(bool active)
		{
			this._visuals.SetActive(active);
			this.SetRecordingState(this._isRecording);
		}

		public void SetRecordingState(bool isRecording)
		{
			this._propertyBlock.SetInt(this._isRecordingID, isRecording ? 1 : 0);
			if (this._visuals.gameObject.activeInHierarchy && !this._bodyRenderer.gameObject.activeInHierarchy)
			{
				foreach (Transform transform in this._visuals.transform.GetComponentsInChildren<Transform>(false))
				{
					if (transform.name == "Body")
					{
						this._bodyRenderer = transform.GetComponent<MeshRenderer>();
						if (this._bodyRenderer != null)
						{
							break;
						}
					}
				}
			}
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
			this._isRecording = isRecording;
		}

		[SerializeField]
		private GameObject _visuals;

		[SerializeField]
		private MeshRenderer _bodyRenderer;

		[SerializeField]
		private bool _isNetworkedVersion;

		private MaterialPropertyBlock _propertyBlock;

		private string IS_RECORDING = "_Is_Recording";

		private int _isRecordingID;

		private bool _isRecording;
	}
}
