using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.StoreKit;

namespace Monobjc.Samples.TutorialMacAppStore
{
	[ObjectiveCClass]
	public partial class AppDelegate : NSObject
	{
		public static readonly Class AppDelegateClass = Class.Get (typeof(AppDelegate));
		
		public AppDelegate ()
		{
		}
		
		public AppDelegate (IntPtr nativePointer) : base(nativePointer)
		{
		}
		
		[ObjectiveCMessage("awakeFromNib")]
		public void AwakeFromNib ()
		{
			// Do anything you want here
		}

		[ObjectiveCMessage ("applicationDidFinishLaunching:")]
		public virtual void ApplicationDidFinishLaunching (NSNotification aNotification)
		{
			if (SKPaymentQueue.CanMakePayments) {
				SKPaymentQueue.DefaultQueue.AddTransactionObserver(this);
			}
			
			// Uncomment to enable product retrieval
			//this.RequestProducts();
		}

		[ObjectiveCMessage ("applicationShouldTerminateAfterLastWindowClosed:")]
		public virtual bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}

		[ObjectiveCMessage ("paymentQueue:removedTransactions:")]
		public virtual void PaymentQueueRemovedTransactions (SKPaymentQueue queue, NSArray transactions)
		{
			// TODO
		}

		[ObjectiveCMessage ("paymentQueue:restoreCompletedTransactionsFailedWithError:")]
		public virtual void PaymentQueueRestoreCompletedTransactionsFailedWithError (SKPaymentQueue queue, NSError error)
		{
			// TODO
		}

		[ObjectiveCMessage ("paymentQueueRestoreCompletedTransactionsFinished:")]
		public virtual void PaymentQueueRestoreCompletedTransactionsFinished (SKPaymentQueue queue)
		{
			// TODO
		}

		[ObjectiveCMessage ("paymentQueue:updatedTransactions:")]
		public virtual void PaymentQueueUpdatedTransactions (SKPaymentQueue queue, NSArray transactions)
		{
			// TODO
		}
		
		[ObjectiveCMessage ("productsRequest:didReceiveResponse:")]
		public virtual void ProductsRequestDidReceiveResponse (SKProductsRequest request, SKProductsResponse response)
		{
			NSArray products = response.Products;
			NSArray invalidProducts = response.InvalidProductIdentifiers;
			
			foreach(SKProduct product in products.GetEnumerator<SKProduct>()) {
				Console.WriteLine(product.ProductIdentifier);
				Console.WriteLine(product.LocalizedTitle);
				Console.WriteLine(product.LocalizedDescription);
				Console.WriteLine(product.Price);
			}
			
			request.Autorelease();
		}
		
		private void RequestProducts()
		{
			// List all the products to retrieve
			SKProductsRequest request = new SKProductsRequest(NSSet.SetWithObjects((NSString)"CONSUMABLE", (NSString)"NON_CONSUMABLE", null));
			request.Delegate = this;
			request.Start();
		}
	}
}
