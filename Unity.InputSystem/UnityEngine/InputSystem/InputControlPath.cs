using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public static class InputControlPath
	{
		internal static string CleanSlashes(this string pathComponent)
		{
			return pathComponent.Replace('/', ' ');
		}

		public static string Combine(InputControl parent, string path)
		{
			if (parent == null)
			{
				if (string.IsNullOrEmpty(path))
				{
					return string.Empty;
				}
				if (path[0] != '/')
				{
					return "/" + path;
				}
				return path;
			}
			else
			{
				if (string.IsNullOrEmpty(path))
				{
					return parent.path;
				}
				return parent.path + "/" + path;
			}
		}

		public static string ToHumanReadableString(string path, InputControlPath.HumanReadableStringOptions options = InputControlPath.HumanReadableStringOptions.None, InputControl control = null)
		{
			string text;
			string text2;
			return InputControlPath.ToHumanReadableString(path, out text, out text2, options, control);
		}

		public static string ToHumanReadableString(string path, out string deviceLayoutName, out string controlPath, InputControlPath.HumanReadableStringOptions options = InputControlPath.HumanReadableStringOptions.None, InputControl control = null)
		{
			deviceLayoutName = null;
			controlPath = null;
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			if (control != null)
			{
				InputControl inputControl = InputControlPath.TryFindControl(control, path, 0) ?? (InputControlPath.Matches(path, control) ? control : null);
				if (inputControl != null)
				{
					string text = ((options & InputControlPath.HumanReadableStringOptions.UseShortNames) != InputControlPath.HumanReadableStringOptions.None && !string.IsNullOrEmpty(inputControl.shortDisplayName)) ? inputControl.shortDisplayName : inputControl.displayName;
					if ((options & InputControlPath.HumanReadableStringOptions.OmitDevice) == InputControlPath.HumanReadableStringOptions.None)
					{
						text = text + " [" + inputControl.device.displayName + "]";
					}
					deviceLayoutName = inputControl.device.layout;
					if (!(inputControl is InputDevice))
					{
						controlPath = inputControl.path.Substring(inputControl.device.path.Length + 1);
					}
					return text;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			InputControlPath.PathParser pathParser = new InputControlPath.PathParser(path);
			string result;
			using (InputControlLayout.CacheRef())
			{
				if (pathParser.MoveToNextComponent())
				{
					string text2;
					string value = pathParser.current.ToHumanReadableString(null, null, out text2, out result, options);
					deviceLayoutName = text2;
					bool flag = true;
					while (pathParser.MoveToNextComponent())
					{
						if (!flag)
						{
							stringBuilder.Append('/');
						}
						stringBuilder.Append(pathParser.current.ToHumanReadableString(text2, controlPath, out text2, out controlPath, options));
						flag = false;
					}
					if ((options & InputControlPath.HumanReadableStringOptions.OmitDevice) == InputControlPath.HumanReadableStringOptions.None && !string.IsNullOrEmpty(value))
					{
						stringBuilder.Append(" [");
						stringBuilder.Append(value);
						stringBuilder.Append(']');
					}
				}
				if (stringBuilder.Length == 0)
				{
					result = path;
				}
				else
				{
					result = stringBuilder.ToString();
				}
			}
			return result;
		}

		public static string[] TryGetDeviceUsages(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			InputControlPath.PathParser pathParser = new InputControlPath.PathParser(path);
			if (!pathParser.MoveToNextComponent())
			{
				return null;
			}
			if (pathParser.current.m_Usages.length > 0)
			{
				return pathParser.current.m_Usages.ToArray<string>((Substring x) => x.ToString());
			}
			return null;
		}

		public static string TryGetDeviceLayout(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			InputControlPath.PathParser pathParser = new InputControlPath.PathParser(path);
			if (!pathParser.MoveToNextComponent())
			{
				return null;
			}
			if (pathParser.current.m_Layout.length > 0)
			{
				return pathParser.current.m_Layout.ToString().Unescape("ntr\\\"", "\n\t\r\\\"");
			}
			if (pathParser.current.isWildcard)
			{
				return "*";
			}
			return null;
		}

		public static string TryGetControlLayout(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			int length = path.Length;
			int num = path.LastIndexOf('/');
			if (num == -1 || num == 0)
			{
				return null;
			}
			if (length > num + 2 && path[num + 1] == '<' && path[length - 1] == '>')
			{
				int num2 = num + 2;
				int length2 = length - num2 - 1;
				return path.Substring(num2, length2);
			}
			InputControlPath.PathParser pathParser = new InputControlPath.PathParser(path);
			if (!pathParser.MoveToNextComponent())
			{
				return null;
			}
			if (pathParser.current.isWildcard)
			{
				throw new NotImplementedException();
			}
			if (pathParser.current.m_Layout.length == 0)
			{
				return null;
			}
			string str = pathParser.current.m_Layout.ToString();
			if (!pathParser.MoveToNextComponent())
			{
				return null;
			}
			if (pathParser.current.isWildcard)
			{
				return "*";
			}
			return InputControlPath.FindControlLayoutRecursive(ref pathParser, str.Unescape("ntr\\\"", "\n\t\r\\\""));
		}

		private static string FindControlLayoutRecursive(ref InputControlPath.PathParser parser, string layoutName)
		{
			string result;
			using (InputControlLayout.CacheRef())
			{
				InputControlLayout inputControlLayout = InputControlLayout.cache.FindOrLoadLayout(new InternedString(layoutName), false);
				if (inputControlLayout == null)
				{
					result = null;
				}
				else
				{
					result = InputControlPath.FindControlLayoutRecursive(ref parser, inputControlLayout);
				}
			}
			return result;
		}

		private static string FindControlLayoutRecursive(ref InputControlPath.PathParser parser, InputControlLayout layout)
		{
			string text = null;
			int count = layout.controls.Count;
			for (int i = 0; i < count; i++)
			{
				if (InputControlPath.ControlLayoutMatchesPathComponent(ref layout.m_Controls[i], ref parser))
				{
					InternedString layout2 = layout.m_Controls[i].layout;
					if (!parser.isAtEnd)
					{
						InputControlPath.PathParser pathParser = parser;
						if (pathParser.MoveToNextComponent())
						{
							string text2 = InputControlPath.FindControlLayoutRecursive(ref pathParser, layout2);
							if (text2 != null)
							{
								if (text != null && text2 != text)
								{
									return null;
								}
								text = text2;
							}
						}
					}
					else
					{
						if (text != null && layout2 != text)
						{
							return null;
						}
						text = layout2.ToString();
					}
				}
			}
			return text;
		}

		private static bool ControlLayoutMatchesPathComponent(ref InputControlLayout.ControlItem controlItem, ref InputControlPath.PathParser parser)
		{
			Substring layout = parser.current.m_Layout;
			if (layout.length > 0 && !InputControlPath.StringMatches(layout, controlItem.layout))
			{
				return false;
			}
			if (parser.current.m_Usages.length > 0)
			{
				for (int i = 0; i < parser.current.m_Usages.length; i++)
				{
					Substring str = parser.current.m_Usages[i];
					if (str.length > 0)
					{
						int count = controlItem.usages.Count;
						bool flag = false;
						for (int j = 0; j < count; j++)
						{
							if (InputControlPath.StringMatches(str, controlItem.usages[j]))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							return false;
						}
					}
				}
			}
			Substring name = parser.current.m_Name;
			return name.length <= 0 || InputControlPath.StringMatches(name, controlItem.name);
		}

		private static bool StringMatches(Substring str, InternedString matchTo)
		{
			int length = str.length;
			int length2 = matchTo.length;
			string text = matchTo.ToLower();
			int num = 0;
			int num2 = 0;
			while (num2 < length && num < length2)
			{
				char c = str[num2];
				if (c == '\\' && num2 + 1 < length)
				{
					c = str[++num2];
				}
				if (c == '*')
				{
					if (num2 == length - 1)
					{
						return true;
					}
					num2++;
					c = char.ToLowerInvariant(str[num2]);
					while (num < length2 && text[num] != c)
					{
						num++;
					}
					if (num == length2)
					{
						return false;
					}
				}
				else if (char.ToLowerInvariant(c) != text[num])
				{
					return false;
				}
				num++;
				num2++;
			}
			return num == length2 && num2 == length;
		}

		public static InputControl TryFindControl(InputControl control, string path, int indexInPath = 0)
		{
			return InputControlPath.TryFindControl<InputControl>(control, path, indexInPath);
		}

		public static InputControl[] TryFindControls(InputControl control, string path, int indexInPath = 0)
		{
			InputControlList<InputControl> inputControlList = new InputControlList<InputControl>(Allocator.Temp, 0);
			InputControl[] result;
			try
			{
				InputControlPath.TryFindControls<InputControl>(control, path, indexInPath, ref inputControlList);
				result = inputControlList.ToArray(false);
			}
			finally
			{
				inputControlList.Dispose();
			}
			return result;
		}

		public static int TryFindControls(InputControl control, string path, ref InputControlList<InputControl> matches, int indexInPath = 0)
		{
			return InputControlPath.TryFindControls<InputControl>(control, path, indexInPath, ref matches);
		}

		public static TControl TryFindControl<TControl>(InputControl control, string path, int indexInPath = 0) where TControl : InputControl
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (string.IsNullOrEmpty(path))
			{
				return default(TControl);
			}
			if (indexInPath == 0 && path[0] == '/')
			{
				indexInPath++;
			}
			InputControlList<TControl> inputControlList = default(InputControlList<TControl>);
			return InputControlPath.MatchControlsRecursive<TControl>(control, path, indexInPath, ref inputControlList, false);
		}

		public static int TryFindControls<TControl>(InputControl control, string path, int indexInPath, ref InputControlList<TControl> matches) where TControl : InputControl
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (indexInPath == 0 && path[0] == '/')
			{
				indexInPath++;
			}
			int count = matches.Count;
			InputControlPath.MatchControlsRecursive<TControl>(control, path, indexInPath, ref matches, true);
			return matches.Count - count;
		}

		public static InputControl TryFindChild(InputControl control, string path, int indexInPath = 0)
		{
			return InputControlPath.TryFindChild<InputControl>(control, path, indexInPath);
		}

		public static TControl TryFindChild<TControl>(InputControl control, string path, int indexInPath = 0) where TControl : InputControl
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			ReadOnlyArray<InputControl> children = control.children;
			int count = children.Count;
			for (int i = 0; i < count; i++)
			{
				TControl tcontrol = InputControlPath.TryFindControl<TControl>(children[i], path, indexInPath);
				if (tcontrol != null)
				{
					return tcontrol;
				}
			}
			return default(TControl);
		}

		public static bool Matches(string expected, InputControl control)
		{
			if (string.IsNullOrEmpty(expected))
			{
				throw new ArgumentNullException("expected");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			InputControlPath.PathParser pathParser = new InputControlPath.PathParser(expected);
			return InputControlPath.MatchesRecursive(ref pathParser, control, false);
		}

		internal static bool MatchControlComponent(ref InputControlPath.ParsedPathComponent expectedControlComponent, ref InputControlLayout.ControlItem controlItem, bool matchAlias = false)
		{
			bool flag = false;
			bool flag2 = false;
			if (!expectedControlComponent.m_Name.isEmpty)
			{
				if (InputControlPath.StringMatches(expectedControlComponent.m_Name, controlItem.name))
				{
					flag = true;
				}
				else
				{
					if (!matchAlias)
					{
						return false;
					}
					ReadOnlyArray<InternedString> aliases = controlItem.aliases;
					for (int i = 0; i < aliases.Count; i++)
					{
						if (InputControlPath.StringMatches(expectedControlComponent.m_Name, aliases[i]))
						{
							flag = true;
							break;
						}
					}
				}
			}
			foreach (Substring str in expectedControlComponent.m_Usages)
			{
				if (!str.isEmpty)
				{
					int count = controlItem.usages.Count;
					for (int j = 0; j < count; j++)
					{
						if (InputControlPath.StringMatches(str, controlItem.usages[j]))
						{
							flag2 = true;
							break;
						}
					}
				}
			}
			return flag || flag2;
		}

		public static bool MatchesPrefix(string expected, InputControl control)
		{
			if (string.IsNullOrEmpty(expected))
			{
				throw new ArgumentNullException("expected");
			}
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			InputControlPath.PathParser pathParser = new InputControlPath.PathParser(expected);
			return InputControlPath.MatchesRecursive(ref pathParser, control, true) && pathParser.isAtEnd;
		}

		private static bool MatchesRecursive(ref InputControlPath.PathParser parser, InputControl currentControl, bool prefixOnly = false)
		{
			InputControl parent = currentControl.parent;
			if (parent != null && !InputControlPath.MatchesRecursive(ref parser, parent, prefixOnly))
			{
				return false;
			}
			if (!parser.MoveToNextComponent())
			{
				return prefixOnly;
			}
			return parser.current.Matches(currentControl);
		}

		private static TControl MatchControlsRecursive<TControl>(InputControl control, string path, int indexInPath, ref InputControlList<TControl> matches, bool matchMultiple) where TControl : InputControl
		{
			int length = path.Length;
			bool flag = true;
			if (path[indexInPath] == '<')
			{
				indexInPath++;
				flag = InputControlPath.MatchPathComponent(control.layout, path, ref indexInPath, InputControlPath.PathComponentType.Layout, 0);
				if (!flag)
				{
					InternedString layout = control.m_Layout;
					while (InputControlLayout.s_Layouts.baseLayoutTable.TryGetValue(layout, out layout))
					{
						flag = InputControlPath.MatchPathComponent(layout, path, ref indexInPath, InputControlPath.PathComponentType.Layout, 0);
						if (flag)
						{
							break;
						}
					}
				}
			}
			while (indexInPath < length && path[indexInPath] == '{' && flag)
			{
				indexInPath++;
				for (int i = 0; i < control.usages.Count; i++)
				{
					flag = InputControlPath.MatchPathComponent(control.usages[i], path, ref indexInPath, InputControlPath.PathComponentType.Usage, 0);
					if (flag)
					{
						break;
					}
				}
			}
			if (indexInPath < length - 1 && flag && path[indexInPath] == '#' && path[indexInPath + 1] == '(')
			{
				indexInPath += 2;
				flag = InputControlPath.MatchPathComponent(control.displayName, path, ref indexInPath, InputControlPath.PathComponentType.DisplayName, 0);
			}
			if (indexInPath < length && flag && path[indexInPath] != '/')
			{
				flag = InputControlPath.MatchPathComponent(control.name, path, ref indexInPath, InputControlPath.PathComponentType.Name, 0);
				if (!flag)
				{
					int num = 0;
					while (num < control.aliases.Count && !flag)
					{
						flag = InputControlPath.MatchPathComponent(control.aliases[num], path, ref indexInPath, InputControlPath.PathComponentType.Name, 0);
						num++;
					}
				}
			}
			if (flag)
			{
				if (indexInPath < length && path[indexInPath] == '*')
				{
					indexInPath++;
				}
				if (indexInPath == length)
				{
					TControl tcontrol = control as TControl;
					if (tcontrol == null)
					{
						return default(TControl);
					}
					if (matchMultiple)
					{
						matches.Add(tcontrol);
					}
					return tcontrol;
				}
				else if (path[indexInPath] == '/')
				{
					indexInPath++;
					if (indexInPath != length)
					{
						TControl result;
						if (path[indexInPath] == '{')
						{
							result = InputControlPath.MatchByUsageAtDeviceRootRecursive<TControl>(control.device, path, indexInPath, ref matches, matchMultiple);
						}
						else
						{
							result = InputControlPath.MatchChildrenRecursive<TControl>(control, path, indexInPath, ref matches, matchMultiple);
						}
						return result;
					}
					TControl tcontrol2 = control as TControl;
					if (tcontrol2 == null)
					{
						return default(TControl);
					}
					if (matchMultiple)
					{
						matches.Add(tcontrol2);
					}
					return tcontrol2;
				}
			}
			return default(TControl);
		}

		private static TControl MatchByUsageAtDeviceRootRecursive<TControl>(InputDevice device, string path, int indexInPath, ref InputControlList<TControl> matches, bool matchMultiple) where TControl : InputControl
		{
			InternedString[] usagesForEachControl = device.m_UsagesForEachControl;
			if (usagesForEachControl == null)
			{
				return default(TControl);
			}
			int num = device.m_UsageToControl.LengthSafe<InputControl>();
			int num2 = indexInPath + 1;
			bool flag = InputControlPath.PathComponentCanYieldMultipleMatches(path, indexInPath);
			int length = path.Length;
			indexInPath++;
			if (indexInPath == length)
			{
				throw new ArgumentException("Invalid path spec '" + path + "'; trailing '{'", "path");
			}
			TControl tcontrol = default(TControl);
			for (int i = 0; i < num; i++)
			{
				if (!InputControlPath.MatchPathComponent(usagesForEachControl[i], path, ref indexInPath, InputControlPath.PathComponentType.Usage, 0))
				{
					indexInPath = num2;
				}
				else
				{
					InputControl inputControl = device.m_UsageToControl[i];
					if (indexInPath < length && path[indexInPath] == '/')
					{
						tcontrol = InputControlPath.MatchChildrenRecursive<TControl>(inputControl, path, indexInPath + 1, ref matches, matchMultiple);
						if (tcontrol != null && !flag)
						{
							break;
						}
						if (tcontrol != null && !matchMultiple)
						{
							break;
						}
					}
					else
					{
						tcontrol = (inputControl as TControl);
						if (tcontrol != null)
						{
							if (!matchMultiple)
							{
								break;
							}
							matches.Add(tcontrol);
						}
					}
				}
			}
			return tcontrol;
		}

		private static TControl MatchChildrenRecursive<TControl>(InputControl control, string path, int indexInPath, ref InputControlList<TControl> matches, bool matchMultiple) where TControl : InputControl
		{
			ReadOnlyArray<InputControl> children = control.children;
			int count = children.Count;
			TControl result = default(TControl);
			bool flag = InputControlPath.PathComponentCanYieldMultipleMatches(path, indexInPath);
			for (int i = 0; i < count; i++)
			{
				TControl tcontrol = InputControlPath.MatchControlsRecursive<TControl>(children[i], path, indexInPath, ref matches, matchMultiple);
				if (tcontrol != null)
				{
					if (!flag)
					{
						return tcontrol;
					}
					if (!matchMultiple)
					{
						return tcontrol;
					}
					result = tcontrol;
				}
			}
			return result;
		}

		private static bool MatchPathComponent(string component, string path, ref int indexInPath, InputControlPath.PathComponentType componentType, int startIndexInComponent = 0)
		{
			int length = component.Length;
			int length2 = path.Length;
			int num = indexInPath;
			int num2 = startIndexInComponent;
			while (indexInPath < length2)
			{
				char c = path[indexInPath];
				if (c == '\\' && indexInPath + 1 < length2)
				{
					indexInPath++;
					c = path[indexInPath];
				}
				else
				{
					if (c == '/' && componentType == InputControlPath.PathComponentType.Name)
					{
						break;
					}
					if ((c == '>' && componentType == InputControlPath.PathComponentType.Layout) || (c == '}' && componentType == InputControlPath.PathComponentType.Usage) || (c == ')' && componentType == InputControlPath.PathComponentType.DisplayName))
					{
						indexInPath++;
						break;
					}
					if (c == '*')
					{
						int num3 = indexInPath + 1;
						if (indexInPath < length2 - 1 && num2 < length && InputControlPath.MatchPathComponent(component, path, ref num3, componentType, num2))
						{
							indexInPath = num3;
							return true;
						}
						if (num2 < length)
						{
							num2++;
							continue;
						}
						return true;
					}
				}
				if (num2 == length)
				{
					indexInPath = num;
					return false;
				}
				char c2 = component[num2];
				if (c2 != c && char.ToLowerInvariant(c2) != char.ToLowerInvariant(c))
				{
					indexInPath = num;
					return false;
				}
				num2++;
				indexInPath++;
			}
			if (num2 == length)
			{
				return true;
			}
			indexInPath = num;
			return false;
		}

		private static bool PathComponentCanYieldMultipleMatches(string path, int indexInPath)
		{
			int num = path.IndexOf('/', indexInPath);
			if (num == -1)
			{
				return path.IndexOf('*', indexInPath) != -1 || path.IndexOf('<', indexInPath) != -1;
			}
			int count = num - indexInPath;
			return path.IndexOf('*', indexInPath, count) != -1 || path.IndexOf('<', indexInPath, count) != -1;
		}

		public static IEnumerable<InputControlPath.ParsedPathComponent> Parse(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			InputControlPath.PathParser parser = new InputControlPath.PathParser(path);
			while (parser.MoveToNextComponent())
			{
				yield return parser.current;
			}
			yield break;
		}

		public const string Wildcard = "*";

		public const string DoubleWildcard = "**";

		public const char Separator = '/';

		internal const char SeparatorReplacement = ' ';

		[Flags]
		public enum HumanReadableStringOptions
		{
			None = 0,
			OmitDevice = 2,
			UseShortNames = 4
		}

		private enum PathComponentType
		{
			Name,
			DisplayName,
			Usage,
			Layout
		}

		public struct ParsedPathComponent
		{
			public string layout
			{
				get
				{
					return this.m_Layout.ToString();
				}
			}

			public IEnumerable<string> usages
			{
				get
				{
					return from x in this.m_Usages
					select x.ToString();
				}
			}

			public string name
			{
				get
				{
					return this.m_Name.ToString();
				}
			}

			public string displayName
			{
				get
				{
					return this.m_DisplayName.ToString();
				}
			}

			internal bool isWildcard
			{
				get
				{
					return this.m_Name == "*";
				}
			}

			internal bool isDoubleWildcard
			{
				get
				{
					return this.m_Name == "**";
				}
			}

			internal string ToHumanReadableString(string parentLayoutName, string parentControlPath, out string referencedLayoutName, out string controlPath, InputControlPath.HumanReadableStringOptions options)
			{
				referencedLayoutName = null;
				controlPath = null;
				string text = string.Empty;
				if (this.isWildcard)
				{
					text += "Any";
				}
				if (this.m_Usages.length > 0)
				{
					string text2 = string.Empty;
					for (int i = 0; i < this.m_Usages.length; i++)
					{
						if (!this.m_Usages[i].isEmpty)
						{
							if (text2 != string.Empty)
							{
								text2 = text2 + " & " + InputControlPath.ParsedPathComponent.ToHumanReadableString(this.m_Usages[i]);
							}
							else
							{
								text2 = InputControlPath.ParsedPathComponent.ToHumanReadableString(this.m_Usages[i]);
							}
						}
					}
					if (text2 != string.Empty)
					{
						if (text != string.Empty)
						{
							text = text + " " + text2;
						}
						else
						{
							text += text2;
						}
					}
				}
				if (!this.m_Layout.isEmpty)
				{
					referencedLayoutName = this.m_Layout.ToString();
					InputControlLayout inputControlLayout = InputControlLayout.cache.FindOrLoadLayout(referencedLayoutName, false);
					string text3;
					if (inputControlLayout != null && !string.IsNullOrEmpty(inputControlLayout.m_DisplayName))
					{
						text3 = inputControlLayout.m_DisplayName;
					}
					else
					{
						text3 = InputControlPath.ParsedPathComponent.ToHumanReadableString(this.m_Layout);
					}
					if (!string.IsNullOrEmpty(text))
					{
						text = text + " " + text3;
					}
					else
					{
						text += text3;
					}
				}
				if (!this.m_Name.isEmpty && !this.isWildcard)
				{
					string text4 = null;
					if (!string.IsNullOrEmpty(parentLayoutName))
					{
						InputControlLayout inputControlLayout2 = InputControlLayout.cache.FindOrLoadLayout(new InternedString(parentLayoutName), false);
						if (inputControlLayout2 != null)
						{
							InternedString str = new InternedString(this.m_Name.ToString());
							int num;
							InputControlLayout.ControlItem? controlItem = inputControlLayout2.FindControlIncludingArrayElements(str, out num);
							if (controlItem != null)
							{
								if (string.IsNullOrEmpty(parentControlPath))
								{
									if (num != -1)
									{
										controlPath = string.Format("{0}{1}", controlItem.Value.name, num);
									}
									else
									{
										controlPath = controlItem.Value.name;
									}
								}
								else if (num != -1)
								{
									controlPath = string.Format("{0}/{1}{2}", parentControlPath, controlItem.Value.name, num);
								}
								else
								{
									controlPath = string.Format("{0}/{1}", parentControlPath, controlItem.Value.name);
								}
								string text5 = ((options & InputControlPath.HumanReadableStringOptions.UseShortNames) != InputControlPath.HumanReadableStringOptions.None) ? controlItem.Value.shortDisplayName : null;
								string text6 = (!string.IsNullOrEmpty(text5)) ? text5 : controlItem.Value.displayName;
								if (!string.IsNullOrEmpty(text6))
								{
									if (num != -1)
									{
										text4 = string.Format("{0} #{1}", text6, num);
									}
									else
									{
										text4 = text6;
									}
								}
								if (string.IsNullOrEmpty(referencedLayoutName))
								{
									referencedLayoutName = controlItem.Value.layout;
								}
							}
						}
					}
					if (text4 == null)
					{
						text4 = InputControlPath.ParsedPathComponent.ToHumanReadableString(this.m_Name);
					}
					if (!string.IsNullOrEmpty(text))
					{
						text = text + " " + text4;
					}
					else
					{
						text += text4;
					}
				}
				if (!this.m_DisplayName.isEmpty)
				{
					string text7 = "\"" + InputControlPath.ParsedPathComponent.ToHumanReadableString(this.m_DisplayName) + "\"";
					if (!string.IsNullOrEmpty(text))
					{
						text = text + " " + text7;
					}
					else
					{
						text += text7;
					}
				}
				return text;
			}

			private static string ToHumanReadableString(Substring substring)
			{
				return substring.ToString().Unescape("/*{<", "/*{<");
			}

			public bool Matches(InputControl control)
			{
				if (!this.m_Layout.isEmpty)
				{
					bool flag = InputControlPath.ParsedPathComponent.ComparePathElementToString(this.m_Layout, control.layout);
					if (!flag)
					{
						InternedString layout = control.m_Layout;
						while (InputControlLayout.s_Layouts.baseLayoutTable.TryGetValue(layout, out layout) && !flag)
						{
							flag = InputControlPath.ParsedPathComponent.ComparePathElementToString(this.m_Layout, layout.ToString());
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				if (this.m_Usages.length > 0)
				{
					for (int i = 0; i < this.m_Usages.length; i++)
					{
						if (!this.m_Usages[i].isEmpty)
						{
							ReadOnlyArray<InternedString> usages = control.usages;
							bool flag2 = false;
							for (int j = 0; j < usages.Count; j++)
							{
								if (InputControlPath.ParsedPathComponent.ComparePathElementToString(this.m_Usages[i], usages[j]))
								{
									flag2 = true;
									break;
								}
							}
							if (!flag2)
							{
								return false;
							}
						}
					}
				}
				return (this.m_Name.isEmpty || this.isWildcard || InputControlPath.ParsedPathComponent.ComparePathElementToString(this.m_Name, control.name)) && (this.m_DisplayName.isEmpty || InputControlPath.ParsedPathComponent.ComparePathElementToString(this.m_DisplayName, control.displayName));
			}

			private static bool ComparePathElementToString(Substring pathElement, string element)
			{
				int length = pathElement.length;
				int length2 = element.Length;
				int num = 0;
				int num2 = 0;
				bool flag;
				bool flag2;
				for (;;)
				{
					flag = (num == length);
					flag2 = (num2 == length2);
					if (flag || flag2)
					{
						break;
					}
					char c = pathElement[num];
					if (c == '\\' && num + 1 < length)
					{
						c = pathElement[++num];
					}
					if (char.ToLowerInvariant(c) != char.ToLowerInvariant(element[num2]))
					{
						return false;
					}
					num++;
					num2++;
				}
				return flag == flag2;
			}

			internal Substring m_Layout;

			internal InlinedArray<Substring> m_Usages;

			internal Substring m_Name;

			internal Substring m_DisplayName;
		}

		private struct PathParser
		{
			public bool isAtEnd
			{
				get
				{
					return this.rightIndexInPath == this.length;
				}
			}

			public PathParser(string path)
			{
				this.path = path;
				this.length = path.Length;
				this.leftIndexInPath = 0;
				this.rightIndexInPath = 0;
				this.current = default(InputControlPath.ParsedPathComponent);
			}

			public bool MoveToNextComponent()
			{
				if (this.rightIndexInPath == this.length)
				{
					return false;
				}
				this.leftIndexInPath = this.rightIndexInPath;
				if (this.path[this.leftIndexInPath] == '/')
				{
					this.leftIndexInPath++;
					this.rightIndexInPath = this.leftIndexInPath;
					if (this.leftIndexInPath == this.length)
					{
						return false;
					}
				}
				Substring layout = default(Substring);
				if (this.rightIndexInPath < this.length && this.path[this.rightIndexInPath] == '<')
				{
					layout = this.ParseComponentPart('>');
				}
				InlinedArray<Substring> usages = default(InlinedArray<Substring>);
				while (this.rightIndexInPath < this.length && this.path[this.rightIndexInPath] == '{')
				{
					usages.AppendWithCapacity(this.ParseComponentPart('}'), 10);
				}
				Substring displayName = default(Substring);
				if (this.rightIndexInPath < this.length - 1 && this.path[this.rightIndexInPath] == '#' && this.path[this.rightIndexInPath + 1] == '(')
				{
					this.rightIndexInPath++;
					displayName = this.ParseComponentPart(')');
				}
				Substring name = default(Substring);
				if (this.rightIndexInPath < this.length && this.path[this.rightIndexInPath] != '/')
				{
					name = this.ParseComponentPart('/');
				}
				this.current = new InputControlPath.ParsedPathComponent
				{
					m_Layout = layout,
					m_Usages = usages,
					m_Name = name,
					m_DisplayName = displayName
				};
				return this.leftIndexInPath != this.rightIndexInPath;
			}

			private Substring ParseComponentPart(char terminator)
			{
				if (terminator != '/')
				{
					this.rightIndexInPath++;
				}
				int num = this.rightIndexInPath;
				while (this.rightIndexInPath < this.length && this.path[this.rightIndexInPath] != terminator)
				{
					if (this.path[this.rightIndexInPath] == '\\' && this.rightIndexInPath + 1 < this.length)
					{
						this.rightIndexInPath++;
					}
					this.rightIndexInPath++;
				}
				int num2 = this.rightIndexInPath - num;
				if (this.rightIndexInPath < this.length && terminator != '/')
				{
					this.rightIndexInPath++;
				}
				return new Substring(this.path, num, num2);
			}

			private string path;

			private int length;

			private int leftIndexInPath;

			private int rightIndexInPath;

			public InputControlPath.ParsedPathComponent current;
		}
	}
}
