using System;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class ItemPackageSpawner : MonoBehaviour
	{
		public ItemPackage itemPackage
		{
			get
			{
				return this._itemPackage;
			}
			set
			{
				this.CreatePreviewObject();
			}
		}

		private void CreatePreviewObject()
		{
			if (!this.useItemPackagePreview)
			{
				return;
			}
			this.ClearPreview();
			if (this.useItemPackagePreview)
			{
				if (this.itemPackage == null)
				{
					return;
				}
				if (!this.useFadedPreview)
				{
					if (this.itemPackage.previewPrefab != null)
					{
						this.previewObject = Object.Instantiate<GameObject>(this.itemPackage.previewPrefab, base.transform.position, Quaternion.identity);
						this.previewObject.transform.parent = base.transform;
						this.previewObject.transform.localRotation = Quaternion.identity;
						return;
					}
				}
				else if (this.itemPackage.fadedPreviewPrefab != null)
				{
					this.previewObject = Object.Instantiate<GameObject>(this.itemPackage.fadedPreviewPrefab, base.transform.position, Quaternion.identity);
					this.previewObject.transform.parent = base.transform;
					this.previewObject.transform.localRotation = Quaternion.identity;
				}
			}
		}

		private void Start()
		{
			this.VerifyItemPackage();
		}

		private void VerifyItemPackage()
		{
			if (this.itemPackage == null)
			{
				this.ItemPackageNotValid();
			}
			if (this.itemPackage.itemPrefab == null)
			{
				this.ItemPackageNotValid();
			}
		}

		private void ItemPackageNotValid()
		{
			Debug.LogError("<b>[SteamVR Interaction]</b> ItemPackage assigned to " + base.gameObject.name + " is not valid. Destroying this game object.", this);
			Object.Destroy(base.gameObject);
		}

		private void ClearPreview()
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (Time.time > 0f)
				{
					Object.Destroy(transform.gameObject);
				}
				else
				{
					Object.DestroyImmediate(transform.gameObject);
				}
			}
		}

		private void Update()
		{
			if (this.itemIsSpawned && this.spawnedItem == null)
			{
				this.itemIsSpawned = false;
				this.useFadedPreview = false;
				this.dropEvent.Invoke();
				this.CreatePreviewObject();
			}
		}

		private void OnHandHoverBegin(Hand hand)
		{
			if (this.GetAttachedItemPackage(hand) == this.itemPackage && this.takeBackItem && !this.requireReleaseActionToReturn)
			{
				this.TakeBackItem(hand);
			}
			if (!this.requireGrabActionToTake)
			{
				this.SpawnAndAttachObject(hand, GrabTypes.Scripted);
			}
			if (this.requireGrabActionToTake && this.showTriggerHint)
			{
				hand.ShowGrabHint("PickUp");
			}
		}

		private void TakeBackItem(Hand hand)
		{
			this.RemoveMatchingItemsFromHandStack(this.itemPackage, hand);
			if (this.itemPackage.packageType == ItemPackage.ItemPackageType.TwoHanded)
			{
				this.RemoveMatchingItemsFromHandStack(this.itemPackage, hand.otherHand);
			}
		}

		private ItemPackage GetAttachedItemPackage(Hand hand)
		{
			if (hand.currentAttachedObject == null)
			{
				return null;
			}
			ItemPackageReference component = hand.currentAttachedObject.GetComponent<ItemPackageReference>();
			if (component == null)
			{
				return null;
			}
			return component.itemPackage;
		}

		private void HandHoverUpdate(Hand hand)
		{
			if (this.takeBackItem && this.requireReleaseActionToReturn && hand.isActive)
			{
				ItemPackage attachedItemPackage = this.GetAttachedItemPackage(hand);
				if (attachedItemPackage == this.itemPackage && hand.IsGrabEnding(attachedItemPackage.gameObject))
				{
					this.TakeBackItem(hand);
					return;
				}
			}
			if (this.requireGrabActionToTake && hand.GetGrabStarting(GrabTypes.None) != GrabTypes.None)
			{
				this.SpawnAndAttachObject(hand, GrabTypes.Scripted);
			}
		}

		private void OnHandHoverEnd(Hand hand)
		{
			if (!this.justPickedUpItem && this.requireGrabActionToTake && this.showTriggerHint)
			{
				hand.HideGrabHint();
			}
			this.justPickedUpItem = false;
		}

		private void RemoveMatchingItemsFromHandStack(ItemPackage package, Hand hand)
		{
			if (hand == null)
			{
				return;
			}
			for (int i = 0; i < hand.AttachedObjects.Count; i++)
			{
				ItemPackageReference component = hand.AttachedObjects[i].attachedObject.GetComponent<ItemPackageReference>();
				if (component != null)
				{
					ItemPackage itemPackage = component.itemPackage;
					if (itemPackage != null && itemPackage == package)
					{
						GameObject attachedObject = hand.AttachedObjects[i].attachedObject;
						hand.DetachObject(attachedObject, true);
					}
				}
			}
		}

		private void RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType packageType, Hand hand)
		{
			for (int i = 0; i < hand.AttachedObjects.Count; i++)
			{
				ItemPackageReference component = hand.AttachedObjects[i].attachedObject.GetComponent<ItemPackageReference>();
				if (component != null && component.itemPackage.packageType == packageType)
				{
					GameObject attachedObject = hand.AttachedObjects[i].attachedObject;
					hand.DetachObject(attachedObject, true);
				}
			}
		}

		private void SpawnAndAttachObject(Hand hand, GrabTypes grabType)
		{
			if (hand.otherHand != null && this.GetAttachedItemPackage(hand.otherHand) == this.itemPackage)
			{
				this.TakeBackItem(hand.otherHand);
			}
			if (this.showTriggerHint)
			{
				hand.HideGrabHint();
			}
			if (this.itemPackage.otherHandItemPrefab != null && hand.otherHand.hoverLocked)
			{
				Debug.Log("<b>[SteamVR Interaction]</b> Not attaching objects because other hand is hoverlocked and we can't deliver both items.");
				return;
			}
			if (this.itemPackage.packageType == ItemPackage.ItemPackageType.OneHanded)
			{
				this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.OneHanded, hand);
				this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand);
				this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand.otherHand);
			}
			if (this.itemPackage.packageType == ItemPackage.ItemPackageType.TwoHanded)
			{
				this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.OneHanded, hand);
				this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.OneHanded, hand.otherHand);
				this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand);
				this.RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand.otherHand);
			}
			this.spawnedItem = Object.Instantiate<GameObject>(this.itemPackage.itemPrefab);
			this.spawnedItem.SetActive(true);
			hand.AttachObject(this.spawnedItem, grabType, this.attachmentFlags, null);
			if (this.itemPackage.otherHandItemPrefab != null && hand.otherHand.isActive)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.itemPackage.otherHandItemPrefab);
				gameObject.SetActive(true);
				hand.otherHand.AttachObject(gameObject, grabType, this.attachmentFlags, null);
			}
			this.itemIsSpawned = true;
			this.justPickedUpItem = true;
			if (this.takeBackItem)
			{
				this.useFadedPreview = true;
				this.pickupEvent.Invoke();
				this.CreatePreviewObject();
			}
		}

		public ItemPackage _itemPackage;

		public bool useItemPackagePreview = true;

		private bool useFadedPreview;

		private GameObject previewObject;

		public bool requireGrabActionToTake;

		public bool requireReleaseActionToReturn;

		public bool showTriggerHint;

		[EnumFlags]
		public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachOthers | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic;

		public bool takeBackItem;

		public bool acceptDifferentItems;

		private GameObject spawnedItem;

		private bool itemIsSpawned;

		public UnityEvent pickupEvent;

		public UnityEvent dropEvent;

		public bool justPickedUpItem;
	}
}
