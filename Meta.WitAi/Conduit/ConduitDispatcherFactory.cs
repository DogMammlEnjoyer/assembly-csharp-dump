using System;

namespace Meta.Conduit
{
	internal class ConduitDispatcherFactory
	{
		public ConduitDispatcherFactory(IInstanceResolver instanceResolver)
		{
			this._instanceResolver = instanceResolver;
		}

		public IConduitDispatcher GetDispatcher()
		{
			return ConduitDispatcherFactory.Instance = (ConduitDispatcherFactory.Instance ?? new ConduitDispatcher(new ManifestLoader(), this._instanceResolver));
		}

		private static IConduitDispatcher Instance;

		private readonly IInstanceResolver _instanceResolver;

		private readonly IParameterProvider _parameterProvider;
	}
}
