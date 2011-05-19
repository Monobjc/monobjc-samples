using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.BoingX
{
	[ObjectiveCClass]
	public partial class BoingController : NSObject
	{
		public static readonly Class BoingControllerClass = Class.Get (typeof(BoingController));
		
		private NSTimer timer;
		private NSWindow window;
		private BoingView view;

		public BoingController ()
		{
		}

		public BoingController (IntPtr nativePointer) : base(nativePointer)
		{
		}
		
		[ObjectiveCMessage("applicationWillFinishLaunching:")]
		public virtual void ApplicationWillFinishLaunching (NSNotification aNotification)
		{
			this.view = new BoingView(new NSRect(0, 0, 640, 480));
			this.window = new BoingWindow(new NSRect(100, 100, 640, 480), NSWindowStyleMask.NSBorderlessWindowMask, NSBackingStoreType.NSBackingStoreBuffered, false);
			this.window.ContentView = this.view;
			this.view.Release();
			
			this.window.MakeKeyAndOrderFront(null);
			
			this.timer = NSTimer.ScheduledTimerWithTimeIntervalTargetSelectorUserInfoRepeats(1.0/60.0, this, ObjectiveCRuntime.Selector("timerFired:"), null, true);
			this.timer.Retain();
		}
		
		[ObjectiveCMessage("timerFired:")]
		public void TimerFired(NSTimer aTimer)
		{
			this.view.Animate();
		}
	}
}
