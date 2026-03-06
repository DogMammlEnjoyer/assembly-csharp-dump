using System;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	[ExecuteInEditMode]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_unity_reference_helper.php")]
	public class UnityReferenceHelper : MonoBehaviour
	{
		public string GetGUID()
		{
			return this.guid;
		}

		public void Awake()
		{
			this.Reset();
		}

		public void Reset()
		{
			if (string.IsNullOrEmpty(this.guid))
			{
				this.guid = Guid.NewGuid().ToString();
				Debug.Log("Created new GUID - " + this.guid, this);
				return;
			}
			if (base.gameObject.scene.name != null)
			{
				foreach (UnityReferenceHelper unityReferenceHelper in Object.FindObjectsOfType(typeof(UnityReferenceHelper)) as UnityReferenceHelper[])
				{
					if (unityReferenceHelper != this && this.guid == unityReferenceHelper.guid)
					{
						this.guid = Guid.NewGuid().ToString();
						Debug.Log("Created new GUID - " + this.guid, this);
						return;
					}
				}
			}
		}

		[HideInInspector]
		[SerializeField]
		private string guid;
	}
}
