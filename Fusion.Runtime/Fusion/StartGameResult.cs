using System;
using Fusion.Photon.Realtime.Async;

namespace Fusion
{
	public class StartGameResult
	{
		public bool Ok
		{
			get
			{
				return this.ShutdownReason == ShutdownReason.Ok;
			}
		}

		public ShutdownReason ShutdownReason { get; private set; }

		public string ErrorMessage { get; private set; }

		public string StackTrace { get; private set; }

		internal StartGameResult(ShutdownReason reason = ShutdownReason.Ok, string message = null, string stackTrace = null)
		{
			this.ShutdownReason = reason;
			this.ErrorMessage = (message ?? reason.ToString());
			this.StackTrace = stackTrace;
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}:{2}, {3}: {4}, {5}={6}, {7}={8}]", new object[]
			{
				"StartGameResult",
				"Ok",
				this.Ok,
				"ShutdownReason",
				this.ShutdownReason,
				"ErrorMessage",
				this.ErrorMessage,
				"StackTrace",
				this.StackTrace
			});
		}

		internal static StartGameResult BuildGameResultFromException(Exception e)
		{
			StartGameException ex = e as StartGameException;
			ShutdownReason reason;
			if (ex == null)
			{
				DisconnectException ex2 = e as DisconnectException;
				if (ex2 == null)
				{
					if (!(e is AuthenticationFailedException))
					{
						OperationException ex3 = e as OperationException;
						if (ex3 == null)
						{
							if (!(e is OperationStartException))
							{
								if (!(e is OperationTimeoutException) && !(e is TimeoutException))
								{
									if (!(e is OperationCanceledException))
									{
										reason = ShutdownReason.Error;
									}
									else
									{
										reason = ShutdownReason.OperationCanceled;
									}
								}
								else
								{
									reason = ShutdownReason.OperationTimeout;
								}
							}
							else
							{
								reason = ShutdownReason.Error;
							}
						}
						else
						{
							reason = ErrorCodeExt.ConvertToShutdownReason(ex3.ErrorCode);
						}
					}
					else
					{
						reason = ShutdownReason.CustomAuthenticationFailed;
					}
				}
				else
				{
					reason = DisconnectCauseExt.ConvertToShutdownReason(ex2.Cause);
				}
			}
			else
			{
				reason = ex.ShutdownReason;
			}
			return new StartGameResult(reason, e.Message, e.StackTrace);
		}
	}
}
