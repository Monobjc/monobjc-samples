using System;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.Quartz;

namespace Monobjc.Samples.PDFKitViewer
{
    [ObjectiveCClass]
    public partial class MyPDFDocument : NSDocument
    {
        private static readonly Class MyPDFDocumentClass = Class.GetClassFromType(typeof (MyPDFDocument));

        private PDFOutline _outline;
        private NSMutableArray _searchResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDelegate"/> class.
        /// </summary>
        public MyPDFDocument() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDelegate"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public MyPDFDocument(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            // No more notifications.
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);

            // Clean up.
            this._searchResults.SafeRelease();

            // Super.
            this.SendMessageSuper(MyPDFDocumentClass, "dealloc");
        }

        public override NSString WindowNibName
        {
            [ObjectiveCMessage("windowNibName")]
            get { return "MyDocument"; }
        }

        [ObjectiveCMessage("windowControllerDidLoadNib:")]
        public override void WindowControllerDidLoadNib(NSWindowController controller)
        {
            // Super.
            this.SendMessageSuper(MyPDFDocumentClass, "windowControllerDidLoadNib:", controller);

            if (this.FileName != null)
            {
                PDFDocument pdfDoc = new PDFDocument(NSURL.FileURLWithPath(this.FileName)).Autorelease<PDFDocument>();
                this._pdfView.Document = pdfDoc;
            }

            // Page changed notification.
            NSNotificationCenter.DefaultCenter.AddObserverSelectorNameObject(this, ObjectiveCRuntime.Selector("pageChanged:"), PDFView.PDFViewPageChangedNotification, this._pdfView);

            // Find notifications.
            NSNotificationCenter.DefaultCenter.AddObserverSelectorNameObject(this, ObjectiveCRuntime.Selector("startFind:"), PDFDocument.PDFDocumentDidBeginFindNotification, this._pdfView.Document);
            NSNotificationCenter.DefaultCenter.AddObserverSelectorNameObject(this, ObjectiveCRuntime.Selector("findProgress:"), PDFDocument.PDFDocumentDidEndPageFindNotification, this._pdfView.Document);
            NSNotificationCenter.DefaultCenter.AddObserverSelectorNameObject(this, ObjectiveCRuntime.Selector("endFind:"), PDFDocument.PDFDocumentDidEndFindNotification, this._pdfView.Document);

            // Set self to be delegate (find).
            this._pdfView.Document.Delegate = this;

            // Get outline.
            this._outline = this._pdfView.Document.OutlineRoot;
            if (this._outline != null)
            {
                this._outline.Retain();

                // Remove text that says, "No outline."
                this._noOutlineText.RemoveFromSuperview();
                this._noOutlineText = null;

                // Force it to load up.
                this._outlineView.ReloadData();
            }
            else
            {
                // Remove outline view (leaving instead text that says, "No outline.").
                this._outlineView.EnclosingScrollView.RemoveFromSuperview();
                this._outlineView = null;
            }

            // Open drawer.
            this._drawer.Open();

            // Size the window.
            NSSize windowSize = this._pdfView.RowSizeForPage(this._pdfView.CurrentPage);
            if (((this._pdfView.DisplayMode & PDFDisplayMode.kPDFDisplaySinglePageContinuous) == PDFDisplayMode.kPDFDisplaySinglePageContinuous) &&
                (this._pdfView.Document.PageCount > 1))
            {
                windowSize.width += NSScroller.ScrollerWidth;
            }

            controller.Window.SetContentSize(windowSize);
        }

        [ObjectiveCMessage("dataRepresentationOfType:")]
        public override NSData DataRepresentationOfType(NSString aType)
        {
            // Insert code here to write your document from the given data.  You can also choose to override 
            // -fileWrapperRepresentationOfType: or -writeToFile:ofType: instead.
            return null;
        }

        [ObjectiveCMessage("loadDataRepresentation:ofType:")]
        public override bool LoadDataRepresentationOfType(NSData data, NSString aType)
        {
            // Insert code here to read your document from the given data.  You can also choose to override 
            // -loadFileWrapperRepresentation:ofType: or -readFromFile:ofType: instead.
            return true;
        }

        partial void ToggleDrawer(Id sender)
        {
            this._drawer.Toggle(this);
        }

        partial void TakeDestinationFromOutline(Id sender)
        {
            // Get the destination associated with the search result list.  Tell the PDFView to go there.
            Id selection = this._outlineView.ItemAtRow(this._outlineView.SelectedRow);
            PDFDestination destination = selection.SendMessage<PDFDestination>("destination");
            this._pdfView.GoToDestination(destination);
        }

        [ObjectiveCMessage("displaySinglePage:")]
        public void DisplaySinglePage(Id sender)
        {
            // Display single page mode.
            if (this._pdfView.DisplayMode > PDFDisplayMode.kPDFDisplaySinglePageContinuous)
            {
                this._pdfView.DisplayMode = this._pdfView.DisplayMode - (int) PDFDisplayMode.kPDFDisplayTwoUp;
            }
        }

        [ObjectiveCMessage("displayTwoUp:")]
        public void DisplayTwoUp(Id sender)
        {
            // Display two-up.
            if (this._pdfView.DisplayMode < PDFDisplayMode.kPDFDisplayTwoUp)
            {
                this._pdfView.DisplayMode = this._pdfView.DisplayMode + (int) PDFDisplayMode.kPDFDisplayTwoUp;
            }
        }

        [ObjectiveCMessage("pageChanged:")]
        public void PageChanged(NSNotification notification)
        {
            // Skip out if there is no outline.
            if (this._pdfView.Document.OutlineRoot == null)
            {
                return;
            }

            // What is the new page number (zero-based).
            uint newPageIndex = this._pdfView.Document.IndexForPage(this._pdfView.CurrentPage);

            // Walk outline view looking for best firstpage number match.
            int newlySelectedRow = -1;
            int numRows = this._outlineView.NumberOfRows;
            for (int i = 0; i < numRows; i++)
            {
                // Get the destination of the given row....
                PDFOutline outlineItem = this._outlineView.ItemAtRow(i).CastTo<PDFOutline>();

                if (this._pdfView.Document.IndexForPage(outlineItem.Destination.Page) == newPageIndex)
                {
                    newlySelectedRow = i;
                    this._outlineView.SelectRowIndexesByExtendingSelection(NSIndexSet.IndexSetWithIndex((uint) newlySelectedRow), false);
                    break;
                }
                if (this._pdfView.Document.IndexForPage(outlineItem.Destination.Page) > newPageIndex)
                {
                    newlySelectedRow = i - 1;
                    this._outlineView.SelectRowIndexesByExtendingSelection(NSIndexSet.IndexSetWithIndex((uint) newlySelectedRow), false);
                    break;
                }
            }

            // Auto-scroll.
            if (newlySelectedRow != -1)
            {
                this._outlineView.ScrollRowToVisible(newlySelectedRow);
            }
        }

        partial void DoFind(Id sender)
        {
            if (this._pdfView.Document.IsFinding)
            {
                this._pdfView.Document.CancelFindString();
            }

            // Lazily allocate _searchResults.
            if (this._searchResults == null)
            {
                this._searchResults = NSMutableArray.ArrayWithCapacity(10).Retain<NSMutableArray>();
            }

            NSString value = sender.SendMessage<NSString>("stringValue");
            this._pdfView.Document.BeginFindStringWithOptions(value, (int) NSStringCompareOptions.NSCaseInsensitiveSearch);
        }

        [ObjectiveCMessage("startFind:")]
        public void StartFind(NSNotification notification)
        {
            // Empty arrays.
            this._searchResults.RemoveAllObjects();

            this._searchTable.ReloadData();
            this._searchProgress.StartAnimation(this);
        }

        [ObjectiveCMessage("findProgress:")]
        public void FindProgress(NSNotification notification)
        {
            Id id = notification.UserInfo.ObjectForKey((NSString) "PDFDocumentPageIndex");
            double pageIndex = id.SendMessage<double>("doubleValue");
            this._searchProgress.DoubleValue = pageIndex/ (uint) this._pdfView.Document.PageCount;
        }

        [ObjectiveCMessage("didMatchString:")]
        public void DidMatchString(PDFSelection instance)
        {
            // Add page label to our array.
            this._searchResults.AddObject(instance.Copy());

            // Force a reload.
            this._searchTable.ReloadData();
        }

        [ObjectiveCMessage("endFind:")]
        public void EndFind(NSNotification notification)
        {
            this._searchProgress.StopAnimation(this);
            this._searchProgress.DoubleValue = 0;
        }

        // The table view is used to hold search results.  Column 1 lists the page number for the search result, 
        // column two the section in the PDF (x-ref with the PDF outline) where the result appears.

        [ObjectiveCMessage("numberOfRowsInTableView:")]
        public NSInteger NumberOfRowsInTableView(NSTableView aTableView)
        {
            return (this._searchResults != null) ? (int) this._searchResults.Count : 0;
        }

        [ObjectiveCMessage("tableView:objectValueForTableColumn:row:")]
        public Id TableViewObjectValueForTableColumnRow(NSTableView aTableView, NSTableColumn theColumn, NSInteger rowIndex)
        {
            String identifier = theColumn.Identifier.CastTo<NSString>();
            if (identifier.Equals("page"))
            {
                PDFSelection selection = this._searchResults.ObjectAtIndex((uint) rowIndex).CastTo<PDFSelection>();
                return selection.Pages.ObjectAtIndex(0).CastTo<PDFPage>().Label;
            }
            else if (identifier.Equals("section"))
            {
                PDFSelection selection = this._searchResults.ObjectAtIndex((uint) rowIndex).CastTo<PDFSelection>();
                PDFOutline outline = this._pdfView.Document.OutlineItemForSelection(selection);
                return (outline != null) ? outline.Label : null;
            }
            else
            {
                return null;
            }
        }

        [ObjectiveCMessage("tableViewSelectionDidChange:")]
        public void TableViewSelectionDidChange(NSNotification notification)
        {
            // What was selected.  Skip out if the row has not changed.
            int rowIndex = notification.Object.CastTo<NSTableView>().SelectedRow;
            if (rowIndex >= 0)
            {
                this._pdfView.CurrentSelection = this._searchResults.ObjectAtIndex((uint) rowIndex).CastTo<PDFSelection>();
                this._pdfView.CenterSelectionInVisibleArea(this);
            }
        }

