using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class MRPassThroughHandVisualize : MonoBehaviour
	{
		private void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
			this._eyeRays = new Ray[this._eyeAnchors.Count];
			this._currentOpacity = this._opacity;
			this._currentOutlineOpacity = this._outlineOpacity;
			List<Vector3> list = new List<Vector3>
			{
				this._handVisual.GetJointPose(HandJointId.HandWristRoot, Space.World).position,
				this._handVisual.GetJointPose(HandJointId.HandThumb1, Space.World).position,
				this._handVisual.GetJointPose(HandJointId.HandIndex1, Space.World).position,
				this._handVisual.GetJointPose(HandJointId.HandMiddle1, Space.World).position,
				this._handVisual.GetJointPose(HandJointId.HandRing1, Space.World).position,
				this._handVisual.GetJointPose(HandJointId.HandPinky1, Space.World).position
			};
			Vector3 vector = Vector3.zero;
			foreach (Vector3 b in list)
			{
				vector += b;
			}
			vector *= 1f / (float)list.Count;
			Vector3 item = this._handVisual.GetTransformByHandJointId(HandJointId.HandWristRoot).InverseTransformPoint(vector);
			float num = 0f;
			foreach (Vector3 b2 in list)
			{
				num = Mathf.Max(num, Vector3.Distance(vector, b2));
			}
			this._palmTarget = new ValueTuple<Vector3, float>(item, num * 0.65f);
		}

		private bool SphereCast(Vector3 target, float radius)
		{
			for (int i = 0; i < this._eyeAnchors.Count; i++)
			{
				Vector3 position = this._eyeAnchors[i].position;
				Vector3 normalized = (target - position).normalized;
				this._eyeRays[i] = new Ray(position, normalized);
			}
			Ray[] eyeRays = this._eyeRays;
			for (int j = 0; j < eyeRays.Length; j++)
			{
				if (Physics.SphereCast(eyeRays[j], radius, this._castDistance, this._layer))
				{
					return true;
				}
			}
			return false;
		}

		private bool SphereCastAllTargets()
		{
			Vector3 target = this._handVisual.GetTransformByHandJointId(HandJointId.HandWristRoot).TransformPoint(this._palmTarget.Item1);
			if (this.SphereCast(target, this._palmTarget.Item2))
			{
				return true;
			}
			foreach (HandJointId jointId in this._handJointTargets)
			{
				Pose jointPose = this._handVisual.GetJointPose(jointId, Space.World);
				if (this.SphereCast(jointPose.position, this._sphereRadius))
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateMaterialPropertyBlock(bool sphereCastHit)
		{
			float b = sphereCastHit ? this._opacity : 0f;
			float b2 = sphereCastHit ? this._outlineOpacity : 0f;
			float t = this._animationSpeed * Time.deltaTime;
			this._currentOpacity = Mathf.Lerp(this._currentOpacity, b, t);
			this._currentOutlineOpacity = Mathf.Lerp(this._currentOutlineOpacity, b2, t);
			foreach (MaterialPropertyBlockEditor materialPropertyBlockEditor in this._handMaterialPropertyBlocks)
			{
				materialPropertyBlockEditor.MaterialPropertyBlock.SetFloat(this._opacityId, this._currentOpacity);
				materialPropertyBlockEditor.MaterialPropertyBlock.SetFloat(this._outlineOpacityId, this._currentOutlineOpacity);
			}
		}

		private void Update()
		{
			if (!MRPassthrough.PassThrough.IsPassThroughOn)
			{
				foreach (MaterialPropertyBlockEditor materialPropertyBlockEditor in this._handMaterialPropertyBlocks)
				{
					materialPropertyBlockEditor.MaterialPropertyBlock.SetFloat(this._opacityId, this._opacity);
					materialPropertyBlockEditor.MaterialPropertyBlock.SetFloat(this._outlineOpacityId, this._outlineOpacity);
				}
				return;
			}
			if (this._eyeAnchors == null || this._handVisual == null)
			{
				return;
			}
			this.UpdateMaterialPropertyBlock(this.SphereCastAllTargets());
		}

		public MRPassThroughHandVisualize()
		{
			HandJointId[] array = new HandJointId[10];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.4B5AFD97F49B39660072A4D5AB453867A32F43F8E7F0581DBDE1ABB27FD9E983).FieldHandle);
			this._handJointTargets = array;
			base..ctor();
		}

		[SerializeField]
		private List<Transform> _eyeAnchors;

		[SerializeField]
		private HandVisual _handVisual;

		[Header("Raycast Properties")]
		[SerializeField]
		private LayerMask _layer;

		[SerializeField]
		private float _sphereRadius;

		[SerializeField]
		private float _castDistance;

		[Header("Material Properties")]
		[SerializeField]
		private MaterialPropertyBlockEditor[] _handMaterialPropertyBlocks;

		[SerializeField]
		private float _opacity;

		[SerializeField]
		private float _outlineOpacity;

		[SerializeField]
		private float _animationSpeed;

		private float _currentOpacity;

		private float _currentOutlineOpacity;

		private readonly int _opacityId = Shader.PropertyToID("_Opacity");

		private readonly int _outlineOpacityId = Shader.PropertyToID("_OutlineOpacity");

		private ValueTuple<Vector3, float> _palmTarget;

		private readonly HandJointId[] _handJointTargets;

		private Ray[] _eyeRays;

		private bool _started;
	}
}
