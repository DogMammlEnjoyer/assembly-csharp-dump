using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio;
using Modio.Authentication;
using Modio.Mods;
using Modio.Users;
using UnityEngine;
using UnityEngine.UI;

public class ModioUnityExample : MonoBehaviour
{
	private void Awake()
	{
		ModioServices.Bind<IModioAuthService>().FromInstance(new ModioEmailAuthService(new Func<Task<string>>(this.GetAuthCode)), ModioServicePriority.DeveloperOverride, null);
		this.authContainer.SetActive(false);
		this.tosContainer.SetActive(false);
		this.randomContainer.SetActive(false);
	}

	private void Start()
	{
		this.InitPlugin();
	}

	private Task InitPlugin()
	{
		ModioUnityExample.<InitPlugin>d__21 <InitPlugin>d__;
		<InitPlugin>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InitPlugin>d__.<>4__this = this;
		<InitPlugin>d__.<>1__state = -1;
		<InitPlugin>d__.<>t__builder.Start<ModioUnityExample.<InitPlugin>d__21>(ref <InitPlugin>d__);
		return <InitPlugin>d__.<>t__builder.Task;
	}

	private void OnInit()
	{
		if (User.Current.IsAuthenticated)
		{
			this.OnAuth();
			return;
		}
		this.authRequest.onClick.AddListener(delegate()
		{
			this.Authenticate();
		});
	}

	private Task Authenticate()
	{
		ModioUnityExample.<Authenticate>d__23 <Authenticate>d__;
		<Authenticate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<Authenticate>d__.<>4__this = this;
		<Authenticate>d__.<>1__state = -1;
		<Authenticate>d__.<>t__builder.Start<ModioUnityExample.<Authenticate>d__23>(ref <Authenticate>d__);
		return <Authenticate>d__.<>t__builder.Task;
	}

	private Task<string> GetAuthCode()
	{
		ModioUnityExample.<GetAuthCode>d__24 <GetAuthCode>d__;
		<GetAuthCode>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetAuthCode>d__.<>4__this = this;
		<GetAuthCode>d__.<>1__state = -1;
		<GetAuthCode>d__.<>t__builder.Start<ModioUnityExample.<GetAuthCode>d__24>(ref <GetAuthCode>d__);
		return <GetAuthCode>d__.<>t__builder.Task;
	}

	private void OnAuth()
	{
		ModioUnityExample.<OnAuth>d__25 <OnAuth>d__;
		<OnAuth>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnAuth>d__.<>4__this = this;
		<OnAuth>d__.<>1__state = -1;
		<OnAuth>d__.<>t__builder.Start<ModioUnityExample.<OnAuth>d__25>(ref <OnAuth>d__);
	}

	private Task AddModsIfNone()
	{
		ModioUnityExample.<AddModsIfNone>d__26 <AddModsIfNone>d__;
		<AddModsIfNone>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AddModsIfNone>d__.<>4__this = this;
		<AddModsIfNone>d__.<>1__state = -1;
		<AddModsIfNone>d__.<>t__builder.Start<ModioUnityExample.<AddModsIfNone>d__26>(ref <AddModsIfNone>d__);
		return <AddModsIfNone>d__.<>t__builder.Task;
	}

	private Task UploadMod(string modName, string summary, Texture2D logo, string path)
	{
		ModioUnityExample.<UploadMod>d__27 <UploadMod>d__;
		<UploadMod>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UploadMod>d__.modName = modName;
		<UploadMod>d__.summary = summary;
		<UploadMod>d__.logo = logo;
		<UploadMod>d__.path = path;
		<UploadMod>d__.<>1__state = -1;
		<UploadMod>d__.<>t__builder.Start<ModioUnityExample.<UploadMod>d__27>(ref <UploadMod>d__);
		return <UploadMod>d__.<>t__builder.Task;
	}

	private Task<Mod[]> GetAllMods()
	{
		ModioUnityExample.<GetAllMods>d__28 <GetAllMods>d__;
		<GetAllMods>d__.<>t__builder = AsyncTaskMethodBuilder<Mod[]>.Create();
		<GetAllMods>d__.<>1__state = -1;
		<GetAllMods>d__.<>t__builder.Start<ModioUnityExample.<GetAllMods>d__28>(ref <GetAllMods>d__);
		return <GetAllMods>d__.<>t__builder.Task;
	}

	private void SetRandomMod()
	{
		ModioUnityExample.<SetRandomMod>d__29 <SetRandomMod>d__;
		<SetRandomMod>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetRandomMod>d__.<>4__this = this;
		<SetRandomMod>d__.<>1__state = -1;
		<SetRandomMod>d__.<>t__builder.Start<ModioUnityExample.<SetRandomMod>d__29>(ref <SetRandomMod>d__);
	}

	private static Mod[] GetSubscribedMods()
	{
		return User.Current.ModRepository.GetSubscribed().ToArray<Mod>();
	}

	private Task SubscribeToMod(Mod mod)
	{
		ModioUnityExample.<SubscribeToMod>d__31 <SubscribeToMod>d__;
		<SubscribeToMod>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SubscribeToMod>d__.mod = mod;
		<SubscribeToMod>d__.<>1__state = -1;
		<SubscribeToMod>d__.<>t__builder.Start<ModioUnityExample.<SubscribeToMod>d__31>(ref <SubscribeToMod>d__);
		return <SubscribeToMod>d__.<>t__builder.Task;
	}

