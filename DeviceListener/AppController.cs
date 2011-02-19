using System;
using System.Text;
using Monobjc.AppKit;
using Monobjc.DiscRecording;
using Monobjc.Foundation;

namespace Monobjc.Samples.DeviceListener
{
    [ObjectiveCClass]
    public partial class AppController : NSObject
    {
        private NSMutableArray deviceList;

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

        [ObjectiveCMessage("applicationDidFinishLaunching:")]
        public void ApplicationDidFinishLaunching(NSNotification notification)
        {
            this.textView.Font = NSFont.FontWithNameSize("Courier", 24.0f);

            this.deviceList = new NSMutableArray(DRDevice.Devices.Count);

            DRNotificationCenter.CurrentRunLoopCenter.AddObserverSelectorNameObject(this, ObjectiveCRuntime.Selector("deviceDisappeared:"), DRDevice.DRDeviceDisappearedNotification, null);
            DRNotificationCenter.CurrentRunLoopCenter.AddObserverSelectorNameObject(this, ObjectiveCRuntime.Selector("deviceAppeared:"), DRDevice.DRDeviceAppearedNotification, null);
        }

        [ObjectiveCMessage("deviceAppeared:")]
        public void DeviceAppeared(NSNotification aNotification)
        {
            DRDevice newDevice = aNotification.Object.CastTo<DRDevice>();

            this.deviceList.AddObject(newDevice);

            DRNotificationCenter.CurrentRunLoopCenter.AddObserverSelectorNameObject(this, ObjectiveCRuntime.Selector("deviceStateChanged:"), DRDevice.DRDeviceStatusChangedNotification, null);

            this.UpdateUI();
        }

        [ObjectiveCMessage("deviceDisappeared:")]
        public void DeviceDisappeared(NSNotification aNotification)
        {
            DRDevice removedDevice = aNotification.Object.CastTo<DRDevice>();

            DRNotificationCenter.CurrentRunLoopCenter.RemoveObserverNameObject(this, DRDevice.DRDeviceStatusChangedNotification, removedDevice);

            this.deviceList.RemoveObject(removedDevice);
            this.UpdateUI();
        }

        [ObjectiveCMessage("deviceStateChanged:")]
        public void DeviceStateChanged(NSNotification aNotification)
        {
            this.UpdateUI();
        }

        public void UpdateUI()
        {
            StringBuilder textString = new StringBuilder("-------\n");

            foreach (DRDevice device in this.deviceList.GetEnumerator<DRDevice>())
            {
                NSDictionary deviceInfo = device.Info;
                NSString deviceName = device.DisplayName;
                NSString connection = deviceInfo[DRDevice.DRDevicePhysicalInterconnectKey].CastTo<NSString>();

                NSDictionary deviceStatus = device.Status;
                bool deviceIsBusy = deviceStatus[DRDevice.DRDeviceIsBusyKey].CastTo<NSNumber>().BoolValue;
                NSString mediaState = deviceStatus[DRDevice.DRDeviceMediaStateKey].CastTo<NSString>();
                NSString mediaType;

                if (mediaState.IsEqualToString(DRDevice.DRDeviceMediaStateMediaPresent))
                {
                    NSDictionary mediaInfo = deviceStatus[DRDevice.DRDeviceMediaInfoKey].CastTo<NSDictionary>();

                    mediaType = mediaInfo[DRDevice.DRDeviceMediaTypeKey].CastTo<NSString>();
                }
                else if (mediaState.IsEqualToString(DRDevice.DRDeviceMediaStateNone))
                {
                    mediaType = "No Disc";
                }
                else
                {
                    mediaType = "In Transition";
                }

                textString.AppendFormat("Device Name: {0}\rMedia Type:  {1}\rConnection:  {2}\rDevice Busy? {3}\r-------\r", deviceName, mediaType, connection, deviceIsBusy ? "YES" : "NO");
            }

            this.textView.String = textString.ToString();
        }
    }
}