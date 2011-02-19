using System;
using Monobjc.ApplicationServices;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.Quartz;

namespace Monobjc.Samples.MiniBooth
{
    [ObjectiveCClass]
    public partial class AppController : NSObject
    {
        public AppController() {}

        public AppController(IntPtr nativePointer) : base(nativePointer) {}

        [ObjectiveCMessage("_didSelectComposition:")]
        public void DidSelectComposition(NSNotification notification)
        {
            QCComposition composition = notification.UserInfo.ObjectForKey((NSString)"QCComposition").CastTo<QCComposition>();

            /* Set the identifier of the selected composition on the "compositionIdentifier" input of the composition,
			   which passes it in turn to a Composition Loader patch which loads the composition and applies it to the video input */
            this.qcView.SetValueForInputKey(composition.Identifier, "compositionIdentifier");
        }

        [ObjectiveCMessage("applicationDidFinishLaunching:")]
        public void ApplicationDidFinishLaunching(NSNotification notification)
        {
            QCCompositionPickerPanel pickerPanel = QCCompositionPickerPanel.SharedCompositionPickerPanel;

            /* Load our composition file on the QCView and start rendering */
            if (!this.qcView.LoadCompositionFromFile(NSBundle.MainBundle.PathForResourceOfType("Composition", "qtz")))
            {
                NSApplication.SharedApplication.Terminate(null);
            }
            this.qcView.StartRendering();

            /* Configure and show the Composition Picker panel */
            pickerPanel.CompositionPickerView.AllowsEmptySelection = true;
            pickerPanel.CompositionPickerView.ShowsCompositionNames = true;
            pickerPanel.CompositionPickerView.SetCompositionsFromRepositoryWithProtocolAndAttributes(QCComposition.QCCompositionProtocolImageFilter, null);
            pickerPanel.CompositionPickerView.StartAnimation(this);
            pickerPanel.OrderOut(null);

            /* Register for composition picker panel notifications */
            NSNotificationCenter.DefaultCenter.AddObserverSelectorNameObject(this, ObjectiveCRuntime.Selector("_didSelectComposition:"), QCCompositionPickerPanel.QCCompositionPickerPanelDidSelectCompositionNotification, null);
        }

        [ObjectiveCMessage("applicationWillTerminate:")]
        public void ApplicationWillTerminate(NSNotification notification)
        {
            /* Unregister from composition picker panel notifications */
            NSNotificationCenter.DefaultCenter.RemoveObserverNameObject(this, QCCompositionPickerPanel.QCCompositionPickerPanelDidSelectCompositionNotification, null);
        }

        [ObjectiveCMessage("windowWillClose:")]
        public void WindowWillClose(NSNotification notification)
        {
            NSApplication.SharedApplication.Terminate(this);
        }

        #region IBActions

        partial void ToggleCompositionPicker(Id sender)
        {
            QCCompositionPickerPanel pickerPanel = QCCompositionPickerPanel.SharedCompositionPickerPanel;

            /* Toggle the Composition Picker panel visibility */
            if (pickerPanel.IsVisible)
            {
                pickerPanel.OrderOut(sender);
            }
            else
            {
                pickerPanel.OrderFront(sender);
            }
        }

        partial void SavePNG(Id sender)
        {
            /* Display the save panel */
            NSSavePanel savePanel = NSSavePanel.SavePanel;
            savePanel.RequiredFileType = new NSString("PNG");
            Id imageRef;
            IntPtr destinationRef;

            if (savePanel.RunModalForDirectoryFile(null, "My Picture.png") == 1)
            {
                /* Grab the current contents of the QCView as a CGImageRef and use ImageIO to save it as a PNG file */
                if ((imageRef = this.qcView.CreateSnapshotImageOfType("CGImage")) != null)
                {
                    if ((destinationRef = CGImageDestination.CreateWithURL(
                        NSURL.FileURLWithPath(savePanel.Filename),
                        "public.png",
                        1,
                        null)
                        )
                        != IntPtr.Zero)
                    {
                        CGImageDestination.AddImage(destinationRef, imageRef.NativePointer, null);
                        if (!CGImageDestination.Finalize(destinationRef))
                        {
#if DEBUG
                            Console.WriteLine("Coult not save image file to destination");
#endif
                        }
                        destinationRef = IntPtr.Zero;
                    }
                    else
                    {
                        CGImage.Release(imageRef.NativePointer);
                    }
                }
            }
        }

        #endregion
    }

    [ObjectiveCClass]
    public class AppView : QCView
    {
		public static readonly Class AppViewClass = Class.Get (typeof(AppView));

        public AppView() {}
		
        public AppView(IntPtr nativePointer) : base(nativePointer) {}
		
        public AppView(NSRect frameRect) : base(frameRect) {}


        /* We override this method to know whenever the composition is rendered in the QCView */

        [ObjectiveCMessage("renderAtTime:arguments:")]
        public override bool RenderAtTimeArguments(double time, NSDictionary arguments)
        {
            Id image;

            /* Call super so that rendering happens */

            /* TODO: using Monobjc derived method... have to fix this... weird bug */
            if (!ObjectiveCRuntime.SendMessageSuper<bool>(this, AppViewClass, "renderAtTime:arguments:", time, arguments))
            {
                return false;
            }

            /* Retrieve the current video input image from the "videoImage" output of the 
			   composition then use it as a default image for the Composition Picker panel
			   Because we pass the image from one Quartz Composer object to another one, 
			   we can use the optimized QCImage type
			*/
            if ((image = this.ValueForOutputKeyOfType(new NSString("videoImage"), new NSString("QCImage"))) != null)
            {
                QCCompositionPickerPanel.SharedCompositionPickerPanel.CompositionPickerView.SetDefaultValueForInputKey(image, QCComposition.QCCompositionInputImageKey);
            }
            return true;
        }
    }
}