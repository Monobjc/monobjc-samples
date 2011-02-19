using System;
using Monobjc.Foundation;
using Monobjc.AppKit;
using Monobjc.ApplicationServices;

namespace Monobjc.Samples.TLayer
{
    [ObjectiveCClass]
    public class Circle : NSObject
    {
        private static readonly Class CircleClass = Class.Get(typeof (Circle));

        private float _radius;
        private NSPoint _center;
        private NSColor _color;

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> class.
        /// </summary>
        public Circle() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public Circle(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this._color.SafeRelease();
            this.SendMessageSuper(CircleClass, "dealloc");
        }

        public float Radius
        {
            [ObjectiveCMessage("radius")]
            get { return this._radius; }
            [ObjectiveCMessage("setRadius:")]
            set { this._radius = value; }
        }

        public NSPoint Center
        {
            [ObjectiveCMessage("center")]
            get { return this._center; }
            [ObjectiveCMessage("setCenter:")]
            set { this._center = value; }
        }

        public NSColor Color
        {
            [ObjectiveCMessage("color")]
            get { return this._color; }
            [ObjectiveCMessage("setColor:")]
            set
            {
                if (this._color != value)
                {
                    this._color.SafeRelease();
                    this._color = value;
                    this._color.SafeRetain();
                }
            }
        }

        public NSRect Bounds
        {
            [ObjectiveCMessage("bounds")]
            get { return NSRect.NSMakeRect(this._center.x - this._radius, this._center.y - this._radius, 2*this._radius, 2*this._radius); }
        }

        [ObjectiveCMessage("draw")]
        public void Draw()
        {
            IntPtr context = NSGraphicsContext.CurrentContext.GraphicsPort;

            this.Color.Set();

            CGContext.SetGrayStrokeColor(context, 0, 1);
            CGContext.SetLineWidth(context, 1.5f);

            CGContext.SaveGState(context);

            CGContext.TranslateCTM(context, this._center.x, this._center.y);
            CGContext.ScaleCTM(context, this._radius, this._radius);
            CGContext.MoveToPoint(context, 1, 0);
            CGContext.AddArc(context, 0, 0, 1, 0, (float) (2*Math.PI), 0);
            CGContext.ClosePath(context);

            CGContext.RestoreGState(context);
            CGContext.DrawPath(context, CGPathDrawingMode.kCGPathFill);
        }
    }
}