using System;

namespace Sirenix.OdinInspector
{
	public struct SelfFix
	{
		public SelfFix(string name, Action action, bool offerInInspector)
		{
			this.Title = name;
			this.Action = action;
			this.OfferInInspector = offerInInspector;
		}

		public SelfFix(string name, Delegate action, bool offerInInspector)
		{
			this.Title = name;
			this.Action = action;
			this.OfferInInspector = offerInInspector;
		}

		public static SelfFix Create(Action action, bool offerInInspector = true)
		{
			return new SelfFix("Fix", action, offerInInspector);
		}

		public static SelfFix Create(string title, Action action, bool offerInInspector = true)
		{
			return new SelfFix(title, action, offerInInspector);
		}

		public static SelfFix Create<T>(Action<T> action, bool offerInInspector = true) where T : new()
		{
			return new SelfFix("Fix", action, offerInInspector);
		}

		public static SelfFix Create<T>(string title, Action<T> action, bool offerInInspector = true) where T : new()
		{
			return new SelfFix(title, action, offerInInspector);
		}

		public string Title;

		public Delegate Action;

		public bool OfferInInspector;
	}
}
