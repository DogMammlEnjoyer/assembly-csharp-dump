using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio;
using Modio.Images;
using Modio.Mods;
using TMPro;
using UnityEngine;

public class NewMapsDisplay : MonoBehaviour
{
	public void OnEnable()
	{
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
		if (!UGCPermissionManager.IsUGCDisabled)
		{
			if (!ModIOManager.IsInitialized() || !ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
			{
				this.initCoroutine = base.StartCoroutine(this.DelayedInitialize());
			}
			else
			{
				if (this.newMapsModId == ModId.Null)
				{
					return;
				}
				this.Initialize();
			}
		}
		this.loadingText.gameObject.SetActive(true);
	}

	public void OnDisable()
	{
		if (this.initCoroutine != null)
		{
			base.StopCoroutine(this.initCoroutine);
			this.initCoroutine = null;
		}
		this.newMapsModProfile = null;
		this.newMapDatas.Clear();
		this.slideshowActive = false;
		this.slideshowIndex = 0;
		this.lastSlideshowUpdate = 0f;
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		this.loadingText.text = this.loadingString;
		this.loadingText.gameObject.SetActive(false);
		UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
	}

	private void OnUGCEnabled()
	{
		if (this.newMapDatas.IsNullOrEmpty<NewMapsDisplay.NewMapData>())
		{
			if (!ModIOManager.IsInitialized() || !ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
			{
				this.initCoroutine = base.StartCoroutine(this.DelayedInitialize());
				return;
			}
			if (this.newMapsModId == ModId.Null)
			{
				return;
			}
			this.Initialize();
		}
	}

	private void OnUGCDisabled()
	{
		this.mapImage.gameObject.SetActive(false);
		this.mapInfoTMP.text = "";
		this.mapInfoTMP.gameObject.SetActive(false);
		this.loadingText.text = this.ugcDisabledString;
		this.loadingText.gameObject.SetActive(true);
	}

	private IEnumerator DelayedInitialize()
	{
		while (!ModIOManager.TryGetNewMapsModId(out this.newMapsModId))
		{
			yield return new WaitForSecondsRealtime(1f);
		}
		this.initCoroutine = null;
		if (this.newMapsModId == ModId.Null)
		{
			yield break;
		}
		this.Initialize();
		yield break;
	}

	private Task<Error> Initialize()
	{
		NewMapsDisplay.<Initialize>d__28 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<Initialize>d__.<>4__this = this;
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<NewMapsDisplay.<Initialize>d__28>(ref <Initialize>d__);
		return <Initialize>d__.<>t__builder.Task;
	}

	private void StartSlideshow()
	{
		if (this.newMapDatas.IsNullOrEmpty<NewMapsDisplay.NewMapData>())
		{
			return;
		}
		this.slideshowIndex = 0;
		this.slideshowActive = true;
		this.UpdateSlideshow();
	}

	public void Update()
	{
		if (!this.slideshowActive || Time.time - this.lastSlideshowUpdate < this.slideshowUpdateInterval)
		{
			return;
		}
		this.UpdateSlideshow();
	}

	private void UpdateSlideshow()
	{
		this.loadingText.gameObject.SetActive(false);
		this.lastSlideshowUpdate = Time.time;
		Texture2D image = this.newMapDatas[this.slideshowIndex].image;
		if (image != null)
		{
			Sprite sprite;
			if (!this.cachedTextures.TryGetValue(image, out sprite))
			{
				sprite = Sprite.Create(image, new Rect(0f, 0f, (float)image.width, (float)image.height), new Vector2(0.5f, 0.5f));
				this.cachedTextures.Add(image, sprite);
			}
			this.mapImage.sprite = sprite;
			this.mapImage.gameObject.SetActive(true);
		}
		else
		{
			this.mapImage.gameObject.SetActive(false);
		}
		this.mapInfoTMP.text = this.newMapDatas[this.slideshowIndex].info;
		this.mapInfoTMP.gameObject.SetActive(true);
		this.slideshowIndex++;
		if (this.slideshowIndex >= this.newMapDatas.Count)
		{
			this.slideshowIndex = 0;
		}
	}

	[SerializeField]
	private SpriteRenderer mapImage;

	[SerializeField]
	private TMP_Text loadingText;

	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modNameText;

	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	[Tooltip("DEPRECATED")]
	[SerializeField]
	private TMP_Text modCreatorText;

	[SerializeField]
	private TMP_Text mapInfoTMP;

	[SerializeField]
	private float slideshowUpdateInterval = 1f;

	[SerializeField]
	private string loadingString = "LOADING...";

	[SerializeField]
	private string ugcDisabledString = "UGC DISABLED BY K-ID SETTINGS";

	private ModId newMapsModId = ModId.Null;

	private Mod newMapsModProfile;

	private List<NewMapsDisplay.NewMapData> newMapDatas = new List<NewMapsDisplay.NewMapData>();

	private bool slideshowActive;

	private int slideshowIndex;

	private float lastSlideshowUpdate;

	private bool requestingNewMapsModProfile;

	private LazyImage<Texture2D> lazyImage;

	private bool downloadingImages;

	private bool downloadingImage;

	private Texture2D lastDownloadedImage;

	private Coroutine initCoroutine;

	private Dictionary<Texture2D, Sprite> cachedTextures = new Dictionary<Texture2D, Sprite>();

	private struct NewMapData
	{
		public Texture2D image;

		public string info;
	}
}
