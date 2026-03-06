using System;
using UnityEngine;
using UnityEngine.UI;

public class AlertViewHUD : MonoBehaviour
{
	private static AlertViewHUD Instance { get; set; }

	public int HideAfterSec
	{
		get
		{
			return this._hideAfterSec;
		}
		set
		{
			this._hideAfterSec = value;
		}
	}

	public bool CenterInCamera
	{
		get
		{
			return this._centerInCamera;
		}
		set
		{
			this._centerInCamera = value;
		}
	}

	private bool Hidden
	{
		get
		{
			return !this._panel.activeSelf;
		}
	}

	private void Awake()
	{
		AlertViewHUD.Instance = this;
		OVRCameraRig ovrcameraRig = Object.FindObjectOfType<OVRCameraRig>();
		this._centerEyeTransform = ((ovrcameraRig != null) ? ovrcameraRig.centerEyeAnchor : null);
		this._initialTime = Time.time;
		this._initialPosition = base.transform.position;
		this._initialRotation = base.transform.rotation;
		this.Hide();
	}

	public static void PostMessage(string message, AlertViewHUD.MessageType messageType = AlertViewHUD.MessageType.Warning)
	{
		if (AlertViewHUD.Instance == null)
		{
			return;
		}
		AlertViewHUD.Instance.Post(message, messageType);
	}

	private void Post(string message, AlertViewHUD.MessageType type)
	{
		switch (type)
		{
		case AlertViewHUD.MessageType.Info:
			this._messageTypeIconField.sprite = this._infoIcon;
			this._messageTypeTextField.text = "Info";
			break;
		case AlertViewHUD.MessageType.Warning:
			this._messageTypeIconField.sprite = this._warningIcon;
			this._messageTypeTextField.text = "Warning";
			break;
		case AlertViewHUD.MessageType.Error:
			this._messageTypeIconField.sprite = this._errorIcon;
			this._messageTypeTextField.text = "Error";
			break;
		}
		this._messageTextField.text = message + "\n";
		this.Reset();
	}

	private void ClearMessage()
	{
		this._messageTextField.text = "";
	}

	private void Update()
	{
		this.CalculateHideAfterMessage();
		this.FollowCamera();
	}

	private void CalculateHideAfterMessage()
	{
		if (this.HideAfterSec == -1 || this.Hidden)
		{
			return;
		}
		if (Time.time - this._initialTime >= (float)this.HideAfterSec)
		{
			this.Hide();
		}
	}

	private void Reset()
	{
		this._initialTime = Time.time;
		this._panel.SetActive(true);
	}

	private void Hide()
	{
		this._panel.SetActive(false);
	}

	private void FollowCamera()
	{
		if (this._centerEyeTransform == null || this.Hidden || !this.CenterInCamera)
		{
			return;
		}
		Vector3 b = this._centerEyeTransform.TransformPoint(this._initialPosition);
		Quaternion b2 = this._centerEyeTransform.rotation * this._initialRotation;
		Vector3 position = Vector3.Lerp(base.transform.position, b, Time.deltaTime * this._speed);
		Quaternion rotation = Quaternion.Lerp(base.transform.rotation, b2, Time.deltaTime * this._speed);
		base.transform.SetPositionAndRotation(position, rotation);
	}

	[Tooltip("Set -1 to show always.")]
	[SerializeField]
	internal int _hideAfterSec = 20;

	[SerializeField]
	internal bool _centerInCamera = true;

	[SerializeField]
	private GameObject _panel;

	[SerializeField]
	private Sprite _warningIcon;

	[SerializeField]
	private Sprite _errorIcon;

	[SerializeField]
	private Sprite _infoIcon;

	[SerializeField]
	private Text _messageTextField;

	[SerializeField]
	private Text _messageTypeTextField;

	[SerializeField]
	private Image _messageTypeIconField;

	private Transform _centerEyeTransform;

	private float _initialTime;

	private Vector3 _initialPosition;

	private Quaternion _initialRotation;

	private float _speed = 7f;

	public enum MessageType
	{
		Info,
		Warning,
		Error
	}
}
