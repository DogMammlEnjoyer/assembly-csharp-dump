using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;

public class HowManyMonke : MonoBehaviour
{
	public static float RecheckDelay
	{
		get
		{
			return Mathf.Max((float)HowManyMonke.recheckDelay / 1000f, 1f);
		}
	}

	public void Start()
	{
		HowManyMonke.<Start>d__11 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<HowManyMonke.<Start>d__11>(ref <Start>d__);
	}

	private Task FetchRecheckDelay()
	{
		HowManyMonke.<FetchRecheckDelay>d__12 <FetchRecheckDelay>d__;
		<FetchRecheckDelay>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<FetchRecheckDelay>d__.<>4__this = this;
		<FetchRecheckDelay>d__.<>1__state = -1;
		<FetchRecheckDelay>d__.<>t__builder.Start<HowManyMonke.<FetchRecheckDelay>d__12>(ref <FetchRecheckDelay>d__);
		return <FetchRecheckDelay>d__.<>t__builder.Task;
	}

	private void onTDError(PlayFabError error)
	{
		this.state = HowManyMonke.State.READY;
		HowManyMonke.recheckDelay = 0;
	}

	private void onTD(string obj)
	{
		this.state = HowManyMonke.State.READY;
		if (int.TryParse(obj, out HowManyMonke.recheckDelay))
		{
			HowManyMonke.recheckDelay *= 1000;
			return;
		}
		HowManyMonke.recheckDelay = 0;
	}

	private Task<int> FetchThisMany()
	{
		HowManyMonke.<FetchThisMany>d__15 <FetchThisMany>d__;
		<FetchThisMany>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
		<FetchThisMany>d__.<>4__this = this;
		<FetchThisMany>d__.<>1__state = -1;
		<FetchThisMany>d__.<>t__builder.Start<HowManyMonke.<FetchThisMany>d__15>(ref <FetchThisMany>d__);
		return <FetchThisMany>d__.<>t__builder.Task;
	}

	private const string preLog = "[GT/HowManyMonke]  ";

	private const string preErr = "ERROR!!!  ";

	public static int ThisMany = 12549;

	public static Action<int> OnCheck;

	[SerializeField]
	private string titleDataKey;

	private HowManyMonke.State state;

	private static int recheckDelay;

	[SerializeField]
	private string CCUEndpoint;

	private enum State
	{
		READY,
		TD_LOOKUP,
		HMM_LOOKUP
	}

	private class CCUResponse
	{
		public int CCUTotal;

		public string ErrorMessage;
	}
}
