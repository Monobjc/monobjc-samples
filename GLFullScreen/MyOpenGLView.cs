using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.CoreFoundation;
using Monobjc.Foundation;
using Monobjc.QuartzCore;
using Monobjc.OpenGL;

namespace GLFullScreen
{
	[ObjectiveCClass]
	public partial class MyOpenGLView : NSView
	{
		public static readonly Class MyOpenGLViewClass = Class.Get (typeof(MyOpenGLView));

		private NSOpenGLContext openGLContext;
		private NSOpenGLPixelFormat pixelFormat;

		private static CVDisplayLinkOutputCallback CALLBACK = new CVDisplayLinkOutputCallback (MyDisplayLinkCallback);

		private MainController controller;

		private IntPtr displayLink;
		private bool isAnimating;

		public MyOpenGLView ()
		{
		}

		public MyOpenGLView (IntPtr nativePointer) : base(nativePointer)
		{
		}

		public MyOpenGLView (NSRect frameRect) : base(frameRect)
		{
		}

		public MyOpenGLView (NSRect frameRect, NSOpenGLContext context) : base(MyOpenGLViewClass.SendMessage<IntPtr>("alloc"))
		{
			this.NativePointer = this.SendMessage<IntPtr>("initWithFrame:shareContext:", frameRect, context);
		}
		
		public NSOpenGLContext OpenGLContext {
			get { return this.openGLContext; }
		}

		public NSOpenGLPixelFormat PixelFormat {
			get { return this.pixelFormat; }
		}

		public MainController MainController {
			get { return this.controller; }
			set { this.controller = value; }
		}

		private CVReturn GetFrameForTime (ref CVTimeStamp outputTime)
		{
			using (NSAutoreleasePool pool = new NSAutoreleasePool ()) {
				double current = DateTime.Now.TimeOfDay.TotalMilliseconds;
				
				this.controller.Scene.AdvanceTimeBy ((float)(current - controller.RenderTime));
				this.controller.RenderTime = current;
				
				this.DrawView ();
			}
			
			return CVReturn.kCVReturnSuccess;
		}

		private static CVReturn MyDisplayLinkCallback (IntPtr displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn, ref CVOptionFlags flagsOut, IntPtr context)
		{
			//Console.WriteLine("MyDisplayLinkCallback " + inOutputTime.videoTime);
			MyOpenGLView view = ObjectiveCRuntime.GetInstance<MyOpenGLView> (context);
			return view.GetFrameForTime (ref inOutputTime);
		}

		private void SetupDisplayLink ()
		{
			// Create a display link capable of being used with all active displays
			CVDisplayLink.CreateWithActiveCGDisplays (ref this.displayLink);
			
			// Set the renderer output callback function
			CVDisplayLink.SetOutputCallback (this.displayLink, CALLBACK, this.NativePointer);
			
			// Set the display link for the current renderer
			IntPtr cglContext = this.OpenGLContext.CGLContextObj;
			IntPtr cglPixelFormat = this.PixelFormat.CGLPixelFormatObj;
			CVDisplayLink.SetCurrentCGDisplayFromOpenGLContext (this.displayLink, cglContext, cglPixelFormat);
		}

		[ObjectiveCMessage("initWithFrame:shareContext:")]
		public Id InitWithFrameShareContext (NSRect frameRect, NSOpenGLContext context)
		{
			Object[] attribs = new Object[] { NSOpenGLPixelFormatAttribute.NSOpenGLPFAAccelerated, NSOpenGLPixelFormatAttribute.NSOpenGLPFANoRecovery, NSOpenGLPixelFormatAttribute.NSOpenGLPFADoubleBuffer, NSOpenGLPixelFormatAttribute.NSOpenGLPFAColorSize, 24, NSOpenGLPixelFormatAttribute.NSOpenGLPFADepthSize, 16, 0 };
			
			this.pixelFormat = new NSOpenGLPixelFormat (attribs);
			
			// NSOpenGLView does not handle context sharing, so we draw to a custom NSView instead
			this.openGLContext = new NSOpenGLContext (this.pixelFormat, context);
			
			this.SendMessageSuper<IntPtr> (MyOpenGLViewClass, "initWithFrame:", frameRect);
			
			this.openGLContext.MakeCurrentContext ();
			
			// Synchronize buffer swaps with vertical refresh rate
			this.OpenGLContext.SetValuesForParameter (new[] { 1 }, NSOpenGLContextParameter.NSOpenGLCPSwapInterval);
			
			this.SetupDisplayLink ();
			
			// Look for changes in view size
			// Note, -reshape will not be called automatically on size changes because NSView does not export it to override 
			NSNotificationCenter.DefaultCenter.AddObserverSelectorNameObject (this, ObjectiveCRuntime.Selector ("reshape"), NSView.NSViewGlobalFrameDidChangeNotification, this);
			
			return this;
		}

