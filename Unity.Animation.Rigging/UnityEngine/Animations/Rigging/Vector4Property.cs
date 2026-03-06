using System;

namespace UnityEngine.Animations.Rigging
{
	public struct Vector4Property : IAnimatableProperty<Vector4>
	{
		public static Vector4Property Bind(Animator animator, Component component, string name)
		{
			Type type = component.GetType();
			return new Vector4Property
			{
				x = animator.BindStreamProperty(component.transform, type, name + ".x"),
				y = animator.BindStreamProperty(component.transform, type, name + ".y"),
				z = animator.BindStreamProperty(component.transform, type, name + ".z"),
				w = animator.BindStreamProperty(component.transform, type, name + ".w")
			};
		}

		public static Vector4Property BindCustom(Animator animator, string name)
		{
			return new Vector4Property
			{
				x = animator.BindCustomStreamProperty(name + ".x", CustomStreamPropertyType.Float),
				y = animator.BindCustomStreamProperty(name + ".y", CustomStreamPropertyType.Float),
				z = animator.BindCustomStreamProperty(name + ".z", CustomStreamPropertyType.Float),
				w = animator.BindCustomStreamProperty(name + ".w", CustomStreamPropertyType.Float)
			};
		}

		public Vector4 Get(AnimationStream stream)
		{
			return new Vector4(this.x.GetFloat(stream), this.y.GetFloat(stream), this.z.GetFloat(stream), this.w.GetFloat(stream));
		}

		public void Set(AnimationStream stream, Vector4 value)
		{
			this.x.SetFloat(stream, value.x);
			this.y.SetFloat(stream, value.y);
			this.z.SetFloat(stream, value.z);
			this.w.SetFloat(stream, value.w);
		}

		public PropertyStreamHandle x;

		public PropertyStreamHandle y;

		public PropertyStreamHandle z;

		public PropertyStreamHandle w;
	}
}
