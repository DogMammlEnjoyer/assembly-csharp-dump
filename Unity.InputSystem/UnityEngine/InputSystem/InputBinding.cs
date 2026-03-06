using System;
using System.Linq;
using System.Text;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[Serializable]
	public struct InputBinding : IEquatable<InputBinding>
	{
		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public Guid id
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_Id))
				{
					return default(Guid);
				}
				return new Guid(this.m_Id);
			}
			set
			{
				this.m_Id = value.ToString();
			}
		}

		public string path
		{
			get
			{
				return this.m_Path;
			}
			set
			{
				this.m_Path = value;
			}
		}

		public string overridePath
		{
			get
			{
				return this.m_OverridePath;
			}
			set
			{
				this.m_OverridePath = value;
			}
		}

		public string interactions
		{
			get
			{
				return this.m_Interactions;
			}
			set
			{
				this.m_Interactions = value;
			}
		}

		public string overrideInteractions
		{
			get
			{
				return this.m_OverrideInteractions;
			}
			set
			{
				this.m_OverrideInteractions = value;
			}
		}

		public string processors
		{
			get
			{
				return this.m_Processors;
			}
			set
			{
				this.m_Processors = value;
			}
		}

		public string overrideProcessors
		{
			get
			{
				return this.m_OverrideProcessors;
			}
			set
			{
				this.m_OverrideProcessors = value;
			}
		}

		public string groups
		{
			get
			{
				return this.m_Groups;
			}
			set
			{
				this.m_Groups = value;
			}
		}

		public string action
		{
			get
			{
				return this.m_Action;
			}
			set
			{
				this.m_Action = value;
			}
		}

		public bool isComposite
		{
			get
			{
				return (this.m_Flags & InputBinding.Flags.Composite) == InputBinding.Flags.Composite;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= InputBinding.Flags.Composite;
					return;
				}
				this.m_Flags &= ~InputBinding.Flags.Composite;
			}
		}

		public bool isPartOfComposite
		{
			get
			{
				return (this.m_Flags & InputBinding.Flags.PartOfComposite) == InputBinding.Flags.PartOfComposite;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= InputBinding.Flags.PartOfComposite;
					return;
				}
				this.m_Flags &= ~InputBinding.Flags.PartOfComposite;
			}
		}

		public bool hasOverrides
		{
			get
			{
				return this.overridePath != null || this.overrideProcessors != null || this.overrideInteractions != null;
			}
		}

		public InputBinding(string path, string action = null, string groups = null, string processors = null, string interactions = null, string name = null)
		{
			this.m_Path = path;
			this.m_Action = action;
			this.m_Groups = groups;
			this.m_Processors = processors;
			this.m_Interactions = interactions;
			this.m_Name = name;
			this.m_Id = null;
			this.m_Flags = InputBinding.Flags.None;
			this.m_OverridePath = null;
			this.m_OverrideInteractions = null;
			this.m_OverrideProcessors = null;
		}

		public string GetNameOfComposite()
		{
			if (!this.isComposite)
			{
				return null;
			}
			return NameAndParameters.Parse(this.effectivePath).name;
		}

		internal void GenerateId()
		{
			this.m_Id = Guid.NewGuid().ToString();
		}

		internal void RemoveOverrides()
		{
			this.m_OverridePath = null;
			this.m_OverrideInteractions = null;
			this.m_OverrideProcessors = null;
		}

		public static InputBinding MaskByGroup(string group)
		{
			return new InputBinding
			{
				groups = group
			};
		}

		public static InputBinding MaskByGroups(params string[] groups)
		{
			InputBinding result = default(InputBinding);
			result.groups = string.Join(";", from x in groups
			where !string.IsNullOrEmpty(x)
			select x);
			return result;
		}

		public string effectivePath
		{
			get
			{
				return this.overridePath ?? this.path;
			}
		}

		public string effectiveInteractions
		{
			get
			{
				return this.overrideInteractions ?? this.interactions;
			}
		}

		public string effectiveProcessors
		{
			get
			{
				return this.overrideProcessors ?? this.processors;
			}
		}

		internal bool isEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.effectivePath) && string.IsNullOrEmpty(this.action) && string.IsNullOrEmpty(this.groups);
			}
		}

		public bool Equals(InputBinding other)
		{
			return string.Equals(this.effectivePath, other.effectivePath, StringComparison.InvariantCultureIgnoreCase) && string.Equals(this.effectiveInteractions, other.effectiveInteractions, StringComparison.InvariantCultureIgnoreCase) && string.Equals(this.effectiveProcessors, other.effectiveProcessors, StringComparison.InvariantCultureIgnoreCase) && string.Equals(this.groups, other.groups, StringComparison.InvariantCultureIgnoreCase) && string.Equals(this.action, other.action, StringComparison.InvariantCultureIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is InputBinding)
			{
				InputBinding other = (InputBinding)obj;
				return this.Equals(other);
			}
			return false;
		}

		public static bool operator ==(InputBinding left, InputBinding right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(InputBinding left, InputBinding right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			return (((((this.effectivePath != null) ? this.effectivePath.GetHashCode() : 0) * 397 ^ ((this.effectiveInteractions != null) ? this.effectiveInteractions.GetHashCode() : 0)) * 397 ^ ((this.effectiveProcessors != null) ? this.effectiveProcessors.GetHashCode() : 0)) * 397 ^ ((this.groups != null) ? this.groups.GetHashCode() : 0)) * 397 ^ ((this.action != null) ? this.action.GetHashCode() : 0);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(this.action))
			{
				stringBuilder.Append(this.action);
				stringBuilder.Append(':');
			}
			string effectivePath = this.effectivePath;
			if (!string.IsNullOrEmpty(effectivePath))
			{
				stringBuilder.Append(effectivePath);
			}
			if (!string.IsNullOrEmpty(this.groups))
			{
				stringBuilder.Append('[');
				stringBuilder.Append(this.groups);
				stringBuilder.Append(']');
			}
			return stringBuilder.ToString();
		}

		public string ToDisplayString(InputBinding.DisplayStringOptions options = (InputBinding.DisplayStringOptions)0, InputControl control = null)
		{
			string text;
			string text2;
			return this.ToDisplayString(out text, out text2, options, control);
		}

		public string ToDisplayString(out string deviceLayoutName, out string controlPath, InputBinding.DisplayStringOptions options = (InputBinding.DisplayStringOptions)0, InputControl control = null)
		{
			if (this.isComposite)
			{
				deviceLayoutName = null;
				controlPath = null;
				return string.Empty;
			}
			InputControlPath.HumanReadableStringOptions humanReadableStringOptions = InputControlPath.HumanReadableStringOptions.None;
			if ((options & InputBinding.DisplayStringOptions.DontOmitDevice) == (InputBinding.DisplayStringOptions)0)
			{
				humanReadableStringOptions |= InputControlPath.HumanReadableStringOptions.OmitDevice;
			}
			if ((options & InputBinding.DisplayStringOptions.DontUseShortDisplayNames) == (InputBinding.DisplayStringOptions)0)
			{
				humanReadableStringOptions |= InputControlPath.HumanReadableStringOptions.UseShortNames;
			}
			string text = InputControlPath.ToHumanReadableString(((options & InputBinding.DisplayStringOptions.IgnoreBindingOverrides) != (InputBinding.DisplayStringOptions)0) ? this.path : this.effectivePath, out deviceLayoutName, out controlPath, humanReadableStringOptions, control);
			if (!string.IsNullOrEmpty(this.effectiveInteractions) && (options & InputBinding.DisplayStringOptions.DontIncludeInteractions) == (InputBinding.DisplayStringOptions)0)
			{
				string text2 = string.Empty;
				foreach (NameAndParameters nameAndParameters in NameAndParameters.ParseMultiple(this.effectiveInteractions))
				{
					string displayName = InputInteraction.GetDisplayName(nameAndParameters.name);
					if (!string.IsNullOrEmpty(displayName))
					{
						if (!string.IsNullOrEmpty(text2))
						{
							text2 = text2 + " or " + displayName;
						}
						else
						{
							text2 = displayName;
						}
					}
				}
				if (!string.IsNullOrEmpty(text2))
				{
					text = text2 + " " + text;
				}
			}
			return text;
		}

		internal bool TriggersAction(InputAction action)
		{
			return string.Compare(action.name, this.action, StringComparison.InvariantCultureIgnoreCase) == 0 || this.action == action.m_Id;
		}

		public bool Matches(InputBinding binding)
		{
			return this.Matches(ref binding, (InputBinding.MatchOptions)0);
		}

		internal bool Matches(ref InputBinding binding, InputBinding.MatchOptions options = (InputBinding.MatchOptions)0)
		{
			if (this.name != null && (binding.name == null || !StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(this.name, binding.name, ';')))
			{
				return false;
			}
			if (this.path != null && (binding.path == null || !StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(this.path, binding.path, ';')))
			{
				return false;
			}
			if (this.action != null && (binding.action == null || !StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(this.action, binding.action, ';')))
			{
				return false;
			}
			if (this.groups != null)
			{
				bool flag = !string.IsNullOrEmpty(binding.groups);
				if (!flag && (options & InputBinding.MatchOptions.EmptyGroupMatchesAny) == (InputBinding.MatchOptions)0)
				{
					return false;
				}
				if (flag && !StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(this.groups, binding.groups, ';'))
				{
					return false;
				}
			}
			return string.IsNullOrEmpty(this.m_Id) || !(binding.id != this.id);
		}

		public const char Separator = ';';

		internal const string kSeparatorString = ";";

		[SerializeField]
		private string m_Name;

		[SerializeField]
		internal string m_Id;

		[Tooltip("Path of the control to bind to. Matched at runtime to controls from InputDevices present at the time.\n\nCan either be graphically from the control picker dropdown UI or edited manually in text mode by clicking the 'T' button. Internally, both methods result in control path strings that look like, for example, \"<Gamepad>/buttonSouth\".")]
		[SerializeField]
		private string m_Path;

		[SerializeField]
		private string m_Interactions;

		[SerializeField]
		private string m_Processors;

		[SerializeField]
		internal string m_Groups;

		[SerializeField]
		private string m_Action;

		[SerializeField]
		internal InputBinding.Flags m_Flags;

		[NonSerialized]
		private string m_OverridePath;

		[NonSerialized]
		private string m_OverrideInteractions;

		[NonSerialized]
		private string m_OverrideProcessors;

		[Flags]
		public enum DisplayStringOptions
		{
			DontUseShortDisplayNames = 1,
			DontOmitDevice = 2,
			DontIncludeInteractions = 4,
			IgnoreBindingOverrides = 8
		}

		[Flags]
		internal enum MatchOptions
		{
			EmptyGroupMatchesAny = 1
		}

		[Flags]
		internal enum Flags
		{
			None = 0,
			Composite = 4,
			PartOfComposite = 8
		}
	}
}