		[ObjectiveCMessage("initWithFrame:")]
		public override Id InitWithFrame (NSRect frameRect)
		{
			this.SendMessage<IntPtr> ("initWithFrame:shareContext:", frameRect, null);
			
			return this;
		}

		[ObjectiveCMessage("lockFocus")]
		public override void LockFocus ()
		{
			this.SendMessageSuper (MyOpenGLViewClass, "lockFocus");
			if (this.openGLContext != null && this.openGLContext.View != this) {
				this.openGLContext.View = this;
			}
		}

		[ObjectiveCMessage("reshape")]
		public void Reshape ()
		{
			// This method will be called on the main thread when resizing, but we may be drawing on a secondary thread through the display link
			// Add a mutex around to avoid the threads accessing the context simultaneously
			CGL.LockContext (this.openGLContext.CGLContextObj);
			
			// Delegate to the scene object to update for a change in the view size
			this.controller.Scene.SetViewportRect (this.Bounds);
			this.openGLContext.Update ();
			
			CGL.UnlockContext (this.openGLContext.CGLContextObj);
		}

		[ObjectiveCMessage("drawRect:")]
		public override void DrawRect (NSRect aRect)
		{
			// Ignore if the display link is still running
			if (!CVDisplayLink.IsRunning (this.displayLink)) {
				this.DrawView ();
			}
		}

		public void DrawView ()
		{
			// This method will be called on both the main thread (through -drawRect:) and a secondary thread (through the display link rendering loop)
			// Also, when resizing the view, -reshape is called on the main thread, but we may be drawing on a secondary thread
			// Add a mutex around to avoid the threads accessing the context simultaneously
			CGL.LockContext (this.openGLContext.CGLContextObj);
			
			// Make sure we draw to the right context
			this.openGLContext.MakeCurrentContext ();
			
			// Delegate to the scene object for rendering
			this.controller.Scene.Render ();
			this.openGLContext.FlushBuffer ();
			
			CGL.UnlockContext (this.openGLContext.CGLContextObj);
		}

		[ObjectiveCMessage("acceptsFirstResponder")]
		public override bool AcceptsFirstResponder ()
		{
			// We want this view to be able to receive key events
			return true;
		}

		[ObjectiveCMessage("keyDown:")]
		public override void KeyDown (NSEvent theEvent)
		{
			// Delegate to the controller object for handling key events
			this.controller.KeyDown (theEvent);
		}

		[ObjectiveCMessage("mouseDown:")]
		public override void MouseDown (NSEvent theEvent)
		{
			// Delegate to the controller object for handling mouse events
			this.controller.MouseDown (theEvent);
		}

		public void StartAnimation ()
		{
			if (this.displayLink != IntPtr.Zero && !CVDisplayLink.IsRunning (this.displayLink)) {
				CVDisplayLink.Start (this.displayLink);
			}
		}

		public void StopAnimation ()
		{
			if (this.displayLink != IntPtr.Zero && CVDisplayLink.IsRunning (this.displayLink)) {
				CVDisplayLink.Stop (this.displayLink);
			}
		}

		[ObjectiveCMessage("dealloc")]
		public override void Dealloc ()
		{
			this.openGLContext.Release ();
			this.pixelFormat.Release ();
			
			CVDisplayLink.Stop (this.displayLink);
			CVDisplayLink.Release (this.displayLink);
			
			NSNotificationCenter.DefaultCenter.RemoveObserverNameObject (this, NSView.NSViewGlobalFrameDidChangeNotification, this);
			
			this.SendMessageSuper (MyOpenGLViewClass, "dealloc");
		}
	}
}
