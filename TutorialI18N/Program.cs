using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace TutorialI18N
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
			#endregion
			
			ObjectiveCRuntime.Initialize ();

			NSApplication.Bootstrap ();
			NSApplication.LoadNib ("MainMenu.nib");
			NSApplication.RunApplication ();
		}
	}
}
