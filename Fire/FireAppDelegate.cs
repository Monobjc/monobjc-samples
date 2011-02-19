using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.Fire
{
    [ObjectiveCClass]
    public class FireAppDelegate : NSObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FireAppDelegate"/> class.
        /// </summary>
        public FireAppDelegate() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="FireAppDelegate"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public FireAppDelegate(IntPtr nativePointer) : base(nativePointer) {}

        [ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
        public bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication application)
        {
            return true;
        }

        [ObjectiveCMessage("applicationShouldTerminate:")]
        public NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication application)
        {
            return NSApplicationTerminateReply.NSTerminateNow;
        }
    }
}