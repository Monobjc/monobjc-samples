using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace net.monobjc.samples.Popover
{
    [ObjectiveCClass]
	public partial class MyWindowController : NSWindowController
	{
		public static readonly Class MyWindowControllerClass = Class.Get (typeof(MyWindowController));
		private NSPopover myPopover;
		
		public MyWindowController ()
		{
		}
		
		public MyWindowController (IntPtr nativePointer) : base(nativePointer)
		{
		}
		
		public NSPopover MyPopover {
			[ObjectiveCMessage("myPopover")]
			get {
				return this.myPopover;
			}
			[ObjectiveCMessage("setMyPopover:")]
			set {
				NSPopover oldValue = this.myPopover;
				this.myPopover = value.SafeRetain<NSPopover> ();
				oldValue.SafeRelease ();
			}
		}
		
		[ObjectiveCMessage("awakeFromNib")]
		public virtual void AwakeFromNib ()
		{
			// setup the default preferences
			this.animatesCheckbox.State = NSCellStateValue.NSOnState;
			this.popoverType.SetStateAtRowColumn ((int)NSCellStateValue.NSOnState, 0, 0);
			this.popoverPosition.SetStateAtRowColumn ((int)NSCellStateValue.NSOnState, 1, 0);
			
			// To make a popover detachable to a separate window you need:
			// 1) a separate NSWindow instance
			//      - it must not be visible:
			//          (if created by Interface Builder: not "Visible at Launch")
			//          (if created in code: must not be ordered front)
			//      - must not be released when closed
			//      - ideally the same size as the view controller's view frame size
			//
			// 2) two separate NSViewController instances
			//      - one for the popover, the other for the detached window
			//      - view best loaded as a sebarate nib (file's owner = NSViewController)
			//
			// To make the popover detached, simply drag the visible popover away from its attached view
			//
			// Fore more detailed information, refer to NSPopover.h
    
			// set separate copies of the view controller's view to each detached window
			this.detachedWindow.ContentView = this.detachedWindowViewController.View;
			this.detachedHUDWindow.ContentView = this.detachedWindowViewControllerHUD.View;
    
			// change the detached HUD window's view controller to use white text and labels
			this.detachedWindowViewControllerHUD.checkButton.SetTextColor (NSColor.WhiteColor);
			this.detachedWindowViewControllerHUD.textLabel.TextColor = NSColor.WhiteColor;
		}
		
		[ObjectiveCMessage("createPopover")]
		public void CreatePopover ()
		{
			if (this.myPopover == null) {
				// create and setup our popover
				this.myPopover = new NSPopover ();
        
				// the popover retains us and we retain the popover,
				// we drop the popover whenever it is closed to avoid a cycle
				//
				// use a different view controller content if normal vs. HUD appearance
				//
				if (this.popoverType.SelectedRow == 0) {
					this.myPopover.ContentViewController = this.popoverViewController;
				} else {
					this.myPopover.ContentViewController = this.popoverViewControllerHUD;
				}
				this.myPopover.Appearance = (NSPopoverAppearance)(int)this.popoverType.SelectedRow;
				this.myPopover.Animates = (this.animatesCheckbox.State == NSCellStateValue.NSOnState);
        
				// AppKit will close the popover when the user interacts with a user interface element outside the popover.
				// note that interacting with menus or panels that become key only when needed will not cause a transient popover to close.
				this.myPopover.Behavior = NSPopoverBehavior.NSPopoverBehaviorTransient;
        
				// so we can be notified when the popover appears or closes
				this.myPopover.Delegate = this;
			}
		}
		
		[ObjectiveCMessage("dealloc")]
		public override void Dealloc ()
		{
			this.myPopover.SafeRelease ();
			this.popoverViewControllerHUD.SafeRelease ();
			
			this.SendMessageSuper (MyWindowControllerClass, "dealloc");
		}
		
		partial void ShowPopoverAction (Id sender)
		{
			NSView view = sender.CastTo<NSView> ();
			
			this.CreatePopover ();
			
			// configure the preferred position of the popover
			NSRectEdge prefEdge = (NSRectEdge)(int)this.popoverPosition.SelectedRow;
			
			this.myPopover.ShowRelativeToRectOfViewPreferredEdge (view.Bounds, view, prefEdge);
		}
		
		[ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
		public virtual bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}

		[ObjectiveCMessage("popoverWillShow:")]
		public virtual void PopoverWillShow (NSNotification notification)
		{
			NSPopover popover = notification.Object.CastTo<NSPopover> ();
			if (popover.Appearance == NSPopoverAppearance.NSPopoverAppearanceHUD) {
				this.popoverViewControllerHUD.checkButton.SetTextColor (NSColor.WhiteColor);
				this.popoverViewControllerHUD.textLabel.TextColor = NSColor.WhiteColor;
			}
		}

		[ObjectiveCMessage("popoverDidShow:")]
		public virtual void PopoverDidShow (NSNotification notification)
		{
		}

		[ObjectiveCMessage("popoverWillClose:")]
		public virtual void PopoverWillClose (NSNotification notification)
		{
			NSString closeReason = notification.UserInfo.ValueForKey<NSString> (NSPopover.NSPopoverCloseReasonKey);
			if (closeReason != null) {
				// closeReason can be:
				//      NSPopoverCloseReasonStandard
				//      NSPopoverCloseReasonDetachToWindow
			}
		}

		[ObjectiveCMessage("popoverDidClose:")]
		public virtual void PopoverDidClose (NSNotification notification)
		{
			NSString closeReason = notification.UserInfo.ValueForKey<NSString> (NSPopover.NSPopoverCloseReasonKey);
			if (closeReason != null) {
				// closeReason can be:
				//      NSPopoverCloseReasonStandard
				//      NSPopoverCloseReasonDetachToWindow
			}
			
			this.myPopover.Release ();
			this.myPopover = null;
		}

		[ObjectiveCMessage("detachableWindowForPopover:")]
		public virtual NSWindow DetachableWindowForPopover (NSPopover popover)
		{
			NSWindow window = this.detachedWindow;
			if (popover.Appearance == NSPopoverAppearance.NSPopoverAppearanceHUD) {
				window = this.detachedHUDWindow;
			}
			return window;
		}
	}
}
