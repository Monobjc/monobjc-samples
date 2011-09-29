using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.ApplicationServices;

namespace net.monobjc.samples.DraggingAndWindowResize
{
	[ObjectiveCClass]
	public partial class DragThumbView : NSView
	{
		public static readonly Class DragThumbViewClass = Class.Get (typeof(DragThumbView));

		public DragThumbView ()
		{
		}

		public DragThumbView (IntPtr nativePointer) : base(nativePointer)
		{
		}

		public DragThumbView (NSRect frameRect) : base(frameRect)
		{
		}
		
		[ObjectiveCMessage("drawRect:")]
		public override void DrawRect (NSRect aRect)
		{
			NSRect bounds = this.Bounds;
			CGFloat width = bounds.size.width;
			CGFloat height = bounds.size.height;
			
			NSColor.WhiteColor.Set ();
			AppKitFramework.NSRectFill (bounds);
			
			NSColor.ColorWithCalibratedWhiteAlpha (0.75, 1.0).Set ();
			AppKitFramework.NSFrameRect (new NSRect (0, -2, width + 2, height + 2));

			AppKitFramework.NSRectClip (bounds.OffsetRect (-1, 1));
			CGFloat offset = 2;
			NSGraphicsContext.CurrentContext.ShouldAntialias = false;
			
			for (int stripeIdx = 0; stripeIdx < 3; stripeIdx++) {
				offset++;
				
				NSColor.ColorWithCalibratedWhiteAlpha (0.85, 1.0).Set ();
				NSBezierPath.StrokeLineFromPointToPoint (new NSPoint (width - offset, 0), new NSPoint (width, offset));
				offset++;
				NSColor.ColorWithCalibratedWhiteAlpha (0.45, 1.0).Set ();
				NSBezierPath.StrokeLineFromPointToPoint (new NSPoint (width - offset, 0), new NSPoint (width, offset));
				offset++;
				NSColor.ColorWithCalibratedWhiteAlpha (0.75, 1.0).Set ();
				NSBezierPath.StrokeLineFromPointToPoint (new NSPoint (width - offset, 0), new NSPoint (width, offset));
				offset++;
			}
		}
	}
}
