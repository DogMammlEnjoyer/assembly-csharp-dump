using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Modio.Customizations
{
	internal static class Wss
	{
		public static Task<ValueTuple<Error, ExternalAuthenticationToken>> BeginAuthenticationProcess(bool restartProcess = false)
		{
			Wss.<BeginAuthenticationProcess>d__0 <BeginAuthenticationProcess>d__;
			<BeginAuthenticationProcess>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ExternalAuthenticationToken>>.Create();
			<BeginAuthenticationProcess>d__.<>1__state = -1;
			<BeginAuthenticationProcess>d__.<>t__builder.Start<Wss.<BeginAuthenticationProcess>d__0>(ref <BeginAuthenticationProcess>d__);
			return <BeginAuthenticationProcess>d__.<>t__builder.Task;
		}

		private static Task<ValueTuple<Error, WssLoginSuccess>> WaitForAccessToken()
		{
			Wss.<WaitForAccessToken>d__1 <WaitForAccessToken>d__;
			<WaitForAccessToken>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, WssLoginSuccess>>.Create();
			<WaitForAccessToken>d__.<>1__state = -1;
			<WaitForAccessToken>d__.<>t__builder.Start<Wss.<WaitForAccessToken>d__1>(ref <WaitForAccessToken>d__);
			return <WaitForAccessToken>d__.<>t__builder.Task;
		}
	}
}
