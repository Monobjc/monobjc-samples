using System;
using Monobjc.Foundation;
using Monobjc.AppKit;

namespace Monobjc.Samples.CroppedImage
{
    [ObjectiveCClass]
    public partial class CropImageController : NSObject
    {
        public CropImageController() {}

        public CropImageController(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // Sync up the image views with the controls in the window.
            this.imageView.TakeSelectionColorFrom(this.colorWell);
            this.imageView.takeSelectionMarkerStyleFrom(this.popUp);
            this.imageView.TakeAntiAliasModeFrom(this.antiAliasCheckbox);
            this.imageView.TakeContinuousModeFrom(this.continousModeCheckbox);
            // Sign up to be told when the user changes the selection.
            NSNotificationCenter.DefaultCenter.AddObserverSelectorNameObject(this, ObjectiveCRuntime.Selector("selectionChanged:"), CroppingImageView.selectionChangedNotification, this.imageView);
        }

        partial void ImageChanged(Id sender)
        {
            //if (sender != null)
            //sender.Cell.IsHighlighted = false;
            this.croppedImageView.Image = this.imageView.CroppedImage();
        }

        [ObjectiveCMessage("selectionChanged:")]
        public void SelectionChanged(NSNotification notification)
        {
            this.croppedImageView.Image = this.imageView.CroppedImage();
            this.croppedImageView.NeedsDisplay = true;
        }

        partial void ShowTheApe(Id sender)
        {
            this.imageView.Image = NSImage.ImageNamed("mandrill");
            this.ImageChanged(null);
        }

        [ObjectiveCMessage("saveCroppedImage:")]
        public void SaveCroppedImage(Id sender)
        {
            NSSavePanel.SavePanel.BeginSheetForDirectoryFileModalForWindowModalDelegateDidEndSelectorContextInfo(
                FoundationFramework.NSHomeDirectory(),
                "Tinted Image",
                this.window,
                this.SavePanelDidEndReturnCodeContextInfo,
                IntPtr.Zero);
        }

        partial void OpenDocument(Id sender)
        {
            NSOpenPanel.OpenPanel.BeginSheetForDirectoryFileModalForWindowModalDelegateDidEndSelectorContextInfo(
                FoundationFramework.NSHomeDirectory(),
                null,
                this.window,
                this.OpenPanelDidEndReturnCodeContextInfo,
                IntPtr.Zero);
        }

        partial void SaveDocument(Id sender)
        {
            NSSavePanel.SavePanel.BeginSheetForDirectoryFileModalForWindowModalDelegateDidEndSelectorContextInfo(
                FoundationFramework.NSHomeDirectory(),
                "Cropped Image",
                this.window,
                this.SavePanelDidEndReturnCodeContextInfo,
                IntPtr.Zero);
        }

        public void OpenPanelDidEndReturnCodeContextInfo(NSOpenPanel sheet, NSInteger returnCode, IntPtr contextInfo)
        {
            NSImage image = new NSImage(sheet.Filename).SafeAutorelease();
            this.imageView.Image = image;
            this.ImageChanged(null);
        }

        public void SavePanelDidEndReturnCodeContextInfo(NSSavePanel sheet, NSInteger returnCode, IntPtr contextInfo)
        {
            if (returnCode == NSPanel.NSOKButton)
            {
                NSString filename = sheet.Filename + ".tiff";
                this.imageView.CroppedImage().TIFFRepresentation.WriteToFileAtomically(filename, true);
            }
        }

        [ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
        public bool applicationShouldTerminateAfterLastWindowClosed(NSApplication theApplication)
        {
            return true;
        }
    }
}