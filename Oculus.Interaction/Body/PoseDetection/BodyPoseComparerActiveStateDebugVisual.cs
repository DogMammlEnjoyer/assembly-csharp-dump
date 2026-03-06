using System;
using System.Collections.Generic;
using Oculus.Interaction.Body.Input;
using UnityEngine;

namespace Oculus.Interaction.Body.PoseDetection
{
	public class BodyPoseComparerActiveStateDebugVisual : MonoBehaviour
	{
		public float Radius
		{
			get
			{
				return this._radius;
			}
			set
			{
				this._radius = value;
			}
		}

		protected virtual void Awake()
		{
			this.BodyPose = (this._bodyPose as IBodyPose);
		}

		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			this.DrawJointSpheres();
		}

		private void DrawJointSpheres()
		{
			foreach (KeyValuePair<BodyPoseComparerActiveState.JointComparerConfig, BodyPoseComparerActiveState.BodyPoseComparerFeatureState> keyValuePair in this._bodyPoseComparer.FeatureStates)
			{
				BodyJointId joint = keyValuePair.Key.Joint;
				BodyPoseComparerActiveState.BodyPoseComparerFeatureState value = keyValuePair.Value;
				Pose pose;
				if (this.BodyPose.GetJointPoseFromRoot(joint, out pose))
				{
					Vector3 p = this._root.TransformPoint(pose.position);
					Color color;
					if (value.Delta <= value.MaxDelta)
					{
						color = Color.green;
					}
					else if (value.MaxDelta > 0f)
					{
						float t = value.Delta / value.MaxDelta / 2f;
						color = Color.Lerp(Color.yellow, Color.red, t);
					}
					else
					{
						color = Color.red;
					}
					DebugGizmos.LineWidth = this._radius / 2f;
					DebugGizmos.Color = color;
					DebugGizmos.DrawPoint(p, null);
				}
			}
		}

		public void InjectAllBodyPoseComparerActiveStateDebugVisual(BodyPoseComparerActiveState bodyPoseComparer, IBodyPose bodyPose, Transform root)
		{
			this.InjectBodyPoseComparer(bodyPoseComparer);
			this.InjectBodyPose(bodyPose);
			this.InjectRootTransform(root);
		}

		public void InjectRootTransform(Transform root)
		{
			this._root = root;
		}

		public void InjectBodyPoseComparer(BodyPoseComparerActiveState bodyPoseComparer)
		{
			this._bodyPoseComparer = bodyPoseComparer;
		}

		public void InjectBodyPose(IBodyPose bodyPose)
		{
			this._bodyPose = (bodyPose as Object);
			this.BodyPose = bodyPose;
		}

		[Tooltip("The PoseComparer to debug.")]
		[SerializeField]
		private BodyPoseComparerActiveState _bodyPoseComparer;

		[Tooltip("The body pose to overlay onto. This gizmo simply draws gizmos at joint locations - you must provide a body pose in order for this component to place the gizmos accurately.")]
		[SerializeField]
		[Interface(typeof(IBodyPose), new Type[]
		{

		})]
		private Object _bodyPose;

		private IBodyPose BodyPose;

		[Tooltip("The root transform of the body on which to overlay the spheres. For BodyPoseDebugGizmos, this is simply the transform of the component. For a skinned body model, this would be the Root transform.")]
		[SerializeField]
		private Transform _root;

		[Tooltip("The radius of the debug spheres.")]
		[SerializeField]
		[Delayed]
		private float _radius = 0.1f;
	}
}
