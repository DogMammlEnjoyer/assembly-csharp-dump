using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Oculus.Interaction.Samples
{
	public class DropDownGroup : MonoBehaviour
	{
		public TextMeshProUGUI Title
		{
			get
			{
				return this._title;
			}
			set
			{
				this._title = value;
			}
		}

		public string TitleName
		{
			get
			{
				return this._titleName;
			}
			set
			{
				this._titleName = value;
			}
		}

		public TextMeshProUGUI Subtitle
		{
			get
			{
				return this._subtitle;
			}
			set
			{
				this._subtitle = value;
			}
		}

		public string SubtitleName
		{
			get
			{
				return this._subtitleName;
			}
			set
			{
				this._subtitleName = value;
			}
		}

		public Image Icon
		{
			get
			{
				return this._icon;
			}
			set
			{
				this._icon = value;
			}
		}

		public string IconName
		{
			get
			{
				return this._iconName;
			}
			set
			{
				this._iconName = value;
			}
		}

		public int SelectedIndex
		{
			get
			{
				return this._selectedIndex;
			}
			private set
			{
				if (this._selectedIndex != value)
				{
					this._selectedIndex = value;
					this.WhenSelectionChanged.Invoke(this._selectedIndex);
				}
			}
		}

		public Toggle SelectedToggle
		{
			get
			{
				if (this._selectedIndex < 0)
				{
					return null;
				}
				return this._toggles[this._selectedIndex];
			}
		}

		protected virtual void Reset()
		{
			this._toggleGroup = base.GetComponentInChildren<ToggleGroup>();
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			if (this._toggleGroup == null)
			{
				this._toggleGroup = base.GetComponentInChildren<ToggleGroup>();
			}
			if (this._toggles == null || this._toggles.Length == 0)
			{
				Toggle[] componentsInChildren = this._toggleGroup.transform.GetComponentsInChildren<Toggle>();
				this._toggles = (from toggle in componentsInChildren
				where toggle.@group == this._toggleGroup
				select toggle).ToArray<Toggle>();
			}
			this.InitializeToggleActions();
			this.InitializeToggleGroup();
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				for (int i = 0; i < this._toggles.Length; i++)
				{
					this._toggles[i].onValueChanged.AddListener(this._toggleActions[i]);
				}
				this.ForceUpdateSelectedIndex();
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				for (int i = 0; i < this._toggles.Length; i++)
				{
					this._toggles[i].onValueChanged.RemoveListener(this._toggleActions[i]);
				}
			}
		}

		private void ForceUpdateSelectedIndex()
		{
			for (int i = 0; i < this._toggles.Length; i++)
			{
				if (this._toggles[i].isOn)
				{
					this.SelectedIndex = i;
					break;
				}
			}
			this.HandleToggleChanged(true, this.SelectedIndex);
		}

		private void InitializeToggleActions()
		{
			this._toggleActions = new UnityAction<bool>[this._toggles.Length];
			for (int i = 0; i < this._toggleActions.Length; i++)
			{
				int toggleIndex = i;
				this._toggleActions[i] = delegate(bool isOn)
				{
					this.HandleToggleChanged(isOn, toggleIndex);
					if (isOn)
					{
						this._headerToggle.isOn = false;
					}
				};
			}
		}

		private void InitializeToggleGroup()
		{
			this._toggleGroup.allowSwitchOff = false;
			foreach (Toggle toggle in this._toggles)
			{
				if (!(toggle.group == this._toggleGroup))
				{
					if (toggle.group != null)
					{
						toggle.group.UnregisterToggle(toggle);
					}
					toggle.group = this._toggleGroup;
					this._toggleGroup.RegisterToggle(toggle);
				}
			}
			this._toggleGroup.EnsureValidState();
		}

		private void HandleToggleChanged(bool isOn, int index)
		{
			if (isOn && index >= 0)
			{
				Toggle toggle = this._toggles[index];
				TextMeshProUGUI textMeshProUGUI;
				if (this._title != null && this._title.gameObject.activeSelf && DropDownGroup.TryGetChildComponent<TextMeshProUGUI>(toggle.transform, this._titleName, out textMeshProUGUI))
				{
					this._title.text = textMeshProUGUI.text;
				}
				TextMeshProUGUI textMeshProUGUI2;
				if (this._subtitle != null && this._subtitle.gameObject.activeSelf && DropDownGroup.TryGetChildComponent<TextMeshProUGUI>(toggle.transform, this._subtitleName, out textMeshProUGUI2))
				{
					this._subtitle.text = textMeshProUGUI2.text;
				}
				Image image;
				if (this._icon != null && this._icon.gameObject.activeSelf && DropDownGroup.TryGetChildComponent<Image>(toggle.transform, this._iconName, out image))
				{
					this._icon.sprite = image.sprite;
				}
				this.SelectedIndex = index;
			}
		}

		private static bool TryGetChildComponent<TComponent>(Transform root, string childName, out TComponent component) where TComponent : Component
		{
			Transform transform = root.FindChildRecursive(childName);
			if (transform != null && transform.gameObject.activeSelf && transform.TryGetComponent<TComponent>(out component))
			{
				return true;
			}
			component = default(TComponent);
			return false;
		}

		public void InjectAllDropDownShowSelectedItem(Toggle[] toggles, ToggleGroup toggleGroup)
		{
			this.InjectToggles(toggles);
			this.InjectToggleGroup(toggleGroup);
		}

		public void InjectToggles(Toggle[] toggles)
		{
			this._toggles = toggles;
		}

		public void InjectToggleGroup(ToggleGroup toggleGroup)
		{
			this._toggleGroup = toggleGroup;
		}

		[SerializeField]
		[Optional(OptionalAttribute.Flag.AutoGenerated)]
		[Tooltip("ToggleGroup for all the options in the dropdown. It will be enforced to dissallow off-state and be referenced in all toggles. If none provided it will be searched in the hierarchy under this component.")]
		private ToggleGroup _toggleGroup;

		[SerializeField]
		[Optional]
		[Tooltip("The toggle for the headerIt will be closed automatically when an option is selected.")]
		private Toggle _headerToggle;

		[SerializeField]
		[Optional(OptionalAttribute.Flag.AutoGenerated)]
		[Tooltip("Toggles for all options in the dropdown, if none provided it will be searched in the hierarchy under the toggle group.")]
		private Toggle[] _toggles;

		public UnityEvent<int> WhenSelectionChanged;

		[Header("Header visuals")]
		[SerializeField]
		[Optional(OptionalAttribute.Flag.DontHide)]
		[Tooltip("Title label in the header")]
		private TextMeshProUGUI _title;

		[SerializeField]
		[ConditionalHide("_title", null, ConditionalHideAttribute.DisplayMode.HideIfTrue)]
		[Tooltip("Name of the Gameobject holding the title in each option.")]
		private string _titleName = "Title";

		[SerializeField]
		[Optional(OptionalAttribute.Flag.DontHide)]
		[Tooltip("Subtitle label in the header.")]
		private TextMeshProUGUI _subtitle;

		[SerializeField]
		[ConditionalHide("_subtitle", null, ConditionalHideAttribute.DisplayMode.HideIfTrue)]
		[Tooltip("Name of the Gameobject holding the subtitle in each option.")]
		private string _subtitleName = "Subtitle";

		[SerializeField]
		[Optional(OptionalAttribute.Flag.DontHide)]
		[Tooltip("Image for the icon in the header.")]
		private Image _icon;

		[SerializeField]
		[ConditionalHide("_icon", null, ConditionalHideAttribute.DisplayMode.HideIfTrue)]
		[Tooltip("Name of the Gameobject holding the icon in each option.")]
		private string _iconName = "Icon";

		private int _selectedIndex = -1;

		private UnityAction<bool>[] _toggleActions;

		protected bool _started;
	}
}
