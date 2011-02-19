using System;
using Monobjc.AppKit;
using Monobjc.DiscRecording;
using Monobjc.DiscRecordingUI;
using Monobjc.Foundation;

namespace Monobjc.Samples.Eraser
{
    [ObjectiveCClass]
    public class AppController : NSObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppController"/> class.
        /// </summary>
        public AppController() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AppController"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public AppController(IntPtr nativePointer)
            : base(nativePointer) {}

        /// <summary>
        /// When the application finishes launching, we'll set up a notification to be sent when the
        /// erase completes. This notification is sent by the DREraseProgressPanel to observers
        /// when the erase is 100% finished.
        /// </summary>
        [ObjectiveCMessage("applicationDidFinishLaunching:")]
        public void ApplicationDidFinishLaunching(NSNotification notification)
        {
            // Gets the DRErase icon and assign it another name
            NSImage icon = NSImage.ImageNamed(DiscRecordingUIFramework.DREraseIcon);
            icon.Name = "OldDREraseIcon";

            // Gets the Monobjc icon and assign it as default icon
            icon = new NSImage(NSBundle.MainBundle.PathForImageResource("Monobjc.icns"));
            icon.Name = DiscRecordingUIFramework.DREraseIcon;

            NSNotificationCenter.DefaultCenter.AddObserverSelectorNameObject(this,
                                                                             ObjectiveCRuntime.Selector("eraseCompleted:"),
                                                                             DREraseProgressPanel.DREraseProgressPanelDidFinishNotification,
                                                                             null);

            this.EraseCompleted(null);
        }

        /// <summary>
        /// Every time the erase completes, put up the erase dialog and let the user pick another
        /// drive to use for erasing discs.
        /// </summary>
        [ObjectiveCMessage("eraseCompleted:")]
        public void EraseCompleted(NSNotification notification)
        {
            DREraseSetupPanel esp = DREraseSetupPanel.SetupPanel;
            esp.Delegate = this;

            if (esp.RunSetupPanel() == NSPanel.NSOKButton)
            {
                DREraseProgressPanel epp = DREraseProgressPanel.ProgressPanel;
                epp.Delegate = this;
                epp.BeginProgressPanelForErase(esp.EraseObject);

                /* If you wanted to run this as a sheet you would have done
                 * [epp beginProgressSheetForErase:[esp eraseObject]];
                 */
            }
            else
            {
                NSApplication.SharedApplication.Terminate(this);
            }
        }

        [ObjectiveCMessage("setupPanel:deviceCouldBeTarget:")]
        public bool SetupPanelDeviceCouldBeTarget(DRSetupPanel aPanel, DRDevice device)
        {
            return true;
        }

        [ObjectiveCMessage("eraseProgressPanel:eraseDidFinish:")]
        public bool EraseProgressPanelEraseDidFinish(DREraseProgressPanel theErasePanel, DRErase erase)
        {
            NSDictionary eraseStatus = erase.Status;
            NSString state = eraseStatus[DRStatus.DRStatusStateKey].CastTo<NSString>();

            if (state.IsEqualToString(DRStatus.DRStatusStateFailed))
            {
                NSDictionary errorStatus = eraseStatus[DRStatus.DRErrorStatusKey].CastTo<NSDictionary>();
                NSString errorString = errorStatus[DRStatus.DRErrorStatusErrorStringKey].CastTo<NSString>();

                Console.WriteLine("The erase failed (" + errorString + ")!");
            }
            else
            {
                Console.WriteLine("Erase finished fine");            }

            return true;
        }    }
}