using System;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

namespace Samples.Purchasing.Core.BuyingConsumables
{
	public class IAPManager: MonoBehaviour, IDetailedStoreListener
	{
		IStoreController m_StoreController; // The Unity Purchasing system.

		//Your products IDs. They should match the ids of your products in your store.
		public string productID = "com.mycompany.mygame.gold1";

		TMP_Text Price;
		TMP_Text Amount;

		int m_GoldCount;
		int m_DiamondCount;

		void Start()
		{
			Amount = this.gameObject.transform.parent.transform.Find("Text_Value").GetComponent<TMP_Text>();
			Price = GetComponent<TMP_Text>();
			this.GetComponent<Button>().onClick.RemoveAllListeners();
			this.GetComponent<Button>().onClick.AddListener(() => BuyItem());
			InitializePurchasing();
		}

		void InitializePurchasing()
		{
			var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			builder.AddProduct(productID, ProductType.Consumable);
			UnityPurchasing.Initialize(this, builder);
		}

		public void BuyItem()
		{
			m_StoreController.InitiatePurchase(productID);
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			Debug.Log("In-App Purchasing successfully initialized");
			m_StoreController = controller;
			Product product = m_StoreController.products.WithID(productID);
			Price.text = product.metadata.localizedPriceString.ToString();
			Amount.text = ExtractQuantityFromReceipt(product.receipt).ToString();
		}
		// Helper method to extract the quantity from the product receipt
		private int ExtractQuantityFromReceipt(string receipt)
		{
			// Implement your own logic here to extract the quantity from the receipt
			// This will depend on the specific format or structure of your receipt data

			// For demonstration purposes, let's assume the receipt is a simple string in the format "Quantity: X"
			string[] receiptParts = receipt.Split(':');
			if (receiptParts.Length >= 2)
			{
				string quantityString = receiptParts[1].Trim();
				int quantity;
				if (int.TryParse(quantityString, out quantity))
				{
					return quantity;
				}
			}

			// Return 0 if the quantity cannot be extracted from the receipt
			return 0;
		}
		public void OnInitializeFailed(InitializationFailureReason error)
		{
			OnInitializeFailed(error, null);
		}

		public void OnInitializeFailed(InitializationFailureReason error, string message)
		{
			var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

			if (message != null)
			{
				errorMessage += $" More details: {message}";
			}

			Debug.Log(errorMessage);
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
		{
			//Retrieve the purchased product
			var product = args.purchasedProduct;

			//Add the purchased product to the players inventory
			if (product.definition.id == productID)
			{

				//Add product here
			}

			Debug.Log($"Purchase Complete - Product: {product.definition.id}");

			//We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
			return PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
		{
			Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
				$" Purchase failure reason: {failureDescription.reason}," +
				$" Purchase failure details: {failureDescription.message}");
		}
	}
}
