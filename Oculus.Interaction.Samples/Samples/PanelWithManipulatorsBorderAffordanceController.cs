using System;
using System.Collections.Generic;
using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class PanelWithManipulatorsBorderAffordanceController : MonoBehaviour
	{
		private static Vector3? _projectToRoundedBoxEdge(Vector3 worldSpacePoint, Transform targetTransform, Transform boneTransform, float arcRadius)
		{
			Vector3 vector = targetTransform.InverseTransformPointUnscaled(worldSpacePoint);
			vector.z = 0f;
			Vector3 vector2 = targetTransform.InverseTransformPointUnscaled(boneTransform.position);
			float num = Mathf.Sign(vector.x) * Mathf.Min(Mathf.Abs(vector2.x), Mathf.Abs(vector.x));
			float num2 = Mathf.Sign(vector.y) * Mathf.Min(Mathf.Abs(vector2.y), Mathf.Abs(vector.y));
			bool flag = Mathf.Abs(Mathf.Abs(num) - Mathf.Abs(vector2.x)) < Mathf.Epsilon;
			bool flag2 = Mathf.Abs(Mathf.Abs(num2) - Mathf.Abs(vector2.y)) < Mathf.Epsilon;
			if (flag || flag2)
			{
				Vector3 vector3 = new Vector3(num, num2, 0f);
				Vector3 normalized = (vector - vector3).normalized;
				return new Vector3?(targetTransform.TransformPointUnscaled(vector3 + normalized * arcRadius));
			}
			return null;
		}

		private void Start()
		{
			this._materialPropertyBlock = new MaterialPropertyBlock();
			this._fadePoints = new Vector4[]
			{
				Vector4.zero,
				Vector4.zero,
				Vector4.zero,
				Vector4.zero
			};
			this._points = new Dictionary<int, PanelWithManipulatorsBorderAffordanceController.FadePoint>();
			this._affordancesInUse = new HashSet<int>();
			this._deletePointKeys = new List<int>();
			this._grabInteractable.WhenStateChanged += this.HandleInteractableStateChanged;
			this._handGrabInteractable.WhenStateChanged += this.HandleInteractableStateChanged;
			if (this._rayInteractable != null)
			{
				this._rayInteractable.WhenStateChanged += this.HandleInteractableStateChanged;
			}
			this._railOpacityAnimator.SetInteger("state", 0);
			this._stateSignaler.WhenStateChanged += this.HandleStateChanged;
			this._grabbale.WhenPointerEventRaised += this.HandlePointerEvent;
		}

		private void OnDestroy()
		{
			this._grabInteractable.WhenStateChanged -= this.HandleInteractableStateChanged;
			this._handGrabInteractable.WhenStateChanged -= this.HandleInteractableStateChanged;
			if (this._rayInteractable != null)
			{
				this._rayInteractable.WhenStateChanged -= this.HandleInteractableStateChanged;
			}
			this._stateSignaler.WhenStateChanged -= this.HandleStateChanged;
			this._grabbale.WhenPointerEventRaised -= this.HandlePointerEvent;
		}

		private void CreateFadePoint(int eventIdentifier)
		{
			int num = -1;
			for (int i = 0; i < this._affordances.Length; i++)
			{
				if (!this._affordancesInUse.Contains(i))
				{
					num = i;
					this._affordancesInUse.Add(i);
					break;
				}
			}
			if (num >= 0)
			{
				this._points.Add(eventIdentifier, new PanelWithManipulatorsBorderAffordanceController.FadePoint(num));
			}
		}

		private void HandlePointerEvent(PointerEvent evt)
		{
			switch (evt.Type)
			{
			case PointerEventType.Hover:
			case PointerEventType.Unselect:
			case PointerEventType.Move:
			{
				Vector3? vector = PanelWithManipulatorsBorderAffordanceController._projectToRoundedBoxEdge(evt.Pose.position, this._grabbale.Transform, this._boneTransform, this._cornerArcRadius);
				Vector3 position = evt.Pose.position;
				if (vector != null)
				{
					position = vector.Value;
				}
				if (!this._points.ContainsKey(evt.Identifier))
				{
					this.CreateFadePoint(evt.Identifier);
					return;
				}
				this._points[evt.Identifier].removeFlag = false;
				this._affordances[this._points[evt.Identifier].affordanceIndex].LastKnownPositionParentSpace = this._grabbale.Transform.InverseTransformPoint(position);
				return;
			}
			case PointerEventType.Unhover:
			case PointerEventType.Cancel:
				if (this._points.ContainsKey(evt.Identifier))
				{
					this._points[evt.Identifier].removeFlag = true;
				}
				break;
			case PointerEventType.Select:
				break;
			default:
				return;
			}
		}

		private void SetRailAnimatorState()
		{
			PanelWithManipulatorsBorderAffordanceController.RailState value;
			if (this._panelHoverState != null)
			{
				if (this._panelHoverState.Hovered)
				{
					value = PanelWithManipulatorsBorderAffordanceController.RailState.Hover;
				}
				else
				{
					value = PanelWithManipulatorsBorderAffordanceController.RailState.Hidden;
				}
			}
			else
			{
				value = PanelWithManipulatorsBorderAffordanceController.RailState.Hover;
			}
			if (this._grabbale.SelectingPoints.Count > 0)
			{
				value = PanelWithManipulatorsBorderAffordanceController.RailState.Selected;
			}
			bool flag = !(this._rayInteractable != null) || this._rayInteractable.State == InteractableState.Disabled;
			if (this._grabInteractable.State == InteractableState.Disabled && this._handGrabInteractable.State == InteractableState.Disabled && flag)
			{
				value = PanelWithManipulatorsBorderAffordanceController.RailState.Hidden;
			}
			this._railOpacityAnimator.SetInteger("state", (int)value);
		}

		private void UpdateFadePoints()
		{
			if (this._points.Count > 0)
			{
				this._deletePointKeys.Clear();
				foreach (KeyValuePair<int, PanelWithManipulatorsBorderAffordanceController.FadePoint> keyValuePair in this._points)
				{
					PanelWithManipulatorsBorderAffordanceController.FadePoint value = keyValuePair.Value;
					PanelWithManipulatorsBorderAffordanceController.Affordance affordance = this._affordances[value.affordanceIndex];
					affordance.AnimationState = (value.removeFlag ? PanelWithManipulatorsBorderAffordanceController.AffordanceState.Hidden : PanelWithManipulatorsBorderAffordanceController.AffordanceState.Visible);
					if (value.removeFlag && affordance.Opacity < 0.05f)
					{
						this._deletePointKeys.Add(keyValuePair.Key);
					}
				}
				foreach (int key in this._deletePointKeys)
				{
					PanelWithManipulatorsBorderAffordanceController.FadePoint fadePoint;
					this._points.Remove(key, out fadePoint);
					this._affordancesInUse.Remove(fadePoint.affordanceIndex);
				}
			}
		}

		private void UpdateMaterialProperties()
		{
			this._materialPropertyBlock.SetFloat("_OpacityMultiplier", this._railOpacityTransform.localPosition.x);
			this._materialPropertyBlock.SetFloat("_SelectedOpacityParam", this._railOpacityTransform.localPosition.y);
			int num = 0;
			foreach (PanelWithManipulatorsBorderAffordanceController.Affordance affordance in this._affordances)
			{
				if (num >= this._fadePoints.Length)
				{
					break;
				}
				Vector3 vector = this._grabbale.Transform.TransformPoint(affordance.LastKnownPositionParentSpace);
				affordance.Geometry.position = vector;
				this._fadePoints[num].x = vector.x;
				this._fadePoints[num].y = vector.y;
				this._fadePoints[num].z = vector.z;
				this._fadePoints[num].w = affordance.Opacity;
				num++;
			}
			this._materialPropertyBlock.SetVectorArray("_WorldSpaceFadePoints", this._fadePoints);
			this._materialPropertyBlock.SetInteger("_UsedPointCount", num);
			this._railRenderer.SetPropertyBlock(this._materialPropertyBlock);
		}

		private void Update()
		{
			this.SetRailAnimatorState();
			this.UpdateFadePoints();
			this.UpdateMaterialProperties();
		}

		private void HandleInteractableStateChanged(InteractableStateChangeArgs args)
		{
			if (args.NewState == InteractableState.Select)
			{
				this._stateSignaler.CurrentState = PanelWithManipulatorsStateSignaler.State.Selected;
				return;
			}
			if (args.PreviousState == InteractableState.Select)
			{
				this._stateSignaler.CurrentState = PanelWithManipulatorsStateSignaler.State.Default;
			}
		}

		private void HandleStateChanged(PanelWithManipulatorsStateSignaler.State state)
		{
			if (state != PanelWithManipulatorsStateSignaler.State.Default)
			{
				bool flag = !(this._rayInteractable != null) || this._rayInteractable.State != InteractableState.Select;
				if (this._grabInteractable.State != InteractableState.Select && this._handGrabInteractable.State != InteractableState.Select && flag)
				{
					this._grabInteractable.enabled = false;
					this._handGrabInteractable.enabled = false;
					if (this._rayInteractable != null)
					{
						this._rayInteractable.enabled = false;
						return;
					}
				}
			}
			else
			{
				this._grabInteractable.enabled = true;
				this._handGrabInteractable.enabled = true;
				if (this._rayInteractable != null)
				{
					this._rayInteractable.enabled = true;
				}
			}
		}

		[Header("Interactables")]
		[SerializeField]
		[Tooltip("The grab interactable for the slate itself (as opposed to the surrounding affordances)")]
		private GrabInteractable _grabInteractable;

		[SerializeField]
		[Tooltip("The hand grab interactable for the slate itself (as opposed to the surrounding affordances)")]
		private HandGrabInteractable _handGrabInteractable;

		[SerializeField]
		[Optional]
		[Tooltip("The hand grab interactable for the slate itself (as opposed to the surrounding affordances)")]
		private RayInteractable _rayInteractable;

		[Header("Panel Signals")]
		[SerializeField]
		[Tooltip("The state signaler for the SlateWithManipulators prefab")]
		private PanelWithManipulatorsStateSignaler _stateSignaler;

		[SerializeField]
		[Optional]
		[Tooltip("Holds the panel hover state")]
		private PanelHoverState _panelHoverState;

		[SerializeField]
		[Tooltip("The grabbable associated with the slate itself (i.e., the grabbable with One- and TwoGrabFreeTransformers")]
		private Grabbable _grabbale;

		[Space(10f)]
		[SerializeField]
		[Tooltip("The transform of one of the bones of the rail affordance (used in calculating capsule placement)")]
		private Transform _boneTransform;

		[SerializeField]
		[Tooltip("The radius of the arcs at the corners of the rail affordance (used in calculating capsule placement)")]
		private float _cornerArcRadius;

		[Space(10f)]
		[SerializeField]
		[Tooltip("The animator controlling the overall opacity of the rail affordance (note that this is independent of the localized opacities associated with the capsule affordances)")]
		private Animator _railOpacityAnimator;

		[SerializeField]
		[Tooltip("The transform being controlled by the rail opacity animator")]
		private Transform _railOpacityTransform;

		[SerializeField]
		[Tooltip("The capsule affordances")]
		private PanelWithManipulatorsBorderAffordanceController.Affordance[] _affordances;

		[SerializeField]
		[Tooltip("The renderer controlling shading for the rail affordance")]
		private SkinnedMeshRenderer _railRenderer;

		private Vector4[] _fadePoints;

		private MaterialPropertyBlock _materialPropertyBlock;

		private Dictionary<int, PanelWithManipulatorsBorderAffordanceController.FadePoint> _points;

		private HashSet<int> _affordancesInUse;

		private List<int> _deletePointKeys;

		private enum RailState
		{
			Hidden,
			Hover,
			Selected
		}

		private enum AffordanceState
		{
			Hidden,
			Visible
		}

		[Serializable]
		private class Affordance
		{
			public PanelWithManipulatorsBorderAffordanceController.AffordanceState AnimationState
			{
				get
				{
					return this._animationState;
				}
				set
				{
					if (value == this._animationState)
					{
						return;
					}
					this._animationState = value;
					Animator[] animators = this._animators;
					for (int i = 0; i < animators.Length; i++)
					{
						animators[i].SetInteger("state", (int)this._animationState);
					}
				}
			}

			public Vector3 LastKnownPositionParentSpace
			{
				get
				{
					return this._lastKnownPositionParentSpace;
				}
				set
				{
					this._lastKnownPositionParentSpace = value;
				}
			}

			public Transform Geometry
			{
				get
				{
					return this._geometry;
				}
			}

			public float Opacity
			{
				get
				{
					return Mathf.Abs(this._opacityTransform.localPosition.x);
				}
			}

			[SerializeField]
			[Tooltip("The parent transform of the geometry (i.e., visuals) which should be moved to place the capsule affordance")]
			private Transform _geometry;

			[SerializeField]
			[Tooltip("Then transform controlled by an animation whose X axis magnitude will be used to control the affordance's opacity")]
			private Transform _opacityTransform;

			[SerializeField]
			[Tooltip("The animators (canonically geometry and opacity) whose 'state' variables should be controlled by this affordance")]
			private Animator[] _animators;

			private PanelWithManipulatorsBorderAffordanceController.AffordanceState _animationState;

			private Vector3 _lastKnownPositionParentSpace;
		}

		private class FadePoint
		{
			public FadePoint(int index)
			{
				this.affordanceIndex = index;
				this.removeFlag = false;
			}

			public int affordanceIndex;

			public bool removeFlag;
		}
	}
}
