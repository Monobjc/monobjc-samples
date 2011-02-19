using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.CroppedImage
{
    [ObjectiveCClass]
    public partial class CroppingImageView : NSImageView
    {
        private static readonly Class CroppingImageViewClass = Class.Get(typeof (CroppingImageView));

        public static readonly NSString selectionChangedNotification = NSString.NSPinnedString("ImageSelectionChanged");

        private CropMarker selectionMarker;
        private bool shouldAntiAlias;

        public CroppingImageView() {}

        public CroppingImageView(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("takeSelectionMarkerStyleFrom:")]
        public void takeSelectionMarkerStyleFrom(NSPopUpButton sender)
        {
            NSRect stash = this.SelectionMarker.SelectedRect; // If we change styles, it's nice to keep the same area selected.
            MarkerStyle tag = (MarkerStyle) (int) sender.SelectedCell.CastTo<NSPopUpButtonCell>().Tag;
            switch (tag)
            {
                case MarkerStyle.PlainMarkerStyle:
                    this.SelectionMarker = new CropMarker(this);
                    this.SelectionMarker.SelectedRect = stash;
                    break;

                case MarkerStyle.FinderMarkerStyle:
                    this.SelectionMarker = new FinderStyleCropMarker(this);
                    this.SelectionMarker.SelectedRect = stash;
                    break;

                case MarkerStyle.IPhotoMarkerStyle:
                    this.SelectionMarker = new IPhotoStyleCropMarker(this);
                    this.SelectionMarker.SelectedRect = stash;
                    break;

                case MarkerStyle.LassoMarkerStyle:
                    this.SelectionMarker = new LassoStyleCropMarker(this);
                    break;
            }
            this.NeedsDisplay = true;
        }

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect rect)
        {
            this.SendMessageSuper(CroppingImageViewClass, "drawRect:", rect);
            this.SelectionMarker.drawCropMarker();
        }

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            this.SelectionMarker.mouseDown(theEvent);
        }

        [ObjectiveCMessage("mouseUp:")]
        public override void MouseUp(NSEvent theEvent)
        {
            this.SelectionMarker.mouseUp(theEvent);
            this.SelectionChanged();
            this.PostSelectionChangedNotification(); // This is how the controller knows to redraw the second NSImageView.
        }

        [ObjectiveCMessage("mouseDragged:")]
        public override void MouseDragged(NSEvent theEvent)
        {
            this.selectionMarker.mouseDragged(theEvent);
            if (this.IsContinuous)
            {
                this.PostSelectionChangedNotification();
            }
            this.SelectionChanged();
        }

        public void SelectionChanged()
        {
            this.NeedsDisplay = true;
        }

        public void PostSelectionChangedNotification()
        {
            NSNotificationCenter.DefaultCenter.PostNotificationNameObject(selectionChangedNotification, this);
        }

        /// <summary>
        /// Returns an autoreleased NSImage, consisting of the selected portion of the reciever's image.  
        /// If there's no selection, this method will return the original image.
        /// </summary>
        public NSImage CroppedImage()
        {
            NSRect sourceImageRect = RectCoveredByImageInBounds(this.Cell.CastTo<NSImageCell>(), this.Bounds);
            NSRect newImageBounds = NSRect.NSIntersectionRect(this.selectionMarker.SelectedRect, sourceImageRect);

            if (!NSRect.NSIsEmptyRect(newImageBounds))
            {
                NSImage newImage = new NSImage(sourceImageRect.size);
                NSAffineTransform pathAdjustment = NSAffineTransform.Transform;
                NSBezierPath croppingPath = this.selectionMarker.SelectedPath;
                pathAdjustment.TranslateXByYBy(-NSRect.NSMinX(sourceImageRect), -NSRect.NSMinY(sourceImageRect));
                croppingPath = pathAdjustment.TransformBezierPath(croppingPath);

                newImage.LockFocus();
                NSGraphicsContext.CurrentContext.ShouldAntialias = this.shouldAntiAlias;

                NSColor.BlackColor.Set();
                croppingPath.Fill();
                this.Image.CompositeToPointOperation(NSPoint.NSZeroPoint, NSCompositingOperation.NSCompositeSourceIn);
                newImage.UnlockFocus();

                newImage.Autorelease();
                return newImage;
            }
            return this.Image;
        }

        /// <summary>
        /// Should be a CropMarker or a subclass thereof, but I'm not in the mood for strong typing.
        /// </summary>
        public CropMarker SelectionMarker
        {
            get
            {
                if (this.selectionMarker == null)
                {
                    this.selectionMarker = new CropMarker(this);
                }
                return this.selectionMarker;
            }
            set { this.selectionMarker = value; }
        }

        [ObjectiveCMessage("takeSelectionColorFrom:")]
        public void TakeSelectionColorFrom(NSColorWell sender)
        {
            this.SelectionMarker.setColor(sender.Color);
            this.NeedsDisplay = true;
        }

        [ObjectiveCMessage("takeAntiAliasModeFrom:")]
        public void TakeAntiAliasModeFrom(NSButtonCell sender)
        {
            this.shouldAntiAlias = (sender.IntValue > 0);
            this.PostSelectionChangedNotification();
        }

        [ObjectiveCMessage("takeContinuousModeFrom:")]
        public void TakeContinuousModeFrom(NSButtonCell sender)
        {
            this.IsContinuous = (sender.IntValue > 0);
        }

        /// <summary>
        /// This is a work-around to deal with the fact that NSImageCell won't tell me the rectangle *actually* covered by its image, but NSCell will.
        /// </summary>
        public static NSRect RectCoveredByImageInBounds(NSImageCell imageCell, NSRect bounds)
        {
            return imageCell.SendMessageSuper<NSRect>(NSImageCell.NSImageCellClass, "imageRectForBounds:", bounds);
        }
    }
}