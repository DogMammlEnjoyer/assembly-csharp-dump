using System;
using System.Threading.Tasks;

namespace Oculus.Interaction
{
	public abstract class ClassToValueDecorator<InstanceT, DecorationT> where InstanceT : class where DecorationT : struct
	{
		protected ClassToValueDecorator()
		{
			this._decorator = new ClassToValueDecorator<InstanceT, DecorationT>.InternalDecorator();
		}

		public void AddDecoration(InstanceT instance, DecorationT decoration)
		{
			this._decorator.AddDecoration(instance, new ClassToValueDecorator<InstanceT, DecorationT>.Wrapper
			{
				_decoration = decoration
			});
		}

		public void RemoveDecoration(InstanceT instance)
		{
			this._decorator.RemoveDecoration(instance);
		}

		public bool TryGetDecoration(InstanceT instance, out DecorationT decoration)
		{
			ClassToValueDecorator<InstanceT, DecorationT>.Wrapper wrapper;
			if (this._decorator.TryGetDecoration(instance, out wrapper))
			{
				decoration = wrapper._decoration;
				return true;
			}
			decoration = default(DecorationT);
			return false;
		}

		public Task<DecorationT> GetDecorationAsync(InstanceT instance)
		{
			return this._decorator.GetDecorationAsync(instance).ContinueWith<DecorationT>((Task<ClassToValueDecorator<InstanceT, DecorationT>.Wrapper> wrapper) => wrapper.Result._decoration, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously);
		}

		private readonly ClassToValueDecorator<InstanceT, DecorationT>.InternalDecorator _decorator;

		private class Wrapper
		{
			public DecorationT _decoration;
		}

		private class InternalDecorator : ClassToClassDecorator<InstanceT, ClassToValueDecorator<InstanceT, DecorationT>.Wrapper>
		{
		}
	}
}
