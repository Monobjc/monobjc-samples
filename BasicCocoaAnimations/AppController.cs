using System;
using Monobjc.Foundation;

namespace Monobjc.Samples.BasicCocoaAnimations
{
    [ObjectiveCClass]
    public partial class AppController : NSObject
    {
        private static readonly Class AppControllerClass = Class.Get(typeof (AppController));

        private MainWindowController mainWindowController;
        private InspectorController inspectorController;

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

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            this.ShowMainWindow(null);
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this.mainWindowController.SafeRelease();
            this.inspectorController.SafeRelease();
            this.SendMessageSuper(AppControllerClass, "dealloc");
        }

        partial void ShowMainWindow(Id sender)
        {
            if (this.mainWindowController == null)
            {
                this.mainWindowController = new MainWindowController();
            }
            this.mainWindowController.ShowWindow(null);
        }

        partial void ShowInspector(Id sender)
        {
            if (this.inspectorController == null)
            {
                this.inspectorController = new InspectorController();
            }
            this.inspectorController.ShowWindow(null);
        }
    }
}