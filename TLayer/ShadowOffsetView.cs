using System;
using Monobjc.AppKit;
using Monobjc.ApplicationServices;
using Monobjc.Foundation;

namespace Monobjc.Samples.TLayer
{
    [ObjectiveCClass]
    public class ShadowOffsetView : NSView
    {
        private static readonly Class ShadowOffsetViewClass = Class.Get(typeof (ShadowOffsetView));
        public static NSString ShadowOffsetChanged = NSString.NSPinnedString("ShadowOffsetChanged");

        private CGSize _offset;
        private float _scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShadowOffsetView"/> class.
        /// </summary>
        public ShadowOffsetView() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ShadowOffsetView"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public ShadowOffsetView(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("initWithFrame:")]
        public override Id InitWithFrame(NSRect frame)
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(ShadowOffsetViewClass, "initWithFrame:", frame);

            this._offset = CGSize.CGSizeZero;

            return this;
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this.SendMessageSuper(ShadowOffsetViewClass, "dealloc");
        }

        public float Scale
        {
            [ObjectiveCMessage("scale")]
            get { return this._scale; }
            [ObjectiveCMessage("setScale:")]
            set { this._scale = value; }
        }

        public CGSize Offset
        {
            [ObjectiveCMessage("offset")]
            get { return CGSize.CGSizeMake(this._offset.width*this._scale, this._offset.height*this._scale); }
            [ObjectiveCMessage("setOffset:")]
            set
            {
                CGSize offset = CGSize.CGSizeMake(value.width/this._scale, value.height/this._scale);
                if (CGSize.CGSizeEqualToSize(this._offset, offset) == 0)
                {
                    this._offset = offset;
                    this.NeedsDisplay = true;
                }
            }
        }

        public override bool IsOpaque
        {
            [ObjectiveCMessage("isOpaque")]
            get { return false; }
        }

        [ObjectiveCMessage("setOffsetFromPoint:")]
        public void SetOffsetFromPoint(NSPoint point)
        {
            CGSize offset;

            NSRect bounds = this.Bounds;
            offset.width = (point.x - NSRect.NSMidX(bounds))/(NSRect.NSWidth(bounds)/2);
            offset.height = (point.y - NSRect.NSMidY(bounds))/(NSRect.NSHeight(bounds)/2);
            float radius = (float) Math.Sqrt(offset.width*offset.width + offset.height*offset.height);
            if (radius > 1)
            {
                offset.width /= radius;
                offset.height /= radius;
            }
            if (CGSize.CGSizeEqualToSize(this._offset, offset) == 0)
            {
                this._offset = offset;
                this.NeedsDisplay = true;
                NSNotificationCenter.DefaultCenter.PostNotificationNameObject(ShadowOffsetChanged, this);
            }
        }

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent evnt)
        {
            NSPoint point = this.ConvertPointFromView(evnt.LocationInWindow, null);
            this.SetOffsetFromPoint(point);
        }

        [ObjectiveCMessage("mouseDragged:")]
        public override void MouseDragged(NSEvent evnt)
        {
            NSPoint point = this.ConvertPointFromView(evnt.LocationInWindow, null);
            this.SetOffsetFromPoint(point);
        }

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect rect)
        {
            NSRect bounds = this.Bounds;
            float x = NSRect.NSMinX(bounds);
            float y = NSRect.NSMinY(bounds);
            float w = NSRect.NSWidth(bounds);
            float h = NSRect.NSHeight(bounds);
            float r = Math.Min(w/2, h/2);

            IntPtr context = NSGraphicsContext.CurrentContext.GraphicsPort;

            CGContext.TranslateCTM(context, x + w/2, y + h/2);

            CGContext.AddArc(context, 0, 0, r, 0, (float) (2*Math.PI), 1);
            CGContext.Clip(context);

            CGContext.SetGrayFillColor(context, 0.910f, 1);
            CGContext.FillRect(context, CGRect.CGRectMake(-w/2, -h/2, w, h));

            CGContext.AddArc(context, 0, 0, r, 0, (float) (2*Math.PI), 1);
            CGContext.SetGrayStrokeColor(context, 0.616f, 1);
            CGContext.StrokePath(context);

            CGContext.AddArc(context, 0, -2, r, 0, (float) (2*Math.PI), 1);
            CGContext.SetGrayStrokeColor(context, 0.784f, 1);
            CGContext.StrokePath(context);

            CGContext.MoveToPoint(context, 0, 0);
            CGContext.AddLineToPoint(context, r*this._offset.width, r*this._offset.height);

            CGContext.SetLineWidth(context, 2);
            CGContext.SetGrayStrokeColor(context, 0.33f, 1.0f);
            CGContext.StrokePath(context);
        }
    }
}