using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.Hierarchy;
using Meta.XR.ImmersiveDebugger.Manager;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	internal class InspectorPanel : DebugPanel, IDebugUIPanel
	{
		private Flex Flex
		{
			get
			{
				return this._scrollView.Flex;
			}
		}

		internal ScrollView ScrollView
		{
			get
			{
				return this._scrollView;
			}
		}

		private Flex CategoryFlex
		{
			get
			{
				return this._categoryScrollView.Flex;
			}
		}

		private Flex HierarchyFlex
		{
			get
			{
				return this._hierarchyScrollView.Flex;
			}
		}

		public ImageStyle CategoryBackgroundStyle
		{
			set
			{
				this._categoryBackground.Sprite = value.sprite;
				this._categoryBackground.Color = value.color;
				this._categoryBackground.PixelDensityMultiplier = value.pixelDensityMultiplier;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._debugInterface = Object.FindObjectOfType<DebugInterface>();
			Flex flex = base.Append<Flex>("div");
			flex.LayoutStyle = Style.Load<LayoutStyle>("InspectorDivFlex");
			this._categoryDiv = flex.Append<Flex>("categories_div");
			this._categoryDiv.LayoutStyle = Style.Load<LayoutStyle>("CategoriesDiv");
			this._categoryBackground = this._categoryDiv.Append<Background>("background");
			this._categoryBackground.LayoutStyle = Style.Load<LayoutStyle>("CategoriesDivBackground");
			this._categoryBackgroundImageStyle = Style.Load<ImageStyle>("CategoriesDivBackground");
			this.CategoryBackgroundStyle = this._categoryBackgroundImageStyle;
			this._buttonsAnchor = this._categoryDiv.Append<Flex>("header");
			this._buttonsAnchor.LayoutStyle = Style.Load<LayoutStyle>("ConsoleButtons");
			this._hierarchyIcon = this.RegisterControl("Hierarchy", Resources.Load<Texture2D>("Textures/hierarchy_icon"), Style.Load<ImageStyle>("InspectorModeIcon"), new Action(this.SelectHierarchyMode));
			this._categoriesIcon = this.RegisterControl("Categories", Resources.Load<Texture2D>("Textures/categories_icon"), Style.Load<ImageStyle>("InspectorModeIcon"), new Action(this.SelectCategoryMode));
			this._selectedModeTitle = this._buttonsAnchor.Append<Label>("title");
			this._selectedModeTitle.LayoutStyle = Style.Load<LayoutStyle>("InspectorModeTitle");
			this._selectedModeTitle.TextStyle = Style.Load<TextStyle>("MemberTitle");
			this._categoryScrollView = this._categoryDiv.Append<ScrollView>("categories");
			this._categoryScrollView.LayoutStyle = Style.Load<LayoutStyle>("CategoriesScrollView");
			this.CategoryFlex.LayoutStyle = Style.Load<LayoutStyle>("InspectorCategoryFlex");
			this._hierarchyScrollView = this._categoryDiv.Append<ScrollView>("categories");
			this._hierarchyScrollView.LayoutStyle = Style.Load<LayoutStyle>("CategoriesScrollView");
			this.HierarchyFlex.LayoutStyle = Style.Load<LayoutStyle>("InspectorCategoryFlex");
			this._scrollView = flex.Append<ScrollView>("main");
			this._scrollView.LayoutStyle = Style.Load<LayoutStyle>("PanelScrollView");
			this.Flex.LayoutStyle = Style.Load<LayoutStyle>("InspectorMainFlex");
			this.SelectCategoryMode();
		}

		private Toggle RegisterControl(string buttonName, Texture2D icon, ImageStyle style, Action callback)
		{
			if (buttonName == null)
			{
				throw new ArgumentNullException("buttonName");
			}
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			Toggle toggle = this._buttonsAnchor.Append<Toggle>(buttonName);
			toggle.LayoutStyle = Style.Load<LayoutStyle>("ConsoleButton");
			toggle.Icon = icon;
			toggle.IconStyle = (style ? style : Style.Default<ImageStyle>());
			toggle.Callback = callback;
			return toggle;
		}

		private void SelectCategoryMode()
		{
			this._selectedModeTitle.Content = "Custom Inspectors";
			this._categoryDiv.Forget(this._hierarchyScrollView);
			this._categoryDiv.Remember(this._categoryScrollView);
			this._categoriesIcon.State = true;
			this._hierarchyIcon.State = false;
		}

		private void SelectHierarchyMode()
		{
			this._selectedModeTitle.Content = "Hierarchy View";
			this._categoryDiv.Forget(this._categoryScrollView);
			this._categoryDiv.Remember(this._hierarchyScrollView);
			this._hierarchyIcon.State = true;
			this._categoriesIcon.State = false;
		}

		protected override void OnTransparencyChanged()
		{
			base.OnTransparencyChanged();
			this._categoryBackground.Color = (base.Transparent ? this._categoryBackgroundImageStyle.colorOff : this._categoryBackgroundImageStyle.color);
		}

		public IInspector RegisterInspector(InstanceHandle instanceHandle, Category category)
		{
			if (instanceHandle.Instance != null && !(instanceHandle.Instance is Component))
			{
				HierarchyItemButton hierarchyItemButton = this.GetHierarchyItemButton(category.Item, true);
				int counter = hierarchyItemButton.Counter;
				hierarchyItemButton.Counter = counter + 1;
				return null;
			}
			Dictionary<InstanceHandle, Inspector> dictionary;
			Inspector inspector = this.GetInspectorInternal(instanceHandle, category, true, out dictionary);
			if (inspector != null)
			{
				return inspector;
			}
			float progress = this._scrollView.Progress;
			Object instance = instanceHandle.Instance;
			string childName = (instance != null) ? instance.name : instanceHandle.Type.Name;
			inspector = this.Flex.Append<Inspector>(childName);
			inspector.LayoutStyle = Style.Load<LayoutStyle>("Inspector");
			inspector.InstanceHandle = instanceHandle;
			dictionary.Add(instanceHandle, inspector);
			this._scrollView.Progress = progress;
			if (category.Item != null)
			{
				inspector.Foldout.State = false;
				HierarchyItemButton hierarchyItemButton2 = this.GetHierarchyItemButton(category.Item, true);
				HierarchyItemButton hierarchyItemButton3 = hierarchyItemButton2;
				int counter = hierarchyItemButton3.Counter;
				hierarchyItemButton3.Counter = counter + 1;
				if (!this._hierarchyScrollView.Visibility || this._selectedItem != hierarchyItemButton2)
				{
					this.Flex.Forget(inspector);
				}
			}
			else
			{
				CategoryButton categoryButton = this.GetCategoryButton(category, true);
				CategoryButton categoryButton2 = categoryButton;
				int counter = categoryButton2.Counter;
				categoryButton2.Counter = counter + 1;
				if (!this._categoryScrollView.Visibility || this._selectedCategory != categoryButton)
				{
					this.Flex.Forget(inspector);
				}
			}
			return inspector;
		}

		public void UnregisterInspector(InstanceHandle instanceHandle, Category category, bool allCategories)
		{
			if (allCategories)
			{
				using (Dictionary<Category, Dictionary<Type, Dictionary<InstanceHandle, Inspector>>>.Enumerator enumerator = this._registries.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<Category, Dictionary<Type, Dictionary<InstanceHandle, Inspector>>> keyValuePair = enumerator.Current;
						Category category2;
						Dictionary<Type, Dictionary<InstanceHandle, Inspector>> dictionary;
						keyValuePair.Deconstruct(out category2, out dictionary);
						Category category3 = category2;
						Dictionary<InstanceHandle, Inspector> dictionary2;
						Inspector inspector;
						if (dictionary.TryGetValue(instanceHandle.Type, out dictionary2) && dictionary2.TryGetValue(instanceHandle, out inspector))
						{
							dictionary2.Remove(instanceHandle);
							this.RemoveInspector(category3, inspector);
						}
					}
					return;
				}
			}
			if (!(instanceHandle.Instance is Component))
			{
				this.TryRemoveHierarchyItemButton(category.Item);
				return;
			}
			Dictionary<InstanceHandle, Inspector> dictionary3;
			Inspector inspectorInternal = this.GetInspectorInternal(instanceHandle, category, false, out dictionary3);
			if (inspectorInternal == null)
			{
				return;
			}
			if (dictionary3 != null)
			{
				dictionary3.Remove(instanceHandle);
			}
			this.RemoveInspector(category, inspectorInternal);
		}

		private void RemoveInspector(Category category, Inspector inspector)
		{
			float progress = this._scrollView.Progress;
			this.Flex.Remove(inspector, true);
			this._scrollView.Progress = progress;
			if (category.Item != null)
			{
				this.TryRemoveHierarchyItemButton(category.Item);
				return;
			}
			CategoryButton categoryButton = this.GetCategoryButton(category, false);
			if (categoryButton != null)
			{
				CategoryButton categoryButton2 = categoryButton;
				int counter = categoryButton2.Counter;
				categoryButton2.Counter = counter - 1;
			}
		}

		public IInspector GetInspector(InstanceHandle instanceHandle, Category category)
		{
			Dictionary<InstanceHandle, Inspector> dictionary;
			return this.GetInspectorInternal(instanceHandle, category, false, out dictionary);
		}

		public Inspector GetInspectorInternal(InstanceHandle instanceHandle, Category category, bool createRegistries, out Dictionary<InstanceHandle, Inspector> registry)
		{
			Inspector result = null;
			Dictionary<Type, Dictionary<InstanceHandle, Inspector>> dictionary;
			if (!this._registries.TryGetValue(category, out dictionary) && createRegistries)
			{
				dictionary = new Dictionary<Type, Dictionary<InstanceHandle, Inspector>>();
				this._registries.Add(category, dictionary);
			}
			if (dictionary == null)
			{
				registry = null;
				return null;
			}
			if (!dictionary.TryGetValue(instanceHandle.Type, out registry))
			{
				if (!createRegistries)
				{
					return result;
				}
				registry = new Dictionary<InstanceHandle, Inspector>();
				dictionary.Add(instanceHandle.Type, registry);
			}
			registry.TryGetValue(instanceHandle, out result);
			return result;
		}

		private CategoryButton GetCategoryButton(Category category, bool create = false)
		{
			CategoryButton button;
			if (this._categories.TryGetValue(category, out button) || !create)
			{
				return button;
			}
			button = this.CategoryFlex.Append<CategoryButton>(category.Id);
			button.LayoutStyle = Style.Instantiate<LayoutStyle>("CategoryButton");
			button.Category = category;
			button.Callback = delegate()
			{
				this.SelectCategoryButton(button);
			};
			this._categories.Add(category, button);
			if (this._selectedCategory == null)
			{
				this.SelectCategoryButton(button);
			}
			return button;
		}

		private Controller ComputeIdealPreviousItem(Item item)
		{
			Item parent = item.Parent;
			if (parent == null || parent is SceneRegistry)
			{
				return null;
			}
			HierarchyItemButton hierarchyItemButton = this.GetHierarchyItemButton(item.Parent, true);
			Controller controller = null;
			foreach (Controller controller2 in this.HierarchyFlex.Children)
			{
				HierarchyItemButton hierarchyItemButton2 = controller2 as HierarchyItemButton;
				if (hierarchyItemButton2 != null)
				{
					if (hierarchyItemButton2.Item.Parent == item.Parent || hierarchyItemButton2 == hierarchyItemButton)
					{
						controller = hierarchyItemButton2;
					}
					else if (controller != null)
					{
						break;
					}
				}
			}
			return controller;
		}

		private HierarchyItemButton GetHierarchyItemButton(Item item, bool create = false)
		{
			HierarchyItemButton button;
			if (this._items.TryGetValue(item, out button) || !create)
			{
				return button;
			}
			Controller controller = this.ComputeIdealPreviousItem(item);
			button = ((controller != null) ? this.HierarchyFlex.InsertAfter<HierarchyItemButton>(item.Label, controller) : this.HierarchyFlex.Append<HierarchyItemButton>(item.Label));
			button.LayoutStyle = Style.Instantiate<LayoutStyle>("HierarchyItemButton");
			button.Item = item;
			button.LayoutStyle.SetIndent((float)((item.Depth - 1) * 10));
			button.LayoutStyle.SetWidth(button.LayoutStyle.size.x - (float)(item.Depth * 10));
			button.Label.Callback = delegate()
			{
				this.SelectHierarchyItemButton(button);
			};
			button.Foldout.Callback = delegate()
			{
				this.ToggleFoldItem(button);
			};
			this._items.Add(item, button);
			return button;
		}

		private void TryRemoveHierarchyItemButton(Item item)
		{
			HierarchyItemButton hierarchyItemButton = this.GetHierarchyItemButton(item, false);
			if (hierarchyItemButton == null)
			{
				return;
			}
			HierarchyItemButton hierarchyItemButton2 = hierarchyItemButton;
			int counter = hierarchyItemButton2.Counter;
			hierarchyItemButton2.Counter = counter - 1;
			if (hierarchyItemButton.Counter != 0)
			{
				return;
			}
			this._items.Remove(item);
			this.HierarchyFlex.Remove(hierarchyItemButton, true);
		}

		private void SelectCategoryButton(CategoryButton categoryButton)
		{
			if (this._selectedCategory == categoryButton)
			{
				return;
			}
			this.SelectHierarchyItemButton(null);
			this.Flex.ForgetAll();
			if (this._selectedCategory != null)
			{
				this._selectedCategory.State = false;
			}
			this._selectedCategory = categoryButton;
			if (this._selectedCategory != null)
			{
				this._selectedCategory.State = true;
				this.SelectCategory(categoryButton.Category);
			}
			this._scrollView.Progress = 1f;
		}

		private void SelectCategory(Category category)
		{
			Dictionary<Type, Dictionary<InstanceHandle, Inspector>> dictionary;
			if (!this._registries.TryGetValue(category, out dictionary))
			{
				return;
			}
			foreach (KeyValuePair<Type, Dictionary<InstanceHandle, Inspector>> keyValuePair in dictionary)
			{
				foreach (KeyValuePair<InstanceHandle, Inspector> keyValuePair2 in keyValuePair.Value)
				{
					this.Flex.Remember(keyValuePair2.Value);
					if (this._debugInterface)
					{
						this._debugInterface.SetTransparencyRecursive(keyValuePair2.Value, !this._debugInterface.OpacityOverride);
					}
				}
			}
		}

		private void ToggleFoldItem(HierarchyItemButton button)
		{
			if (button == null)
			{
				return;
			}
			if (button.Foldout.State)
			{
				this.FoldItem(button);
				return;
			}
			this.UnfoldItem(button);
		}

		private void FoldItem(HierarchyItemButton button)
		{
			button.Foldout.State = false;
		}

		private void UnfoldItem(HierarchyItemButton button)
		{
			button.Foldout.State = true;
		}

		private void SelectHierarchyItemButton(HierarchyItemButton button)
		{
			if (this._selectedItem == button)
			{
				this.ToggleFoldItem(button);
				return;
			}
			this.SelectCategoryButton(null);
			this.Flex.ForgetAll();
			if (this._selectedItem != null)
			{
				Item item = this._selectedItem.Item;
				if (item != null)
				{
					item.ClearContent();
				}
				this._selectedItem.Label.State = false;
			}
			this._selectedItem = button;
			if (this._selectedItem != null)
			{
				this._selectedItem.Label.State = true;
				this.SelectItem(this._selectedItem.Item);
				this.UnfoldItem(button);
			}
			this._scrollView.Progress = 1f;
		}

		private void SelectItem(Item item)
		{
			item.BuildContent();
			this.SelectCategory(item.Category);
		}

		internal void SetPanelPosition(RuntimeSettings.DistanceOption distanceOption, bool skipAnimation = false)
		{
			ValueContainer<Vector3> valueContainer = ValueContainer<Vector3>.Load("InspectorsPanelPositions");
			Vector3 targetPosition;
			if (distanceOption != RuntimeSettings.DistanceOption.Close)
			{
				if (distanceOption != RuntimeSettings.DistanceOption.Far)
				{
					targetPosition = valueContainer["Default"];
				}
				else
				{
					targetPosition = valueContainer["Far"];
				}
			}
			else
			{
				targetPosition = valueContainer["Close"];
			}
			this._targetPosition = targetPosition;
			if (skipAnimation)
			{
				base.SphericalCoordinates = this._targetPosition;
				this._currentPosition = this._targetPosition;
				return;
			}
			this._lerpCompleted = false;
		}

		private void Update()
		{
			if (this._hierarchyIcon.State)
			{
				Manager instance = DebugManagerAddon<Manager>.Instance;
				if (instance != null)
				{
					instance.Refresh();
				}
			}
			if (this._lerpCompleted)
			{
				return;
			}
			this._currentPosition = Utils.LerpPosition(this._currentPosition, this._targetPosition, this._lerpSpeed);
			this._lerpCompleted = (this._currentPosition == this._targetPosition);
			base.SphericalCoordinates = this._currentPosition;
		}

		private ScrollView _scrollView;

		private ScrollView _categoryScrollView;

		private ScrollView _hierarchyScrollView;

		private readonly Dictionary<Category, Dictionary<Type, Dictionary<InstanceHandle, Inspector>>> _registries = new Dictionary<Category, Dictionary<Type, Dictionary<InstanceHandle, Inspector>>>();

		private readonly Dictionary<Category, CategoryButton> _categories = new Dictionary<Category, CategoryButton>();

		private readonly Dictionary<Item, HierarchyItemButton> _items = new Dictionary<Item, HierarchyItemButton>();

		private CategoryButton _selectedCategory;

		private HierarchyItemButton _selectedItem;

		private Background _categoryBackground;

		private Vector3 _currentPosition;

		private Vector3 _targetPosition;

		private readonly float _lerpSpeed = 10f;

		private bool _lerpCompleted = true;

		private ImageStyle _categoryBackgroundImageStyle;

		private DebugInterface _debugInterface;

		private Flex _buttonsAnchor;

		private Label _selectedModeTitle;

		private Toggle _hierarchyIcon;

		private Toggle _categoriesIcon;

		private Flex _categoryDiv;
	}
}
