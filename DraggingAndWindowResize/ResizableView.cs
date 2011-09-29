using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace net.monobjc.samples.DraggingAndWindowResize
{
	[ObjectiveCClass]
	public partial class ResizableView : NSView
	{
		public static readonly Class ResizableViewClass = Class.Get (typeof(ResizableView));
		private NSLayoutConstraint horizontalDragConstraint;
		private NSLayoutConstraint verticalDragConstraint;
		private NSArray sizeConstraints;
		private NSSize dragOffsetIntoGrowBox;

		public ResizableView ()
		{
		}

		public ResizableView (IntPtr nativePointer) : base(nativePointer)
		{
		}

		public ResizableView (NSRect frameRect) : base(frameRect)
		{
		}

		[ObjectiveCMessage("awakeFromNib")]
		public virtual void AwakeFromNib ()
		{
			this.dragThumbView.TranslatesAutoresizingMaskIntoConstraints = false;
			NSDictionary views = NSDictionary.DictionaryWithObjectForKey (this.dragThumbView, (NSString)"dragThumbView");
			this.AddConstraints (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews ("H:|-(>=0)-[dragThumbView(15)]|", 0, null, views));
			this.AddConstraints (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews ("V:|-(>=0)-[dragThumbView(15)]|", 0, null, views));
		}
		
		[ObjectiveCMessage("drawRect:")]
		public override void DrawRect (NSRect aRect)
		{
			NSColor.WhiteColor.Set ();
			NSRect bounds = this.Bounds;
			AppKitFramework.NSRectFill (bounds);
			NSColor.ColorWithCalibratedWhiteAlpha (0.75, 1.0).Set ();
			AppKitFramework.NSFrameRect (bounds);
		}
		
		public void SetOwnSizeConstraints (NSArray newConstraints)
		{
			if (this.sizeConstraints != newConstraints) {
				if (this.sizeConstraints != null) {
					this.RemoveConstraints (this.sizeConstraints);
				}
				this.sizeConstraints.SafeRelease ();
				this.sizeConstraints = newConstraints.SafeRetain<NSArray> ();
				if (this.sizeConstraints != null) {
					this.AddConstraints (this.sizeConstraints);
				}
			}
		}
		
		public NSArray SizeConstraintsForCurrentPosition ()
		{
			NSMutableArray constraints = NSMutableArray.Array;
			NSSize size = this.Bounds.size;
			NSDictionary metrics = NSDictionary.DictionaryWithObjectsAndKeys (
				NSNumber.NumberWithFloat (size.width), (NSString)"width",
				NSNumber.NumberWithFloat (size.height), (NSString)"height",
				null);
			NSDictionary views = NSDictionary.DictionaryWithObjectForKey (this, (NSString)"self");
			constraints.AddObjectsFromArray (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews ("H:[self(width@250)]", 0, metrics, views));
			constraints.AddObjectsFromArray (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews ("V:[self(height@250)]", 0, metrics, views));
			return constraints;
		}
		
		[ObjectiveCMessage("updateConstraints")]
		public override void UpdateConstraints ()
		{
			if (this.sizeConstraints == null) {
				this.SetOwnSizeConstraints (this.SizeConstraintsForCurrentPosition ());
			}
			this.SendMessageSuper (ResizableViewClass, "updateConstraints");
		}
		
		public override bool IsFlipped {
		[ObjectiveCMessage("isFlipped")]
			get {
				return true;
			}
		}
		
		[ObjectiveCMessage("mouseDown:")]
		public override void MouseDown (NSEvent theEvent)
		{
			NSPoint locationInSelf = this.ConvertPointFromView (theEvent.LocationInWindow, null);
			NSRect dragThumbFrame = this.dragThumbView.Frame;
			
			if (dragThumbFrame.PointInRect (locationInSelf)) {
				this.dragOffsetIntoGrowBox = new NSSize (locationInSelf.x - dragThumbFrame.origin.x, locationInSelf.y - dragThumbFrame.origin.y);
				NSDictionary metrics = NSDictionary.DictionaryWithObjectsAndKeys (
				NSNumber.NumberWithFloat (dragThumbFrame.MinX), (NSString)"initialThumbX",
				NSNumber.NumberWithFloat (dragThumbFrame.MinY), (NSString)"initialThumbY",
				null);
				NSDictionary views = NSDictionary.DictionaryWithObjectForKey (this.dragThumbView, (NSString)"dragThumbView");
				
				this.horizontalDragConstraint = NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews ("H:|-(initialThumbX)-[dragThumbView]", 0, metrics, views).LastObject.Retain<NSLayoutConstraint> ();
				this.verticalDragConstraint = NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews ("V:|-(initialThumbY)-[dragThumbView]", 0, metrics, views).LastObject.Retain<NSLayoutConstraint> ();
        
				// try lowering the priority to NSLayoutPriorityDragThatCannotResizeWindow to see the difference
				this.horizontalDragConstraint.Priority = NSLayoutPriority.NSLayoutPriorityDragThatCanResizeWindow;
				this.verticalDragConstraint.Priority = NSLayoutPriority.NSLayoutPriorityDragThatCanResizeWindow;
				
				this.AddConstraint (this.horizontalDragConstraint);
				this.AddConstraint (this.verticalDragConstraint);
				
				// just for fun.  Try it out!
				this.Window.VisualizeConstraints (NSArray.ArrayWithObjects (this.horizontalDragConstraint, this.verticalDragConstraint, null));
			}
		}
		
		[ObjectiveCMessage("mouseDragged:")]
		public override void MouseDragged (NSEvent theEvent)
		{
			if (this.horizontalDragConstraint != null) {
				// update the dragging constraints for the new location
				NSPoint locationInSelf = this.ConvertPointFromView (theEvent.LocationInWindow, null);
				this.horizontalDragConstraint.Constant = locationInSelf.x - this.dragOffsetIntoGrowBox.width;
				this.verticalDragConstraint.Constant = locationInSelf.y - this.dragOffsetIntoGrowBox.height;
			} else {
				this.SendMessageSuper (ResizableViewClass, "mouseDragged:", theEvent);
			}
		}
		
		[ObjectiveCMessage("mouseUp:")]
		public override void MouseUp (NSEvent theEvent)
		{
			if (this.horizontalDragConstraint != null) {
				this.RemoveConstraint(this.horizontalDragConstraint);
				this.horizontalDragConstraint.Release();
				this.horizontalDragConstraint=null;
				this.RemoveConstraint(this.verticalDragConstraint);
				this.verticalDragConstraint.Release();
				this.verticalDragConstraint=null;
				
				this.SetOwnSizeConstraints(this.SizeConstraintsForCurrentPosition());
				
				// just for fun.  Try it out!
				this.Window.VisualizeConstraints (null);
			} else {
				this.SendMessageSuper (ResizableViewClass, "mouseUp:", theEvent);
			}
		}
	}
}
