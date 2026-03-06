using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
	[AddComponentMenu("XR/Locomotion/XR Body Transformer", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.XRBodyTransformer.html")]
	[DefaultExecutionOrder(-205)]
	public class XRBodyTransformer : MonoBehaviour
	{
		public XROrigin xrOrigin
		{
			get
			{
				return this.m_XROrigin;
			}
			set
			{
				this.m_XROrigin = value;
				if (Application.isPlaying)
				{
					this.InitializeMovableBody();
				}
			}
		}

		public IXRBodyPositionEvaluator bodyPositionEvaluator
		{
			get
			{
				return this.m_BodyPositionEvaluator;
			}
			set
			{
				this.m_BodyPositionEvaluator = value;
				if (Application.isPlaying)
				{
					this.InitializeMovableBody();
				}
			}
		}

		public IConstrainedXRBodyManipulator constrainedBodyManipulator
		{
			get
			{
				return this.m_ConstrainedBodyManipulator;
			}
			set
			{
				this.m_ConstrainedBodyManipulator = value;
				if (this.m_MovableBody != null)
				{
					this.m_MovableBody.UnlinkConstrainedManipulator();
					if (this.m_ConstrainedBodyManipulator != null)
					{
						this.m_MovableBody.LinkConstrainedManipulator(this.m_ConstrainedBodyManipulator);
					}
				}
			}
		}

		public bool useCharacterControllerIfExists
		{
			get
			{
				return this.m_UseCharacterControllerIfExists;
			}
			set
			{
				this.m_UseCharacterControllerIfExists = value;
			}
		}

		public event Action<XRBodyTransformer> beforeApplyTransformations;

		public event Action<ApplyBodyTransformationsEventArgs> afterApplyTransformations;

		protected virtual void Reset()
		{
			this.m_XROrigin = ComponentLocatorUtility<XROrigin>.FindComponent();
		}

		protected virtual void OnEnable()
		{
			if (this.m_XROrigin == null && !ComponentLocatorUtility<XROrigin>.TryFindComponent(out this.m_XROrigin))
			{
				Debug.LogError("XR Body Transformer requires an XR Origin in the scene.", this);
				base.enabled = false;
				return;
			}
			this.m_BodyPositionEvaluator = (this.m_BodyPositionEvaluatorObject as IXRBodyPositionEvaluator);
			if (this.m_BodyPositionEvaluator == null)
			{
				this.m_UsingDynamicBodyPositionEvaluator = true;
				this.m_BodyPositionEvaluator = ScriptableSingletonCache<UnderCameraBodyPositionEvaluator>.GetInstance(this);
			}
			this.m_ConstrainedBodyManipulator = (this.m_ConstrainedBodyManipulatorObject as IConstrainedXRBodyManipulator);
			CharacterController characterController;
			if (this.m_ConstrainedBodyManipulator == null && this.m_UseCharacterControllerIfExists && this.m_XROrigin.Origin.TryGetComponent<CharacterController>(out characterController))
			{
				this.m_UsingDynamicConstrainedBodyManipulator = true;
				this.m_ConstrainedBodyManipulator = ScriptableSingletonCache<CharacterControllerBodyManipulator>.GetInstance(this);
			}
			this.InitializeMovableBody();
		}

		protected virtual void OnDisable()
		{
			XRMovableBody movableBody = this.m_MovableBody;
			if (movableBody != null)
			{
				movableBody.UnlinkConstrainedManipulator();
			}
			this.m_MovableBody = null;
			if (this.m_UsingDynamicBodyPositionEvaluator)
			{
				ScriptableSingletonCache<UnderCameraBodyPositionEvaluator>.ReleaseInstance(this);
				this.m_UsingDynamicBodyPositionEvaluator = false;
			}
			if (this.m_UsingDynamicConstrainedBodyManipulator)
			{
				ScriptableSingletonCache<CharacterControllerBodyManipulator>.ReleaseInstance(this);
				this.m_UsingDynamicConstrainedBodyManipulator = false;
			}
		}

		protected virtual void Update()
		{
			if (this.m_TransformationsQueue.Count == 0)
			{
				return;
			}
			Action<XRBodyTransformer> action = this.beforeApplyTransformations;
			if (action != null)
			{
				action(this);
			}
			while (this.m_TransformationsQueue.Count > 0)
			{
				this.m_TransformationsQueue.First.Value.transformation.Apply(this.m_MovableBody);
				this.m_TransformationsQueue.RemoveFirst();
			}
			this.m_ApplyTransformationsEventArgs.bodyTransformer = this;
			Action<ApplyBodyTransformationsEventArgs> action2 = this.afterApplyTransformations;
			if (action2 == null)
			{
				return;
			}
			action2(this.m_ApplyTransformationsEventArgs);
		}

		private void InitializeMovableBody()
		{
			this.m_MovableBody = new XRMovableBody(this.m_XROrigin, this.m_BodyPositionEvaluator);
			if (this.m_ConstrainedBodyManipulator != null)
			{
				this.m_MovableBody.LinkConstrainedManipulator(this.m_ConstrainedBodyManipulator);
			}
		}

		public void QueueTransformation(IXRBodyTransformation transformation, int priority = 0)
		{
			XRBodyTransformer.OrderedTransformation value = new XRBodyTransformer.OrderedTransformation
			{
				transformation = transformation,
				priority = priority
			};
			LinkedListNode<XRBodyTransformer.OrderedTransformation> linkedListNode = this.m_TransformationsQueue.First;
			if (linkedListNode == null || linkedListNode.Value.priority > priority)
			{
				this.m_TransformationsQueue.AddFirst(value);
				return;
			}
			while (linkedListNode.Next != null && linkedListNode.Next.Value.priority <= priority)
			{
				linkedListNode = linkedListNode.Next;
			}
			this.m_TransformationsQueue.AddAfter(linkedListNode, value);
		}

		protected virtual void OnDrawGizmosSelected()
		{
			if (this.m_UseCharacterControllerIfExists && this.m_ConstrainedBodyManipulator != null)
			{
				CharacterControllerBodyManipulator characterControllerBodyManipulator = this.m_ConstrainedBodyManipulator as CharacterControllerBodyManipulator;
				if (characterControllerBodyManipulator != null && characterControllerBodyManipulator.characterController != null)
				{
					CharacterController characterController = characterControllerBodyManipulator.characterController;
					Vector3 center = characterController.center + characterController.transform.position + Vector3.up * ((characterController.stepOffset - characterController.skinWidth) * 0.5f);
					float height = characterController.height + characterController.stepOffset + characterController.skinWidth;
					float radius = characterController.radius + characterController.skinWidth;
					GizmoHelpers.DrawCapsule(center, height, radius, Vector3.up, new Color(1f, 0.92f, 0.016f, 0.5f));
				}
			}
		}

		[SerializeField]
		[Tooltip("The XR Origin to transform (will find one if None).")]
		private XROrigin m_XROrigin;

		[SerializeField]
		[RequireInterface(typeof(IXRBodyPositionEvaluator))]
		[Tooltip("Object that determines the position of the user's body. If set to None, this behavior will estimate the position to be the camera position projected onto the XZ plane of the XR Origin.")]
		private Object m_BodyPositionEvaluatorObject;

		private IXRBodyPositionEvaluator m_BodyPositionEvaluator;

		[SerializeField]
		[RequireInterface(typeof(IConstrainedXRBodyManipulator))]
		[Tooltip("Object used to perform movement that is constrained by collision (optional, may be None).")]
		private Object m_ConstrainedBodyManipulatorObject;

		private IConstrainedXRBodyManipulator m_ConstrainedBodyManipulator;

		[SerializeField]
		[Tooltip("When enabled and if a Constrained Manipulator is not already assigned, this behavior will use the XR Origin's Character Controller to perform constrained movement, if one exists on the XR Origin's base GameObject.")]
		private bool m_UseCharacterControllerIfExists = true;

		private bool m_UsingDynamicBodyPositionEvaluator;

		private bool m_UsingDynamicConstrainedBodyManipulator;

		private XRMovableBody m_MovableBody;

		private readonly LinkedList<XRBodyTransformer.OrderedTransformation> m_TransformationsQueue = new LinkedList<XRBodyTransformer.OrderedTransformation>();

		private readonly ApplyBodyTransformationsEventArgs m_ApplyTransformationsEventArgs = new ApplyBodyTransformationsEventArgs();

		private struct OrderedTransformation
		{
			public IXRBodyTransformation transformation;

			public int priority;
		}
	}
}
