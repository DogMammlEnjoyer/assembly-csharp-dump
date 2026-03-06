using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables.Visuals
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[AddComponentMenu("XR/Visual/XR Tint Interactable Visual", 11)]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactables.Visuals.XRTintInteractableVisual.html")]
	public class XRTintInteractableVisual : MonoBehaviour
	{
		public Color tintColor
		{
			get
			{
				return this.m_TintColor;
			}
			set
			{
				this.m_TintColor = value;
			}
		}

		public bool tintOnHover
		{
			get
			{
				return this.m_TintOnHover;
			}
			set
			{
				this.m_TintOnHover = value;
			}
		}

		public bool tintOnSelection
		{
			get
			{
				return this.m_TintOnSelection;
			}
			set
			{
				this.m_TintOnSelection = value;
			}
		}

		public List<Renderer> tintRenderers
		{
			get
			{
				return this.m_TintRenderers;
			}
			set
			{
				this.m_TintRenderers = value;
			}
		}

		protected void Awake()
		{
			this.m_Interactable = base.GetComponent<IXRInteractable>();
			Object @object = this.m_Interactable as Object;
			if (@object != null && @object != null)
			{
				this.m_HoverInteractable = (this.m_Interactable as IXRHoverInteractable);
				this.m_SelectInteractable = (this.m_Interactable as IXRSelectInteractable);
				if (this.m_HoverInteractable != null)
				{
					this.m_HoverInteractable.firstHoverEntered.AddListener(new UnityAction<HoverEnterEventArgs>(this.OnFirstHoverEntered));
					this.m_HoverInteractable.lastHoverExited.AddListener(new UnityAction<HoverExitEventArgs>(this.OnLastHoverExited));
				}
				if (this.m_SelectInteractable != null)
				{
					this.m_SelectInteractable.firstSelectEntered.AddListener(new UnityAction<SelectEnterEventArgs>(this.OnFirstSelectEntered));
					this.m_SelectInteractable.lastSelectExited.AddListener(new UnityAction<SelectExitEventArgs>(this.OnLastSelectExited));
				}
			}
			else
			{
				Debug.LogWarning(string.Format("Could not find required interactable component on {0} for tint visual.", base.gameObject) + " Cannot respond to hover or selection.", this);
			}
			if (this.m_TintRenderers.Count == 0)
			{
				base.GetComponents<Renderer>(this.m_TintRenderers);
				if (this.m_TintRenderers.Count == 0)
				{
					Debug.LogWarning(string.Format("Could not find required Renderer component on {0} for tint visual.", base.gameObject), this);
				}
			}
			this.m_EmissionEnabled = this.GetEmissionEnabled();
			this.m_TintPropertyBlock = new MaterialPropertyBlock();
			if (this.m_TintOnHover)
			{
				IXRHoverInteractable hoverInteractable = this.m_HoverInteractable;
				if (hoverInteractable != null && hoverInteractable.isHovered)
				{
					goto IL_179;
				}
			}
			if (!this.m_TintOnSelection)
			{
				return;
			}
			IXRSelectInteractable selectInteractable = this.m_SelectInteractable;
			if (selectInteractable == null || !selectInteractable.isSelected)
			{
				return;
			}
			IL_179:
			this.SetTint(true);
		}

		protected void OnDestroy()
		{
			Object @object = this.m_Interactable as Object;
			if (@object != null && @object != null)
			{
				if (this.m_HoverInteractable != null)
				{
					this.m_HoverInteractable.firstHoverEntered.RemoveListener(new UnityAction<HoverEnterEventArgs>(this.OnFirstHoverEntered));
					this.m_HoverInteractable.lastHoverExited.RemoveListener(new UnityAction<HoverExitEventArgs>(this.OnLastHoverExited));
				}
				if (this.m_SelectInteractable != null)
				{
					this.m_SelectInteractable.firstSelectEntered.RemoveListener(new UnityAction<SelectEnterEventArgs>(this.OnFirstSelectEntered));
					this.m_SelectInteractable.lastSelectExited.RemoveListener(new UnityAction<SelectExitEventArgs>(this.OnLastSelectExited));
				}
			}
		}

		protected virtual void SetTint(bool on)
		{
			Color value = on ? (this.m_TintColor * Mathf.LinearToGammaSpace(1f)) : Color.black;
			if (!this.m_EmissionEnabled && !this.m_HasLoggedMaterialInstance)
			{
				Debug.LogWarning("Emission is not enabled on a Material used by a tint visual, a Material instance will need to be created.", this);
				this.m_HasLoggedMaterialInstance = true;
			}
			foreach (Renderer renderer in this.m_TintRenderers)
			{
				if (!(renderer == null))
				{
					if (!this.m_EmissionEnabled)
					{
						renderer.GetMaterials(XRTintInteractableVisual.s_Materials);
						foreach (Material material in XRTintInteractableVisual.s_Materials)
						{
							if (on)
							{
								material.EnableKeyword("_EMISSION");
							}
							else
							{
								material.DisableKeyword("_EMISSION");
							}
						}
						XRTintInteractableVisual.s_Materials.Clear();
					}
					renderer.GetPropertyBlock(this.m_TintPropertyBlock);
					this.m_TintPropertyBlock.SetColor(XRTintInteractableVisual.ShaderPropertyLookup.emissionColor, value);
					renderer.SetPropertyBlock(this.m_TintPropertyBlock);
				}
			}
		}

		protected virtual bool GetEmissionEnabled()
		{
			foreach (Renderer renderer in this.m_TintRenderers)
			{
				if (!(renderer == null))
				{
					renderer.GetSharedMaterials(XRTintInteractableVisual.s_Materials);
					using (List<Material>.Enumerator enumerator2 = XRTintInteractableVisual.s_Materials.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (!enumerator2.Current.IsKeywordEnabled("_EMISSION"))
							{
								XRTintInteractableVisual.s_Materials.Clear();
								return false;
							}
						}
					}
				}
			}
			XRTintInteractableVisual.s_Materials.Clear();
			return true;
		}

		private void OnFirstHoverEntered(HoverEnterEventArgs args)
		{
			if (this.m_TintOnHover)
			{
				this.SetTint(true);
			}
		}

		private void OnLastHoverExited(HoverExitEventArgs args)
		{
			if (this.m_TintOnHover)
			{
				bool tint;
				if (this.m_TintOnSelection)
				{
					IXRSelectInteractable selectInteractable = this.m_SelectInteractable;
					tint = (selectInteractable != null && selectInteractable.isSelected);
				}
				else
				{
					tint = false;
				}
				this.SetTint(tint);
			}
		}

		private void OnFirstSelectEntered(SelectEnterEventArgs args)
		{
			if (this.m_TintOnSelection)
			{
				this.SetTint(true);
			}
		}

		private void OnLastSelectExited(SelectExitEventArgs args)
		{
			if (this.m_TintOnSelection)
			{
				bool tint;
				if (this.m_TintOnHover)
				{
					IXRHoverInteractable hoverInteractable = this.m_HoverInteractable;
					tint = (hoverInteractable != null && hoverInteractable.isHovered);
				}
				else
				{
					tint = false;
				}
				this.SetTint(tint);
			}
		}

		[SerializeField]
		[Tooltip("Tint color for interactable.")]
		private Color m_TintColor = Color.yellow;

		[SerializeField]
		[Tooltip("Tint on hover.")]
		private bool m_TintOnHover = true;

		[SerializeField]
		[Tooltip("Tint on selection.")]
		private bool m_TintOnSelection = true;

		[SerializeField]
		[Tooltip("Renderer(s) to use for tinting (will default to any Renderer on the GameObject if not specified).")]
		private List<Renderer> m_TintRenderers = new List<Renderer>();

		private IXRInteractable m_Interactable;

		private IXRHoverInteractable m_HoverInteractable;

		private IXRSelectInteractable m_SelectInteractable;

		private MaterialPropertyBlock m_TintPropertyBlock;

		private bool m_EmissionEnabled;

		private bool m_HasLoggedMaterialInstance;

		private static readonly List<Material> s_Materials = new List<Material>();

		private struct ShaderPropertyLookup
		{
			public static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");
		}
	}
}
