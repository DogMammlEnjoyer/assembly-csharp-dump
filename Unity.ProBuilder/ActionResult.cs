using System;

namespace UnityEngine.ProBuilder
{
	public sealed class ActionResult
	{
		public ActionResult.Status status { get; private set; }

		public string notification { get; private set; }

		public ActionResult(ActionResult.Status status, string notification)
		{
			this.status = status;
			this.notification = notification;
		}

		public static implicit operator bool(ActionResult res)
		{
			return res != null && res.status == ActionResult.Status.Success;
		}

		public bool ToBool()
		{
			return this.status == ActionResult.Status.Success;
		}

		public static bool FromBool(bool success)
		{
			return success ? ActionResult.Success : new ActionResult(ActionResult.Status.Failure, "Failure");
		}

		public static ActionResult Success
		{
			get
			{
				return new ActionResult(ActionResult.Status.Success, "");
			}
		}

		public static ActionResult NoSelection
		{
			get
			{
				return new ActionResult(ActionResult.Status.Canceled, "Nothing Selected");
			}
		}

		public static ActionResult UserCanceled
		{
			get
			{
				return new ActionResult(ActionResult.Status.Canceled, "User Canceled");
			}
		}

		public enum Status
		{
			Success,
			Failure,
			Canceled,
			NoChange
		}
	}
}
