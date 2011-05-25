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
	public partial class MainController : NSResponder
	{
		public static readonly Class MainControllerClass = Class.Get (typeof(MainController));

		private bool isInFullScreenMode;

		// full-screen mode
		private NSWindow fullScreenWindow;
		private MyOpenGLView fullScreenView;

		private Scene scene;
		private bool isAnimating;
		private double renderTime;

		public MainController ()
		{
		}

		public MainController (IntPtr nativePointer) : base(nativePointer)
		{
		}

		[ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
		public virtual bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}

		partial void GoFullScreen (Id sender)
		{
			this.isInFullScreenMode = true;
			
			// Pause the non-fullscreen view
			this.openGLView.StopAnimation ();
			
			// Mac OS X 10.6 and later offer a simplified mechanism to create full-screen contexts
			NSRect mainDisplayRect, viewRect;
			
			// Create a screen-sized window on the display you want to take over
			// Note, mainDisplayRect has a non-zero origin if the key window is on a secondary display
			mainDisplayRect = NSScreen.MainScreen.Frame;
			this.fullScreenWindow = new NSWindow (mainDisplayRect, NSWindowStyleMask.NSBorderlessWindowMask, NSBackingStoreType.NSBackingStoreBuffered, true);
			
			// Set the window level to be above the menu bar
			this.fullScreenWindow.Level = NSWindow.NSMainMenuWindowLevel + 1;
			
			// Perform any other window configuration you desire
			this.fullScreenWindow.IsOpaque = true;
			this.fullScreenWindow.HidesOnDeactivate = true;
			
			// Create a view with a double-buffered OpenGL context and attach it to the window
			// By specifying the non-fullscreen context as the shareContext, we automatically inherit the OpenGL objects (textures, etc) it has defined
			viewRect = new NSRect (0, 0, mainDisplayRect.size.width, mainDisplayRect.size.height);
			this.fullScreenView = new MyOpenGLView (viewRect, this.openGLView.OpenGLContext);
			this.fullScreenWindow.ContentView = this.fullScreenView;
			
			// Set the scene with the full-screen viewport and viewing transformation
			this.scene.SetViewportRect (viewRect);
			
			// Assign the view's MainController to self
			this.fullScreenView.MainController = this;
			
			// Show the window
			this.fullScreenWindow.MakeKeyAndOrderFront (this);
			
			if (!this.isAnimating) {
				// Mark the view as needing drawing to initalize its contents
				this.fullScreenView.NeedsDisplay = true;
			} else {
				// Start playing the animation
				this.fullScreenView.StartAnimation ();
			}
		}

		public void GoWindow ()
		{
			this.isInFullScreenMode = false;
			
			// Release the screen-sized window and view
			this.fullScreenWindow.Release ();
			this.fullScreenView.Release ();
			
			// Switch to the non-fullscreen context
			this.openGLView.OpenGLContext.MakeCurrentContext ();
			
			if (!this.isAnimating) {
				// Mark the view as needing drawing
				// The animation has advanced while we were in full-screen mode, so its current contents are stale
				this.openGLView.NeedsDisplay = true;
			} else {
				// Continue playing the animation
				this.openGLView.StartAnimation ();
			}
		}

		[ObjectiveCMessage("awakeFromNib")]
		public void AwakeFromNib ()
		{
			// Allocate the scene object
			this.scene = new Scene ();
			
			// Assign the view's MainController to self
			this.openGLView.MainController = this;
			
			// Activate the display link now
			this.openGLView.StartAnimation ();
			this.isAnimating = true;
		}

		[ObjectiveCMessage("dealloc")]
		public override void Dealloc ()
		{
			this.scene.Release ();
			this.SendMessageSuper (MainControllerClass, "dealloc");
		}

		public Scene Scene {
			get { return this.scene; }
			set { this.scene = value; }
		}

		public double RenderTime {
			get { return this.renderTime; }
			set { this.renderTime = value; }
		}

		public void StartAnimation ()
		{
			if (!isAnimating) {
				if (!this.isInFullScreenMode) {
					this.openGLView.StartAnimation ();
				} else {
					this.fullScreenView.StartAnimation ();
				}
				
				this.isAnimating = true;
			}
		}

		public void StopAnimation ()
		{
			if (isAnimating) {
				if (!this.isInFullScreenMode) {
					this.openGLView.StopAnimation ();
				} else {
					this.fullScreenView.StopAnimation ();
				}
				
				this.isAnimating = false;
			}
		}

		public void ToggleAnimation ()
		{
			if (this.isAnimating) {
				this.StopAnimation ();
			} else {
				this.StartAnimation ();
			}
		}

		[ObjectiveCMessage("keyDown:")]
		public override void KeyDown (NSEvent theEvent)
		{
			char c = theEvent.CharactersIgnoringModifiers[0];
			switch (c) {
			case (char)27:
				if (this.isInFullScreenMode) {
					this.GoWindow ();
				}
				break;
			case (char)32:
				this.ToggleAnimation ();
				break;
			case 'w':
			case 'W':
				this.scene.ToggleWireframe ();
				break;
			default:
				break;
			}
		}

		// Holding the mouse button and dragging the mouse changes the "roll" angle (y-axis) and the direction from which sunlight is coming (x-axis).
		[ObjectiveCMessage("mouseDown:")]
		public override void MouseDown (NSEvent theEvent)
		{
			bool dragging = true;
			NSPoint windowPoint;
			NSPoint lastWindowPoint = theEvent.LocationInWindow;
			float dx, dy;
			bool wasAnimating = isAnimating;
			
			if (wasAnimating) {
				this.StopAnimation ();
			}
			
			while (dragging) {
				theEvent = this.openGLView.Window.NextEventMatchingMask (NSEventMask.NSLeftMouseUpMask | NSEventMask.NSLeftMouseDraggedMask);
				windowPoint = theEvent.LocationInWindow;
				switch (theEvent.Type) {
				case NSEventType.NSLeftMouseUp:
					dragging = false;
					break;
				
				case NSEventType.NSLeftMouseDragged:
					dx = windowPoint.x - lastWindowPoint.x;
					dy = windowPoint.y - lastWindowPoint.y;
					scene.SunAngle = scene.SunAngle - 1.0f * dx;
					scene.RollAngle = scene.RollAngle - 0.5f * dy;
					lastWindowPoint = windowPoint;
					
					if (this.isInFullScreenMode) {
						this.fullScreenView.Display ();
					} else {
						this.openGLView.Display ();
					}
					break;
				default:
					
					break;
				}
			}
			
			if (wasAnimating) {
				this.StartAnimation ();
				renderTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
			}
		}
	}
}
