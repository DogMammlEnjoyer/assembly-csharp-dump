using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace UnityEngine.Rendering
{
	public class DebugUI
	{
		public class Container : DebugUI.Widget, DebugUI.IContainer
		{
			internal bool hideDisplayName
			{
				get
				{
					return string.IsNullOrEmpty(base.displayName) || base.displayName.StartsWith("#");
				}
			}

			public ObservableList<DebugUI.Widget> children { get; private set; }

			public override DebugUI.Panel panel
			{
				get
				{
					return this.m_Panel;
				}
				internal set
				{
					if (value != null && value.flags.HasFlag(DebugUI.Flags.FrequentlyUsed))
					{
						return;
					}
					this.m_Panel = value;
					int count = this.children.Count;
					for (int i = 0; i < count; i++)
					{
						this.children[i].panel = value;
					}
				}
			}

			public Container() : this(string.Empty, new ObservableList<DebugUI.Widget>())
			{
			}

			public Container(string id) : this("#" + id, new ObservableList<DebugUI.Widget>())
			{
			}

			public Container(string displayName, ObservableList<DebugUI.Widget> children)
			{
				base.displayName = displayName;
				this.children = children;
				children.ItemAdded += this.OnItemAdded;
				children.ItemRemoved += this.OnItemRemoved;
				for (int i = 0; i < this.children.Count; i++)
				{
					this.OnItemAdded(this.children, new ListChangedEventArgs<DebugUI.Widget>(i, this.children[i]));
				}
			}

			internal override void GenerateQueryPath()
			{
				base.GenerateQueryPath();
				int count = this.children.Count;
				for (int i = 0; i < count; i++)
				{
					this.children[i].GenerateQueryPath();
				}
			}

			protected virtual void OnItemAdded(ObservableList<DebugUI.Widget> sender, ListChangedEventArgs<DebugUI.Widget> e)
			{
				if (e.item != null)
				{
					e.item.panel = this.m_Panel;
					e.item.parent = this;
				}
				if (this.m_Panel != null)
				{
					this.m_Panel.SetDirty();
				}
			}

			protected virtual void OnItemRemoved(ObservableList<DebugUI.Widget> sender, ListChangedEventArgs<DebugUI.Widget> e)
			{
				if (e.item != null)
				{
					e.item.panel = null;
					e.item.parent = null;
				}
				if (this.m_Panel != null)
				{
					this.m_Panel.SetDirty();
				}
			}

			public override int GetHashCode()
			{
				int num = 17;
				num = num * 23 + base.queryPath.GetHashCode();
				num = num * 23 + base.isHidden.GetHashCode();
				int count = this.children.Count;
				for (int i = 0; i < count; i++)
				{
					num = num * 23 + this.children[i].GetHashCode();
				}
				return num;
			}

			private const string k_IDToken = "#";
		}

		public class Foldout : DebugUI.Container, DebugUI.IValueField
		{
			public bool isReadOnly
			{
				get
				{
					return false;
				}
			}

			public bool opened { get; set; }

			public string documentationUrl { get; set; }

			public string[] columnLabels
			{
				get
				{
					return this.m_ColumnLabels;
				}
				set
				{
					this.m_ColumnLabels = value;
					this.m_Dirty = true;
				}
			}

			public string[] columnTooltips
			{
				get
				{
					return this.m_ColumnTooltips;
				}
				set
				{
					this.m_ColumnTooltips = value;
					this.m_Dirty = true;
				}
			}

			internal List<GUIContent> rowContents
			{
				get
				{
					if (this.m_Dirty)
					{
						if (this.m_ColumnTooltips == null)
						{
							this.m_ColumnTooltips = new string[this.m_ColumnLabels.Length];
							Array.Fill<string>(this.columnTooltips, string.Empty);
						}
						else if (this.m_ColumnTooltips.Length != this.m_ColumnLabels.Length)
						{
							throw new Exception("Dimension for labels and tooltips on Foldout - " + base.displayName + ", do not match");
						}
						this.m_RowContents.Clear();
						for (int i = 0; i < this.m_ColumnLabels.Length; i++)
						{
							string text = this.columnLabels[i] ?? string.Empty;
							string tooltip = this.m_ColumnTooltips[i] ?? string.Empty;
							this.m_RowContents.Add(new GUIContent(text, tooltip));
						}
						this.m_Dirty = false;
					}
					return this.m_RowContents;
				}
			}

			public Foldout()
			{
			}

			public Foldout(string displayName, ObservableList<DebugUI.Widget> children, string[] columnLabels = null, string[] columnTooltips = null) : base(displayName, children)
			{
				this.columnLabels = columnLabels;
				this.columnTooltips = columnTooltips;
			}

			public bool GetValue()
			{
				return this.opened;
			}

			object DebugUI.IValueField.GetValue()
			{
				return this.GetValue();
			}

			public void SetValue(object value)
			{
				this.SetValue((bool)value);
			}

			public object ValidateValue(object value)
			{
				return value;
			}

			public void SetValue(bool value)
			{
				this.opened = value;
			}

			public bool isHeader;

			public List<DebugUI.Foldout.ContextMenuItem> contextMenuItems;

			private bool m_Dirty;

			private string[] m_ColumnLabels;

			private string[] m_ColumnTooltips;

			private List<GUIContent> m_RowContents = new List<GUIContent>();

			public struct ContextMenuItem
			{
				public string displayName;

				public Action action;
			}
		}

		public class HBox : DebugUI.Container
		{
			public HBox()
			{
				base.displayName = "HBox";
			}
		}

		public class VBox : DebugUI.Container
		{
			public VBox()
			{
				base.displayName = "VBox";
			}
		}

		public class Table : DebugUI.Container
		{
			public Table()
			{
				base.displayName = "Array";
			}

			public void SetColumnVisibility(int index, bool visible)
			{
				bool[] visibleColumns = this.VisibleColumns;
				if (index < 0 || index > visibleColumns.Length)
				{
					return;
				}
				visibleColumns[index] = visible;
			}

			public bool GetColumnVisibility(int index)
			{
				bool[] visibleColumns = this.VisibleColumns;
				return index >= 0 && index <= visibleColumns.Length && visibleColumns[index];
			}

			public bool[] VisibleColumns
			{
				get
				{
					if (this.m_Header != null)
					{
						return this.m_Header;
					}
					int num = 0;
					if (base.children.Count != 0)
					{
						num = ((DebugUI.Container)base.children[0]).children.Count;
						for (int i = 1; i < base.children.Count; i++)
						{
							if (((DebugUI.Container)base.children[i]).children.Count != num)
							{
								Debug.LogError("All rows must have the same number of children.");
								return null;
							}
						}
					}
					this.m_Header = new bool[num];
					for (int j = 0; j < num; j++)
					{
						this.m_Header[j] = true;
					}
					return this.m_Header;
				}
			}

			protected override void OnItemAdded(ObservableList<DebugUI.Widget> sender, ListChangedEventArgs<DebugUI.Widget> e)
			{
				base.OnItemAdded(sender, e);
				this.m_Header = null;
			}

			protected override void OnItemRemoved(ObservableList<DebugUI.Widget> sender, ListChangedEventArgs<DebugUI.Widget> e)
			{
				base.OnItemRemoved(sender, e);
				this.m_Header = null;
			}

			private static GUIStyle columnHeaderStyle = new GUIStyle
			{
				alignment = TextAnchor.MiddleCenter
			};

			public bool isReadOnly;

			private bool[] m_Header;

			public class Row : DebugUI.Foldout
			{
				public Row()
				{
					base.displayName = "Row";
				}
			}
		}

		[Flags]
		public enum Flags
		{
			None = 0,
			EditorOnly = 2,
			RuntimeOnly = 4,
			EditorForceUpdate = 8,
			FrequentlyUsed = 16
		}

		public abstract class Widget
		{
			public int order { get; set; }

			public virtual DebugUI.Panel panel
			{
				get
				{
					return this.m_Panel;
				}
				internal set
				{
					this.m_Panel = value;
				}
			}

			public virtual DebugUI.IContainer parent
			{
				get
				{
					return this.m_Parent;
				}
				internal set
				{
					this.m_Parent = value;
				}
			}

			public DebugUI.Flags flags { get; set; }

			public string displayName { get; set; }

			public string tooltip { get; set; }

			public string queryPath { get; private set; }

			public bool isEditorOnly
			{
				get
				{
					return this.flags.HasFlag(DebugUI.Flags.EditorOnly);
				}
			}

			public bool isRuntimeOnly
			{
				get
				{
					return this.flags.HasFlag(DebugUI.Flags.RuntimeOnly);
				}
			}

			public bool isInactiveInEditor
			{
				get
				{
					return this.isRuntimeOnly && !Application.isPlaying;
				}
			}

			public bool isHidden
			{
				get
				{
					Func<bool> func = this.isHiddenCallback;
					return func != null && func();
				}
			}

			internal virtual void GenerateQueryPath()
			{
				this.queryPath = this.displayName.Trim();
				if (this.m_Parent != null)
				{
					this.queryPath = this.m_Parent.queryPath + " -> " + this.queryPath;
				}
			}

			public override int GetHashCode()
			{
				return this.queryPath.GetHashCode() ^ this.isHidden.GetHashCode();
			}

			public DebugUI.Widget.NameAndTooltip nameAndTooltip
			{
				set
				{
					this.displayName = value.name;
					this.tooltip = value.tooltip;
				}
			}

			protected DebugUI.Panel m_Panel;

			protected DebugUI.IContainer m_Parent;

			public Func<bool> isHiddenCallback;

			public struct NameAndTooltip
			{
				public string name;

				public string tooltip;
			}
		}

		public interface IContainer
		{
			ObservableList<DebugUI.Widget> children { get; }

			string displayName { get; set; }

			string queryPath { get; }
		}

		public interface IValueField
		{
			object GetValue();

			void SetValue(object value);

			object ValidateValue(object value);
		}

		public class Button : DebugUI.Widget
		{
			public Action action { get; set; }
		}

		public class Value : DebugUI.Widget
		{
			public Func<object> getter { get; set; }

			public Value()
			{
				base.displayName = "";
			}

			public virtual object GetValue()
			{
				return this.getter();
			}

			public virtual string FormatString(object value)
			{
				if (!string.IsNullOrEmpty(this.formatString))
				{
					return string.Format(this.formatString, value);
				}
				return string.Format("{0}", value);
			}

			public float refreshRate = 0.1f;

			public string formatString;
		}

		public class ProgressBarValue : DebugUI.Value
		{
			public override string FormatString(object value)
			{
				float num = DebugUI.ProgressBarValue.<FormatString>g__Remap01|2_0(Mathf.Clamp((float)value, this.min, this.max), this.min, this.max);
				return string.Format("{0:P1}", num);
			}

			[CompilerGenerated]
			internal static float <FormatString>g__Remap01|2_0(float v, float x0, float y0)
			{
				return (v - x0) / (y0 - x0);
			}

			public float min;

			public float max = 1f;
		}

		public class ValueTuple : DebugUI.Widget
		{
			public int numElements
			{
				get
				{
					return this.values.Length;
				}
			}

			public float refreshRate
			{
				get
				{
					DebugUI.Value value = this.values.FirstOrDefault<DebugUI.Value>();
					if (value == null)
					{
						return 0.1f;
					}
					return value.refreshRate;
				}
			}

			public DebugUI.Value[] values;

			public int pinnedElementIndex = -1;
		}

		public abstract class Field<T> : DebugUI.Widget, DebugUI.IValueField
		{
			public Func<T> getter { get; set; }

			public Action<T> setter { get; set; }

			object DebugUI.IValueField.ValidateValue(object value)
			{
				return this.ValidateValue((T)((object)value));
			}

			public virtual T ValidateValue(T value)
			{
				return value;
			}

			object DebugUI.IValueField.GetValue()
			{
				return this.GetValue();
			}

			public T GetValue()
			{
				return this.getter();
			}

			public void SetValue(object value)
			{
				this.SetValue((T)((object)value));
			}

			public virtual void SetValue(T value)
			{
				if (this.setter == null)
				{
					return;
				}
				T t = this.ValidateValue(value);
				if (t == null || !t.Equals(this.getter()))
				{
					this.setter(t);
					Action<DebugUI.Field<T>, T> action = this.onValueChanged;
					if (action == null)
					{
						return;
					}
					action(this, t);
				}
			}

			public Action<DebugUI.Field<T>, T> onValueChanged;
		}

		public class BoolField : DebugUI.Field<bool>
		{
		}

		public class HistoryBoolField : DebugUI.BoolField
		{
			public Func<bool>[] historyGetter { get; set; }

			public int historyDepth
			{
				get
				{
					Func<bool>[] historyGetter = this.historyGetter;
					if (historyGetter == null)
					{
						return 0;
					}
					return historyGetter.Length;
				}
			}

			public bool GetHistoryValue(int historyIndex)
			{
				return this.historyGetter[historyIndex]();
			}
		}

		public class IntField : DebugUI.Field<int>
		{
			public override int ValidateValue(int value)
			{
				if (this.min != null)
				{
					value = Mathf.Max(value, this.min());
				}
				if (this.max != null)
				{
					value = Mathf.Min(value, this.max());
				}
				return value;
			}

			public Func<int> min;

			public Func<int> max;

			public int incStep = 1;

			public int intStepMult = 10;
		}

		public class UIntField : DebugUI.Field<uint>
		{
			public override uint ValidateValue(uint value)
			{
				if (this.min != null)
				{
					value = (uint)Mathf.Max((int)value, (int)this.min());
				}
				if (this.max != null)
				{
					value = (uint)Mathf.Min((int)value, (int)this.max());
				}
				return value;
			}

			public Func<uint> min;

			public Func<uint> max;

			public uint incStep = 1U;

			public uint intStepMult = 10U;
		}

		public class FloatField : DebugUI.Field<float>
		{
			public override float ValidateValue(float value)
			{
				if (this.min != null)
				{
					value = Mathf.Max(value, this.min());
				}
				if (this.max != null)
				{
					value = Mathf.Min(value, this.max());
				}
				return value;
			}

			public Func<float> min;

			public Func<float> max;

			public float incStep = 0.1f;

			public float incStepMult = 10f;

			public int decimals = 3;
		}

		public class RenderingLayerField : DebugUI.Field<RenderingLayerMask>, DebugUI.IContainer
		{
			private int maxRenderingLayerCount
			{
				get
				{
					return RenderingLayerMask.GetRenderingLayerCount();
				}
			}

			private void Resize()
			{
				this.m_DefinedRenderingLayersCount = RenderingLayerMask.GetDefinedRenderingLayerCount();
				this.m_RenderingLayersNames = new string[this.maxRenderingLayerCount];
				for (int i = 0; i < this.maxRenderingLayerCount; i++)
				{
					string text = RenderingLayerMask.RenderingLayerToName(i);
					if (string.IsNullOrEmpty(text))
					{
						text = string.Format("Unused Rendering Layer {0}", i);
					}
					this.m_RenderingLayersNames[i] = text;
				}
				this.m_RenderingLayersColors.Clear();
				DebugUI.Foldout foldout = new DebugUI.Foldout
				{
					nameAndTooltip = DebugUI.RenderingLayerField.s_RenderingLayerColors,
					flags = DebugUI.Flags.EditorOnly,
					parent = this
				};
				this.m_RenderingLayersColors.Add(foldout);
				for (int j = 0; j < this.m_RenderingLayersNames.Length; j++)
				{
					int index = j;
					foldout.children.Add(new DebugUI.ColorField
					{
						displayName = this.m_RenderingLayersNames[index],
						getter = (() => this.getRenderingLayerColor(index)),
						setter = delegate(Color value)
						{
							this.setRenderingLayerColor(value, index);
						}
					});
				}
				this.GenerateQueryPath();
			}

			public string[] renderingLayersNames
			{
				get
				{
					if (this.m_DefinedRenderingLayersCount != RenderingLayerMask.GetDefinedRenderingLayerCount())
					{
						this.Resize();
					}
					return this.m_RenderingLayersNames;
				}
			}

			public ObservableList<DebugUI.Widget> children
			{
				get
				{
					if (this.m_DefinedRenderingLayersCount != RenderingLayerMask.GetDefinedRenderingLayerCount())
					{
						this.Resize();
					}
					return this.m_RenderingLayersColors;
				}
			}

			public Func<int, Vector4> getRenderingLayerColor { get; set; }

			public Action<Vector4, int> setRenderingLayerColor { get; set; }

			internal override void GenerateQueryPath()
			{
				base.GenerateQueryPath();
				int count = this.children.Count;
				for (int i = 0; i < count; i++)
				{
					this.children[i].GenerateQueryPath();
				}
			}

			private static readonly DebugUI.Widget.NameAndTooltip s_RenderingLayerColors = new DebugUI.Widget.NameAndTooltip
			{
				name = "Layers Color",
				tooltip = "Select the display color for each Rendering Layer"
			};

			private string[] m_RenderingLayersNames = Array.Empty<string>();

			private int m_DefinedRenderingLayersCount = -1;

			private ObservableList<DebugUI.Widget> m_RenderingLayersColors = new ObservableList<DebugUI.Widget>();
		}

		public abstract class EnumField<T> : DebugUI.Field<T>
		{
			public int[] enumValues
			{
				get
				{
					return this.m_EnumValues;
				}
				set
				{
					int? num = (value != null) ? new int?(value.Distinct<int>().Count<int>()) : null;
					int? num2 = (value != null) ? new int?(value.Count<int>()) : null;
					if (!(num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null)))
					{
						Debug.LogWarning(base.displayName + " - The values of the enum are duplicated, this might lead to a errors displaying the enum");
					}
					this.m_EnumValues = value;
				}
			}

			protected void AutoFillFromType(Type enumType)
			{
				if (enumType == null || !enumType.IsEnum)
				{
					throw new ArgumentException("enumType must not be null and it must be an Enum type");
				}
				List<GUIContent> list;
				using (ListPool<GUIContent>.Get(out list))
				{
					List<int> list2;
					using (ListPool<int>.Get(out list2))
					{
						foreach (FieldInfo fieldInfo2 in from fieldInfo in enumType.GetFields(BindingFlags.Static | BindingFlags.Public)
						where !fieldInfo.IsDefined(typeof(ObsoleteAttribute)) && !fieldInfo.IsDefined(typeof(HideInInspector))
						select fieldInfo)
						{
							InspectorNameAttribute customAttribute = fieldInfo2.GetCustomAttribute<InspectorNameAttribute>();
							GUIContent item = new GUIContent((customAttribute == null) ? DebugUI.EnumField<T>.s_NicifyRegEx.Replace(fieldInfo2.Name, "$1 ") : customAttribute.displayName);
							list.Add(item);
							list2.Add((int)Enum.Parse(enumType, fieldInfo2.Name));
						}
						this.enumNames = list.ToArray();
						this.enumValues = list2.ToArray();
					}
				}
			}

			public GUIContent[] enumNames;

			private int[] m_EnumValues;

			private static Regex s_NicifyRegEx = new Regex("([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", RegexOptions.Compiled);
		}

		public class EnumField : DebugUI.EnumField<int>
		{
			internal int[] indexes
			{
				get
				{
					int[] result;
					if ((result = this.m_Indexes) == null)
					{
						int start = 0;
						GUIContent[] enumNames = this.enumNames;
						result = (this.m_Indexes = Enumerable.Range(start, (enumNames != null) ? enumNames.Length : 0).ToArray<int>());
					}
					return result;
				}
			}

			public Func<int> getIndex { get; set; }

			public Action<int> setIndex { get; set; }

			public int currentIndex
			{
				get
				{
					return this.getIndex();
				}
				set
				{
					this.setIndex(value);
				}
			}

			public Type autoEnum
			{
				set
				{
					base.AutoFillFromType(value);
					this.InitQuickSeparators();
				}
			}

			internal void InitQuickSeparators()
			{
				IEnumerable<string> source = this.enumNames.Select(delegate(GUIContent x)
				{
					string[] array = x.text.Split('/', StringSplitOptions.None);
					if (array.Length == 1)
					{
						return "";
					}
					return array[0];
				});
				this.quickSeparators = new int[source.Distinct<string>().Count<string>()];
				string a = null;
				int i = 0;
				int num = 0;
				while (i < this.quickSeparators.Length)
				{
					string text = source.ElementAt(num);
					while (a == text)
					{
						text = source.ElementAt(++num);
					}
					a = text;
					this.quickSeparators[i] = num++;
					i++;
				}
			}

			public override void SetValue(int value)
			{
				int num = this.ValidateValue(value);
				int num2 = Array.IndexOf<int>(base.enumValues, num);
				if (this.currentIndex != num2 && !num.Equals(base.getter()))
				{
					base.setter(num);
					Action<DebugUI.Field<int>, int> onValueChanged = this.onValueChanged;
					if (onValueChanged != null)
					{
						onValueChanged(this, num);
					}
					if (num2 > -1)
					{
						this.currentIndex = num2;
					}
				}
			}

			internal int[] quickSeparators;

			private int[] m_Indexes;
		}

		public class ObjectPopupField : DebugUI.Field<Object>
		{
			public Func<IEnumerable<Object>> getObjects { get; set; }
		}

		public class CameraSelector : DebugUI.ObjectPopupField
		{
			public CameraSelector()
			{
				base.displayName = "Camera";
				base.getObjects = (() => this.cameras);
			}

			private IEnumerable<Camera> cameras
			{
				get
				{
					this.m_Cameras.Clear();
					if (this.m_CamerasArray == null || this.m_CamerasArray.Length != Camera.allCamerasCount)
					{
						this.m_CamerasArray = new Camera[Camera.allCamerasCount];
					}
					Camera.GetAllCameras(this.m_CamerasArray);
					foreach (Camera camera in this.m_CamerasArray)
					{
						if (!(camera == null) && camera.cameraType != CameraType.Preview && camera.cameraType != CameraType.Reflection)
						{
							IAdditionalData additionalData;
							if (!camera.TryGetComponent<IAdditionalData>(out additionalData))
							{
								Debug.LogWarning("Camera " + camera.name + " does not contain an additional camera data component. Open the Game Object in the inspector to add additional camera data.");
							}
							else
							{
								this.m_Cameras.Add(camera);
							}
						}
					}
					return this.m_Cameras;
				}
			}

			private Camera[] m_CamerasArray;

			private List<Camera> m_Cameras = new List<Camera>();
		}

		public class HistoryEnumField : DebugUI.EnumField
		{
			public Func<int>[] historyIndexGetter { get; set; }

			public int historyDepth
			{
				get
				{
					Func<int>[] historyIndexGetter = this.historyIndexGetter;
					if (historyIndexGetter == null)
					{
						return 0;
					}
					return historyIndexGetter.Length;
				}
			}

			public int GetHistoryValue(int historyIndex)
			{
				return this.historyIndexGetter[historyIndex]();
			}
		}

		public class BitField : DebugUI.EnumField<Enum>
		{
			public Type enumType
			{
				get
				{
					return this.m_EnumType;
				}
				set
				{
					this.m_EnumType = value;
					base.AutoFillFromType(value);
				}
			}

			private Type m_EnumType;
		}

		public class ColorField : DebugUI.Field<Color>
		{
			public override Color ValidateValue(Color value)
			{
				if (!this.hdr)
				{
					value.r = Mathf.Clamp01(value.r);
					value.g = Mathf.Clamp01(value.g);
					value.b = Mathf.Clamp01(value.b);
					value.a = Mathf.Clamp01(value.a);
				}
				return value;
			}

			public bool hdr;

			public bool showAlpha = true;

			public bool showPicker = true;

			public float incStep = 0.025f;

			public float incStepMult = 5f;

			public int decimals = 3;
		}

		public class Vector2Field : DebugUI.Field<Vector2>
		{
			public float incStep = 0.025f;

			public float incStepMult = 10f;

			public int decimals = 3;
		}

		public class Vector3Field : DebugUI.Field<Vector3>
		{
			public float incStep = 0.025f;

			public float incStepMult = 10f;

			public int decimals = 3;
		}

		public class Vector4Field : DebugUI.Field<Vector4>
		{
			public float incStep = 0.025f;

			public float incStepMult = 10f;

			public int decimals = 3;
		}

		public class ObjectField : DebugUI.Field<Object>
		{
			public Type type = typeof(Object);
		}

		public class ObjectListField : DebugUI.Field<Object[]>
		{
			public Type type = typeof(Object);
		}

		public class MessageBox : DebugUI.Widget
		{
			public string message
			{
				get
				{
					if (this.messageCallback != null)
					{
						return this.messageCallback();
					}
					return base.displayName;
				}
			}

			public DebugUI.MessageBox.Style style;

			public Func<string> messageCallback;

			public enum Style
			{
				Info,
				Warning,
				Error
			}
		}

		public class RuntimeDebugShadersMessageBox : DebugUI.MessageBox
		{
			public RuntimeDebugShadersMessageBox()
			{
				base.displayName = "Warning: the debug shader variants are missing. Ensure that the \"Strip Runtime Debug Shaders\" option is disabled in the SRP Graphics Settings.";
				this.style = DebugUI.MessageBox.Style.Warning;
				this.isHiddenCallback = delegate()
				{
					ShaderStrippingSetting shaderStrippingSetting;
					return !GraphicsSettings.TryGetRenderPipelineSettings<ShaderStrippingSetting>(out shaderStrippingSetting) || !shaderStrippingSetting.stripRuntimeDebugShaders;
				};
			}
		}

		public class Panel : DebugUI.IContainer, IComparable<DebugUI.Panel>
		{
			public DebugUI.Flags flags { get; set; }

			public string displayName { get; set; }

			public int groupIndex { get; set; }

			public string queryPath
			{
				get
				{
					return this.displayName;
				}
			}

			public bool isEditorOnly
			{
				get
				{
					return (this.flags & DebugUI.Flags.EditorOnly) > DebugUI.Flags.None;
				}
			}

			public bool isRuntimeOnly
			{
				get
				{
					return (this.flags & DebugUI.Flags.RuntimeOnly) > DebugUI.Flags.None;
				}
			}

			public bool isInactiveInEditor
			{
				get
				{
					return this.isRuntimeOnly && !Application.isPlaying;
				}
			}

			public bool editorForceUpdate
			{
				get
				{
					return (this.flags & DebugUI.Flags.EditorForceUpdate) > DebugUI.Flags.None;
				}
			}

			public ObservableList<DebugUI.Widget> children { get; private set; }

			public event Action<DebugUI.Panel> onSetDirty = delegate(DebugUI.Panel <p0>)
			{
			};

			public Panel()
			{
				this.children = new ObservableList<DebugUI.Widget>(0, (DebugUI.Widget widget, DebugUI.Widget widget1) => widget.order.CompareTo(widget1.order));
				this.children.ItemAdded += this.OnItemAdded;
				this.children.ItemRemoved += this.OnItemRemoved;
			}

			protected virtual void OnItemAdded(ObservableList<DebugUI.Widget> sender, ListChangedEventArgs<DebugUI.Widget> e)
			{
				if (e.item != null)
				{
					e.item.panel = this;
					e.item.parent = this;
				}
				this.SetDirty();
			}

			protected virtual void OnItemRemoved(ObservableList<DebugUI.Widget> sender, ListChangedEventArgs<DebugUI.Widget> e)
			{
				if (e.item != null)
				{
					e.item.panel = null;
					e.item.parent = null;
				}
				this.SetDirty();
			}

			public void SetDirty()
			{
				int count = this.children.Count;
				for (int i = 0; i < count; i++)
				{
					this.children[i].GenerateQueryPath();
				}
				this.onSetDirty(this);
			}

			public override int GetHashCode()
			{
				int num = 17;
				num = num * 23 + this.displayName.GetHashCode();
				int count = this.children.Count;
				for (int i = 0; i < count; i++)
				{
					num = num * 23 + this.children[i].GetHashCode();
				}
				return num;
			}

			int IComparable<DebugUI.Panel>.CompareTo(DebugUI.Panel other)
			{
				if (other != null)
				{
					return this.groupIndex.CompareTo(other.groupIndex);
				}
				return 1;
			}
		}

		[Obsolete("Mask field is not longer supported. Please use a BitField or implement your own Widget. #from(6000.2)", false)]
		public class MaskField : DebugUI.EnumField<uint>
		{
			public void Fill(string[] names)
			{
				List<GUIContent> list;
				using (ListPool<GUIContent>.Get(out list))
				{
					List<int> list2;
					using (ListPool<int>.Get(out list2))
					{
						for (int i = 0; i < names.Length; i++)
						{
							list.Add(new GUIContent(names[i]));
							list2.Add(i);
						}
						this.enumNames = list.ToArray();
						base.enumValues = list2.ToArray();
					}
				}
			}

			public override void SetValue(uint value)
			{
				uint num = this.ValidateValue(value);
				if (!num.Equals(base.getter()))
				{
					base.setter(num);
					Action<DebugUI.Field<uint>, uint> onValueChanged = this.onValueChanged;
					if (onValueChanged == null)
					{
						return;
					}
					onValueChanged(this, num);
				}
			}
		}
	}
}
