using System;

namespace WebSocketSharp.Server
{
	internal class WebSocketServiceHost<TBehavior> : WebSocketServiceHost where TBehavior : WebSocketBehavior, new()
	{
		internal WebSocketServiceHost(string path, Action<TBehavior> initializer, Logger log) : base(path, log)
		{
			this._creator = WebSocketServiceHost<TBehavior>.createSessionCreator(initializer);
		}

		public override Type BehaviorType
		{
			get
			{
				return typeof(TBehavior);
			}
		}

		private static Func<TBehavior> createSessionCreator(Action<TBehavior> initializer)
		{
			bool flag = initializer == null;
			Func<TBehavior> result;
			if (flag)
			{
				result = (() => Activator.CreateInstance<TBehavior>());
			}
			else
			{
				result = delegate()
				{
					TBehavior tbehavior = Activator.CreateInstance<TBehavior>();
					initializer(tbehavior);
					return tbehavior;
				};
			}
			return result;
		}

		protected override WebSocketBehavior CreateSession()
		{
			return this._creator();
		}

		private Func<TBehavior> _creator;
	}
}
