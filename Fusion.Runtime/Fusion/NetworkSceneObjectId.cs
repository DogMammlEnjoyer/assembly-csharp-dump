using System;

namespace Fusion
{
	public struct NetworkSceneObjectId : IEquatable<NetworkSceneObjectId>
	{
		[Obsolete("Use LoadId instead.")]
		public int SceneLoadId
		{
			get
			{
				return (int)this.LoadId.Value;
			}
		}

		public NetworkSceneObjectId(SceneRef scene, int objectId, NetworkSceneLoadId loadId = default(NetworkSceneLoadId))
		{
			this.Scene = scene;
			this.ObjectId = objectId;
			this.LoadId = loadId;
		}

		public bool IsValid
		{
			get
			{
				return this.Scene.IsValid;
			}
		}

		public override string ToString()
		{
			return string.Format("[Scene: {0}, ObjectId: {1}, SceneLoadId: {2}]", this.Scene.ToString(false, false), this.ObjectId, this.LoadId.Value);
		}

		public bool Equals(NetworkSceneObjectId other)
		{
			return this.Scene.Equals(other.Scene) && this.ObjectId == other.ObjectId && this.LoadId == other.LoadId;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetworkSceneObjectId)
			{
				NetworkSceneObjectId other = (NetworkSceneObjectId)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			int num = this.Scene.GetHashCode();
			num = (num * 397 ^ this.ObjectId);
			return num * 397 ^ (int)this.LoadId.Value;
		}

		public SceneRef Scene;

		public int ObjectId;

		public NetworkSceneLoadId LoadId;
	}
}
