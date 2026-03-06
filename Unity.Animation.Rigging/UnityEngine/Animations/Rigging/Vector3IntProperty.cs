using System;

namespace UnityEngine.Animations.Rigging
{
	public struct Vector3IntProperty : IAnimatableProperty<Vector3Int>
	{
		public static Vector3IntProperty Bind(Animator animator, Component component, string name)
		{
			Type type = component.GetType();
			return new Vector3IntProperty
			{
				x = animator.BindStreamProperty(component.transform, type, name + ".x"),
				y = animator.BindStreamProperty(component.transform, type, name + ".y"),
				z = animator.BindStreamProperty(component.transform, type, name + ".z")
			};
		}

		public static Vector3IntProperty BindCustom(Animator animator, string name)
		{
			return new Vector3IntProperty
			{
				x = animator.BindCustomStreamProperty(name + ".x", CustomStreamPropertyType.Int),
				y = animator.BindCustomStreamProperty(name + ".y", CustomStreamPropertyType.Int),
				z = animator.BindCustomStreamProperty(name + ".z", CustomStreamPropertyType.Int)
			};
		}

		public Vector3Int Get(AnimationStream stream)
		{
			return new Vector3Int(this.x.GetInt(stream), this.y.GetInt(stream), this.z.GetInt(stream));
		}

		public void Set(AnimationStream stream, Vector3Int value)
		{
			this.x.SetInt(stream, value.x);
			this.y.SetInt(stream, value.y);
			this.z.SetInt(stream, value.z);
		}

		public PropertyStreamHandle x;

		public PropertyStreamHandle y;

		public PropertyStreamHandle z;
	}
}
