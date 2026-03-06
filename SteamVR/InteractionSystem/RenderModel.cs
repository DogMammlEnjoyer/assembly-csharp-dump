using System;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	public class RenderModel : MonoBehaviour
	{
		public event Action onControllerLoaded;

		protected void Awake()
		{
			this.renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction(new UnityAction<SteamVR_RenderModel, bool>(this.OnRenderModelLoaded));
			this.InitializeHand();
			this.InitializeController();
		}

		protected void InitializeHand()
		{
			if (this.handPrefab != null)
			{
				this.handInstance = Object.Instantiate<GameObject>(this.handPrefab);
				this.handInstance.transform.parent = base.transform;
				this.handInstance.transform.localPosition = Vector3.zero;
				this.handInstance.transform.localRotation = Quaternion.identity;
				this.handInstance.transform.localScale = this.handPrefab.transform.localScale;
				this.handSkeleton = this.handInstance.GetComponent<SteamVR_Behaviour_Skeleton>();
				this.handSkeleton.origin = Player.instance.trackingOriginTransform;
				this.handSkeleton.updatePose = false;
				this.handSkeleton.skeletonAction.onActiveChange += this.OnSkeletonActiveChange;
				this.handRenderers = this.handInstance.GetComponentsInChildren<Renderer>();
				if (!this.displayHandByDefault)
				{
					this.SetHandVisibility(false, false);
				}
				this.handAnimator = this.handInstance.GetComponentInChildren<Animator>();
				if (!this.handSkeleton.skeletonAction.activeBinding && this.handSkeleton.fallbackPoser == null)
				{
					Debug.LogWarning("Skeleton action: " + this.handSkeleton.skeletonAction.GetPath() + " is not bound. Your controller may not support SteamVR Skeleton Input. Please add a fallback skeleton poser to your skeleton if you want hands to be visible");
					this.DestroyHand();
				}
			}
		}

		protected void InitializeController()
		{
			if (this.controllerPrefab != null)
			{
				this.controllerInstance = Object.Instantiate<GameObject>(this.controllerPrefab);
				this.controllerInstance.transform.parent = base.transform;
				this.controllerInstance.transform.localPosition = Vector3.zero;
				this.controllerInstance.transform.localRotation = Quaternion.identity;
				this.controllerInstance.transform.localScale = this.controllerPrefab.transform.localScale;
				this.controllerRenderModel = this.controllerInstance.GetComponent<SteamVR_RenderModel>();
			}
		}

		protected virtual void DestroyHand()
		{
			if (this.handSkeleton != null)
			{
				this.handSkeleton.skeletonAction.onActiveChange -= this.OnSkeletonActiveChange;
			}
			if (this.handInstance != null)
			{
				Object.Destroy(this.handInstance);
				this.handRenderers = null;
				this.handInstance = null;
				this.handSkeleton = null;
				this.handAnimator = null;
			}
		}

		protected virtual void OnSkeletonActiveChange(SteamVR_Action_Skeleton changedAction, bool newState)
		{
			if (newState)
			{
				this.InitializeHand();
				return;
			}
			this.DestroyHand();
		}

		protected void OnEnable()
		{
			this.renderModelLoadedAction.enabled = true;
		}

		protected void OnDisable()
		{
			this.renderModelLoadedAction.enabled = false;
		}

		protected void OnDestroy()
		{
			this.DestroyHand();
		}

		public SteamVR_Behaviour_Skeleton GetSkeleton()
		{
			return this.handSkeleton;
		}

		public virtual void SetInputSource(SteamVR_Input_Sources newInputSource)
		{
			this.inputSource = newInputSource;
			if (this.controllerRenderModel != null)
			{
				this.controllerRenderModel.SetInputSource(this.inputSource);
			}
		}

		public virtual void OnHandInitialized(int deviceIndex)
		{
			this.controllerRenderModel.SetInputSource(this.inputSource);
			this.controllerRenderModel.SetDeviceIndex(deviceIndex);
		}

		public void MatchHandToTransform(Transform match)
		{
			if (this.handInstance != null)
			{
				this.handInstance.transform.position = match.transform.position;
				this.handInstance.transform.rotation = match.transform.rotation;
			}
		}

		public void SetHandPosition(Vector3 newPosition)
		{
			if (this.handInstance != null)
			{
				this.handInstance.transform.position = newPosition;
			}
		}

		public void SetHandRotation(Quaternion newRotation)
		{
			if (this.handInstance != null)
			{
				this.handInstance.transform.rotation = newRotation;
			}
		}

		public Vector3 GetHandPosition()
		{
			if (this.handInstance != null)
			{
				return this.handInstance.transform.position;
			}
			return Vector3.zero;
		}

		public Quaternion GetHandRotation()
		{
			if (this.handInstance != null)
			{
				return this.handInstance.transform.rotation;
			}
			return Quaternion.identity;
		}

		private void OnRenderModelLoaded(SteamVR_RenderModel loadedRenderModel, bool success)
		{
			if (this.controllerRenderModel == loadedRenderModel)
			{
				this.controllerRenderers = this.controllerInstance.GetComponentsInChildren<Renderer>();
				if (!this.displayControllerByDefault)
				{
					this.SetControllerVisibility(false, false);
				}
				if (this.delayedSetMaterial != null)
				{
					this.SetControllerMaterial(this.delayedSetMaterial);
				}
				if (this.onControllerLoaded != null)
				{
					this.onControllerLoaded();
				}
			}
		}

		public void SetVisibility(bool state, bool overrideDefault = false)
		{
			if (!state || this.displayControllerByDefault || overrideDefault)
			{
				this.SetControllerVisibility(state, false);
			}
			if (!state || this.displayHandByDefault || overrideDefault)
			{
				this.SetHandVisibility(state, false);
			}
		}

		public void Show(bool overrideDefault = false)
		{
			this.SetVisibility(true, overrideDefault);
		}

		public void Hide()
		{
			this.SetVisibility(false, false);
		}

		public virtual void SetMaterial(Material material)
		{
			this.SetControllerMaterial(material);
			this.SetHandMaterial(material);
		}

		public void SetControllerMaterial(Material material)
		{
			if (this.controllerRenderers == null)
			{
				this.delayedSetMaterial = material;
				return;
			}
			for (int i = 0; i < this.controllerRenderers.Length; i++)
			{
				this.controllerRenderers[i].material = material;
			}
		}

		public void SetHandMaterial(Material material)
		{
			for (int i = 0; i < this.handRenderers.Length; i++)
			{
				this.handRenderers[i].material = material;
			}
		}

		public void SetControllerVisibility(bool state, bool permanent = false)
		{
			if (permanent)
			{
				this.displayControllerByDefault = state;
			}
			if (this.controllerRenderers == null)
			{
				return;
			}
			for (int i = 0; i < this.controllerRenderers.Length; i++)
			{
				this.controllerRenderers[i].enabled = state;
			}
		}

		public void SetHandVisibility(bool state, bool permanent = false)
		{
			if (permanent)
			{
				this.displayHandByDefault = state;
			}
			if (this.handRenderers == null)
			{
				return;
			}
			for (int i = 0; i < this.handRenderers.Length; i++)
			{
				this.handRenderers[i].enabled = state;
			}
		}

		public bool IsHandVisibile()
		{
			if (this.handRenderers == null)
			{
				return false;
			}
			for (int i = 0; i < this.handRenderers.Length; i++)
			{
				if (this.handRenderers[i].enabled)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsControllerVisibile()
		{
			if (this.controllerRenderers == null)
			{
				return false;
			}
			for (int i = 0; i < this.controllerRenderers.Length; i++)
			{
				if (this.controllerRenderers[i].enabled)
				{
					return true;
				}
			}
			return false;
		}

		public Transform GetBone(int boneIndex)
		{
			if (this.handSkeleton != null)
			{
				return this.handSkeleton.GetBone(boneIndex);
			}
			return null;
		}

		public Vector3 GetBonePosition(int boneIndex, bool local = false)
		{
			if (this.handSkeleton != null)
			{
				return this.handSkeleton.GetBonePosition(boneIndex, local);
			}
			return Vector3.zero;
		}

		public Vector3 GetControllerPosition(string componentName = null)
		{
			if (this.controllerRenderModel != null)
			{
				return this.controllerRenderModel.GetComponentTransform(componentName).position;
			}
			return Vector3.zero;
		}

		public Quaternion GetBoneRotation(int boneIndex, bool local = false)
		{
			if (this.handSkeleton != null)
			{
				return this.handSkeleton.GetBoneRotation(boneIndex, local);
			}
			return Quaternion.identity;
		}

		public void SetSkeletonRangeOfMotion(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds = 0.1f)
		{
			if (this.handSkeleton != null)
			{
				this.handSkeleton.SetRangeOfMotion(newRangeOfMotion, blendOverSeconds);
			}
		}

		public EVRSkeletalMotionRange GetSkeletonRangeOfMotion
		{
			get
			{
				if (this.handSkeleton != null)
				{
					return this.handSkeleton.rangeOfMotion;
				}
				return EVRSkeletalMotionRange.WithController;
			}
		}

		public void SetTemporarySkeletonRangeOfMotion(SkeletalMotionRangeChange temporaryRangeOfMotionChange, float blendOverSeconds = 0.1f)
		{
			if (this.handSkeleton != null)
			{
				this.handSkeleton.SetTemporaryRangeOfMotion((EVRSkeletalMotionRange)temporaryRangeOfMotionChange, blendOverSeconds);
			}
		}

		public void ResetTemporarySkeletonRangeOfMotion(float blendOverSeconds = 0.1f)
		{
			if (this.handSkeleton != null)
			{
				this.handSkeleton.ResetTemporaryRangeOfMotion(blendOverSeconds);
			}
		}

		public void SetAnimationState(int stateValue)
		{
			if (this.handSkeleton != null)
			{
				if (!this.handSkeleton.isBlending)
				{
					this.handSkeleton.BlendToAnimation(0.1f);
				}
				if (this.CheckAnimatorInit())
				{
					this.handAnimator.SetInteger(this.handAnimatorStateId, stateValue);
				}
			}
		}

		public void StopAnimation()
		{
			if (this.handSkeleton != null)
			{
				if (!this.handSkeleton.isBlending)
				{
					this.handSkeleton.BlendToSkeleton(0.1f);
				}
				if (this.CheckAnimatorInit())
				{
					this.handAnimator.SetInteger(this.handAnimatorStateId, 0);
				}
			}
		}

		private bool CheckAnimatorInit()
		{
			if (this.handAnimatorStateId == -1 && this.handAnimator != null && this.handAnimator.gameObject.activeInHierarchy && this.handAnimator.isInitialized)
			{
				AnimatorControllerParameter[] parameters = this.handAnimator.parameters;
				for (int i = 0; i < parameters.Length; i++)
				{
					if (string.Equals(parameters[i].name, this.animatorParameterStateName, StringComparison.CurrentCultureIgnoreCase))
					{
						this.handAnimatorStateId = parameters[i].nameHash;
					}
				}
			}
			return this.handAnimatorStateId != -1 && this.handAnimator != null && this.handAnimator.isInitialized;
		}

		public GameObject handPrefab;

		protected GameObject handInstance;

		protected Renderer[] handRenderers;

		public bool displayHandByDefault = true;

		protected SteamVR_Behaviour_Skeleton handSkeleton;

		protected Animator handAnimator;

		protected string animatorParameterStateName = "AnimationState";

		protected int handAnimatorStateId = -1;

		[Space]
		public GameObject controllerPrefab;

		protected GameObject controllerInstance;

		protected Renderer[] controllerRenderers;

		protected SteamVR_RenderModel controllerRenderModel;

		public bool displayControllerByDefault = true;

		protected Material delayedSetMaterial;

		protected SteamVR_Events.Action renderModelLoadedAction;

		protected SteamVR_Input_Sources inputSource;
	}
}
