using System;

namespace System.ComponentModel
{
	/// <summary>Specifies the name of the category in which to group the property or event when displayed in a <see cref="T:System.Windows.Forms.PropertyGrid" /> control set to Categorized mode.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public class CategoryAttribute : Attribute
	{
		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Action category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the action category.</returns>
		public static CategoryAttribute Action
		{
			get
			{
				if (CategoryAttribute.action == null)
				{
					CategoryAttribute.action = new CategoryAttribute("Action");
				}
				return CategoryAttribute.action;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Appearance category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the appearance category.</returns>
		public static CategoryAttribute Appearance
		{
			get
			{
				if (CategoryAttribute.appearance == null)
				{
					CategoryAttribute.appearance = new CategoryAttribute("Appearance");
				}
				return CategoryAttribute.appearance;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Asynchronous category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the asynchronous category.</returns>
		public static CategoryAttribute Asynchronous
		{
			get
			{
				if (CategoryAttribute.asynchronous == null)
				{
					CategoryAttribute.asynchronous = new CategoryAttribute("Asynchronous");
				}
				return CategoryAttribute.asynchronous;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Behavior category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the behavior category.</returns>
		public static CategoryAttribute Behavior
		{
			get
			{
				if (CategoryAttribute.behavior == null)
				{
					CategoryAttribute.behavior = new CategoryAttribute("Behavior");
				}
				return CategoryAttribute.behavior;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Data category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the data category.</returns>
		public static CategoryAttribute Data
		{
			get
			{
				if (CategoryAttribute.data == null)
				{
					CategoryAttribute.data = new CategoryAttribute("Data");
				}
				return CategoryAttribute.data;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Default category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the default category.</returns>
		public static CategoryAttribute Default
		{
			get
			{
				if (CategoryAttribute.defAttr == null)
				{
					CategoryAttribute.defAttr = new CategoryAttribute();
				}
				return CategoryAttribute.defAttr;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Design category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the design category.</returns>
		public static CategoryAttribute Design
		{
			get
			{
				if (CategoryAttribute.design == null)
				{
					CategoryAttribute.design = new CategoryAttribute("Design");
				}
				return CategoryAttribute.design;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the DragDrop category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the drag-and-drop category.</returns>
		public static CategoryAttribute DragDrop
		{
			get
			{
				if (CategoryAttribute.dragDrop == null)
				{
					CategoryAttribute.dragDrop = new CategoryAttribute("DragDrop");
				}
				return CategoryAttribute.dragDrop;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Focus category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the focus category.</returns>
		public static CategoryAttribute Focus
		{
			get
			{
				if (CategoryAttribute.focus == null)
				{
					CategoryAttribute.focus = new CategoryAttribute("Focus");
				}
				return CategoryAttribute.focus;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Format category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the format category.</returns>
		public static CategoryAttribute Format
		{
			get
			{
				if (CategoryAttribute.format == null)
				{
					CategoryAttribute.format = new CategoryAttribute("Format");
				}
				return CategoryAttribute.format;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Key category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the key category.</returns>
		public static CategoryAttribute Key
		{
			get
			{
				if (CategoryAttribute.key == null)
				{
					CategoryAttribute.key = new CategoryAttribute("Key");
				}
				return CategoryAttribute.key;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Layout category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the layout category.</returns>
		public static CategoryAttribute Layout
		{
			get
			{
				if (CategoryAttribute.layout == null)
				{
					CategoryAttribute.layout = new CategoryAttribute("Layout");
				}
				return CategoryAttribute.layout;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the Mouse category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the mouse category.</returns>
		public static CategoryAttribute Mouse
		{
			get
			{
				if (CategoryAttribute.mouse == null)
				{
					CategoryAttribute.mouse = new CategoryAttribute("Mouse");
				}
				return CategoryAttribute.mouse;
			}
		}

		/// <summary>Gets a <see cref="T:System.ComponentModel.CategoryAttribute" /> representing the WindowStyle category.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.CategoryAttribute" /> for the window style category.</returns>
		public static CategoryAttribute WindowStyle
		{
			get
			{
				if (CategoryAttribute.windowStyle == null)
				{
					CategoryAttribute.windowStyle = new CategoryAttribute("WindowStyle");
				}
				return CategoryAttribute.windowStyle;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.CategoryAttribute" /> class using the category name Default.</summary>
		public CategoryAttribute() : this("Default")
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.CategoryAttribute" /> class using the specified category name.</summary>
		/// <param name="category">The name of the category.</param>
		public CategoryAttribute(string category)
		{
			this.categoryValue = category;
			this.localized = false;
		}

		/// <summary>Gets the name of the category for the property or event that this attribute is applied to.</summary>
		/// <returns>The name of the category for the property or event that this attribute is applied to.</returns>
		public string Category
		{
			get
			{
				if (!this.localized)
				{
					this.localized = true;
					string localizedString = this.GetLocalizedString(this.categoryValue);
					if (localizedString != null)
					{
						this.categoryValue = localizedString;
					}
				}
				return this.categoryValue;
			}
		}

		/// <summary>Returns whether the value of the given object is equal to the current <see cref="T:System.ComponentModel.CategoryAttribute" />.</summary>
		/// <param name="obj">The object to test the value equality of.</param>
		/// <returns>
		///   <see langword="true" /> if the value of the given object is equal to that of the current; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj == this || (obj is CategoryAttribute && this.Category.Equals(((CategoryAttribute)obj).Category));
		}

		/// <summary>Returns the hash code for this attribute.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return this.Category.GetHashCode();
		}

		/// <summary>Looks up the localized name of the specified category.</summary>
		/// <param name="value">The identifer for the category to look up.</param>
		/// <returns>The localized name of the category, or <see langword="null" /> if a localized name does not exist.</returns>
		protected virtual string GetLocalizedString(string value)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(value);
			if (num <= 1062369733U)
			{
				if (num <= 630759034U)
				{
					if (num <= 433860734U)
					{
						if (num != 175614239U)
						{
							if (num == 433860734U)
							{
								if (value == "Default")
								{
									return "Misc";
								}
							}
						}
						else if (value == "Action")
						{
							return "Action";
						}
					}
					else if (num != 521774151U)
					{
						if (num == 630759034U)
						{
							if (value == "DragDrop")
							{
								return "Drag Drop";
							}
						}
					}
					else if (value == "Behavior")
					{
						return "Behavior";
					}
				}
				else if (num <= 723360612U)
				{
					if (num != 676498961U)
					{
						if (num == 723360612U)
						{
							if (value == "Mouse")
							{
								return "Mouse";
							}
						}
					}
					else if (value == "Scale")
					{
						return "Scale";
					}
				}
				else if (num != 822184863U)
				{
					if (num != 1041509726U)
					{
						if (num == 1062369733U)
						{
							if (value == "Data")
							{
								return "Data";
							}
						}
					}
					else if (value == "Text")
					{
						return "Text";
					}
				}
				else if (value == "Appearance")
				{
					return "Appearance";
				}
			}
			else if (num <= 2809814704U)
			{
				if (num <= 1779622119U)
				{
					if (num != 1762750224U)
					{
						if (num == 1779622119U)
						{
							if (value == "Config")
							{
								return "Configurations";
							}
						}
					}
					else if (value == "DDE")
					{
						return "DDE";
					}
				}
				else if (num != 2055433310U)
				{
					if (num != 2368288673U)
					{
						if (num == 2809814704U)
						{
							if (value == "Font")
							{
								return "Font";
							}
						}
					}
					else if (value == "List")
					{
						return "List";
					}
				}
				else if (value == "WindowStyle")
				{
					return "Window Style";
				}
			}
			else if (num <= 3441084684U)
			{
				if (num != 3159863731U)
				{
					if (num == 3441084684U)
					{
						if (value == "Key")
						{
							return "Key";
						}
					}
				}
				else if (value == "Focus")
				{
					return "Focus";
				}
			}
			else if (num != 3799987242U)
			{
				if (num != 3901555439U)
				{
					if (num == 4152902175U)
					{
						if (value == "Layout")
						{
							return "Layout";
						}
					}
				}
				else if (value == "Design")
				{
					return "Design";
				}
			}
			else if (value == "Position")
			{
				return "Position";
			}
			return value;
		}

		/// <summary>Determines if this attribute is the default.</summary>
		/// <returns>
		///   <see langword="true" /> if the attribute is the default value for this attribute class; otherwise, <see langword="false" />.</returns>
		public override bool IsDefaultAttribute()
		{
			return this.Category.Equals(CategoryAttribute.Default.Category);
		}

		private static volatile CategoryAttribute appearance;

		private static volatile CategoryAttribute asynchronous;

		private static volatile CategoryAttribute behavior;

		private static volatile CategoryAttribute data;

		private static volatile CategoryAttribute design;

		private static volatile CategoryAttribute action;

		private static volatile CategoryAttribute format;

		private static volatile CategoryAttribute layout;

		private static volatile CategoryAttribute mouse;

		private static volatile CategoryAttribute key;

		private static volatile CategoryAttribute focus;

		private static volatile CategoryAttribute windowStyle;

		private static volatile CategoryAttribute dragDrop;

		private static volatile CategoryAttribute defAttr;

		private bool localized;

		private string categoryValue;
	}
}
