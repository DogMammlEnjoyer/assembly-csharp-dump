using System;
using System.Collections.Generic;
using Oculus.Interaction.UnityCanvas;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class InteractableObjectLabel : MonoBehaviour
	{
		private void Start()
		{
			this._targetAlpha = 0f;
			this.BeginStart(ref this._started, null);
			if (this.playerHead == null)
			{
				this.playerHead = Camera.main.transform;
			}
			this.CreateTextureQuad();
			this._startScale = this._quadTransform.localScale;
			this._block = new MaterialPropertyBlock();
			this.EndStart(ref this._started);
		}

		private void CreateTextureQuad()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "CanvasTexture";
			this._quadTransform = gameObject.transform;
			this._quadTransform.parent = base.transform;
			this._quadTransform.localScale = new Vector3
			{
				x = this.canvasTransform.sizeDelta.x * this.canvasTransform.localScale.x,
				y = this.canvasTransform.sizeDelta.y * this.canvasTransform.localScale.y,
				z = 1f
			};
			gameObject.AddComponent<MeshFilter>().sharedMesh = this.quadMesh;
			this._quadRenderer = gameObject.AddComponent<MeshRenderer>();
			this._quadRenderer.sharedMaterial = this.quadMaterial;
		}

		private void OnEnable()
		{
			this.interactableGroup.WhenStateChanged += this.InteractableStateChange;
		}

		private void OnDisable()
		{
			this.interactableGroup.WhenStateChanged -= this.InteractableStateChange;
		}

		private void SetTargetAlpha()
		{
			switch (this._currentState)
			{
			case InteractableObjectLabel.LabelState.Hidden:
				this._targetAlpha = 0f;
				return;
			case InteractableObjectLabel.LabelState.FocusCheck:
				this._targetAlpha = 0f;
				return;
			case InteractableObjectLabel.LabelState.Focused:
				this._targetAlpha = 1f;
				return;
			case InteractableObjectLabel.LabelState.HideCheck:
				this._targetAlpha = 1f;
				return;
			case InteractableObjectLabel.LabelState.Used:
				this._targetAlpha = 0f;
				return;
			default:
				return;
			}
		}

		private float MaximizedDotView()
		{
			if (this.viewTargets == null)
			{
				return 0f;
			}
			float num = -1f;
			foreach (Transform transform in this.viewTargets)
			{
				Vector3 rhs = Vector3.Normalize(transform.position - this.playerHead.position);
				float b = Vector3.Dot(this.playerHead.forward, rhs);
				num = Mathf.Max(num, b);
			}
			return num;
		}

		private void StateTransition()
		{
			float num = this.MaximizedDotView();
			switch (this._currentState)
			{
			case InteractableObjectLabel.LabelState.Hidden:
				if (num >= this.alignmentThreshold)
				{
					this._currentState = InteractableObjectLabel.LabelState.FocusCheck;
					this._startTime = Time.time;
					return;
				}
				break;
			case InteractableObjectLabel.LabelState.FocusCheck:
				if (num < this.alignmentThreshold)
				{
					this._currentState = InteractableObjectLabel.LabelState.Hidden;
					return;
				}
				if (Time.time - this._startTime >= this.focusDelay)
				{
					this._currentState = InteractableObjectLabel.LabelState.Focused;
					return;
				}
				break;
			case InteractableObjectLabel.LabelState.Focused:
				if (num <= this.alignmentThreshold)
				{
					this._currentState = InteractableObjectLabel.LabelState.HideCheck;
					this._startTime = Time.time;
					return;
				}
				break;
			case InteractableObjectLabel.LabelState.HideCheck:
				if (num > this.alignmentThreshold)
				{
					this._currentState = InteractableObjectLabel.LabelState.Focused;
					return;
				}
				if (Time.time - this._startTime >= this.hideDelay)
				{
					this._currentState = InteractableObjectLabel.LabelState.Hidden;
				}
				break;
			default:
				return;
			}
		}

		private void InteractableStateChange(InteractableStateChangeArgs args)
		{
			InteractableState newState = args.NewState;
			if (newState != InteractableState.Normal)
			{
				if (newState == InteractableState.Select)
				{
					this._currentState = InteractableObjectLabel.LabelState.Used;
					return;
				}
			}
			else if (this._currentState == InteractableObjectLabel.LabelState.Used)
			{
				this._currentState = InteractableObjectLabel.LabelState.Hidden;
			}
		}

		private Vector3 FindHighestLabelPosition()
		{
			if (this.labelPositions == null)
			{
				return base.transform.position;
			}
			int num = -1;
			float num2 = base.transform.position.y;
			for (int i = 0; i < this.labelPositions.Count; i++)
			{
				float y = this.labelPositions[i].position.y;
				if (y > num2)
				{
					num = i;
					num2 = y;
				}
			}
			if (num == -1)
			{
				return base.transform.position;
			}
			return this.labelPositions[num].position;
		}

		private void UpdateLabelTransform()
		{
			this._startScale = new Vector3
			{
				x = this.canvasTransform.sizeDelta.x * this.canvasTransform.localScale.x,
				y = this.canvasTransform.sizeDelta.y * this.canvasTransform.localScale.y,
				z = 1f
			};
			Vector3 b = this.FindHighestLabelPosition();
			this.currentLabelPosition = Vector3.Lerp(this.currentLabelPosition, b, this.positionAnimationSpeed);
			float b2 = Vector3.Distance(this.currentLabelPosition, this.playerHead.position);
			float d = Mathf.Max(this.minScale, b2);
			this._quadTransform.localScale = this._startScale * d;
			float d2 = this._quadTransform.localScale.y * 0.5f;
			this._quadTransform.position = this.currentLabelPosition + this._quadTransform.up * d2;
			this._quadTransform.LookAt(this.playerHead.position);
			this._quadTransform.localRotation *= Quaternion.Euler(0f, 180f, 0f);
		}

		private void Update()
		{
			this.UpdateLabelTransform();
			this.SetTargetAlpha();
			this.StateTransition();
			this._currentAlpha = Mathf.Lerp(this._currentAlpha, this._targetAlpha, this.alphaAnimationSpeed);
			this.group.alpha = Mathf.Clamp01(this._currentAlpha);
			this._block.SetTexture("_MainTex", this.canvasTexture.Texture);
			this._quadRenderer.SetPropertyBlock(this._block);
		}

		[Tooltip("The positions of these transforms are used to check if the user is facing the object")]
		public List<Transform> viewTargets;

		[Tooltip("The possible positions for the label, the component always selected the one that has the highest y position component")]
		public List<Transform> labelPositions;

		[Tooltip("The position between the left and right cameras")]
		[Optional(OptionalAttribute.Flag.AutoGenerated)]
		public Transform playerHead;

		[Tooltip("This group should contain all the interactions in the object, and when one is triggered the label is hidden")]
		public InteractableGroupView interactableGroup;

		private Vector3 _startScale;

		[Space(10f)]
		[Tooltip("Canvas group at the root of the label canvas, used to make the canvas completely transparent")]
		public CanvasGroup group;

		public float alphaAnimationSpeed;

		public float focusDelay;

		public float hideDelay;

		public float alignmentThreshold;

		public float minScale;

		public float positionAnimationSpeed;

		[Space(10f)]
		public Mesh quadMesh;

		public Material quadMaterial;

		public RectTransform canvasTransform;

		public CanvasRenderTexture canvasTexture;

		private InteractableObjectLabel.LabelState _currentState;

		private float _targetAlpha;

		private float _currentAlpha;

		private float _startTime;

		private MaterialPropertyBlock _block;

		private Transform _quadTransform;

		private MeshRenderer _quadRenderer;

		private Vector3 currentLabelPosition;

		protected bool _started;

		private enum LabelState
		{
			Hidden,
			FocusCheck,
			Focused,
			HideCheck,
			Used
		}
	}
}
