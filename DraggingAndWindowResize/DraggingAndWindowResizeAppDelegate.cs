using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace net.monobjc.samples.DraggingAndWindowResize
{
    [ObjectiveCClass]
	public partial class DraggingAndWindowResizeAppDelegate : NSObject
	{
		public static readonly Class DraggingAndWindowResizeAppDelegateClass = Class.Get (typeof(DraggingAndWindowResizeAppDelegate));
		
		public DraggingAndWindowResizeAppDelegate ()
		{
		}
		
		public DraggingAndWindowResizeAppDelegate (IntPtr nativePointer) : base(nativePointer)
		{
		}
		
		[ObjectiveCMessage("awakeFromNib")]
		public virtual void AwakeFromNib ()
		{
			this.resizableView.TranslatesAutoresizingMaskIntoConstraints = false;
			NSDictionary views = NSDictionary.DictionaryWithObjectForKey(this.resizableView, (NSString)"resizableView");
			this.contentView.AddConstraints(NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews("H:|-100-[resizableView]-(>=15)-|", 0, null, views));
			this.contentView.AddConstraints(NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews("V:|-100-[resizableView]-(>=15)-|", 0, null, views));
		}

		[ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
		public virtual bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}
	}
}
