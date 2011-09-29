using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace net.monobjc.samples.Popover
{
    [ObjectiveCCategory("NSButton")]
	public static partial class NSButton_Extended
	{
		[ObjectiveCMessage("textColor")]
		public static NSColor TextColor(this NSButton button)
		{
			NSAttributedString attrTitle = button.AttributedTitle;
			NSUInteger len = attrTitle.Length;
			NSRange range = new NSRange(0, Math.Min((uint)len, 1));
			NSDictionary attrs = attrTitle.AttributesAtIndexEffectiveRange(0, ref range);
			NSColor textColor = NSColor.ControlTextColor;
			if (attrs != null) {
				textColor = attrs.ValueForKey<NSColor>(NSAttributedString_AppKitAdditions.NSForegroundColorAttributeName);
			}
			return textColor;
		}		
		
		[ObjectiveCMessage("setTextColor:")]
		public static void SetTextColor(this NSButton button, NSColor textColor)
		{
			NSMutableAttributedString attrTitle = new NSMutableAttributedString(button.AttributedTitle);
			NSUInteger len = attrTitle.Length;
			NSRange range = new NSRange(0, len);
			attrTitle.AddAttributeValueRange(NSAttributedString_AppKitAdditions.NSForegroundColorAttributeName, textColor, range);
			attrTitle.FixAttributesInRange(range);
			button.AttributedTitle = attrTitle;
			attrTitle.Release();
		}
	}
}
