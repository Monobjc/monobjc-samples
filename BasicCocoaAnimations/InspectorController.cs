using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.BasicCocoaAnimations
{
    [ObjectiveCClass]
    public class InspectorController : NSWindowController
    {
        private static readonly Class InspectorControllerClass = Class.Get(typeof (InspectorController));

        private NSTimer fadeTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorController"/> class.
        /// </summary>
        public InspectorController() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorController"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public InspectorController(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("init")]
        public override Id Init()
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(InspectorControllerClass, "initWithWindowNibName:", (NSString) "Inspector");
            return this;
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this.fadeTimer.SafeRelease();
            this.SendMessageSuper(InspectorControllerClass, "dealloc");
        }

        /// <summary>
        /// When the window loads add the tracking area to catch mouse entered and mouse exited events
        /// </summary>
        [ObjectiveCMessage("windowDidLoad")]
        public override void WindowDidLoad()
        {
            NSView containingView = this.Window.ContentView.Superview;

            NSTrackingArea area = new NSTrackingArea(containingView.Frame,
                                                     NSTrackingAreaOptions.NSTrackingMouseEnteredAndExited |
                                                     NSTrackingAreaOptions.NSTrackingActiveInActiveApp |
                                                     NSTrackingAreaOptions.NSTrackingInVisibleRect, this, null);
            containingView.AddTrackingArea(area);
            area.Release();

            NSTimer timer = NSTimer.ScheduledTimerWithTimeIntervalTargetSelectorUserInfoRepeats(4.0f, this, ObjectiveCRuntime.Selector("startFade:"), null, false);
            this.SetFadeTimer(timer);
        }

        /// <summary>
        /// Mouse entered.  Turn off the fade timer, and animate to full opacity
        /// </summary>
        [ObjectiveCMessage("mouseEntered:")]
        public override void MouseEntered(NSEvent theEvent)
        {
            this.SetFadeTimer(null);
            this.Window.Animator.AlphaValue = 1.0f;
        }

        /// <summary>
        /// Mouse exited.  Set a timer to give a few seconds before we start fading.
        /// (Note that this effect could also be done with a custom CAKeyframeAnimation)
        /// </summary>
        [ObjectiveCMessage("mouseExited:")]
        public override void MouseExited(NSEvent theEvent)
        {
            NSTimer timer = NSTimer.ScheduledTimerWithTimeIntervalTargetSelectorUserInfoRepeats(2.0f, this, ObjectiveCRuntime.Selector("startFade:"), null, false);
            this.SetFadeTimer(timer);
        }

        /// <summary>
        /// After a delay start the fade.
        /// </summary>
        [ObjectiveCMessage("setFadeTimer:")]
        public void SetFadeTimer(NSTimer timer)
        {
            if (this.fadeTimer != timer)
            {
                if (this.fadeTimer != null)
                {
                    this.fadeTimer.Invalidate();
                    this.fadeTimer.SafeRelease();
                }
                this.fadeTimer = timer.SafeRetain();
            }
        }

        [ObjectiveCMessage("startFade:")]
        public void StartFade(NSTimer timer)
        {
            NSAnimationContext.BeginGrouping();
            NSAnimationContext.CurrentContext.Duration = 10.0f;
            this.Window.Animator.AlphaValue = 0.3f;
            NSAnimationContext.EndGrouping();
            this.SetFadeTimer(null);
        }
    }
}