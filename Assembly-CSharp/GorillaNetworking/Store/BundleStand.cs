using System;
using Cosmetics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GorillaNetworking.Store
{
	public class BundleStand : MonoBehaviour, IBuildValidation
	{
		public string playfabBundleID
		{
			get
			{
				return this._bundleDataReference.playfabBundleID;
			}
		}

		bool IBuildValidation.BuildValidationCheck()
		{
			ICreatorCodeProvider creatorCodeProvider;
			if (this.creatorCodeProvider == null || !this.creatorCodeProvider.TryGetComponent<ICreatorCodeProvider>(out creatorCodeProvider))
			{
				Debug.LogError(base.name + " has no Creator Code Provider. This will break bundle purchasing.");
				return false;
			}
			return true;
		}

		public void Awake()
		{
			this._bundlePurchaseButton.playfabID = this.playfabBundleID;
			if (this._bundleIcon != null && this._bundleDataReference != null && this._bundleDataReference.bundleImage != null)
			{
				this._bundleIcon.sprite = this._bundleDataReference.bundleImage;
			}
			this._bundlePurchaseButton.codeProvider = this.creatorCodeProvider.GetComponent<ICreatorCodeProvider>();
		}

		public void InitializeEventListeners()
		{
			this.AlreadyOwnEvent.AddListener(new UnityAction(this._bundlePurchaseButton.AlreadyOwn));
			this.ErrorHappenedEvent.AddListener(new UnityAction(this._bundlePurchaseButton.ErrorHappened));
		}

		public void NotifyAlreadyOwn()
		{
			this.AlreadyOwnEvent.Invoke();
		}

		public void ErrorHappened()
		{
			this.ErrorHappenedEvent.Invoke();
		}

		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (this._bundlePurchaseButton != null)
			{
				this._bundlePurchaseButton.UpdatePurchaseButtonText(purchaseText);
			}
		}

		public void UpdateDescriptionText(string descriptionText)
		{
			if (this._bundleDescriptionText != null)
			{
				this._bundleDescriptionText.text = descriptionText;
			}
		}

		public BundlePurchaseButton _bundlePurchaseButton;

		[SerializeField]
		public StoreBundleData _bundleDataReference;

		[SerializeField]
		private GameObject creatorCodeProvider;

		public GameObject[] EditorOnlyObjects;

		public Text _bundleDescriptionText;

		public Image _bundleIcon;

		public UnityEvent AlreadyOwnEvent;

		public UnityEvent ErrorHappenedEvent;
	}
}
