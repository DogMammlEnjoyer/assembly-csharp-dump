using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	[CreateAssetMenu(fileName = "LIV/GT UI Settings", menuName = "GT UI Settings", order = 0)]
	public class GtUiSettings : ScriptableObject
	{
		public Material DefaultBodyMaterial
		{
			get
			{
				return this._defaultBodyMaterial;
			}
		}

		public Material SelectedBodyMaterial
		{
			get
			{
				return this._selectedBodyMaterial;
			}
		}

		public Material RecordingBodyMaterial
		{
			get
			{
				return this._recordingBodyMaterial;
			}
		}

		public Color PrimaryColor
		{
			get
			{
				return this._primaryColor;
			}
		}

		public Color PrimaryTextColor
		{
			get
			{
				return this._primaryTextColor;
			}
		}

		public Color SecondaryTextColor
		{
			get
			{
				return this._secondaryTextColor;
			}
		}

		public Color DisabledTextColor
		{
			get
			{
				return this._disabledTextColor;
			}
		}

		public Color PrimaryCounterButtonDefaultColor
		{
			get
			{
				return this._primaryCounterButtonDefaultColor;
			}
		}

		public Color PrimaryCounterButtonActiveColor
		{
			get
			{
				return this._primaryCounterButtonActiveColor;
			}
		}

		public CameraModeAsset SelfieModeAsset
		{
			get
			{
				return this._selfieMode;
			}
		}

		public CameraModeAsset FirstPersonModeAsset
		{
			get
			{
				return this._firstPersonMode;
			}
		}

		public CameraModeAsset ThirdPersonModeAsset
		{
			get
			{
				return this._thirdPersonMode;
			}
		}

		public float ActiveButtonOffset
		{
			get
			{
				return this._activeButtonOffset;
			}
		}

		public float CounterAngleOffset
		{
			get
			{
				return this._counterAngleOffset;
			}
		}

		public Color PrimaryIconColor
		{
			get
			{
				return this._primaryIconColor;
			}
		}

		public Color SecondaryIconColor
		{
			get
			{
				return this._secondaryIconColor;
			}
		}

		public Color InactiveIconColor
		{
			get
			{
				return this._inactiveIconColor;
			}
		}

		[Header("Body Materials")]
		[SerializeField]
		private Material _defaultBodyMaterial;

		[SerializeField]
		private Material _selectedBodyMaterial;

		[SerializeField]
		private Material _recordingBodyMaterial;

		[Space(10f)]
		[Header("UI")]
		[SerializeField]
		private Color _primaryColor;

		[Space(10f)]
		[Header("Text Colors")]
		[SerializeField]
		private Color _primaryTextColor;

		[SerializeField]
		private Color _secondaryTextColor;

		[SerializeField]
		private Color _disabledTextColor;

		[Space(10f)]
		[SerializeField]
		private Color _primaryCounterButtonDefaultColor;

		[SerializeField]
		private Color _primaryCounterButtonActiveColor;

		[Space(10f)]
		[Header("Icon Colors")]
		[SerializeField]
		private Color _primaryIconColor;

		[SerializeField]
		private Color _secondaryIconColor;

		[SerializeField]
		private Color _inactiveIconColor;

		[Space(10f)]
		[Header("Offsets")]
		[SerializeField]
		private float _activeButtonOffset;

		[SerializeField]
		private float _counterAngleOffset;

		[Space(10f)]
		[Header("Elements for Selector Modes")]
		[SerializeField]
		private CameraModeAsset _selfieMode;

		[SerializeField]
		private CameraModeAsset _firstPersonMode;

		[SerializeField]
		private CameraModeAsset _thirdPersonMode;
	}
}
