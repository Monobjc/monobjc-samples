using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace net.monobjc.samples.FindPanelLayout
{
    [ObjectiveCClass]
	public partial class FindPanelLayoutAppDelegate : NSObject
	{
		public static readonly Class FindPanelLayoutAppDelegateClass = Class.Get (typeof(FindPanelLayoutAppDelegate));
		
		public FindPanelLayoutAppDelegate ()
		{
		}
		
		public FindPanelLayoutAppDelegate (IntPtr nativePointer) : base(nativePointer)
		{
		}

		[ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
		public virtual bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}
		
		public NSButton AddPushButtonWithTitleIdentifierSuperView (NSString title, NSString identifier, NSView superview)
		{
			NSButton pushButton = new NSButton ();
			pushButton.Identifier = identifier;
			pushButton.BezelStyle = NSBezelStyle.NSRoundRectBezelStyle;
			pushButton.Font = NSFont.SystemFontOfSize (12);
			pushButton.AutoresizingMask = NSAutoresizingMask.NSViewMaxXMargin | NSAutoresizingMask.NSViewMinYMargin;
			pushButton.TranslatesAutoresizingMaskIntoConstraints = false;
			superview.AddSubview (pushButton);
			if (title != null) {
				pushButton.Title = title;
			}
			pushButton.Target = this;
			pushButton.Action = ObjectiveCRuntime.Selector ("shuffleTitleOfSender:");
			
			return pushButton.Autorelease<NSButton> ();
		}
		
		public NSTextField AddTextFieldWithIdentifierSuperView (NSString identifier, NSView superview)
		{
			NSTextField textField = new NSTextField ();
			textField.Identifier = identifier;
			textField.Cell.ControlSize = NSControlSize.NSSmallControlSize;
			textField.IsBordered = true;
			textField.IsBezeled = true;
			textField.IsSelectable = true;
			textField.IsEditable = true;
			textField.Font = NSFont.SystemFontOfSize (11);
			textField.AutoresizingMask = NSAutoresizingMask.NSViewMaxXMargin | NSAutoresizingMask.NSViewMinYMargin;
			textField.TranslatesAutoresizingMaskIntoConstraints = false;
			superview.AddSubview (textField);
			
			return textField.Autorelease<NSTextField> ();
		}

		[ObjectiveCMessage("awakeFromNib")]
		public virtual void AwakeFromNib ()
		{
			NSView contentView = this.window.ContentView;
			
			NSButton find = this.AddPushButtonWithTitleIdentifierSuperView (FoundationFramework.NSLocalizedString("Find", null), "find", contentView);
			NSButton findNext = this.AddPushButtonWithTitleIdentifierSuperView (FoundationFramework.NSLocalizedString("Find Next", null), "findNext", contentView);
			NSTextField findField = this.AddTextFieldWithIdentifierSuperView ("findField", contentView);
			NSButton replace = this.AddPushButtonWithTitleIdentifierSuperView (FoundationFramework.NSLocalizedString("Replace", null), "replace", contentView);
			NSButton replaceAndFind = this.AddPushButtonWithTitleIdentifierSuperView (FoundationFramework.NSLocalizedString("Replace & Find", null), "replaceAndFind", contentView);
			NSTextField replaceField = this.AddTextFieldWithIdentifierSuperView ("replaceField", contentView);
			
			NSDictionary views = NSDictionary.DictionaryWithObjectsAndKeys (
				find, (NSString)"find",
				findNext, (NSString)"findNext",
				findField, (NSString)"findField",
				replace, (NSString)"replace",
				replaceAndFind, (NSString)"replaceAndFind",
				replaceField, (NSString)"replaceField",
				null);
			
			// Basic layout of the two rows
			// Give the text fields a hard minimum width, because it looks good.
			contentView.AddConstraints ( NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews (
				"H:|-[find]-[findNext]-[findField(>=20)]-|", 
				NSLayoutFormatOptions.NSLayoutFormatAlignAllTop,
				null, 
				views));
			contentView.AddConstraints (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews (
				"H:|-[replace]-[replaceAndFind]-[replaceField(>=20)]-|", 
				NSLayoutFormatOptions.NSLayoutFormatAlignAllTop,
				null, 
				views));
			
			// Vertical layout.  We just need to specify what happens to one thing in each row, since everything within a row is already top aligned.  We'll use the text fields, since then we can align their leading edges as well in one step.
			contentView.AddConstraints (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews (
				"V:|-[findField]-[replaceField]-(>=20)-|",
				NSLayoutFormatOptions.NSLayoutFormatAlignAllLeading, 
				null, 
				views));
			
			// lower the content hugging priority of the text fields from NSLayoutPriorityDefaultLow, so that they expand to fill extra space rather than the buttons.
			foreach (NSView view in NSArray.ArrayWithObjects(findField, replaceField, null).GetEnumerator<NSView>()) {
				view.SetContentHuggingPriorityForOrientation (NSLayoutPriority.NSLayoutPriorityDefaultLow, NSLayoutConstraintOrientation.NSLayoutConstraintOrientationHorizontal);
			}

			// In the row in which the buttons are smaller (whichever that is), it is still ambiguous how the buttons expand from their preferred widths to fill the extra space between the window edge and the text field. 
			// They should prefer to be equal width, more weakly than our other constraints.
			contentView.AddConstraints (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews (
				"H:[find(==findNext@25)]", 
				(NSLayoutFormatOptions) 0, 
				null, 
				views));
			contentView.AddConstraints (NSLayoutConstraint.ConstraintsWithVisualFormatOptionsMetricsViews (
				"H:[replace(==replaceAndFind@25)]", 
				(NSLayoutFormatOptions) 0, 
				null, 
				views));
		}
		
		[ObjectiveCMessage("shuffleTitleOfSender:")]
		public void ShuffleTitleOfSender (Id sender)
		{
			NSArray strings = NSArray.ArrayWithObjects ((NSString)"S", (NSString)"Short", (NSString)"Absolutely ginormous string (for a button)", null);
			NSInteger previousStringIndex = strings.IndexOfObject (sender.SendMessage<NSString> ("title"));
			NSInteger nextStringIndex;
			if (previousStringIndex == NSUInteger.NSNotFound) {
				nextStringIndex = 0;
			} else {
				nextStringIndex = (previousStringIndex + 1) % 3;
			}
			sender.SendMessage ("setTitle:", strings.ObjectAtIndex (nextStringIndex));
		}
	}
}
