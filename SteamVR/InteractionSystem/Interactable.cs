using System;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class Interactable : MonoBehaviour
	{
		public event Interactable.OnAttachedToHandDelegate onAttachedToHand;

		public event Interactable.OnDetachedFromHandDelegate onDetachedFromHand;

		public Hand hoveringHand
		{
			get
			{
				if (this.hoveringHands.Count > 0)
				{
					return this.hoveringHands[0];
				}
				return null;
			}
		}

		public bool isDestroying { get; protected set; }

		public bool isHovering { get; protected set; }

		public bool wasHovering { get; protected set; }

		private void Awake()
		{
			this.skeletonPoser = base.GetComponent<SteamVR_Skeleton_Poser>();
		}

		protected virtual void Start()
		{
			if (Interactable.highlightMat == null)
			{
				Interactable.highlightMat = (Material)Resources.Load("SteamVR_HoverHighlight_URP", typeof(Material));
			}
			if (Interactable.highlightMat == null)
			{
				Debug.LogError("<b>[SteamVR Interaction]</b> Hover Highlight Material is missing. Please create a material named 'SteamVR_HoverHighlight' and place it in a Resources folder", this);
			}
			if (this.skeletonPoser != null && this.useHandObjectAttachmentPoint)
			{
				this.useHandObjectAttachmentPoint = false;
			}
		}

		protected virtual bool ShouldIgnoreHighlight(Component component)
		{
			return this.ShouldIgnore(component.gameObject);
		}

		protected virtual bool ShouldIgnore(GameObject check)
		{
			for (int i = 0; i < this.hideHighlight.Length; i++)
			{
				if (check == this.hideHighlight[i])
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void CreateHighlightRenderers()
		{
			this.existingSkinnedRenderers = base.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			this.highlightHolder = new GameObject("Highlighter");
			this.highlightSkinnedRenderers = new SkinnedMeshRenderer[this.existingSkinnedRenderers.Length];
			for (int i = 0; i < this.existingSkinnedRenderers.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = this.existingSkinnedRenderers[i];
				if (!this.ShouldIgnoreHighlight(skinnedMeshRenderer))
				{
					SkinnedMeshRenderer skinnedMeshRenderer2 = new GameObject("SkinnedHolder")
					{
						transform = 
						{
							parent = this.highlightHolder.transform
						}
					}.AddComponent<SkinnedMeshRenderer>();
					Material[] array = new Material[skinnedMeshRenderer.sharedMaterials.Length];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = Interactable.highlightMat;
					}
					skinnedMeshRenderer2.sharedMaterials = array;
					skinnedMeshRenderer2.sharedMesh = skinnedMeshRenderer.sharedMesh;
					skinnedMeshRenderer2.rootBone = skinnedMeshRenderer.rootBone;
					skinnedMeshRenderer2.updateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
					skinnedMeshRenderer2.bones = skinnedMeshRenderer.bones;
					this.highlightSkinnedRenderers[i] = skinnedMeshRenderer2;
				}
			}
			MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>(true);
			this.existingRenderers = new MeshRenderer[componentsInChildren.Length];
			this.highlightRenderers = new MeshRenderer[componentsInChildren.Length];
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				MeshFilter meshFilter = componentsInChildren[k];
				MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
				if (!(meshFilter == null) && !(component == null) && !this.ShouldIgnoreHighlight(meshFilter))
				{
					GameObject gameObject = new GameObject("FilterHolder");
					gameObject.transform.parent = this.highlightHolder.transform;
					gameObject.AddComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
					MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
					Material[] array2 = new Material[component.sharedMaterials.Length];
					for (int l = 0; l < array2.Length; l++)
					{
						array2[l] = Interactable.highlightMat;
					}
					meshRenderer.sharedMaterials = array2;
					this.highlightRenderers[k] = meshRenderer;
					this.existingRenderers[k] = component;
				}
			}
		}

		protected virtual void UpdateHighlightRenderers()
		{
			if (this.highlightHolder == null)
			{
				return;
			}
			for (int i = 0; i < this.existingSkinnedRenderers.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = this.existingSkinnedRenderers[i];
				SkinnedMeshRenderer skinnedMeshRenderer2 = this.highlightSkinnedRenderers[i];
				if (skinnedMeshRenderer != null && skinnedMeshRenderer2 != null && !this.attachedToHand)
				{
					skinnedMeshRenderer2.transform.position = skinnedMeshRenderer.transform.position;
					skinnedMeshRenderer2.transform.rotation = skinnedMeshRenderer.transform.rotation;
					skinnedMeshRenderer2.transform.localScale = skinnedMeshRenderer.transform.lossyScale;
					skinnedMeshRenderer2.localBounds = skinnedMeshRenderer.localBounds;
					skinnedMeshRenderer2.enabled = (this.isHovering && skinnedMeshRenderer.enabled && skinnedMeshRenderer.gameObject.activeInHierarchy);
					int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
					for (int j = 0; j < blendShapeCount; j++)
					{
						skinnedMeshRenderer2.SetBlendShapeWeight(j, skinnedMeshRenderer.GetBlendShapeWeight(j));
					}
				}
				else if (skinnedMeshRenderer2 != null)
				{
					skinnedMeshRenderer2.enabled = false;
				}
			}
			for (int k = 0; k < this.highlightRenderers.Length; k++)
			{
				MeshRenderer meshRenderer = this.existingRenderers[k];
				MeshRenderer meshRenderer2 = this.highlightRenderers[k];
				if (meshRenderer != null && meshRenderer2 != null && !this.attachedToHand)
				{
					meshRenderer2.transform.position = meshRenderer.transform.position;
					meshRenderer2.transform.rotation = meshRenderer.transform.rotation;
					meshRenderer2.transform.localScale = meshRenderer.transform.lossyScale;
					meshRenderer2.enabled = (this.isHovering && meshRenderer.enabled && meshRenderer.gameObject.activeInHierarchy);
				}
				else if (meshRenderer2 != null)
				{
					meshRenderer2.enabled = false;
				}
			}
		}

		protected virtual void OnHandHoverBegin(Hand hand)
		{
			this.wasHovering = this.isHovering;
			this.isHovering = true;
			this.hoveringHands.Add(hand);
			if (this.highlightOnHover && !this.wasHovering)
			{
				this.CreateHighlightRenderers();
				this.UpdateHighlightRenderers();
			}
		}

		protected virtual void OnHandHoverEnd(Hand hand)
		{
			this.wasHovering = this.isHovering;
			this.hoveringHands.Remove(hand);
			if (this.hoveringHands.Count == 0)
			{
				this.isHovering = false;
				if (this.highlightOnHover && this.highlightHolder != null)
				{
					Object.Destroy(this.highlightHolder);
				}
			}
		}

		protected virtual void Update()
		{
			if (this.highlightOnHover)
			{
				this.UpdateHighlightRenderers();
				if (!this.isHovering && this.highlightHolder != null)
				{
					Object.Destroy(this.highlightHolder);
				}
			}
		}

		protected virtual void OnAttachedToHand(Hand hand)
		{
			if (this.activateActionSetOnAttach != null)
			{
				this.activateActionSetOnAttach.Activate(hand.handType, 0, false);
			}
			if (this.onAttachedToHand != null)
			{
				this.onAttachedToHand(hand);
			}
			if (this.skeletonPoser != null && hand.skeleton != null)
			{
				hand.skeleton.BlendToPoser(this.skeletonPoser, this.blendToPoseTime);
			}
			this.attachedToHand = hand;
		}

		protected virtual void OnDetachedFromHand(Hand hand)
		{
			if (this.activateActionSetOnAttach != null && (hand.otherHand == null || hand.otherHand.currentAttachedObjectInfo == null || (hand.otherHand.currentAttachedObjectInfo.Value.interactable != null && hand.otherHand.currentAttachedObjectInfo.Value.interactable.activateActionSetOnAttach != this.activateActionSetOnAttach)))
			{
				this.activateActionSetOnAttach.Deactivate(hand.handType);
			}
			if (this.onDetachedFromHand != null)
			{
				this.onDetachedFromHand(hand);
			}
			if (this.skeletonPoser != null && hand.skeleton != null)
			{
				hand.skeleton.BlendToSkeleton(this.releasePoseBlendTime);
			}
			this.attachedToHand = null;
		}

		protected virtual void OnDestroy()
		{
			this.isDestroying = true;
			if (this.attachedToHand != null)
			{
				this.attachedToHand.DetachObject(base.gameObject, false);
				this.attachedToHand.skeleton.BlendToSkeleton(0.1f);
			}
			if (this.highlightHolder != null)
			{
				Object.Destroy(this.highlightHolder);
			}
		}

		protected virtual void OnDisable()
		{
			this.isDestroying = true;
			if (this.attachedToHand != null)
			{
				this.attachedToHand.ForceHoverUnlock();
			}
			if (this.highlightHolder != null)
			{
				Object.Destroy(this.highlightHolder);
			}
		}

		[Tooltip("Activates an action set on attach and deactivates on detach")]
		public SteamVR_ActionSet activateActionSetOnAttach;

		[Tooltip("Hide the whole hand on attachment and show on detach")]
		public bool hideHandOnAttach = true;

		[Tooltip("Hide the skeleton part of the hand on attachment and show on detach")]
		public bool hideSkeletonOnAttach;

		[Tooltip("Hide the controller part of the hand on attachment and show on detach")]
		public bool hideControllerOnAttach;

		[Tooltip("The integer in the animator to trigger on pickup. 0 for none")]
		public int handAnimationOnPickup;

		[Tooltip("The range of motion to set on the skeleton. None for no change.")]
		public SkeletalMotionRangeChange setRangeOfMotionOnPickup = SkeletalMotionRangeChange.None;

		[Tooltip("Specify whether you want to snap to the hand's object attachment point, or just the raw hand")]
		public bool useHandObjectAttachmentPoint = true;

		public bool attachEaseIn;

		[HideInInspector]
		public AnimationCurve snapAttachEaseInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		public float snapAttachEaseInTime = 0.15f;

		public bool snapAttachEaseInCompleted;

		[HideInInspector]
		public SteamVR_Skeleton_Poser skeletonPoser;

		[Tooltip("Should the rendered hand lock on to and follow the object")]
		public bool handFollowTransform = true;

		[Tooltip("Set whether or not you want this interactible to highlight when hovering over it")]
		public bool highlightOnHover = true;

		protected MeshRenderer[] highlightRenderers;

		protected MeshRenderer[] existingRenderers;

		protected GameObject highlightHolder;

		protected SkinnedMeshRenderer[] highlightSkinnedRenderers;

		protected SkinnedMeshRenderer[] existingSkinnedRenderers;

		protected static Material highlightMat;

		[Tooltip("An array of child gameObjects to not render a highlight for. Things like transparent parts, vfx, etc.")]
		public GameObject[] hideHighlight;

		[Tooltip("Higher is better")]
		public int hoverPriority;

		[NonSerialized]
		public Hand attachedToHand;

		[NonSerialized]
		public List<Hand> hoveringHands = new List<Hand>();

		protected float blendToPoseTime = 0.1f;

		protected float releasePoseBlendTime = 0.2f;

		public delegate void OnAttachedToHandDelegate(Hand hand);

		public delegate void OnDetachedFromHandDelegate(Hand hand);
	}
}
