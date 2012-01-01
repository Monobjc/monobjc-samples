using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.TutorialToolbar
{
    [ObjectiveCClass]
	public partial class AppDelegate : NSObject, INSNibAwaking
	{
		public static readonly Class AppDelegateClass = Class.Get (typeof(AppDelegate));
		
		private static NSString TOOLBAR = new NSString("TOOLBAR");
		private static NSString TOOLBAR_ITEM1 = new NSString("TOOLBAR_ITEM1");
		private static NSString TOOLBAR_ITEM2 = new NSString("TOOLBAR_ITEM2");
		private static NSString TOOLBAR_ITEM3 = new NSString("TOOLBAR_ITEM3");
		
		public AppDelegate ()
		{
		}
		
		public AppDelegate (IntPtr nativePointer) : base(nativePointer)
		{
		}
		
        [ObjectiveCMessage("awakeFromNib")]
		public void AwakeFromNib ()
		{
			// Create the toolbar and install it
			NSToolbar toolbar = new NSToolbar(TOOLBAR);
			toolbar.AllowsUserCustomization = true;
			toolbar.DisplayMode = NSToolbarDisplayMode.NSToolbarDisplayModeDefault;
			toolbar.Delegate = this;
			this.window.Toolbar = toolbar;
			toolbar.Release();
		}

		[ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
		public virtual bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}

		[ObjectiveCMessage("toolbarAllowedItemIdentifiers:")]
		public virtual NSArray ToolbarAllowedItemIdentifiers (NSToolbar toolbar)
		{
			NSArray array = NSArray.ArrayWithObjects(
				NSToolbarItem.NSToolbarCustomizeToolbarItemIdentifier,
				NSToolbarItem.NSToolbarFlexibleSpaceItemIdentifier,
				NSToolbarItem.NSToolbarSeparatorItemIdentifier,
				NSToolbarItem.NSToolbarSpaceItemIdentifier,
				TOOLBAR_ITEM1,
				TOOLBAR_ITEM2,
				TOOLBAR_ITEM3,
				null);
			return array;
		}

		[ObjectiveCMessage("toolbarDefaultItemIdentifiers:")]
		public virtual NSArray ToolbarDefaultItemIdentifiers (NSToolbar toolbar)
		{
			NSArray array = NSArray.ArrayWithObjects(
				TOOLBAR_ITEM1,
				NSToolbarItem.NSToolbarSeparatorItemIdentifier,
				TOOLBAR_ITEM2,
				NSToolbarItem.NSToolbarFlexibleSpaceItemIdentifier,
				TOOLBAR_ITEM3,
				null);
			return array;
		}

		[ObjectiveCMessage("toolbar:itemForItemIdentifier:willBeInsertedIntoToolbar:")]
		public virtual NSToolbarItem ToolbarItemForItemIdentifierWillBeInsertedIntoToolbar (NSToolbar toolbar, NSString itemIdentifier, bool flag)
		{
			NSToolbarItem item = new NSToolbarItem(itemIdentifier);
			
			if (itemIdentifier.IsEqualToString(TOOLBAR_ITEM1)) {
				item.Image = NSImage.ImageNamed(NSImage.NSImageNameAdvanced);
				item.Label = "Toolbar Item 1";
				item.Target = this;
				item.Action = ObjectiveCRuntime.Selector("doSomething1:");
			}
			if (itemIdentifier.IsEqualToString(TOOLBAR_ITEM2)) {
				item.Image = NSImage.ImageNamed(NSImage.NSImageNameEveryone);
				item.Label = "Toolbar Item 2";
				item.Target = this;
				item.Action = ObjectiveCRuntime.Selector("doSomething2:");
			}
			if (itemIdentifier.IsEqualToString(TOOLBAR_ITEM3)) {
				item.Image = NSImage.ImageNamed(NSImage.NSImageNameDotMac);
				item.Label = "Toolbar Item 3";
				item.Target = this;
				item.Action = ObjectiveCRuntime.Selector("doSomething3:");
			}
			
			item.PaletteLabel = item.Label;
			
			return item;
		}

		[ObjectiveCMessage("validateToolbarItem:")]
		public virtual bool ValidateToolbarItem (NSToolbarItem theItem)
		{
			return true;
		}
		
		[IBAction]
		[ObjectiveCMessage("doSomething1:")]
		public void DoSomething1 (Id sender)
		{
		}

		[IBAction]
		[ObjectiveCMessage("doSomething2:")]
		public void DoSomething2 (Id sender)
		{
		}

		[IBAction]
		[ObjectiveCMessage("doSomething3:")]
		public void DoSomething3 (Id sender)
		{
		}
	}
}
