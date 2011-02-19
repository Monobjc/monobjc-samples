using System;
using Monobjc.Growl;
using Monobjc.Foundation;
using Monobjc.AppKit;

namespace Monobjc.Samples.GrowlSampleApp
{
    [ObjectiveCClass]
    public partial class GrowlSampleAppDelegate : NSObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowlSampleAppDelegate"/> class.
        /// </summary>
        public GrowlSampleAppDelegate() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="GrowlSampleAppDelegate"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public GrowlSampleAppDelegate(IntPtr nativePointer) : base(nativePointer) {}

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // Register ourselves as a Growl delegate
            GrowlApplicationBridge.GrowlDelegate = this;

            GrowlApplicationBridge.NotifyWithTitleDescriptionNotificationNameIconDataPriorityIsStickyClickContext(@"Hello!",
                                                                                                                  @"Sample App Started",
                                                                                                                  @"Example",
                                                                                                                  null,
                                                                                                                  0,
                                                                                                                  false,
                                                                                                                  NSDate.Date);
		}
        
        [ObjectiveCMessage("notify:")]
        public void Notify()
        {
            GrowlApplicationBridge.NotifyWithTitleDescriptionNotificationNameIconDataPriorityIsStickyClickContext(@"Hello Again!",
                                                                                                                  @"You clicked the button",
                                                                                                                  @"Example",
                                                                                                                  null,
                                                                                                                  0,
                                                                                                                  false,
                                                                                                                  NSDate.Date);
        }
        
        [ObjectiveCMessage("growlNotificationWasClicked:")]
        public void GrowlNotificationWasClicked(Id context)
        {
			// Use a standard alert to display the message
            AppKitFramework.NSRunAlertPanel("Notification Clicked", "Hello, Growl!", "OK", null, null);
        }
    }
}
