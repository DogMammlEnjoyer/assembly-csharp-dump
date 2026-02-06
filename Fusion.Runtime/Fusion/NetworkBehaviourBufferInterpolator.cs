using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion
{
	public readonly ref struct NetworkBehaviourBufferInterpolator
	{
		public NetworkBehaviourBufferInterpolator(NetworkBehaviour nb)
		{
			this.Behaviour = nb;
			this.Valid = nb.TryGetSnapshotsBuffers(out this.From, out this.To, out this.Alpha);
		}

		public float Angle(string property)
		{
			return this.Angle(this.Behaviour.GetPropertyReader<Angle>(property));
		}

		public float Angle(NetworkBehaviour.PropertyReader<Angle> property)
		{
			Assert.Check(this.Valid);
			ValueTuple<Angle, Angle> valueTuple = property.Read(this.From, this.To);
			Angle item = valueTuple.Item1;
			Angle item2 = valueTuple.Item2;
			return (float)item + ((float)item2 - (float)item) * this.Alpha;
		}

		public float Float(string property)
		{
			return this.Float(this.Behaviour.GetPropertyReader<float>(property));
		}

		public float Float(NetworkBehaviour.PropertyReader<float> property)
		{
			Assert.Check(this.Valid);
			ValueTuple<float, float> valueTuple = property.Read(this.From, this.To);
			float item = valueTuple.Item1;
			float item2 = valueTuple.Item2;
			return item + (item2 - item) * this.Alpha;
		}

		public int Int(string property)
		{
			return this.Select<int>(property);
		}

		public int Int(NetworkBehaviour.PropertyReader<int> property)
		{
			return this.Select<int>(property);
		}

		public bool Bool(NetworkBehaviour.PropertyReader<bool> property)
		{
			return this.Select<bool>(property);
		}

		public bool Bool(string property)
		{
			return this.Select<bool>(property);
		}

		public T Select<[IsUnmanaged] T>(string property) where T : struct, ValueType
		{
			return this.Select<T>(this.Behaviour.GetPropertyReader<T>(property));
		}

		public T Select<[IsUnmanaged] T>(NetworkBehaviour.PropertyReader<T> property) where T : struct, ValueType
		{
			Assert.Check(this.Valid);
			ValueTuple<T, T> valueTuple = property.Read(this.From, this.To);
			T item = valueTuple.Item1;
			T item2 = valueTuple.Item2;
			return ((double)this.Alpha < 0.5) ? item : item2;
		}

		public Vector3 Vector3(string property)
		{
			return this.Vector3(this.Behaviour.GetPropertyReader<Vector3>(property));
		}

		public Vector3 Vector3(NetworkBehaviour.PropertyReader<Vector3> property)
		{
			Assert.Check(this.Valid);
			ValueTuple<Vector3, Vector3> valueTuple = property.Read(this.From, this.To);
			Vector3 item = valueTuple.Item1;
			Vector3 item2 = valueTuple.Item2;
			return UnityEngine.Vector3.Lerp(item, item2, this.Alpha);
		}

		public Vector2 Vector2(string property)
		{
			return this.Vector2(this.Behaviour.GetPropertyReader<Vector2>(property));
		}

		public Vector2 Vector2(NetworkBehaviour.PropertyReader<Vector2> property)
		{
			Assert.Check(this.Valid);
			ValueTuple<Vector2, Vector2> valueTuple = property.Read(this.From, this.To);
			Vector2 item = valueTuple.Item1;
			Vector2 item2 = valueTuple.Item2;
			return UnityEngine.Vector2.Lerp(item, item2, this.Alpha);
		}

		public Vector4 Vector4(string property)
		{
			return this.Vector4(this.Behaviour.GetPropertyReader<Vector4>(property));
		}

		public Vector4 Vector4(NetworkBehaviour.PropertyReader<Vector4> property)
		{
			Assert.Check(this.Valid);
			ValueTuple<Vector4, Vector4> valueTuple = property.Read(this.From, this.To);
			Vector4 item = valueTuple.Item1;
			Vector4 item2 = valueTuple.Item2;
			return UnityEngine.Vector4.Lerp(item, item2, this.Alpha);
		}

		public Quaternion Quaternion(string property)
		{
			return this.Quaternion(this.Behaviour.GetPropertyReader<Quaternion>(property));
		}

		public Quaternion Quaternion(NetworkBehaviour.PropertyReader<Quaternion> property)
		{
			Assert.Check(this.Valid);
			ValueTuple<Quaternion, Quaternion> valueTuple = property.Read(this.From, this.To);
			Quaternion item = valueTuple.Item1;
			Quaternion item2 = valueTuple.Item2;
			return UnityEngine.Quaternion.Slerp(item, item2, this.Alpha);
		}

		public static implicit operator bool(NetworkBehaviourBufferInterpolator i)
		{
			return i.Valid;
		}

		public readonly NetworkBehaviour Behaviour;

		public readonly NetworkBehaviourBuffer From;

		public readonly NetworkBehaviourBuffer To;

		public readonly float Alpha;

		public readonly bool Valid;
	}
}
