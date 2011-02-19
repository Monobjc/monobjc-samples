using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.Fireworks
{
	[ObjectiveCClass]
	public partial class FireworksAppDelegate : NSObject
	{
		public static readonly Class FireworksAppDelegateClass = Class.Get (typeof(FireworksAppDelegate));

		public FireworksAppDelegate ()
		{
		}

		public FireworksAppDelegate (IntPtr nativePointer) : base(nativePointer)
		{
		}
		
		[ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
		public bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}
		
		[ObjectiveCMessage("applicationShouldTerminate:")]
		public NSApplicationTerminateReply ApplicationShouldTerminate (NSApplication theApplication)
		{
			return NSApplicationTerminateReply.NSTerminateNow;
		}
	}
}
