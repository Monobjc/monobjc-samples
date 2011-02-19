using System;
using Monobjc.ApplicationServices;
using Monobjc.AppKit;
using Monobjc.Quartz;
using Monobjc.QuartzCore;
using Monobjc.Foundation;

namespace Monobjc.Samples.AnimatedCompositionLayer
{
    [ObjectiveCClass]
    public partial class AppController : NSObject
    {
        public AppController() {}
		
        public AppController(IntPtr nativePointer) : base(nativePointer) {}

        [ObjectiveCMessage("applicationDidFinishLaunching:")]
        public void ApplicationDidFinishLaunching(NSNotification notification)
        {
            QCComposition composition;
            AnimatedView view;

            /* Get a composition from the repository */
            composition = QCCompositionRepository.SharedCompositionRepository.CompositionWithIdentifier("/defocus");
            if (composition == null)
            {
                NSApplication.SharedApplication.Terminate(null);
            }

            /* Configure the content view of the window to use a Core Animation layer */
            view = new AnimatedView(NSRect.NSMakeRect(0, 0, 100, 100));
            view.WantsLayer = true;
            view.Layer = QCCompositionLayer.CompositionLayerWithComposition(composition);
            this.window.ContentView = view;
            view.Release();

            /* Show window */
            this.window.MakeKeyAndOrderFront(null);
        }
		
		[ObjectiveCMessage("applicationShouldTerminateAfterLastWindowClosed:")]
		public virtual bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication theApplication)
		{
			return true;
		}
    }

    [ObjectiveCClass]
    public class AnimatedView : NSView
    {
        private static Random random;

        public AnimatedView() {}
        public AnimatedView(IntPtr nativePointer) : base(nativePointer) {}
        public AnimatedView(NSRect frameRect) : base(frameRect) {}

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            /* Animate composition parameters "size" and "primary color" through a Core Animation transaction of 1 second duration */
            CATransaction.Begin();
            CATransaction.SetValueForKey(NSNumber.NumberWithFloat(1.0f), CATransaction.kCATransactionAnimationDuration);
            this.Layer.SetValueForKeyPath(NSNumber.NumberWithFloat(this.rand()), "patch.size.value");
            Id rgb = new Id();
            rgb.NativePointer = CGColor.CreateGenericRGB(this.rand(), this.rand(), this.rand(), 1.0f);
            this.Layer.SetValueForKeyPath(rgb, NSString.StringWithFormat("patch.%@.value", QCComposition.QCCompositionInputPrimaryColorKey));
            CATransaction.Commit();
        }

        private float rand()
        {
            if (random == null)
            {
                random = new Random();
            }
            return Convert.ToSingle(random.NextDouble());
        }
    }
}