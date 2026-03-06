using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class OneGrabPhysicsJointTransformer : MonoBehaviour, ITransformer
	{
		public bool IsKinematicGrab
		{
			get
			{
				return this._isKinematicGrab;
			}
			set
			{
				this._isKinematicGrab = value;
			}
		}

		private void OnValidate()
		{
			if (this._customJoint != null)
			{
				if (this._customJoint.gameObject == base.gameObject)
				{
					Debug.LogWarning("The OptionalCustomJoint must be placed in a disabled child GameObject. Moving it.", base.gameObject);
					GameObject destination = this.CreateJointHolder();
					this._customJoint = OneGrabPhysicsJointTransformer.CloneJoint(this._customJoint, destination);
					return;
				}
				this._customJoint.gameObject.SetActive(false);
			}
		}

		public void Initialize(IGrabbable grabbable)
		{
			this._grabbable = grabbable;
		}

		public void BeginTransform()
		{
			Vector3 position = this._grabbable.GrabPoints[0].position;
			Quaternion rotation = this._grabbable.GrabPoints[0].rotation;
			this._grabbingRigidbody = this.GetGrabRigidbody();
			this._grabbingRigidbody.transform.SetPositionAndRotation(position, rotation);
			this._joint = this.AddJoint(this._grabbingRigidbody);
		}

		public void UpdateTransform()
		{
			Pose pose = this._grabbable.GrabPoints[0];
			this._targetPosition = pose.position;
			this._targetRotation = pose.rotation;
			if (this._isKinematicGrab)
			{
				this._grabbingRigidbody.transform.SetPositionAndRotation(this._targetPosition, this._targetRotation);
			}
		}

		private void FixedUpdate()
		{
			if (!this._isKinematicGrab && this._grabbingRigidbody != null)
			{
				this._grabbingRigidbody.MovePosition(this._targetPosition);
				this._grabbingRigidbody.MoveRotation(this._targetRotation);
			}
		}

		public void EndTransform()
		{
			this.RemoveCurrentJoint();
			this.RemoveCurrentGrabRigidbody();
		}

		private Joint AddJoint(Rigidbody rigidbody)
		{
			this.RemoveCurrentJoint();
			Joint joint;
			if (this._customJoint != null)
			{
				joint = OneGrabPhysicsJointTransformer.CloneJoint(this._customJoint, base.gameObject);
			}
			else
			{
				joint = this.CreateDefaultJoint();
			}
			joint.connectedBody = rigidbody;
			joint.autoConfigureConnectedAnchor = false;
			joint.anchor = joint.transform.InverseTransformPoint(rigidbody.transform.position);
			joint.connectedAnchor = Vector3.zero;
			return joint;
		}

		private void RemoveCurrentJoint()
		{
			if (this._joint != null)
			{
				Object.Destroy(this._joint);
			}
		}

		private Rigidbody GetGrabRigidbody()
		{
			Rigidbody rigidbody = OneGrabPhysicsJointTransformer._cachedGrabbingRigidbodies.Find((Rigidbody rb) => rb != null && !rb.gameObject.activeSelf);
			if (rigidbody == null)
			{
				rigidbody = this.CreateRigidBody();
				OneGrabPhysicsJointTransformer._cachedGrabbingRigidbodies.Add(rigidbody);
			}
			rigidbody.gameObject.SetActive(true);
			rigidbody.isKinematic = this._isKinematicGrab;
			return rigidbody;
		}

		private void RemoveCurrentGrabRigidbody()
		{
			if (this._grabbingRigidbody != null)
			{
				this._grabbingRigidbody.gameObject.SetActive(false);
				this._grabbingRigidbody.isKinematic = true;
				this._grabbingRigidbody = null;
			}
		}

		private Rigidbody CreateRigidBody()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "Proxy RigidBody";
			gameObject.SetActive(false);
			gameObject.transform.SetParent(this._rigidbodiesRoot);
			Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
			rigidbody.useGravity = false;
			rigidbody.isKinematic = false;
			return rigidbody;
		}

		private Joint CreateDefaultJoint()
		{
			FixedJoint fixedJoint = base.gameObject.AddComponent<FixedJoint>();
			fixedJoint.breakForce = float.PositiveInfinity;
			fixedJoint.enablePreprocessing = false;
			return fixedJoint;
		}

		protected GameObject CreateJointHolder()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "Saved Joint";
			gameObject.SetActive(false);
			gameObject.transform.SetParent(base.transform);
			gameObject.AddComponent<Rigidbody>().isKinematic = true;
			return gameObject;
		}

		private static ConfigurableJoint CloneJoint(ConfigurableJoint joint, GameObject destination)
		{
			ConfigurableJoint configurableJoint = destination.gameObject.AddComponent<ConfigurableJoint>();
			configurableJoint.connectedBody = joint.connectedBody;
			configurableJoint.axis = joint.axis;
			configurableJoint.anchor = joint.anchor;
			configurableJoint.connectedAnchor = joint.connectedAnchor;
			configurableJoint.autoConfigureConnectedAnchor = joint.autoConfigureConnectedAnchor;
			configurableJoint.breakForce = joint.breakForce;
			configurableJoint.breakTorque = joint.breakTorque;
			configurableJoint.enableCollision = joint.enableCollision;
			configurableJoint.enablePreprocessing = joint.enablePreprocessing;
			configurableJoint.massScale = joint.massScale;
			configurableJoint.connectedMassScale = joint.connectedMassScale;
			configurableJoint.projectionAngle = joint.projectionAngle;
			configurableJoint.projectionDistance = joint.projectionDistance;
			configurableJoint.projectionMode = joint.projectionMode;
			configurableJoint.slerpDrive = joint.slerpDrive;
			configurableJoint.angularYZDrive = joint.angularYZDrive;
			configurableJoint.angularXDrive = joint.angularXDrive;
			configurableJoint.rotationDriveMode = joint.rotationDriveMode;
			configurableJoint.targetAngularVelocity = joint.targetAngularVelocity;
			configurableJoint.targetRotation = joint.targetRotation;
			configurableJoint.zDrive = joint.zDrive;
			configurableJoint.yDrive = joint.yDrive;
			configurableJoint.xDrive = joint.xDrive;
			configurableJoint.targetVelocity = joint.targetVelocity;
			configurableJoint.targetPosition = joint.targetPosition;
			configurableJoint.angularZLimit = joint.angularZLimit;
			configurableJoint.angularYLimit = joint.angularYLimit;
			configurableJoint.highAngularXLimit = joint.highAngularXLimit;
			configurableJoint.lowAngularXLimit = joint.lowAngularXLimit;
			configurableJoint.linearLimit = joint.linearLimit;
			configurableJoint.angularYZLimitSpring = joint.angularYZLimitSpring;
			configurableJoint.angularXLimitSpring = joint.angularXLimitSpring;
			configurableJoint.linearLimitSpring = joint.linearLimitSpring;
			configurableJoint.angularZMotion = joint.angularZMotion;
			configurableJoint.angularYMotion = joint.angularYMotion;
			configurableJoint.angularXMotion = joint.angularXMotion;
			configurableJoint.zMotion = joint.zMotion;
			configurableJoint.yMotion = joint.yMotion;
			configurableJoint.xMotion = joint.xMotion;
			configurableJoint.secondaryAxis = joint.secondaryAxis;
			configurableJoint.configuredInWorldSpace = joint.configuredInWorldSpace;
			configurableJoint.swapBodies = joint.swapBodies;
			return configurableJoint;
		}

		public void InjectOptionalCustomJoint(ConfigurableJoint customJoint)
		{
			this._customJoint = customJoint;
		}

		public void InjectOptionalRigidbodiesRoot(Transform rigidbodiesRoot)
		{
			this._rigidbodiesRoot = rigidbodiesRoot;
		}

		[SerializeField]
		[Optional]
		[Tooltip("Specify a custom joint to use when grabbing; should be disabled.")]
		private ConfigurableJoint _customJoint;

		[SerializeField]
		[Tooltip("Indicates if the grabbing rigidbody should be kinematic or not.")]
		private bool _isKinematicGrab = true;

		[SerializeField]
		[Optional]
		[Tooltip("Newly created rigidbodies will be appended to this transform")]
		private Transform _rigidbodiesRoot;

		private Joint _joint;

		private Rigidbody _grabbingRigidbody;

		private static List<Rigidbody> _cachedGrabbingRigidbodies = new List<Rigidbody>();

		private IGrabbable _grabbable;

		private Vector3 _targetPosition;

		private Quaternion _targetRotation;
	}
}
