using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.BasicCocoaAnimations
{
    [ObjectiveCClass]
    public partial class MainWindowController : NSWindowController
    {
        private static readonly Class MainWindowControllerClass = Class.Get(typeof (MainWindowController));

        private int currentViewTag;
        private bool didMoveView;
        private bool didMoveAllViews;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowController"/> class.
        /// </summary>
        public MainWindowController() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowController"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public MainWindowController(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("init")]
        public override Id Init()
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(MainWindowControllerClass, "initWithWindowNibName:", (NSString) "MainWindow");
            return this;
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this.SendMessageSuper(MainWindowControllerClass, "dealloc");
        }

        [ObjectiveCMessage("validateToolbarItem:")]
        public bool ValidateToolbarItem(NSToolbarItem item)
        {
            return item.Tag != this.currentViewTag;
        }

        /// <summary>
        /// We need to be layer-backed to have subview transitions.
        /// </summary>
        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            this.Window.SetContentSize(this.smallView.Frame.size);
            this.Window.ContentView.AddSubview(this.smallView);
            this.Window.ContentView.WantsLayer = true;
        }

        partial void SwitchView(Id sender)
        {
            // Figure out the new view, the old view, and the new size of the window
            int tag = sender.SendMessage<int>("tag");
            NSView view = this.ViewForTag(tag);
            NSView previousView = this.ViewForTag(this.currentViewTag);
            this.currentViewTag = tag;

            NSRect newFrame = this.NewFrameForNewContentView(view);

            // Using an animation grouping because we may be changing the duration
            NSAnimationContext.BeginGrouping();

            // With the shift key down, do slow-mo animation
            if ((NSApplication.NSApp.CurrentEvent.ModifierFlags & NSModifierFlag.NSShiftKeyMask) == NSModifierFlag.NSShiftKeyMask)
            {
                NSAnimationContext.CurrentContext.Duration = 1.0f;
            }

            // Call the animator instead of the view / window directly
            this.Window.ContentView.Animator.ReplaceSubviewWith(previousView, view);
            this.Window.Animator.SetFrameDisplay(newFrame, true);

            NSAnimationContext.EndGrouping();
        }

        /// <summary>
        /// In this case, just moving an image view back and forth.
        /// Note the use of the animator.
        /// </summary>
        partial void MoveView(Id sender)
        {
            NSPoint newPoint = this.didMoveView ? new NSPoint(17.0f, 87.0f) : new NSPoint(339.0f, 87.0f);
            this.imageView.Animator.SetFrameOrigin(newPoint);
            this.didMoveView = !this.didMoveView;
        }

        /// <summary>
        /// Moving all subviews except the button that triggers the action.
        /// Note the use of the animator, and that there is an implied transaction
        /// All of the animations start simultaneously.
        /// </summary>
        partial void MoveAllViews(Id sender)
        {
            float deltaY = this.didMoveAllViews ? 222.0f : -222.0f;

            foreach (NSView subview in this.mediumView.Subviews.GetEnumerator<NSView>())
            {
                if (subview.Tag != -12)
                {
                    NSRect frame = subview.Frame;
                    frame.origin.y += deltaY;
                    subview.Animator.Frame = frame;
                }
            }
            this.didMoveAllViews = !this.didMoveAllViews;
        }

        /// <summary>
        /// NOTE: One key to having the contained view resize correctly is to have its autoresizing set correctly in IB.
        /// Based on the new content view frame, calculate the window's new frame
        /// </summary>
        public NSRect NewFrameForNewContentView(NSView view)
        {
            NSWindow window = this.Window;
            NSRect newFrameRect = window.FrameRectForContentRect(view.Frame);
            NSRect oldFrameRect = window.Frame;
            NSSize newSize = newFrameRect.size;
            NSSize oldSize = oldFrameRect.size;

            NSRect frame = window.Frame;
            frame.size = newSize;
            frame.origin.y -= (newSize.height - oldSize.height);

            return frame;
        }

        public NSView ViewForTag(int tag)
        {
            NSView view;
            switch (tag)
            {
                case 0:
                    view = this.smallView;
                    break;
                case 1:
                    view = this.mediumView;
                    break;
                case 2:
                default:
                    view = this.largeView;
                    break;
            }
            return view;
        }
    }
}