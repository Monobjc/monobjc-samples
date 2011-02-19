using System;
using System.Threading;
using Monobjc.AppKit;
using Monobjc.Foundation;

/*
 * Original ray tracing code from Luke Hoban.
 * http://blogs.msdn.com/lukeh/archive/2007/04/03/a-ray-tracer-in-c-3-0.aspx
 */

namespace Monobjc.Samples.RayTracer
{
	[ObjectiveCClass]
	public partial class AppController : NSObject
	{
		private const int width = 600;
		private const int height = 600;

		private NSBitmapImageRep imageRep;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppController"/> class.
		/// </summary>
		public AppController ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppController"/> class.
		/// </summary>
		/// <param name="nativePointer">The native pointer.</param>
		public AppController (IntPtr nativePointer) : base(nativePointer)
		{
		}

		[ObjectiveCMessage("applicationDidFinishLaunching:")]
		public void ApplicationDidFinishLaunching (NSNotification notification)
		{
			NSThread.MakeMultiThreaded ();
			
			this.imageRep = new NSBitmapImageRep (IntPtr.Zero, width, height, 8, 4, true, false, "NSCalibratedRGBColorSpace", 4 * width, 32);
			NSImage image = new NSImage (new NSSize (width, height));
			image.AddRepresentation (this.imageRep);
			this.imageView.Image = image;
			
			Thread t = new Thread (this.DoComputation);
			t.IsBackground = true;
			t.Start ();
		}

		private void DoComputation ()
		{
			NSAutoreleasePool pool = new NSAutoreleasePool ();
			
			RayTracer rayTracer = new RayTracer (width, height, (int x, int y, NSColor color) =>
			{
				this.imageRep.SetColorAtXy (color, x, y);
				if (x == 0) {
					this.imageView.PerformSelectorOnMainThreadWithObjectWaitUntilDone (ObjectiveCRuntime.Selector ("setNeedsDisplay"), null, false);
				}
			});
			rayTracer.Render (rayTracer.DefaultScene);
			
			this.imageView.PerformSelectorOnMainThreadWithObjectWaitUntilDone (ObjectiveCRuntime.Selector ("setNeedsDisplay"), null, false);
			
			pool.Release ();
		}
	}
}
