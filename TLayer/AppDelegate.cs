using System;
using Monobjc.Foundation;
using Monobjc.AppKit;

namespace Monobjc.Samples.TLayer
{
    [ObjectiveCClass]
    public class AppDelegate : NSObject
    {
        private static readonly Class AppDelegateClass = Class.Get(typeof (AppDelegate));

        private TLayerDemo shadowDemo;

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

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this.shadowDemo.SafeRelease();
            this.SendMessageSuper(AppDelegateClass, "dealloc");
        }

        [ObjectiveCMessage("applicationDidFinishLaunching:")]
        public void ApplicationDidFinishLaunching(NSNotification notification)
        {
            this.ShowTLayerDemoWindow(this);
        }

        [ObjectiveCMessage("showTLayerDemoWindow:")]
        public void ShowTLayerDemoWindow(Id sender)
        {
            if (this.shadowDemo == null)
            {
                this.shadowDemo = new TLayerDemo();
            }
            this.shadowDemo.Window.OrderFront(this);
        }

        [ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
        public bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication application)
        {
            return false;
        }
    }
}