// The outline view is for the PDF outline.  Not all PDF's have an outline.

        [ObjectiveCMessage("outlineView:numberOfChildrenOfItem:")]
        public NSInteger OutlineViewNumberOfChildrenOfItem(NSOutlineView outlineView, Id item)
        {
            if (item == null)
            {
                if (this._outline != null)
                {
                    return (int) this._outline.NumberOfChildren;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return item.CastTo<PDFOutline>().NumberOfChildren;
            }
        }

        [ObjectiveCMessage("outlineView:child:ofItem:")]
        public Id OutlineViewChildOfItem(NSOutlineView outlineView, NSInteger index, Id item)
        {
            if (item == null)
            {
                if (this._outline != null)
                {
                    return this._outline.ChildAtIndex(index).Retain();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return item.CastTo<PDFOutline>().ChildAtIndex((uint) index).Retain();
            }
        }

        [ObjectiveCMessage("outlineView:isItemExpandable:")]
        public bool OutlineViewIsItemExpandable(NSOutlineView outlineView, Id item)
        {
            if (item == null)
            {
                if (this._outline != null)
                {
                    return (this._outline.NumberOfChildren > 0);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return (item.CastTo<PDFOutline>().NumberOfChildren > 0);
            }
        }

        [ObjectiveCMessage("outlineView:objectValueForTableColumn:byItem:")]
        public Id OutlineViewObjectValueForTableColumnByItem(NSOutlineView outlineView, NSTableColumn tableColumn, Id item)
        {
            PDFOutline outline = item.CastTo<PDFOutline>();
            return outline.Label;
        }
    }
}