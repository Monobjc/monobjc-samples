using System;
using Monobjc.AppKit;

namespace Monobjc.Samples.ContentBurn
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
			ObjectiveCRuntime.LoadFramework("DiscRecording");
			ObjectiveCRuntime.LoadFramework("DiscRecordingUI");
			ObjectiveCRuntime.LoadFramework("Foundation");
			#endregion
			
			ObjectiveCRuntime.Initialize ();
			
			NSApplication.Bootstrap ();
			NSApplication.LoadNib ("MainMenu.nib");
			NSApplication.RunApplication ();
		}
    }
}
