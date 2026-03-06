using System;

namespace UnityEngine.Animations.Rigging
{
	public struct Vector3Property : IAnimatableProperty<Vector3>
	{
		public static Vector3Property Bind(Animator animator, Component component, string name)
		{
			Type type = component.GetType();
			return new Vector3Property
			{
				x = animator.BindStreamProperty(component.transform, type, name + ".x"),
				y = animator.BindStreamProperty(component.transform, type, name + ".y"),
				z = animator.BindStreamProperty(component.transform, type, name + ".z")
			};
		}

		public static Vector3Property BindCustom(Animator animator, string name)
		{
			return new Vector3Property
			{
				x = animator.BindCustomStreamProperty(name + ".x", CustomStreamPropertyType.Float),
				y = animator.BindCustomStreamProperty(name + ".y", CustomStreamPropertyType.Float),
				z = animator.BindCustomStreamProperty(name + ".z", CustomStreamPropertyType.Float)
			};
		}

		public Vector3 Get(AnimationStream stream)
		{
			return new Vector3(this.x.GetFloat(stream), this.y.GetFloat(stream), this.z.GetFloat(stream));
		}

		public void Set(AnimationStream stream, Vector3 value)
		{
			this.x.SetFloat(stream, value.x);
			this.y.SetFloat(stream, value.y);
			this.z.SetFloat(stream, value.z);
		}

		public PropertyStreamHandle x;

		public PropertyStreamHandle y;

		public PropertyStreamHandle z;
	}
}
