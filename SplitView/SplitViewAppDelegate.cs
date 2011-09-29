using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace SplitView
{
    [ObjectiveCClass]
	public partial class SplitViewAppDelegate : NSObject
	{
		public static readonly Class SplitViewAppDelegateClass = Class.Get (typeof(SplitViewAppDelegate));
		
		public SplitViewAppDelegate ()
		{
		}
		
		public SplitViewAppDelegate (IntPtr nativePointer) : base(nativePointer)
		{
		}

		[ObjectiveCMessage("awakeFromNib")]
		public virtual void AwakeFromNib ()
		{
			this.yellowLabel.TranslatesAutoresizingMaskIntoConstraints = false;
			this.labelAlignedToTopOfYellowView.TranslatesAutoresizingMaskIntoConstraints = false;
			
			NSView yellowView = this.yellowLabel.Superview;
			NSView contentView = this.window.ContentView;
			
			// Center the yellow label
			splitView.AddConstraint (NSLayoutConstraint.ConstraintWithItemAttributeRelatedByToItemAttributeMultiplierConstant (
				this.yellowLabel,
				NSLayoutAttribute.NSLayoutAttributeCenterX,
				NSLayoutRelation.NSLayoutRelationEqual,
				this.splitView,
				NSLayoutAttribute.NSLayoutAttributeCenterX,
				1.0,
				0));
			splitView.AddConstraint (NSLayoutConstraint.ConstraintWithItemAttributeRelatedByToItemAttributeMultiplierConstant (
				this.yellowLabel,
				NSLayoutAttribute.NSLayoutAttributeCenterY,
				NSLayoutRelation.NSLayoutRelationEqual,
				yellowView,
				NSLayoutAttribute.NSLayoutAttributeCenterY,
				1.0,
				0));
    
			// don't let the splitview get too small for the label
			splitView.AddConstraints (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews (
				"|-(>=0)-[yellowLabel]-(>=0)-|",
				0,
				null,
				NSDictionary.DictionaryWithObjectForKey (yellowLabel, (NSString)"yellowLabel")));
    
			// Make the labelAlignedToTopOfYellowView stick to the outside right edge of the splitview, aligned with the top of the yellow view
			contentView.AddConstraint (NSLayoutConstraint.ConstraintWithItemAttributeRelatedByToItemAttributeMultiplierConstant (
				yellowView,
				NSLayoutAttribute.NSLayoutAttributeTop,
				NSLayoutRelation.NSLayoutRelationEqual,
				this.labelAlignedToTopOfYellowView,
				NSLayoutAttribute.NSLayoutAttributeTop,
				1.0,
				0));
			contentView.AddConstraints (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews (
				"[splitView][labelAlignedToTopOfYellowView]",
				0,
				null,
				NSDictionary.DictionaryWithObjectsAndKeys(this.splitView, (NSString)"splitView", this.labelAlignedToTopOfYellowView, (NSString)"labelAlignedToTopOfYellowView", null)));
		}

		[ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
		public virtual bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}
	}
}
