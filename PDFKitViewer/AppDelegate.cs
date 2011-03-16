using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.PDFKitViewer
{
    [ObjectiveCClass]
    public partial class AppDelegate : NSObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDelegate"/> class.
        /// </summary>
        public AppDelegate() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDelegate"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public AppDelegate(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("applicationShouldOpenUntitledFile:")]
        public bool ApplicationShouldOpenUntitledFile(NSApplication application)
        {
            return false;
        }
    }
}