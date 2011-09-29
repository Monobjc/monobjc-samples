using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace SplitView
{
	[ObjectiveCClass]
	public partial class ColoredView : NSView
	{
		public static readonly Class ColoredViewClass = Class.Get (typeof(ColoredView));
		private NSColor backgroundColor;
		private NSString backgroundColorName;
		
		public ColoredView ()
		{
		}

		public ColoredView (IntPtr nativePointer) : base(nativePointer)
		{
		}

		public ColoredView (NSRect frameRect) : base(frameRect)
		{
		}
		
		[ObjectiveCMessage("dealloc")]
		public override void Dealloc ()
		{
			this.backgroundColor.SafeRelease ();
			this.backgroundColorName.SafeRelease ();
			
			this.SendMessageSuper (ColoredViewClass, "init");
		}
		
		[ObjectiveCMessage("drawRect:")]
		public override void DrawRect (NSRect aRect)
		{
			if (this.BackgroundColor != null) {
				this.BackgroundColor.Set ();
				AppKitFramework.NSRectFill (aRect);
			}
		}
		
		public NSColor BackgroundColor {
			[ObjectiveCMessage("backgroundColor")]
			get {
				return this.backgroundColor;
			}
			[ObjectiveCMessage("setBackgroundColor:")]
			set {
				NSColor oldValue = this.backgroundColor;
				this.backgroundColor = value.SafeRetain<NSColor> ();
				oldValue.SafeRelease ();
			}
		}
		
		public NSString BackgroundColorName {
			[ObjectiveCMessage("backgroundColorName")]
			get {
				return this.backgroundColorName;
			}
			[ObjectiveCMessage("setBackgroundColorName:")]
			set {
				NSString oldValue = this.backgroundColorName;
				this.backgroundColorName = value.SafeRetain<NSString> ();
				oldValue.SafeRelease ();
				
				if (this.backgroundColorName != null) {
					IntPtr colorSelector = ObjectiveCRuntime.Selector (this.backgroundColorName);
					if (colorSelector != IntPtr.Zero) {
						NSColor color = NSColor.NSColorClass.SendMessage<NSColor> ("performSelector:", colorSelector);
						this.BackgroundColor = color;
					}
				}
			}
		}
	}
}
