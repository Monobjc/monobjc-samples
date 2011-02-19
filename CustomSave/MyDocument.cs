using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.CustomSave
{
    [ObjectiveCClass]
    public partial class MyDocument : NSDocument
    {
        private static readonly Class MyDocumentClass = Class.Get(typeof (MyDocument));

        private NSData model;
		
        private NSSavePanel savePanel;

        public MyDocument() {}

        public MyDocument(IntPtr nativePointer)
            : base(nativePointer) {}

        public override NSString WindowNibName
        {
            [ObjectiveCMessage("windowNibName")]
            get { return "MyDocument"; }
        }

        [ObjectiveCMessage("windowControllerDidLoadNib:")]
        public override void WindowControllerDidLoadNib(NSWindowController aController)
        {
            this.SendMessageSuper(MyDocumentClass, "windowControllerDidLoadNib:", aController);

            // Add any code here that need to be executed once the windowController has loaded the document's window.
            // ..

            // the following code adds the horizontal scroll bar to the scroll view and makes the text view horizontally
            // resizable so it can display text of any width:
            //
            NSRect frameRect;
            NSWindow theWindow = this.textView.Window;

            frameRect = this.textView.Frame;

            NSScrollView scrollview = this.textView.EnclosingScrollView;

            NSSize contentSize = scrollview.ContentSize;
            scrollview.BorderType = NSBorderType.NSNoBorder;
            scrollview.HasVerticalScroller = true;
            scrollview.HasHorizontalScroller = true; // for horizontal scrolling

            scrollview.AutoresizingMask = NSAutoresizingMask .NSViewWidthSizable | NSAutoresizingMask.NSViewHeightSizable;
            frameRect = NSRect.NSMakeRect(0, 0, contentSize.width, contentSize.height);
            this.textView.Frame = frameRect;

            this.textView.MinSize = NSSize.NSMakeSize(contentSize.width, contentSize.height);
            this.textView.MaxSize = NSSize.NSMakeSize(float.MaxValue, float.MaxValue);

            this.textView.IsVerticallyResizable = true;
            this.textView.IsHorizontallyResizable = true; // for horizontal scrolling
            this.textView.AutoresizingMask = NSAutoresizingMask.NSViewWidthSizable | NSAutoresizingMask.NSViewHeightSizable;
            this.textView.TextContainer.ContainerSize = NSSize.NSMakeSize(float.MaxValue, float.MaxValue);
            this.textView.TextContainer.WidthTracksTextView = true;

            scrollview.DocumentView = this.textView;
            theWindow.ContentView = scrollview;
            theWindow.MakeKeyAndOrderFront(null);
            theWindow.MakeFirstResponder(this.textView);

            this.UpdateView();
        }

        [ObjectiveCMessage("dataOfType:error:")]
        public NSData DataOfType(NSString typeName, IntPtr outError)
        {
            // return a data object that contains the contents of the document
            this.UpdateModel();
            return this.model;
        }

        [ObjectiveCMessage("readFromData:ofType:error:")]
        public bool readFromData(NSData data, NSString typeName, IntPtr outError)
        {
            // set the contents of this document by reading from data of a specified type
            this.Model = data;
            this.UpdateView();
            return true;
        }

        public NSData Model
        {
            [ObjectiveCMessage("model")]
            get { return this.model; }
            [ObjectiveCMessage("setModel:")]
            set
            {
				NSData oldValue = this.model;
                this.model = value.SafeRetain<NSData>();
                oldValue.SafeRelease();
            }
        }

        public NSRange EntireRange
        {
            [ObjectiveCMessage("entireRange")]
            get { return NSRange.NSMakeRange(0, this.textView.String.Length); }
        }

        [ObjectiveCMessage("updateModel")]
        public void UpdateModel()
        {
            this.Model = this.textView.RTFFromRange(this.EntireRange);
        }

        [ObjectiveCMessage("updateView")]
        public void UpdateView()
        {
            this.textView.ReplaceCharactersInRangeWithRTF(this.EntireRange, this.Model);
        }

        [ObjectiveCMessage("prepareSavePanel:")]
        public override bool PrepareSavePanel(NSSavePanel inSavePanel)
        {
            inSavePanel.Directory = FoundationFramework.NSHomeDirectory();
            inSavePanel.Delegate = this;
            inSavePanel.Message = "This is a customized save dialog for saving text files:";
            inSavePanel.AccessoryView = this.saveDialogCustomView;
            inSavePanel.RequiredFileType = "txt";
            inSavePanel.NameFieldLabel = "FILE NAME:";
            this.savePanel = inSavePanel;
            return true;
        }

        [ObjectiveCMessage("compareFilename:name1:name2:caseSensitive:")]
        public NSComparisonResult PanelCompareFilenameWithCaseSensitive(Id sender, NSString name1, NSString name2, bool caseSensitive)
        {
            return name1.Compare(name2);
        }

        [ObjectiveCMessage("panel:isValidFilename:")]
        public bool PanelIsValiFilename(Id sender, NSString filename)
        {
            bool result = true;

            NSURL url = new NSURL(filename);
            if ((url.NativePointer != IntPtr.Zero) && url.IsFileURL)
            {
                NSArray pathPieces = url.Path.PathComponents;
                NSString actualFilename = pathPieces.ObjectAtIndex(pathPieces.Count - 1).CastTo<NSString>();
                if (actualFilename.IsEqualToString("text.txt"))
                {
                    NSAlert alert = NSAlert.AlertWithMessageTextDefaultButtonAlternateButtonOtherButtonInformativeTextWithFormat(NSString.String,
                                                                                                                                 "OK",
                                                                                                                                 null,
                                                                                                                                 null,
                                                                                                                                 "Please pick a new name.");
                    alert.RunModal();
                    result = false;
                }
            }

            return result;
        }

        [ObjectiveCMessage("panel:userEnteredFilename:confirmed:")]
        public NSString PanelUserEnteredFilenameConfirmed(Id sender, NSString filename, bool okFlag)
        {
            if (okFlag && (this.appendCheck.State > 0))
            {
                NSMutableString returnFileName = new NSMutableString(filename);
                NSRange searchRange = returnFileName.RangeOfString(".");
                if (searchRange.length > 0)
                {
                    returnFileName.InsertStringAtIndex("!", searchRange.location);
                }
                else
                {
                    returnFileName.AppendString("!");
                }
                return returnFileName;
            }
            return filename;
        }

        [ObjectiveCMessage("panel:willExpand:")]
        public void PanelWillExpand(Id sender, bool expanding)
        {
            if (this.soundOnCheck.State > 0)
            {
                if (expanding)
                {
                    NSSound.SoundNamed("Pop").Play();
                }
                else
                {
                    NSSound.SoundNamed("Blow").Play();
                }
            }

            this.navigatePackages.IsHidden = !expanding;
        }

        [ObjectiveCMessage("panel:directoryDidChange:")]
        public void PanelDirectoryDidChange(Id sender, NSString path)
        {
            if (this.soundOnCheck.State > 0)
            {
                NSSound.SoundNamed("Frog").Play();
            }
        }

        [ObjectiveCMessage("panelSelectionDidChange:")]
        public void PanelSelectionDidChange(Id sender)
        {
            if (this.soundOnCheck.State > 0)
            {
                NSSound.SoundNamed("Hero").Play();
            }
        }

        [ObjectiveCMessage("filePackagesAsDirAction:")]
        public void FilePackagesAsDirAction(NSButton sender)
        {
            this.savePanel.TreatsFilePackagesAsDirectories = (sender.State > 0);
        }
    }
}