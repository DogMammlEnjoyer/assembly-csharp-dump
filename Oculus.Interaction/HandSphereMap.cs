using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandSphereMap : MonoBehaviour, IHandSphereMap
	{
		protected virtual void Awake()
		{
			for (int i = 0; i < 26; i++)
			{
				this._sourceSphereMap[i] = new List<HandSphere>();
			}
		}

		protected virtual void Start()
		{
			for (int i = 0; i < 26; i++)
			{
				List<HandSphere> list = this._sourceSphereMap[i];
				HandJointId handJointId = (HandJointId)i;
				Transform transformFor = this._handPrefabDataSource.GetTransformFor(handJointId);
				if (!(transformFor == null))
				{
					foreach (object obj in transformFor)
					{
						Transform transform = (Transform)obj;
						if (!(transform.name != "sphere") && transform.gameObject.activeSelf)
						{
							Vector3 position = transform.GetPose(Space.Self).position;
							list.Add(new HandSphere(position, transform.lossyScale.x * 0.5f, handJointId));
							transform.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		public void GetSpheres(Handedness handedness, HandJointId jointId, Pose jointPose, float scale, List<HandSphere> spheres)
		{
			bool flag = handedness != this._handPrefabDataSource.Handedness;
			for (int i = 0; i < this._sourceSphereMap[(int)jointId].Count; i++)
			{
				HandSphere handSphere = this._sourceSphereMap[(int)jointId][i];
				Vector3 point = handSphere.Position * scale;
				if (flag)
				{
					point = HandMirroring.Mirror(point);
				}
				Vector3 position = jointPose.position + jointPose.rotation * point;
				HandSphere item = new HandSphere(position, handSphere.Radius * scale, handSphere.Joint);
				spheres.Add(item);
			}
		}

		[SerializeField]
		public FromHandPrefabDataSource _handPrefabDataSource;

		private readonly List<HandSphere>[] _sourceSphereMap = new List<HandSphere>[26];
	}
}
