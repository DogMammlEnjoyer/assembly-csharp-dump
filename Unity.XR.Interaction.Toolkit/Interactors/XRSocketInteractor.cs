using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[DisallowMultipleComponent]
	[AddComponentMenu("XR/Interactors/XR Socket Interactor", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor.html")]
	public class XRSocketInteractor : XRBaseInteractor
	{
		public bool showInteractableHoverMeshes
		{
			get
			{
				return this.m_ShowInteractableHoverMeshes;
			}
			set
			{
				this.m_ShowInteractableHoverMeshes = value;
			}
		}

		public Material interactableHoverMeshMaterial
		{
			get
			{
				return this.m_InteractableHoverMeshMaterial;
			}
			set
			{
				this.m_InteractableHoverMeshMaterial = value;
			}
		}

		public Material interactableCantHoverMeshMaterial
		{
			get
			{
				return this.m_InteractableCantHoverMeshMaterial;
			}
			set
			{
				this.m_InteractableCantHoverMeshMaterial = value;
			}
		}

		public bool socketActive
		{
			get
			{
				return this.m_SocketActive;
			}
			set
			{
				this.m_SocketActive = value;
				this.m_SocketGrabTransformer.canProcess = (value && base.isActiveAndEnabled);
			}
		}

		public float interactableHoverScale
		{
			get
			{
				return this.m_InteractableHoverScale;
			}
			set
			{
				this.m_InteractableHoverScale = value;
			}
		}

		public float recycleDelayTime
		{
			get
			{
				return this.m_RecycleDelayTime;
			}
			set
			{
				this.m_RecycleDelayTime = value;
			}
		}

		public bool hoverSocketSnapping
		{
			get
			{
				return this.m_HoverSocketSnapping;
			}
			set
			{
				this.m_HoverSocketSnapping = value;
			}
		}

		public float socketSnappingRadius
		{
			get
			{
				return this.m_SocketSnappingRadius;
			}
			set
			{
				this.m_SocketSnappingRadius = value;
				this.m_SocketGrabTransformer.socketSnappingRadius = value;
			}
		}

		public SocketScaleMode socketScaleMode
		{
			get
			{
				return this.m_SocketScaleMode;
			}
			set
			{
				this.m_SocketScaleMode = value;
				this.m_SocketGrabTransformer.scaleMode = value;
			}
		}

		public Vector3 fixedScale
		{
			get
			{
				return this.m_FixedScale;
			}
			set
			{
				this.m_FixedScale = value;
				this.m_SocketGrabTransformer.fixedScale = value;
			}
		}

		public Vector3 targetBoundsSize
		{
			get
			{
				return this.m_TargetBoundsSize;
			}
			set
			{
				this.m_TargetBoundsSize = value;
				this.m_SocketGrabTransformer.targetBoundsSize = value;
			}
		}

		protected List<IXRInteractable> unsortedValidTargets { get; } = new List<IXRInteractable>();

		protected virtual int socketSnappingLimit
		{
			get
			{
				return 1;
			}
		}

		protected virtual bool ejectExistingSocketsWhenSnapping
		{
			get
			{
				return true;
			}
		}

		protected virtual void OnValidate()
		{
			this.SyncTransformerParams();
		}

		protected override void Awake()
		{
			base.Awake();
			this.m_TriggerContactMonitor.interactionManager = base.interactionManager;
			this.m_UpdateCollidersAfterTriggerStay = this.UpdateCollidersAfterOnTriggerStay();
			this.SyncTransformerParams();
			this.CreateDefaultHoverMaterials();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_TriggerContactMonitor.contactAdded += this.OnContactAdded;
			this.m_TriggerContactMonitor.contactRemoved += this.OnContactRemoved;
			this.SyncTransformerParams();
			this.ResetCollidersAndValidTargets();
			base.StartCoroutine(this.m_UpdateCollidersAfterTriggerStay);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_SocketGrabTransformer.canProcess = false;
			this.m_TriggerContactMonitor.contactAdded -= this.OnContactAdded;
			this.m_TriggerContactMonitor.contactRemoved -= this.OnContactRemoved;
			this.ResetCollidersAndValidTargets();
			base.StopCoroutine(this.m_UpdateCollidersAfterTriggerStay);
		}

		protected void OnTriggerEnter(Collider other)
		{
			this.m_TriggerContactMonitor.AddCollider(other);
		}

		protected void OnTriggerStay(Collider other)
		{
			this.m_StayedColliders.Add(other);
		}

		protected void OnTriggerExit(Collider other)
		{
			this.m_TriggerContactMonitor.RemoveCollider(other);
		}

		private IEnumerator UpdateCollidersAfterOnTriggerStay()
		{
			for (;;)
			{
				yield return XRSocketInteractor.s_WaitForFixedUpdate;
				this.m_TriggerContactMonitor.UpdateStayedColliders(this.m_StayedColliders);
			}
			yield break;
		}

		public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.ProcessInteractor(updatePhase);
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
			{
				this.m_StayedColliders.Clear();
				return;
			}
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && this.m_ShowInteractableHoverMeshes && base.hasHover && this.isHoverRecycleAllowed)
			{
				this.DrawHoveredInteractables();
			}
		}

		protected virtual void CreateDefaultHoverMaterials()
		{
			if (this.m_InteractableHoverMeshMaterial != null && this.m_InteractableCantHoverMeshMaterial != null)
			{
				return;
			}
			string text = GraphicsSettings.currentRenderPipeline ? "Universal Render Pipeline/Lit" : "Standard";
			Shader shader = Shader.Find(text);
			if (shader == null)
			{
				Debug.LogWarning("Failed to create default materials for Socket Interactor, was unable to find \"" + text + "\" Shader. Make sure the shader is included into the game build.", this);
				return;
			}
			if (this.m_InteractableHoverMeshMaterial == null)
			{
				this.m_InteractableHoverMeshMaterial = new Material(shader);
				XRSocketInteractor.SetMaterialFade(this.m_InteractableHoverMeshMaterial, new Color(0f, 0f, 1f, 0.6f));
			}
			if (this.m_InteractableCantHoverMeshMaterial == null)
			{
				this.m_InteractableCantHoverMeshMaterial = new Material(shader);
				XRSocketInteractor.SetMaterialFade(this.m_InteractableCantHoverMeshMaterial, new Color(1f, 0f, 0f, 0.6f));
			}
		}

		private static void SetMaterialFade(Material material, Color color)
		{
			material.SetOverrideTag("RenderType", "Transparent");
			bool flag = GraphicsSettings.currentRenderPipeline != null;
			if (flag)
			{
				material.SetFloat(XRSocketInteractor.ShaderPropertyLookup.surface, 1f);
			}
			material.SetFloat(XRSocketInteractor.ShaderPropertyLookup.mode, 2f);
			material.SetInt(XRSocketInteractor.ShaderPropertyLookup.srcBlend, 5);
			material.SetInt(XRSocketInteractor.ShaderPropertyLookup.dstBlend, 10);
			material.SetInt(XRSocketInteractor.ShaderPropertyLookup.zWrite, 0);
			material.DisableKeyword("_ALPHATEST_ON");
			material.EnableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 3000;
			material.SetColor(flag ? XRSocketInteractor.ShaderPropertyLookup.baseColor : XRSocketInteractor.ShaderPropertyLookup.color, color);
		}

		protected override void OnHoverEntering(HoverEnterEventArgs args)
		{
			base.OnHoverEntering(args);
			if (!this.m_ShowInteractableHoverMeshes)
			{
				return;
			}
			IXRHoverInteractable interactableObject = args.interactableObject;
			XRSocketInteractor.s_MeshFilters.Clear();
			interactableObject.transform.GetComponentsInChildren<MeshFilter>(true, XRSocketInteractor.s_MeshFilters);
			if (XRSocketInteractor.s_MeshFilters.Count == 0)
			{
				return;
			}
			ValueTuple<MeshFilter, Renderer>[] array = new ValueTuple<MeshFilter, Renderer>[XRSocketInteractor.s_MeshFilters.Count];
			for (int i = 0; i < XRSocketInteractor.s_MeshFilters.Count; i++)
			{
				MeshFilter meshFilter = XRSocketInteractor.s_MeshFilters[i];
				array[i] = new ValueTuple<MeshFilter, Renderer>(meshFilter, meshFilter.GetComponent<Renderer>());
			}
			this.m_MeshFilterCache.Add(interactableObject, array);
		}

		protected override void OnHoverEntered(HoverEnterEventArgs args)
		{
			base.OnHoverEntered(args);
			if (!this.CanHoverSnap(args.interactableObject))
			{
				return;
			}
			XRGrabInteractable xrgrabInteractable = args.interactableObject as XRGrabInteractable;
			if (xrgrabInteractable != null)
			{
				this.StartSocketSnapping(xrgrabInteractable);
			}
		}

		protected virtual bool CanHoverSnap(IXRInteractable interactable)
		{
			return this.m_HoverSocketSnapping && (!base.hasSelection || base.IsSelecting(interactable));
		}

		protected override void OnHoverExiting(HoverExitEventArgs args)
		{
			base.OnHoverExiting(args);
			IXRHoverInteractable interactableObject = args.interactableObject;
			this.m_MeshFilterCache.Remove(interactableObject);
			XRGrabInteractable xrgrabInteractable = interactableObject as XRGrabInteractable;
			if (xrgrabInteractable != null)
			{
				this.EndSocketSnapping(xrgrabInteractable);
			}
		}

		protected override void OnSelectEntered(SelectEnterEventArgs args)
		{
			base.OnSelectEntered(args);
			XRGrabInteractable xrgrabInteractable = args.interactableObject as XRGrabInteractable;
			if (xrgrabInteractable != null)
			{
				this.StartSocketSnapping(xrgrabInteractable);
			}
		}

		protected override void OnSelectExiting(SelectExitEventArgs args)
		{
			base.OnSelectExiting(args);
			this.m_LastRemoveTime = Time.time;
		}

		protected override void OnSelectExited(SelectExitEventArgs args)
		{
			base.OnSelectExited(args);
			XRGrabInteractable xrgrabInteractable = args.interactableObject as XRGrabInteractable;
			if (xrgrabInteractable != null)
			{
				if (base.IsHovering(args.interactableObject))
				{
					this.m_SocketGrabTransformer.scaleOnlyMode = true;
					return;
				}
				this.EndSocketSnapping(xrgrabInteractable);
			}
		}

		private Matrix4x4 GetHoverMeshMatrix(IXRInteractable interactable, MeshFilter meshFilter, float hoverScale)
		{
			Transform attachTransform = interactable.GetAttachTransform(this);
			XRGrabInteractable xrgrabInteractable = interactable as XRGrabInteractable;
			Pose pose;
			if (xrgrabInteractable != null && !xrgrabInteractable.useDynamicAttach && xrgrabInteractable.isSelected && attachTransform != interactable.transform && attachTransform.IsChildOf(interactable.transform))
			{
				Pose localAttachPoseOnSelect = xrgrabInteractable.GetLocalAttachPoseOnSelect(xrgrabInteractable.firstInteractorSelecting);
				Transform parent = attachTransform.parent;
				pose = new Pose(parent.TransformPoint(localAttachPoseOnSelect.position), parent.rotation * localAttachPoseOnSelect.rotation);
			}
			else
			{
				pose = new Pose(attachTransform.position, attachTransform.rotation);
			}
			Vector3 direction = meshFilter.transform.position - pose.position;
			Vector3 point = XRSocketInteractor.InverseTransformDirection(pose, direction) * hoverScale;
			Quaternion rhs = Quaternion.Inverse(Quaternion.Inverse(meshFilter.transform.rotation) * pose.rotation);
			Transform attachTransform2 = this.GetAttachTransform(interactable);
			Pose pose2 = new Pose(attachTransform2.position, attachTransform2.rotation);
			Vector3 pos;
			Quaternion q;
			if (xrgrabInteractable == null || xrgrabInteractable.trackRotation)
			{
				pos = pose2.rotation * point + pose2.position;
				q = pose2.rotation * rhs;
			}
			else
			{
				pos = pose.rotation * point + pose2.position;
				q = meshFilter.transform.rotation;
			}
			if (xrgrabInteractable != null && !xrgrabInteractable.trackPosition)
			{
				pos = meshFilter.transform.position;
			}
			Vector3 s = meshFilter.transform.lossyScale * hoverScale;
			return Matrix4x4.TRS(pos, q, s);
		}

		private static Vector3 InverseTransformDirection(Pose pose, Vector3 direction)
		{
			return Quaternion.Inverse(pose.rotation) * direction;
		}

		protected virtual void DrawHoveredInteractables()
		{
			if (!this.m_ShowInteractableHoverMeshes || this.m_InteractableHoverScale <= 0f)
			{
				return;
			}
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			foreach (IXRHoverInteractable ixrhoverInteractable in base.interactablesHovered)
			{
				ValueTuple<MeshFilter, Renderer>[] array;
				if (ixrhoverInteractable != null && !base.IsSelecting(ixrhoverInteractable) && this.m_MeshFilterCache.TryGetValue(ixrhoverInteractable, out array) && array != null && array.Length != 0)
				{
					Material hoveredInteractableMaterial = this.GetHoveredInteractableMaterial(ixrhoverInteractable);
					if (!(hoveredInteractableMaterial == null))
					{
						foreach (ValueTuple<MeshFilter, Renderer> valueTuple in array)
						{
							MeshFilter item = valueTuple.Item1;
							Renderer item2 = valueTuple.Item2;
							if (this.ShouldDrawHoverMesh(item, item2, main))
							{
								Matrix4x4 hoverMeshMatrix = this.GetHoverMeshMatrix(ixrhoverInteractable, item, this.m_InteractableHoverScale);
								Mesh sharedMesh = item.sharedMesh;
								for (int j = 0; j < sharedMesh.subMeshCount; j++)
								{
									Graphics.DrawMesh(sharedMesh, hoverMeshMatrix, hoveredInteractableMaterial, base.gameObject.layer, null, j);
								}
							}
						}
					}
				}
			}
		}

		protected virtual Material GetHoveredInteractableMaterial(IXRHoverInteractable interactable)
		{
			if (!base.hasSelection)
			{
				return this.m_InteractableHoverMeshMaterial;
			}
			return this.m_InteractableCantHoverMeshMaterial;
		}

		public override void GetValidTargets(List<IXRInteractable> targets)
		{
			targets.Clear();
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			IXRTargetFilter targetFilter = base.targetFilter;
			if (targetFilter != null && targetFilter.canProcess)
			{
				targetFilter.Process(this, this.unsortedValidTargets, targets);
				return;
			}
			SortingHelpers.SortByDistanceToInteractor(this, this.unsortedValidTargets, targets);
		}

		public override bool isHoverActive
		{
			get
			{
				return base.isHoverActive && this.m_SocketActive;
			}
		}

		public override bool isSelectActive
		{
			get
			{
				return base.isSelectActive && this.m_SocketActive;
			}
		}

		public override XRBaseInteractable.MovementType? selectedInteractableMovementTypeOverride
		{
			get
			{
				return new XRBaseInteractable.MovementType?(XRBaseInteractable.MovementType.Instantaneous);
			}
		}

		public override bool CanHover(IXRHoverInteractable interactable)
		{
			return base.CanHover(interactable) && this.isHoverRecycleAllowed;
		}

		private bool isHoverRecycleAllowed
		{
			get
			{
				return this.m_HoverSocketSnapping || this.m_LastRemoveTime < 0f || this.m_RecycleDelayTime <= 0f || Time.time > this.m_LastRemoveTime + this.m_RecycleDelayTime;
			}
		}

		public override bool CanSelect(IXRSelectInteractable interactable)
		{
			return base.CanSelect(interactable) && ((!base.hasSelection && !interactable.isSelected) || (base.IsSelecting(interactable) && interactable.interactorsSelecting.Count == 1));
		}

		protected virtual bool ShouldDrawHoverMesh(MeshFilter meshFilter, Renderer meshRenderer, Camera mainCamera)
		{
			int cullingMask = mainCamera.cullingMask;
			return meshFilter != null && (cullingMask & 1 << meshFilter.gameObject.layer) != 0 && meshRenderer != null && meshRenderer.enabled;
		}

		protected override void OnRegistered(InteractorRegisteredEventArgs args)
		{
			base.OnRegistered(args);
			args.manager.interactableRegistered += this.OnInteractableRegistered;
			args.manager.interactableUnregistered += this.OnInteractableUnregistered;
			this.m_TriggerContactMonitor.interactionManager = args.manager;
			this.m_TriggerContactMonitor.ResolveUnassociatedColliders();
			XRInteractionManager.RemoveAllUnregistered(args.manager, this.unsortedValidTargets);
		}

		protected override void OnUnregistered(InteractorUnregisteredEventArgs args)
		{
			base.OnUnregistered(args);
			args.manager.interactableRegistered -= this.OnInteractableRegistered;
			args.manager.interactableUnregistered -= this.OnInteractableUnregistered;
		}

		private void OnInteractableRegistered(InteractableRegisteredEventArgs args)
		{
			this.m_TriggerContactMonitor.ResolveUnassociatedColliders(args.interactableObject);
			if (this.m_TriggerContactMonitor.IsContacting(args.interactableObject) && !this.unsortedValidTargets.Contains(args.interactableObject))
			{
				this.unsortedValidTargets.Add(args.interactableObject);
			}
		}

		private void OnInteractableUnregistered(InteractableUnregisteredEventArgs args)
		{
			this.unsortedValidTargets.Remove(args.interactableObject);
		}

		private void OnContactAdded(IXRInteractable interactable)
		{
			if (!this.unsortedValidTargets.Contains(interactable))
			{
				this.unsortedValidTargets.Add(interactable);
			}
		}

		private void OnContactRemoved(IXRInteractable interactable)
		{
			this.unsortedValidTargets.Remove(interactable);
		}

		private void ResetCollidersAndValidTargets()
		{
			this.unsortedValidTargets.Clear();
			this.m_StayedColliders.Clear();
			this.m_TriggerContactMonitor.UpdateStayedColliders(this.m_StayedColliders);
		}

		protected virtual bool StartSocketSnapping(XRGrabInteractable grabInteractable)
		{
			this.m_SocketGrabTransformer.scaleOnlyMode = false;
			int count = this.m_InteractablesWithSocketTransformer.Count;
			if (count >= this.socketSnappingLimit || this.m_InteractablesWithSocketTransformer.Contains(grabInteractable))
			{
				return false;
			}
			if (count > 0 && this.ejectExistingSocketsWhenSnapping)
			{
				foreach (XRGrabInteractable xrgrabInteractable in this.m_InteractablesWithSocketTransformer.AsList())
				{
					xrgrabInteractable.RemoveSingleGrabTransformer(this.m_SocketGrabTransformer);
				}
				this.m_InteractablesWithSocketTransformer.Clear();
			}
			grabInteractable.AddSingleGrabTransformer(this.m_SocketGrabTransformer);
			this.m_InteractablesWithSocketTransformer.Add(grabInteractable);
			return true;
		}

		protected virtual bool EndSocketSnapping(XRGrabInteractable grabInteractable)
		{
			grabInteractable.RemoveSingleGrabTransformer(this.m_SocketGrabTransformer);
			return this.m_InteractablesWithSocketTransformer.Remove(grabInteractable);
		}

		private void SyncTransformerParams()
		{
			this.m_SocketGrabTransformer.canProcess = (this.m_SocketActive && base.isActiveAndEnabled);
			this.m_SocketGrabTransformer.socketInteractor = this;
			this.m_SocketGrabTransformer.socketSnappingRadius = this.socketSnappingRadius;
			this.m_SocketGrabTransformer.scaleMode = this.socketScaleMode;
			this.m_SocketGrabTransformer.fixedScale = this.fixedScale;
			this.m_SocketGrabTransformer.targetBoundsSize = this.targetBoundsSize;
		}

		[SerializeField]
		private bool m_ShowInteractableHoverMeshes = true;

		[SerializeField]
		private Material m_InteractableHoverMeshMaterial;

		[SerializeField]
		private Material m_InteractableCantHoverMeshMaterial;

		[SerializeField]
		private bool m_SocketActive = true;

		[SerializeField]
		private float m_InteractableHoverScale = 1f;

		[SerializeField]
		private float m_RecycleDelayTime = 1f;

		private float m_LastRemoveTime = -1f;

		[SerializeField]
		private bool m_HoverSocketSnapping;

		[SerializeField]
		private float m_SocketSnappingRadius = 0.1f;

		[SerializeField]
		private SocketScaleMode m_SocketScaleMode;

		[SerializeField]
		private Vector3 m_FixedScale = Vector3.one;

		[SerializeField]
		private Vector3 m_TargetBoundsSize = Vector3.one;

		private readonly HashSet<Collider> m_StayedColliders = new HashSet<Collider>();

		private readonly TriggerContactMonitor m_TriggerContactMonitor = new TriggerContactMonitor();

		private readonly Dictionary<IXRInteractable, ValueTuple<MeshFilter, Renderer>[]> m_MeshFilterCache = new Dictionary<IXRInteractable, ValueTuple<MeshFilter, Renderer>[]>();

		private static readonly List<MeshFilter> s_MeshFilters = new List<MeshFilter>();

		private static readonly WaitForFixedUpdate s_WaitForFixedUpdate = new WaitForFixedUpdate();

		private IEnumerator m_UpdateCollidersAfterTriggerStay;

		private readonly XRSocketGrabTransformer m_SocketGrabTransformer = new XRSocketGrabTransformer();

		private readonly HashSetList<XRGrabInteractable> m_InteractablesWithSocketTransformer = new HashSetList<XRGrabInteractable>(0);

		private struct ShaderPropertyLookup
		{
			public static readonly int surface = Shader.PropertyToID("_Surface");

			public static readonly int mode = Shader.PropertyToID("_Mode");

			public static readonly int srcBlend = Shader.PropertyToID("_SrcBlend");

			public static readonly int dstBlend = Shader.PropertyToID("_DstBlend");

			public static readonly int zWrite = Shader.PropertyToID("_ZWrite");

			public static readonly int baseColor = Shader.PropertyToID("_BaseColor");

			public static readonly int color = Shader.PropertyToID("_Color");
		}
	}
}