	private void WakeUpModManagement()
	{
		ModInstallationManagement.ManagementEvents += this.<WakeUpModManagement>g__HandleModManagementEvent|32_0;
	}

	private void Update()
	{
		if (this.currentDownload == null)
		{
			return;
		}
		this.timeToProgressCheck -= Time.deltaTime;
		if (this.timeToProgressCheck > 0f)
		{
			return;
		}
		Debug.Log(string.Format("Downloading {0}: [{1}%]", this.currentDownload.Name, Mathf.RoundToInt(this.currentDownload.File.FileStateProgress * 100f)));
		this.timeToProgressCheck += 1f;
	}

	private Task<ModioUnityExample.DummyModData> GenerateDummyMod(string dummyName, string summary, string backgroundColor, string textColor, int megabytes)
	{
		ModioUnityExample.<GenerateDummyMod>d__34 <GenerateDummyMod>d__;
		<GenerateDummyMod>d__.<>t__builder = AsyncTaskMethodBuilder<ModioUnityExample.DummyModData>.Create();
		<GenerateDummyMod>d__.<>4__this = this;
		<GenerateDummyMod>d__.dummyName = dummyName;
		<GenerateDummyMod>d__.summary = summary;
		<GenerateDummyMod>d__.backgroundColor = backgroundColor;
		<GenerateDummyMod>d__.textColor = textColor;
		<GenerateDummyMod>d__.megabytes = megabytes;
		<GenerateDummyMod>d__.<>1__state = -1;
		<GenerateDummyMod>d__.<>t__builder.Start<ModioUnityExample.<GenerateDummyMod>d__34>(ref <GenerateDummyMod>d__);
		return <GenerateDummyMod>d__.<>t__builder.Task;
	}

	private Task<Texture2D> GenerateLogo(string text, string backgroundColor, string textColor)
	{
		ModioUnityExample.<GenerateLogo>d__35 <GenerateLogo>d__;
		<GenerateLogo>d__.<>t__builder = AsyncTaskMethodBuilder<Texture2D>.Create();
		<GenerateLogo>d__.text = text;
		<GenerateLogo>d__.backgroundColor = backgroundColor;
		<GenerateLogo>d__.textColor = textColor;
		<GenerateLogo>d__.<>1__state = -1;
		<GenerateLogo>d__.<>t__builder.Start<ModioUnityExample.<GenerateLogo>d__35>(ref <GenerateLogo>d__);
		return <GenerateLogo>d__.<>t__builder.Task;
	}

	[CompilerGenerated]
	private void <WakeUpModManagement>g__HandleModManagementEvent|32_0(Mod mod, Modfile modfile, ModInstallationManagement.OperationType jobType, ModInstallationManagement.OperationPhase jobPhase)
	{
		Debug.Log(string.Format("{0} {1}: {2}", jobType, jobPhase, mod.Name));
		switch (jobPhase)
		{
		case ModInstallationManagement.OperationPhase.Started:
			if (jobType != ModInstallationManagement.OperationType.Uninstall)
			{
				this.currentDownload = mod;
				return;
			}
			break;
		case ModInstallationManagement.OperationPhase.Completed:
			if (jobType != ModInstallationManagement.OperationType.Uninstall)
			{
				Debug.Log("Mod " + mod.Name + " installed at " + mod.File.InstallLocation);
				this.currentDownload = null;
				return;
			}
			Debug.Log("Mod " + mod.Name + " uninstalled");
			break;
		case ModInstallationManagement.OperationPhase.Cancelled:
		case ModInstallationManagement.OperationPhase.Failed:
			this.currentDownload = null;
			return;
		default:
			return;
		}
	}

	private static readonly byte[] Megabyte = new byte[1048576];

	private static readonly Random RandomBytes = new Random();

	[Header("Authentication")]
	[SerializeField]
	private GameObject authContainer;

	[SerializeField]
	private InputField authInput;

	[SerializeField]
	private Button authRequest;

	[SerializeField]
	private Button authSubmit;

	[Header("Terms of Use")]
	[SerializeField]
	private GameObject tosContainer;

	[SerializeField]
	private Button termsLink;

	[SerializeField]
	private Button privacyLink;

	[SerializeField]
	private Button denyButton;

	[SerializeField]
	private Button acceptButton;

	[Header("Random Mod")]
	[SerializeField]
	private GameObject randomContainer;

	[SerializeField]
	private Text randomName;

	[SerializeField]
	private Image randomLogo;

	[SerializeField]
	private Button randomButton;

	private Mod[] allMods;

	private Mod currentDownload;

	private float downloadProgress;

	private float timeToProgressCheck = 1f;

	private readonly struct DummyModData
	{
		public DummyModData(string name, string summary, Texture2D logo, string path)
		{
			this.name = name;
			this.summary = summary;
			this.logo = logo;
			this.path = path;
		}

		public readonly string name;

		public readonly string summary;

		public readonly Texture2D logo;

		public readonly string path;
	}
}
