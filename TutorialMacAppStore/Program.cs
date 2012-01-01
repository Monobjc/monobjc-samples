using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.TutorialMacAppStore
{
	internal class Program
	{
		private static void Main (String[] args)
		{
			#region --- Monobjc Generated Code ---
			//
			// DO NOT ALTER OR REMOVE
			//
			ObjectiveCRuntime.LoadFramework("AppKit");
			ObjectiveCRuntime.LoadFramework("Foundation");
			ObjectiveCRuntime.LoadFramework("StoreKit");
			#endregion
			
			ObjectiveCRuntime.Initialize ();

			// Check the receipt
			ReceiptChecker.Check();
			
			NSApplication.Bootstrap ();
			NSApplication.LoadNib ("MainMenu.nib");
			NSApplication.RunApplication ();
		}
	}
}
