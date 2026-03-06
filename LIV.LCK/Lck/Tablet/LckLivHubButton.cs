using System;
using System.Collections;
using UnityEngine;

namespace Liv.Lck.Tablet
{
	public class LckLivHubButton : MonoBehaviour
	{
		private void Start()
		{
			if (Application.platform != RuntimePlatform.Android && !Application.isEditor && this._livHubButtonGameObject)
			{
				this._livHubButtonGameObject.SetActive(false);
			}
		}

		public void OpenMetaStoreApp()
		{
			base.StartCoroutine(this.OpenStoreAppCoroutine());
		}

		private IEnumerator OpenStoreAppCoroutine()
		{
			if (Application.platform != RuntimePlatform.Android || Application.isEditor)
			{
				yield break;
			}
			AndroidJavaClass androidJavaClass = null;
			AndroidJavaObject androidJavaObject = null;
			AndroidJavaObject androidJavaObject2 = null;
			AndroidJavaObject androidJavaObject3 = null;
			AndroidJavaObject androidJavaObject4 = null;
			try
			{
				androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				if (androidJavaClass == null)
				{
					throw new Exception("Failed to create UnityPlayer class");
				}
				androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				if (androidJavaObject == null)
				{
					throw new Exception("Failed to get current activity");
				}
				androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageManager", Array.Empty<object>());
				if (androidJavaObject2 == null)
				{
					throw new Exception("Failed to get package manager");
				}
				androidJavaObject4 = androidJavaObject2.Call<AndroidJavaObject>("getLaunchIntentForPackage", new object[]
				{
					"tv.liv.controlcenter"
				});
				if (androidJavaObject4 != null)
				{
					androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getLaunchIntentForPackage", new object[]
					{
						"com.oculus.vrshell"
					});
					if (androidJavaObject3 == null)
					{
						throw new Exception("Failed to find com.oculus.vrshell package.");
					}
					androidJavaObject3.Call<AndroidJavaObject>("putExtra", new object[]
					{
						"intent_data",
						"tv.liv.controlcenter/.MainActivity"
					});
					if (androidJavaObject3 == null)
					{
						throw new Exception("Failed to add extra intent data tv.liv.controlcenter/.MainActivity");
					}
					androidJavaObject3.Call<AndroidJavaObject>("addFlags", new object[]
					{
						268697600
					});
					androidJavaObject.Call("startActivity", new object[]
					{
						androidJavaObject3
					});
				}
				else
				{
					androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getLaunchIntentForPackage", new object[]
					{
						"com.oculus.vrshell"
					});
					if (androidJavaObject3 == null)
					{
						throw new Exception("Failed to set launch intent to com.oculus.vrshell");
					}
					androidJavaObject3.Call<AndroidJavaObject>("putExtra", new object[]
					{
						"intent_data",
						"com.oculus.store/.StoreActivity"
					});
					if (androidJavaObject3 == null)
					{
						throw new Exception("Failed to put extra intent data com.oculus.store/.StoreActivity");
					}
					androidJavaObject3.Call<AndroidJavaObject>("putExtra", new object[]
					{
						"uri",
						"/item/" + 24199129276346881L.ToString()
					});
					if (androidJavaObject3 == null)
					{
						throw new Exception("Failed to put extra intent data appID: " + 24199129276346881L.ToString());
					}
					androidJavaObject.Call("startActivity", new object[]
					{
						androidJavaObject3
					});
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to launch store app: " + ex.Message + "\n" + ex.StackTrace);
			}
			if (androidJavaObject3 != null)
			{
				androidJavaObject3.Dispose();
			}
			if (androidJavaObject4 != null)
			{
				androidJavaObject4.Dispose();
			}
			if (androidJavaObject2 != null)
			{
				androidJavaObject2.Dispose();
			}
			if (androidJavaObject != null)
			{
				androidJavaObject.Dispose();
			}
			if (androidJavaClass != null)
			{
				androidJavaClass.Dispose();
			}
			yield break;
		}

		private const long PRODUCTION_APPID = 24199129276346881L;

		[SerializeField]
		private GameObject _livHubButtonGameObject;
	}
}
