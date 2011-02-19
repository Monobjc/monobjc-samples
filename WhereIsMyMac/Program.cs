using System;
using Monobjc.AppKit;

namespace Monobjc.Samples.WhereIsMyMac
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
			ObjectiveCRuntime.LoadFramework("CoreLocation");
			ObjectiveCRuntime.LoadFramework("Foundation");
			ObjectiveCRuntime.LoadFramework("WebKit");
			#endregion
			
			ObjectiveCRuntime.Initialize ();
			
			NSApplication.Bootstrap ();
			NSApplication.LoadNib ("MainMenu.nib");
			NSApplication.RunApplication ();
		}
    }
}
