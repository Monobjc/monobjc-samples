using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.CocoaDragAndDrop
{
    [ObjectiveCClass]
    public class DragDropImageView : NSImageView
    {
        private static readonly Class DragDropImageViewClass = Class.Get(typeof (DragDropImageView));
        private bool highlight; //highlight the drop zone

        public DragDropImageView() {}

        public DragDropImageView(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("initWithCoder:")]
        public override Id InitWithCoder(NSCoder coder)
        {
            this.SendMessageSuper<Id>(DragDropImageViewClass, "initWithCoder:", coder);
            this.RegisterForDraggedTypes(NSImage.ImagePasteboardTypes);
            return this;
        }

        [ObjectiveCMessage("draggingEntered:")]
        public NSDragOperation DraggingEntered(INSDraggingInfo sender)
        {
            NSPasteboard pboard = sender.DraggingPasteboard;
            NSDragOperation operation = sender.DraggingSourceOperationMask;
            if (NSImage.CanInitWithPasteboard(pboard) && (operation & NSDragOperation.NSDragOperationCopy) == NSDragOperation.NSDragOperationCopy)
            {
                this.highlight = true;
                this.NeedsDisplay = true;
                return NSDragOperation.NSDragOperationCopy;
            }
            return NSDragOperation.NSDragOperationNone;
        }

        [ObjectiveCMessage("draggingExited:")]
        public void DraggingExited(INSDraggingInfo sender)
        {
            this.highlight = false;
            this.NeedsDisplay = true;
        }

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect rect)
        {
            this.SendMessageSuper(DragDropImageViewClass, "drawRect:", rect);
            if (this.highlight)
            {
                NSColor.GrayColor.Set();
                NSBezierPath.DefaultLineWidth = 5;
                NSBezierPath.StrokeRect(rect);
            }
        }

        [ObjectiveCMessage("prepareForDragOperation:")]
        public bool PrepareForDragOperation(INSDraggingInfo sender)
        {
            this.highlight = false;
            this.NeedsDisplay = true;
            NSPasteboard pboard = sender.DraggingPasteboard;
            return NSImage.CanInitWithPasteboard(pboard);
        }

        [ObjectiveCMessage("performDragOperation:")]
        public bool PerformDragOperation(INSDraggingInfo sender)
        {
            Id source = sender.DraggingSource;
            if (!this.Equals(source))
            {
                NSPasteboard pboard = sender.DraggingPasteboard;

                if (NSImage.CanInitWithPasteboard(pboard))
                {
                    this.Image.InitWithPasteboard(pboard);
                }

                NSURL fileURL = NSURL_AppKitAdditions.URLFromPasteboard(pboard);
                if (fileURL != null)
                {
                    this.Window.Title = fileURL.AbsoluteString;
                }
                else
                {
                    this.Window.Title = "No Name";
                }
            }
            return true;
        }

        [ObjectiveCMessage("windowWillUseStandardFrame:defaultFrame:")]
        public NSRect WindowWillUseStandardFrameDefaultFrame(NSWindow window, NSRect newFrame)
        {
            NSRect contentRect = this.Window.Frame;
            contentRect.size = this.Image.Size;
            return NSWindow.FrameRectForContentRectStyleMask(contentRect, window.StyleMask);
        }

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            NSPasteboard dragPasteboard = NSPasteboard.PasteboardWithName(NSPasteboard.NSDragPboard);
            NSImage dragImage = new NSImage(this.Image.Size);
            dragPasteboard.DeclareTypesOwner(NSArray.ArrayWithObject(NSPasteboard.NSTIFFPboardType), this);
            dragPasteboard.AddTypesOwner(NSArray.ArrayWithObject(NSPasteboard.NSPDFPboardType), this);

            dragImage.LockFocus();
            this.Image.DissolveToPointFraction(NSPoint.NSZeroPoint, 0.5f);
            dragImage.UnlockFocus();
            dragImage.ScalesWhenResized = true;
            dragImage.Size = this.Bounds.size;

            this.DragImageAtOffsetEventPasteboardSourceSlideBack(dragImage,
                                                                 this.Bounds.origin,
                                                                 NSSize.NSZeroSize,
                                                                 theEvent,
                                                                 dragPasteboard,
                                                                 this,
                                                                 true);
            dragImage.Release();
        }

        [ObjectiveCMessage("draggingSourceOperationMaskForLocal:")]
        public NSDragOperation DraggingSourceOperationMaskForLocal(bool flag)
        {
            return NSDragOperation.NSDragOperationCopy;
        }

        [ObjectiveCMessage("acceptsFirstMouse:")]
        public override bool AcceptsFirstMouse(NSEvent theEvent)
        {
            return true;
        }

        [ObjectiveCMessage("pasteboard:provideDataForType:")]
        public void PasteboardProvideDataForType(NSPasteboard sender, NSString type)
        {
            // Sender has accepted the drag and now we need to send the data for the type we promised
            if (type.Compare(NSPasteboard.NSTIFFPboardType) == NSComparisonResult.NSOrderedSame)
            {
                sender.SetDataForType(this.Image.TIFFRepresentation, NSPasteboard.NSTIFFPboardType);
            }
            else if (type.Compare(NSPasteboard.NSPDFPboardType) == NSComparisonResult.NSOrderedSame)
            {
                sender.SetDataForType(this.DataWithPDFInsideRect(this.Bounds), NSPasteboard.NSPDFPboardType);
            }
        }
    }
}