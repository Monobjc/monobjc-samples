using System;
using Monobjc.Foundation;
using Monobjc.AppKit;
using Monobjc.ApplicationServices;

namespace Monobjc.Samples.TLayer
{
    [ObjectiveCClass]
    public partial class TLayerDemo : NSObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TLayerDemo"/> class.
        /// </summary>
        public TLayerDemo() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="TLayerDemo"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public TLayerDemo(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("initialize")]
        public new static void Initialize()
        {
            NSColorPanel.SharedColorPanel.ShowsAlpha = true;
        }

        [ObjectiveCMessage("init")]
        public override Id Init()
        {
            Id self = ObjectiveCRuntime.SendMessageSuper<Id>(this, Class.Get(typeof (TLayerDemo)), "init");
            if (self == null)
            {
                return null;
            }

            if (!NSBundle.LoadNibNamedOwner("TLayerDemo", this))
            {
                Console.WriteLine("Failed to load TLayerDemo.nib");
                this.Release();
                return null;
            }

            this.shadowOffsetView.Scale = 40;
            this.shadowOffsetView.Offset = CGSize.CGSizeMake(-30, -30);
            this.tlayerView.ShadowOffset = CGSize.CGSizeMake(-30, -30);

            this.ShadowRadiusChanged(this.shadowRadiusSlider);

            /* Better to do this as a subclass of NSControl.... */
            NSNotificationCenter.DefaultCenter.AddObserverSelectorNameObject(this,
                                                                             ObjectiveCRuntime.Selector("shadowOffsetChanged:"),
                                                                             ShadowOffsetView.ShadowOffsetChanged,
                                                                             null);

            return self;
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            // Needed in order to have instance field filled.
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);
            ObjectiveCRuntime.SendMessageSuper(this, Class.Get(typeof (AppDelegate)), "dealloc");
        }

        public NSWindow Window
        {
            [ObjectiveCMessage("window")]
            get { return this.tlayerView.Window; }
        }

        partial void ShadowRadiusChanged(Id sender)
        {
            this.tlayerView.ShadowRadius = this.shadowRadiusSlider.FloatValue;
        }

        partial void ToggleTransparencyLayers(Id sender)
        {
            this.tlayerView.UsesTransparencyLayers = (this.transparencyLayerButton.State == NSCellStateValue.NSOnState);
        }

        [ObjectiveCMessage("shadowOffsetChanged:")]
        public void ShadowOffsetChanged(NSNotification notification)
        {
            CGSize offset = notification.Object.SendMessage<CGSize>("offset");
            this.tlayerView.ShadowOffset = offset;
        }
    }
}