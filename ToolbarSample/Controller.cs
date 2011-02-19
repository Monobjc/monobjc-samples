using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.ToolbarSample
{
    [ObjectiveCClass]
    public partial class Controller : NSObject
    {
        private static readonly Class ControllerClass = Class.Get(typeof (Controller));

        private int fontStylePicked; //a state variable that keeps track of what style has been picked (plain, bold, italic)

        private NSMutableDictionary toolbarItems; //The dictionary that holds all our "master" copies of the NSToolbarItems

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        public Controller() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public Controller(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            NSFont theFont;
            NSToolbar toolbar = new NSToolbar("myToolbar");

            // Here we create the dictionary to hold all of our "master" NSToolbarItems.
            this.toolbarItems = new NSMutableDictionary();
            this.toolbarItems.Retain();

            // Now lets create three NSToolbarItems; 2 using custom views, and a standard one using an image.
            // We call our special processing function to do the initialization and add them to the dictionary.
            AddToolbarItem(this.toolbarItems, "FontStyle", "Font Style", "Font Style", "Change your font style", this, ObjectiveCRuntime.Selector("setView:"), this.popUpView, IntPtr.Zero, this.fontStyleMenu);
            AddToolbarItem(this.toolbarItems, "FontSize", "Font Size", "Font Size", "Grow or shrink the size of your font", this, ObjectiveCRuntime.Selector("setView:"), this.fontSizeView, IntPtr.Zero, this.fontSizeMenu);
            // often using an image will be your standard case.  You'll notice that a selector is passed
            // for the action (blueText:), which will be called when the image-containing toolbar item is clicked.
            AddToolbarItem(this.toolbarItems, "BlueLetter", "Blue Text", "Blue Text", "This toggles blue text on/off", this, ObjectiveCRuntime.Selector("setImage:"), NSImage.ImageNamed("blueLetter.tif"), ObjectiveCRuntime.Selector("blueText:"), null);

            // the toolbar wants to know who is going to handle processing of NSToolbarItems for it.  This controller will.
            toolbar.SetDelegate(d =>
                                    {
                                        d.ToolbarAllowedItemIdentifiers += this.ToolbarAllowedItemIdentifiers;
                                        d.ToolbarDefaultItemIdentifiers += this.ToolbarDefaultItemIdentifiers;
                                        d.ToolbarItemForItemIdentifierWillBeInsertedIntoToolbar += this.ToolbarItemForItemIdentifierWillBeInsertedIntoToolbar;
                                        d.ToolbarWillAddItem += this.ToolbarWillAddItem;
                                    });

            // If you pass NO here, you turn off the customization palette.  The palette is normally handled automatically
            // for you by NSWindow's -runToolbarCustomizationPalette: method; you'll notice that the "Customize Toolbar"
            // menu item is hooked up to that method in Interface Builder.  Interface Builder currently doesn't automatically 
            // show this action (or the -toggleToolbarShown: action) for First Responder/NSWindow (this is a bug), so you 
            // have to manually add those methods to the First Responder in Interface Builder (by hitting return on the First Responder and 
            // adding the new actions in the usual way) if you want to wire up menus to them.
            toolbar.AllowsUserCustomization = true;

            // tell the toolbar that it should save any configuration changes to user defaults.  ie. mode changes, or reordering will persist. 
            // specifically they will be written in the app domain using the toolbar identifier as the key. 
            toolbar.AutosavesConfiguration = true;

            // tell the toolbar to show icons only by default
            toolbar.DisplayMode = NSToolbarDisplayMode.NSToolbarDisplayModeIconOnly;
            // install the toolbar.
            this.theWindow.Toolbar = toolbar;

            // We initialize our font size control here to 12-point font, and set our contentView (an NSTextView) to that size
            this.fontSizeStepper.IntValue = 12;
            theFont = NSFont.UserFontOfSize(this.fontSizeStepper.IntValue);
            this.contentView.Font = theFont;

            this.fontStylePicked = 1;
        }

        partial void FontSizeBigger(Id sender)
        {
            this.fontSizeStepper.IntValue = this.fontSizeStepper.IntValue++;
            this.ChangeFontSize(null);
        }

        partial void FontSizeSmaller(Id sender)
        {
            this.fontSizeStepper.IntValue = this.fontSizeStepper.IntValue--;
            this.ChangeFontSize(null);
        }

        partial void ChangeFontSize(Id sender)
        {
            NSFont theFont;

            this.fontSizeField.TakeIntValueFrom(this.fontSizeStepper);
            theFont = this.contentView.Font;
            theFont = NSFontManager.SharedFontManager.ConvertFontToSize(theFont, this.fontSizeStepper.IntValue);
            this.contentView.Font = theFont;
        }

        [ObjectiveCMessage("validateMenuItem:")]
        public bool ValidateMenuItem(NSMenuItem menuItem)
        {
            if (menuItem.Action == ObjectiveCRuntime.Selector("changeFontStyle:"))
            {
                // If we got this far, then the menuItem is either a part of the font-style-toobar-item-custom-view's
                // (wow, say that six times fast) NSPopUp Button, or it's a part of the menuFormRepresentation's NSMenu.
                // If it's the former, then the menu item's state (whether there is a check next to it, etc.) is handled
                // for us, but if not, then we have to do it ourselves.  The tags are changed on the menu so that they
                // match the fontStylePicked variable, unlike the popup button's, which don't.
                if (menuItem.Tag == this.fontStylePicked)
                {
                    menuItem.State = NSCellStateValue.NSOnState;
                }
            }
            return true;
        }

        partial void ChangeFontStyle(Id sender)
        {
            NSFont theFont;
            int itemIndex;

            // If the sender is an NSMenuItem then this is the menuFormRepresentation.  Otherwise, we're being called from
            // the NSPopUpButton.  We need to check this to find out how to calculate the index of the picked menu item.
            if (FoundationFramework.NSStringFromClass(sender.Class).IsEqualToString("NSMenuItem"))
            {
                NSMenuItem item = sender.CastTo<NSMenuItem>();
                itemIndex = item.Menu.IndexOfItem(item) - 1; // for ordinary NSMenus, the title is item #0, so we have to offset things
            }
            else
            {
                NSPopUpButton button = sender.CastTo<NSPopUpButton>();
                itemIndex = button.IndexOfSelectedItem; // this is an NSPopUpButton, so the first useful item really is #0
            }

            this.fontSizeField.TakeIntValueFrom(this.fontSizeStepper);
            theFont = this.contentView.Font;

            // set the font properties depending upon what was selected
            if (itemIndex == 0)
            {
                theFont = NSFontManager.SharedFontManager.ConvertFontToNotHaveTrait(theFont, NSFontTraitMask.NSItalicFontMask);
                theFont = NSFontManager.SharedFontManager.ConvertFontToNotHaveTrait(theFont, NSFontTraitMask.NSBoldFontMask);
            }
            else if (itemIndex == 1)
            {
                theFont = NSFontManager.SharedFontManager.ConvertFontToNotHaveTrait(theFont, NSFontTraitMask.NSItalicFontMask);
                theFont = NSFontManager.SharedFontManager.ConvertFontToHaveTrait(theFont, NSFontTraitMask.NSBoldFontMask);
            }
            else if (itemIndex == 2)
            {
                theFont = NSFontManager.SharedFontManager.ConvertFontToHaveTrait(theFont, NSFontTraitMask.NSItalicFontMask);
                theFont = NSFontManager.SharedFontManager.ConvertFontToNotHaveTrait(theFont, NSFontTraitMask.NSBoldFontMask);
            }
            // make sure the fontStylePicked variable matches the menu selection plus 1, which also matches
            // the menu item tags in the menuFormRepresentation (see the menu in Interface Builder), so
            // that -validateMenuItem: can do its work. 
            this.fontStylePicked = itemIndex + 1;
            this.contentView.Font = theFont;
        }

        partial void BlueText(Id sender)
        {
            if (!NSColor.BlueColor.IsEqual(this.contentView.TextColor))
            {
                this.contentView.TextColor = NSColor.BlueColor;
            }
            else
            {
                this.contentView.TextColor = NSColor.BlackColor;
            }
        }

        [ObjectiveCMessage("validateToolbarItem:")]
        public bool ValidateToolbarItem(NSToolbarItem theItem)
        {
            // You could check [theItem itemIdentifier] here and take appropriate action if you wanted to
            return true;
        }

        [ObjectiveCMessage("printDocument:")]
        public void PrintDocument(Id sender)
        {
            NSPrintOperation printOperation = NSPrintOperation.PrintOperationWithView(this.contentView);
            printOperation.RunOperationModalForWindowDelegateDidRunSelectorContextInfo(this.contentView.Window, null, IntPtr.Zero, IntPtr.Zero);
        }

        public void ToolbarWillAddItem(NSNotification notif)
        {
            NSToolbarItem addedItem = notif.UserInfo[(NSString) "item"].CastTo<NSToolbarItem>();
            // Is this the printing toolbar item?  If so, then we want to redirect it's action to ourselves
            // so we can handle the printing properly; hence, we give it a new target.
            if (addedItem.ItemIdentifier.IsEqual(NSToolbarItem.NSToolbarPrintItemIdentifier))
            {
                addedItem.ToolTip = "Print your document";
                addedItem.Target = this;
            }
        }

        public NSToolbarItem ToolbarItemForItemIdentifierWillBeInsertedIntoToolbar(NSToolbar toolbar, NSString itemIdentifier, bool flag)
        {
            // We create and autorelease a new NSToolbarItem, and then go through the process of setting up its
            // attributes from the master toolbar item matching that identifier in our dictionary of items.
            NSToolbarItem newItem = new NSToolbarItem(itemIdentifier);
            newItem.Autorelease();
            NSToolbarItem item = this.toolbarItems[itemIdentifier].CastTo<NSToolbarItem>();

            newItem.Label = item.Label;
            newItem.PaletteLabel = item.PaletteLabel;
            if (item.View != null)
            {
                newItem.View = item.View;
            }
            else
            {
                newItem.Image = item.Image;
            }
            newItem.ToolTip = item.ToolTip;
            newItem.Target = item.Target;
            newItem.Action = item.Action;
            newItem.MenuFormRepresentation = item.MenuFormRepresentation;
            // If we have a custom view, we *have* to set the min/max size - otherwise, it'll default to 0,0 and the custom
            // view won't show up at all!  This doesn't affect toolbar items with images, however.
            if (newItem.View != null)
            {
                newItem.MinSize = item.View.Bounds.size;
                newItem.MaxSize = item.View.Bounds.size;
            }

            return newItem;
        }

        public NSArray ToolbarDefaultItemIdentifiers(NSToolbar toolbar)
        {
            NSMutableArray array = new NSMutableArray();
            array.AddObject((NSString) "FontStyle");
            array.AddObject((NSString) "FontSize");
            array.AddObject(NSToolbarItem.NSToolbarSeparatorItemIdentifier);
            array.AddObject((NSString) "BlueLetter");
            array.AddObject(NSToolbarItem.NSToolbarPrintItemIdentifier);
            return array;
        }

        public NSArray ToolbarAllowedItemIdentifiers(NSToolbar toolbar)
        {
            NSMutableArray array = new NSMutableArray();
            array.AddObject((NSString) "FontStyle");
            array.AddObject((NSString) "FontSize");
            array.AddObject(NSToolbarItem.NSToolbarSeparatorItemIdentifier);
            array.AddObject((NSString) "BlueLetter");
            array.AddObject(NSToolbarItem.NSToolbarPrintItemIdentifier);
            array.AddObject(NSToolbarItem.NSToolbarFlexibleSpaceItemIdentifier);
            array.AddObject(NSToolbarItem.NSToolbarPrintItemIdentifier);
            return array;
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this.toolbarItems.Release();
            this.SendMessageSuper(ControllerClass, "dealloc");
        }

        private static void AddToolbarItem(NSMutableDictionary theDict, NSString identifier, NSString label, NSString paletteLabel, NSString toolTip, Id target, IntPtr settingSelector, Id itemContent, IntPtr action, NSMenu menu)
        {
            NSMenuItem mItem;

            // here we create the NSToolbarItem and setup its attributes in line with the parameters
            NSToolbarItem item = new NSToolbarItem(identifier);
            item.Autorelease();
            item.Label = label;
            item.PaletteLabel = paletteLabel;
            item.ToolTip = toolTip;
            // the settingSelector parameter can either be @selector(setView:) or @selector(setImage:).  Pass in the right
            // one depending upon whether your NSToolbarItem will have a custom view or an image, respectively
            // (in the itemContent parameter).  Then this next line will do the right thing automatically.
            item.PerformSelectorWithObject(settingSelector, itemContent);
            item.Target = target;
            item.Action = action;
            // If this NSToolbarItem is supposed to have a menu "form representation" associated with it (for text-only mode),
            // we set it up here.  Actually, you have to hand an NSMenuItem (not a complete NSMenu) to the toolbar item,
            // so we create a dummy NSMenuItem that has our real menu as a submenu.
            if (menu != null)
            {
                // we actually need an NSMenuItem here, so we construct one
                mItem = new NSMenuItem();
                mItem.Autorelease();
                mItem.Submenu = menu;
                mItem.Title = menu.Title;
                item.MenuFormRepresentation = mItem;
            }

            // Now that we've setup all the settings for this new toolbar item, we add it to the dictionary.
            // The dictionary retains the toolbar item for us, which is why we could autorelease it when we created
            // it (above).
            theDict[identifier] = item;
        }
    }
}