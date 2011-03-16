using System;
using Monobjc.AppKit;

namespace Monobjc.Samples.PDFKitViewer
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
			ObjectiveCRuntime.LoadFramework("SM2DGraphView");
			#endregion
			
			ObjectiveCRuntime.Initialize ();
			
			NSApplication.Bootstrap ();
			NSApplication.LoadNib ("GraphingAppMain.nib");
			NSApplication.RunApplication ();
		}
    }
}
