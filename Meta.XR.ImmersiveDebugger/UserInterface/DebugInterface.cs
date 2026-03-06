using System;
using Meta.XR.ImmersiveDebugger.Manager;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	public class DebugInterface : Interface
	{
		protected override bool FollowOverride
		{
			get
			{
				return this._followButton.State;
			}
			set
			{
				this._followButton.State = value;
			}
		}

		protected override bool RotateOverride
		{
			get
			{
				return this._rotateButton.State;
			}
			set
			{
				this._rotateButton.State = value;
			}
		}

		public bool OpacityOverride
		{
			get
			{
				return this._opacityButton.State;
			}
			set
			{
				this._opacityButton.State = value;
				foreach (Controller controller in base.Children)
				{
					InteractableController interactableController = controller as InteractableController;
					if (interactableController != null)
					{
						this.SetTransparencyRecursive(interactableController, !this.OpacityOverride);
					}
				}
			}
		}

		internal void SetTransparencyRecursive(Controller controller, bool transparent)
		{
			controller.Transparent = transparent;
			if (controller.Children == null)
			{
				return;
			}
			foreach (Controller controller2 in controller.Children)
			{
				this.SetTransparencyRecursive(controller2, transparent);
			}
		}

		internal override void Awake()
		{
			base.Awake();
			base.Hide();
			this._inspectorPanel = base.Append<InspectorPanel>("inspectors");
			this._inspectorPanel.LayoutStyle = Style.Load<LayoutStyle>("InspectorPanel");
			this._inspectorPanel.BackgroundStyle = Style.Load<ImageStyle>("PanelBackground");
			this._inspectorPanel.Title = "Inspectors";
			this._inspectorPanel.Icon = Resources.Load<Texture2D>("Textures/inspectors_icon");
			this._inspectorPanel.SetPanelPosition(RuntimeSettings.Instance.PanelDistance, true);
			this._console = base.Append<Console>("console");
			this._console.LayoutStyle = Style.Load<LayoutStyle>("ConsolePanel");
			this._console.BackgroundStyle = Style.Load<ImageStyle>("PanelBackground");
			this._console.Title = "Console";
			this._console.Icon = Resources.Load<Texture2D>("Textures/console_icon");
			this._console.SetPanelPosition(RuntimeSettings.Instance.PanelDistance, true);
			this._distanceToggleIndex = (int)RuntimeSettings.Instance.PanelDistance;
			this._bar = base.Append<DebugBar>("bar");
			this._bar.LayoutStyle = Style.Load<LayoutStyle>("Bar");
			this._bar.BackgroundStyle = Style.Load<ImageStyle>("BarBackground");
			this._bar.SphericalCoordinates = new Vector3(0.7f, 0f, -0.5f);
			this._bar.RegisterPanel(this._console);
			this._bar.RegisterPanel(this._inspectorPanel);
			this._opacityButton = this._bar.RegisterControl("opacity", Resources.Load<Texture2D>("Textures/opacity_icon"), delegate
			{
				this.OpacityOverride = !this.OpacityOverride;
			});
			this._followButton = this._bar.RegisterControl("followMove", Resources.Load<Texture2D>("Textures/move_icon"), new Action(this.ToggleFollowTranslation));
			this._rotateButton = this._bar.RegisterControl("followRotation", Resources.Load<Texture2D>("Textures/rotate_icon"), new Action(this.ToggleFollowRotation));
			this._distanceButton = this._bar.RegisterControl("setDistance", Resources.Load<Texture2D>("Textures/shift_icon"), new Action(this.ToggleDistances));
			this._distanceButton.State = true;
			RuntimeSettings instance = RuntimeSettings.Instance;
			this.FollowOverride = instance.FollowOverride;
			this.RotateOverride = instance.RotateOverride;
			this.OpacityOverride = true;
			if (instance.ShowInspectors)
			{
				this._inspectorPanel.Show();
			}
			if (instance.ShowConsole)
			{
				this._console.Show();
			}
			if (instance.ImmersiveDebuggerDisplayAtStartup)
			{
				base.Show();
			}
			DebugManager instance2 = DebugManager.Instance;
			if (instance2 != null)
			{
				instance2.OnUpdateAction += this.UpdateVisibility;
				instance2.CustomShouldRetrieveInstanceCondition += this.IsInspectorPanelVisible;
			}
		}

		private void ToggleDistances()
		{
			int num = this._distanceToggleIndex + 1;
			this._distanceToggleIndex = num;
			this._distanceToggleIndex = num % this._distanceOptionSize;
			this._inspectorPanel.SetPanelPosition((RuntimeSettings.DistanceOption)this._distanceToggleIndex, false);
			this._console.SetPanelPosition((RuntimeSettings.DistanceOption)this._distanceToggleIndex, false);
		}

		private void ToggleFollowTranslation()
		{
			this.FollowOverride = !this.FollowOverride;
		}

		private void ToggleFollowRotation()
		{
			this.RotateOverride = !this.RotateOverride;
		}

		private void UpdateVisibility()
		{
			if (OVRInput.GetDown(RuntimeSettings.Instance.ImmersiveDebuggerToggleDisplayButton, OVRInput.Controller.Active))
			{
				base.ToggleVisibility();
			}
		}

		private void Update()
		{
			RuntimeSettings instance = RuntimeSettings.Instance;
			if (OVRInput.GetDown(instance.ToggleFollowTranslationButton, OVRInput.Controller.Active))
			{
				this.ToggleFollowTranslation();
			}
			if (OVRInput.GetDown(instance.ToggleFollowRotationButton, OVRInput.Controller.Active))
			{
				this.ToggleFollowRotation();
			}
		}

		private bool IsInspectorPanelVisible()
		{
			if (base.Visibility)
			{
				InspectorPanel inspectorPanel = this._inspectorPanel;
				return inspectorPanel != null && inspectorPanel.Visibility;
			}
			return false;
		}

		private DebugBar _bar;

		private Toggle _showAllButton;

		private Toggle _followButton;

		private Toggle _rotateButton;

		private Toggle _opacityButton;

		private Toggle _distanceButton;

		private InspectorPanel _inspectorPanel;

		private Console _console;

		private int _distanceToggleIndex;

		private readonly int _distanceOptionSize = Enum.GetValues(typeof(RuntimeSettings.DistanceOption)).Length;
	}
}
