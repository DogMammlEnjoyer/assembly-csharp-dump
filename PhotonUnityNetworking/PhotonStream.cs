using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun
{
	public class PhotonStream
	{
		public bool IsWriting { get; private set; }

		public bool IsReading
		{
			get
			{
				return !this.IsWriting;
			}
		}

		public int Count
		{
			get
			{
				if (!this.IsWriting)
				{
					return this.readData.Length;
				}
				return this.writeData.Count;
			}
		}

		public PhotonStream(bool write, object[] incomingData)
		{
			this.IsWriting = write;
			if (!write && incomingData != null)
			{
				this.readData = incomingData;
			}
		}

		public void SetReadStream(object[] incomingData, int pos = 0)
		{
			this.readData = incomingData;
			this.currentItem = pos;
			this.IsWriting = false;
		}

		internal void SetWriteStream(List<object> newWriteData, int pos = 0)
		{
			if (pos != newWriteData.Count)
			{
				throw new Exception("SetWriteStream failed, because count does not match position value. pos: " + pos.ToString() + " newWriteData.Count:" + newWriteData.Count.ToString());
			}
			this.writeData = newWriteData;
			this.currentItem = pos;
			this.IsWriting = true;
		}

		internal List<object> GetWriteStream()
		{
			return this.writeData;
		}

		[Obsolete("Either SET the writeData with an empty List or use Clear().")]
		internal void ResetWriteStream()
		{
			this.writeData.Clear();
		}

		public object ReceiveNext()
		{
			if (this.IsWriting)
			{
				Debug.LogError("Error: you cannot read this stream that you are writing!");
				return null;
			}
			object result = this.readData[this.currentItem];
			this.currentItem++;
			return result;
		}

		public object PeekNext()
		{
			if (this.IsWriting)
			{
				Debug.LogError("Error: you cannot read this stream that you are writing!");
				return null;
			}
			return this.readData[this.currentItem];
		}

		public void SendNext(object obj)
		{
			if (!this.IsWriting)
			{
				Debug.LogError("Error: you cannot write/send to this stream that you are reading!");
				return;
			}
			this.writeData.Add(obj);
		}

		[Obsolete("writeData is a list now. Use and re-use it directly.")]
		public bool CopyToListAndClear(List<object> target)
		{
			if (!this.IsWriting)
			{
				return false;
			}
			target.AddRange(this.writeData);
			this.writeData.Clear();
			return true;
		}

		public object[] ToArray()
		{
			if (!this.IsWriting)
			{
				return this.readData;
			}
			return this.writeData.ToArray();
		}

		public void Serialize(ref bool myBool)
		{
			if (this.IsWriting)
			{
				this.writeData.Add(myBool);
				return;
			}
			if (this.readData.Length > this.currentItem)
			{
				myBool = (bool)this.readData[this.currentItem];
				this.currentItem++;
			}
		}

		public void Serialize(ref int myInt)
		{
			if (this.IsWriting)
			{
				this.writeData.Add(myInt);
				return;
			}
			if (this.readData.Length > this.currentItem)
			{
				myInt = (int)this.readData[this.currentItem];
				this.currentItem++;
			}
		}

		public void Serialize(ref string value)
		{
			if (this.IsWriting)
			{
				this.writeData.Add(value);
				return;
			}
			if (this.readData.Length > this.currentItem)
			{
				value = (string)this.readData[this.currentItem];
				this.currentItem++;
			}
		}

		public void Serialize(ref char value)
		{
			if (this.IsWriting)
			{
				this.writeData.Add(value);
				return;
			}
			if (this.readData.Length > this.currentItem)
			{
				value = (char)this.readData[this.currentItem];
				this.currentItem++;
			}
		}

		public void Serialize(ref short value)
		{
			if (this.IsWriting)
			{
				this.writeData.Add(value);
				return;
			}
			if (this.readData.Length > this.currentItem)
			{
				value = (short)this.readData[this.currentItem];
				this.currentItem++;
			}
		}

		public void Serialize(ref float obj)
		{
			if (this.IsWriting)
			{
				this.writeData.Add(obj);
				return;
			}
			if (this.readData.Length > this.currentItem)
			{
				obj = (float)this.readData[this.currentItem];
				this.currentItem++;
			}
		}

		public void Serialize(ref Player obj)
		{
			if (this.IsWriting)
			{
				this.writeData.Add(obj);
				return;
			}
			if (this.readData.Length > this.currentItem)
			{
				obj = (Player)this.readData[this.currentItem];
				this.currentItem++;
			}
		}

		public void Serialize(ref Vector3 obj)
		{
			if (this.IsWriting)
			{
				this.writeData.Add(obj);
				return;
			}
			if (this.readData.Length > this.currentItem)
			{
				obj = (Vector3)this.readData[this.currentItem];
				this.currentItem++;
			}
		}

		public void Serialize(ref Vector2 obj)
		{
			if (this.IsWriting)
			{
				this.writeData.Add(obj);
				return;
			}
			if (this.readData.Length > this.currentItem)
			{
				obj = (Vector2)this.readData[this.currentItem];
				this.currentItem++;
			}
		}

		public void Serialize(ref Quaternion obj)
		{
			if (this.IsWriting)
			{
				this.writeData.Add(obj);
				return;
			}
			if (this.readData.Length > this.currentItem)
			{
				obj = (Quaternion)this.readData[this.currentItem];
				this.currentItem++;
			}
		}

		private List<object> writeData;

		private object[] readData;

		private int currentItem;
	}
}
