using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	[RequireComponent(typeof(PhotonView))]
	public class CullingHandler : MonoBehaviour, IPunObservable
	{
		private void OnEnable()
		{
			if (this.pView == null)
			{
				this.pView = base.GetComponent<PhotonView>();
				if (!this.pView.IsMine)
				{
					return;
				}
			}
			if (this.cullArea == null)
			{
				this.cullArea = Object.FindObjectOfType<CullArea>();
			}
			this.previousActiveCells = new List<byte>(0);
			this.activeCells = new List<byte>(0);
			this.currentPosition = (this.lastPosition = base.transform.position);
		}

		private void Start()
		{
			if (!this.pView.IsMine)
			{
				return;
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.cullArea.NumberOfSubdivisions == 0)
				{
					this.pView.Group = this.cullArea.FIRST_GROUP_ID;
					PhotonNetwork.SetInterestGroups(this.cullArea.FIRST_GROUP_ID, true);
					return;
				}
				this.pView.ObservedComponents.Add(this);
			}
		}

		private void Update()
		{
			if (!this.pView.IsMine)
			{
				return;
			}
			this.timeSinceUpdate += Time.deltaTime;
			if (this.timeSinceUpdate < this.timeBetweenUpdatesMin)
			{
				return;
			}
			this.lastPosition = this.currentPosition;
			this.currentPosition = base.transform.position;
			if (this.currentPosition != this.lastPosition && this.HaveActiveCellsChanged())
			{
				this.UpdateInterestGroups();
				this.timeSinceUpdate = 0f;
			}
		}

		private void OnGUI()
		{
			if (!this.pView.IsMine)
			{
				return;
			}
			string text = "Inside cells:\n";
			string text2 = "Subscribed cells:\n";
			for (int i = 0; i < this.activeCells.Count; i++)
			{
				if (i <= this.cullArea.NumberOfSubdivisions)
				{
					text = text + this.activeCells[i].ToString() + " | ";
				}
				text2 = text2 + this.activeCells[i].ToString() + " | ";
			}
			GUI.Label(new Rect(20f, (float)Screen.height - 120f, 200f, 40f), "<color=white>PhotonView Group: " + this.pView.Group.ToString() + "</color>", new GUIStyle
			{
				alignment = TextAnchor.UpperLeft,
				fontSize = 16
			});
			GUI.Label(new Rect(20f, (float)Screen.height - 100f, 200f, 40f), "<color=white>" + text + "</color>", new GUIStyle
			{
				alignment = TextAnchor.UpperLeft,
				fontSize = 16
			});
			GUI.Label(new Rect(20f, (float)Screen.height - 60f, 200f, 40f), "<color=white>" + text2 + "</color>", new GUIStyle
			{
				alignment = TextAnchor.UpperLeft,
				fontSize = 16
			});
		}

		private bool HaveActiveCellsChanged()
		{
			if (this.cullArea.NumberOfSubdivisions == 0)
			{
				return false;
			}
			this.previousActiveCells = new List<byte>(this.activeCells);
			this.activeCells = this.cullArea.GetActiveCells(base.transform.position);
			while (this.activeCells.Count <= this.cullArea.NumberOfSubdivisions)
			{
				this.activeCells.Add(this.cullArea.FIRST_GROUP_ID);
			}
			return this.activeCells.Count != this.previousActiveCells.Count || this.activeCells[this.cullArea.NumberOfSubdivisions] != this.previousActiveCells[this.cullArea.NumberOfSubdivisions];
		}

		private void UpdateInterestGroups()
		{
			List<byte> list = new List<byte>(0);
			foreach (byte item in this.previousActiveCells)
			{
				if (!this.activeCells.Contains(item))
				{
					list.Add(item);
				}
			}
			PhotonNetwork.SetInterestGroups(list.ToArray(), this.activeCells.ToArray());
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			while (this.activeCells.Count <= this.cullArea.NumberOfSubdivisions)
			{
				this.activeCells.Add(this.cullArea.FIRST_GROUP_ID);
			}
			if (this.cullArea.NumberOfSubdivisions == 1)
			{
				int num = this.orderIndex + 1;
				this.orderIndex = num;
				this.orderIndex = num % this.cullArea.SUBDIVISION_FIRST_LEVEL_ORDER.Length;
				this.pView.Group = this.activeCells[this.cullArea.SUBDIVISION_FIRST_LEVEL_ORDER[this.orderIndex]];
				return;
			}
			if (this.cullArea.NumberOfSubdivisions == 2)
			{
				int num = this.orderIndex + 1;
				this.orderIndex = num;
				this.orderIndex = num % this.cullArea.SUBDIVISION_SECOND_LEVEL_ORDER.Length;
				this.pView.Group = this.activeCells[this.cullArea.SUBDIVISION_SECOND_LEVEL_ORDER[this.orderIndex]];
				return;
			}
			if (this.cullArea.NumberOfSubdivisions == 3)
			{
				int num = this.orderIndex + 1;
				this.orderIndex = num;
				this.orderIndex = num % this.cullArea.SUBDIVISION_THIRD_LEVEL_ORDER.Length;
				this.pView.Group = this.activeCells[this.cullArea.SUBDIVISION_THIRD_LEVEL_ORDER[this.orderIndex]];
			}
		}

		private int orderIndex;

		private CullArea cullArea;

		private List<byte> previousActiveCells;

		private List<byte> activeCells;

		private PhotonView pView;

		private Vector3 lastPosition;

		private Vector3 currentPosition;

		private float timeSinceUpdate;

		private float timeBetweenUpdatesMin = 0.33f;
	}
}
