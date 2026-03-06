using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oculus.Interaction
{
	public class UniqueIdentifier
	{
		public int ID { get; private set; }

		private UniqueIdentifier(int identifier, Context context)
		{
			this.ID = identifier;
			this._context = context;
		}

		[Obsolete]
		public static UniqueIdentifier Generate()
		{
			int num;
			do
			{
				num = UniqueIdentifier.Random.Next(int.MaxValue);
			}
			while (UniqueIdentifier._identifierSet.Contains(num));
			UniqueIdentifier._identifierSet.Add(num);
			return new UniqueIdentifier(num, Context.Global.GetInstance());
		}

		public static UniqueIdentifier Generate(Context context, object instance)
		{
			int num;
			do
			{
				num = UniqueIdentifier.Random.Next(int.MaxValue);
			}
			while (UniqueIdentifier._identifierSet.Contains(num));
			UniqueIdentifier._identifierSet.Add(num);
			UniqueIdentifier.Decorator.GetFromContext(context).AddDecoration(num, instance);
			return new UniqueIdentifier(num, context);
		}

		public static void Release(UniqueIdentifier identifier)
		{
			UniqueIdentifier._identifierSet.Remove(identifier.ID);
			UniqueIdentifier.Decorator.GetFromContext(identifier._context).RemoveDecoration(identifier.ID);
		}

		public static bool TryGetInstanceFromIdentifier(Context context, int identifier, out object instance)
		{
			return UniqueIdentifier.Decorator.GetFromContext(context).TryGetDecoration(identifier, out instance);
		}

		public static Task<object> GetInstanceFromIdentifierAsync(Context context, int identifier)
		{
			return UniqueIdentifier.Decorator.GetFromContext(context).GetDecorationAsync(identifier);
		}

		private Context _context;

		private static Random Random = new Random();

		private static HashSet<int> _identifierSet = new HashSet<int>();

		private class Decorator : ValueToClassDecorator<int, object>
		{
			private Decorator()
			{
			}

			public static UniqueIdentifier.Decorator GetFromContext(Context context)
			{
				return context.GetOrCreateSingleton<UniqueIdentifier.Decorator>(() => new UniqueIdentifier.Decorator());
			}
		}
	}
}
