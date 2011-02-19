using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.SearchField
{
    [ObjectiveCClass]
    public partial class MyWindowController : NSWindowController
    {
        private static readonly Class MyWindowControllerClass = Class.Get(typeof (MyWindowController));

        private NSMutableArray _allKeywords;
        private NSMutableArray _builtInKeywords;

        private bool completePosting;
        private bool commandHandling;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyWindowController"/> class.
        /// </summary>
        public MyWindowController() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MyWindowController"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public MyWindowController(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this._builtInKeywords.Release();
            this._allKeywords.Release();

            // The dealloc is sent directly to the base class in
            // the Objective-C runtime to make the class hiearchy happy
            this.SendMessageSuper(MyWindowControllerClass, "dealloc");
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            if (this.searchField.RespondsToSelector(ObjectiveCRuntime.Selector("setRecentSearches:")))
            {
                NSMenu searchMenu = new NSMenu("Search Menu");
                searchMenu.Autorelease();
                searchMenu.AutoenablesItems = true;

                // first add our custom menu item (Important note: "action" MUST be valid or the menu item is disabled)
                NSMenuItem item = new NSMenuItem("Custom", ObjectiveCRuntime.Selector("actionMenuItem:"), NSString.Empty);
                item.Target = this;
                searchMenu.InsertItemAtIndex(item, 0);
                item.Release();

                // add our own separator to keep our custom menu separate
                NSMenuItem separator = NSMenuItem.SeparatorItem;
                searchMenu.InsertItemAtIndex(separator, 1);

                NSMenuItem recentsTitleItem = new NSMenuItem("Recent Searches", IntPtr.Zero, NSString.Empty);
                // tag this menu item so NSSearchField can use it and respond to it appropriately
                recentsTitleItem.Tag = NSSearchFieldCell.NSSearchFieldRecentsTitleMenuItemTag;
                searchMenu.InsertItemAtIndex(recentsTitleItem, 2);
                recentsTitleItem.Release();

                NSMenuItem norecentsTitleItem = new NSMenuItem("No recent searches", IntPtr.Zero, NSString.Empty);
                // tag this menu item so NSSearchField can use it and respond to it appropriately
                norecentsTitleItem.Tag = NSSearchFieldCell.NSSearchFieldNoRecentsMenuItemTag;
                searchMenu.InsertItemAtIndex(norecentsTitleItem, 3);
                norecentsTitleItem.Release();

                NSMenuItem recentsItem = new NSMenuItem("Recents", IntPtr.Zero, NSString.Empty);
                // tag this menu item so NSSearchField can use it and respond to it appropriately
                recentsItem.Tag = NSSearchFieldCell.NSSearchFieldRecentsMenuItemTag;
                searchMenu.InsertItemAtIndex(recentsItem, 4);
                recentsItem.Release();

                NSMenuItem separatorItem = NSMenuItem.SeparatorItem;
                // tag this menu item so NSSearchField can use it, by hiding/show it appropriately:
                separatorItem.Tag = NSSearchFieldCell.NSSearchFieldRecentsTitleMenuItemTag;
                searchMenu.InsertItemAtIndex(separatorItem, 5);

                NSMenuItem clearItem = new NSMenuItem("Clear", IntPtr.Zero, NSString.Empty);
                // tag this menu item so NSSearchField can use it and respond to it appropriately
                clearItem.Tag = NSSearchFieldCell.NSSearchFieldClearRecentsMenuItemTag;
                searchMenu.InsertItemAtIndex(clearItem, 6);
                clearItem.Release();

                NSSearchFieldCell searchCell = this.searchField.Cell.CastTo<NSSearchFieldCell>();
                searchCell.MaximumRecents = 20;
                searchCell.SearchMenuTemplate = searchMenu;
            }

            // build the list of keyword strings for our type completion dropdown list in NSSearchField
            this._builtInKeywords = new NSMutableArray();
            this._builtInKeywords.AddObject((NSString) "Favorite");
            this._builtInKeywords.AddObject((NSString) "Favorite1");
            this._builtInKeywords.AddObject((NSString) "Favorite11");
            this._builtInKeywords.AddObject((NSString) "Favorite3");
            this._builtInKeywords.AddObject((NSString) "Vacations1");
            this._builtInKeywords.AddObject((NSString) "Vacations2");
            this._builtInKeywords.AddObject((NSString) "Hawaii");
            this._builtInKeywords.AddObject((NSString) "Family");
            this._builtInKeywords.AddObject((NSString) "Important");
            this._builtInKeywords.AddObject((NSString) "Important2");
            this._builtInKeywords.AddObject((NSString) "Personal");
        }

        [ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
        public bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }

        public void SheetDidEndReturnCodeContextInfo(NSWindow sheet, NSInteger returnCode, IntPtr contextInfo)
        {
            sheet.OrderOut(this);
        }

        [ObjectiveCMessage("sheetDoneButtonAction:")]
        public void SheetDoneButtonAction(Id sender)
        {
            NSApplication.SharedApplication.EndSheet(this.simpleSheet);
        }

        [ObjectiveCMessage("actionMenuItem:")]
        public void ActionMenuItem(Id sender)
        {
            NSApplication.NSApp.BeginSheetModalForWindowModalDelegateDidEndSelectorContextInfo(this.simpleSheet, this.Window, this.SheetDidEndReturnCodeContextInfo, IntPtr.Zero);
        }

        [ObjectiveCMessage("allKeywords")]
        public NSArray AllKeywords()
        {
            NSArray array = new NSArray();
            array.Autorelease();

            if (this._allKeywords == null)
            {
                uint i, count;

                this._allKeywords = this._builtInKeywords.MutableCopy<NSMutableArray>();

                count = array.Count;
                for (i = 0; i < count; i++)
                {
                    if (this._allKeywords.IndexOfObject(array.ObjectAtIndex(i)) == NSUInteger.NSNotFound)
                    {
                        this._allKeywords.AddObject(array.ObjectAtIndex(i));
                    }
                }

                this._allKeywords.SortUsingSelector(ObjectiveCRuntime.Selector("compare:"));
            }
            return this._allKeywords;
        }

        [ObjectiveCMessage("control:textView:completions:forPartialWordRange:indexOfSelectedItem:")]
        public NSArray ControlTextViewCompletionsForPartialWordRangeIndexOfSelectedItem(NSControl control, NSTextView textView, NSArray words, NSRange charRange, ref int index)
        {
            NSMutableArray matches;
            NSString partialString;
            NSArray keywords;
            int i;
            uint count;
            NSString str;

            partialString = textView.String.SubstringWithRange(charRange);
            keywords = this.AllKeywords();
            count = keywords.Count;
            matches = new NSMutableArray();

            // find any match in our keyword array against what was typed
            for (i = 0; i < count; i++)
            {
                str = keywords[i].CastTo<NSString>();
                if (str.RangeOfStringOptionsRange(partialString, NSStringCompareOptions.NSAnchoredSearch | NSStringCompareOptions.NSCaseInsensitiveSearch, NSRange.NSMakeRange(0, str.Length)).location != NSUInteger.NSNotFound)
                {
                    matches.AddObject(str);
                }
            }
            matches.SortUsingSelector(ObjectiveCRuntime.Selector("compare:"));

            return matches;
        }

        [ObjectiveCMessage("controlTextDidChange:")]
        public void ControlTextDidChange(NSNotification obj)
        {
            if (obj.UserInfo == null)
            {
                return;
            }

            NSTextView textView = obj.UserInfo[(NSString) "NSFieldEditor"].CastTo<NSTextView>();

            if (!this.completePosting && !this.commandHandling) // prevent calling "complete" too often
            {
                this.completePosting = true;
                textView.Complete(null);
                this.completePosting = false;
            }
        }

        [ObjectiveCMessage("control:textView:doCommandSelector:")]
        public bool ControlTextViewDoCommandBySelector(NSControl control, NSTextView textView, IntPtr commandSelector)
        {
            if (textView.RespondsToSelector(commandSelector))
            {
                this.commandHandling = true;
                textView.PerformSelectorWithObject(commandSelector, null);
                this.commandHandling = false;
            }
            return true;
        }
    }
}