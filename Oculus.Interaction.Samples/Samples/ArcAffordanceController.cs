using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class ArcAffordanceController : MonoBehaviour
	{
		private void Start()
		{
			this._endPositions = new Vector4[2];
			this._endPositions[0].w = 1f;
			this._endPositions[1].w = 1f;
		}

		private void Update()
		{
			this._animator.SetFloat("curvature", this._distanceToCurvatureCurve.Evaluate(Vector3.Distance(base.transform.position, this._pivot.position)));
			this._endPositions[0].x = this._topBone.position.x;
			this._endPositions[0].y = this._topBone.position.y;
			this._endPositions[0].z = this._topBone.position.z;
			this._endPositions[1].x = this._bottomBone.position.x;
			this._endPositions[1].y = this._bottomBone.position.y;
			this._endPositions[1].z = this._bottomBone.position.z;
			this._renderer.material.SetVectorArray("_WorldSpaceFadePoints", this._endPositions);
		}

		[SerializeField]
		[Tooltip("The animator controlling the curvature of the affordance")]
		private Animator _animator;

		[SerializeField]
		[Tooltip("The transform from which world-space distance will be calculated; intuitively, 'the center of the arc's circle'")]
		private Transform _pivot;

		[SerializeField]
		[Tooltip("The function converting distance (from the pivot, a world-space observation) into curvature (an animation parameter)")]
		private AnimationCurve _distanceToCurvatureCurve;

		[SerializeField]
		[Tooltip("The renderer for the arc affordance, on which transparency values must be set.")]
		private SkinnedMeshRenderer _renderer;

		[SerializeField]
		[Tooltip("The bone at the 'top' end of the arc's armature")]
		private Transform _topBone;

		[SerializeField]
		[Tooltip("The bone at the 'bottom' end of the arc's armature")]
		private Transform _bottomBone;

		private Vector4[] _endPositions;
	}
}
