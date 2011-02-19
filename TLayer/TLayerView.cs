using System;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.ApplicationServices;

namespace Monobjc.Samples.TLayer
{
    public static class NSEvent_TLayerViewInternal
    {
        public static NSPoint LocationInView(this NSEvent evnt, NSView view)
        {
            return view.ConvertPointFromView(evnt.LocationInWindow, null);
        }
    }

    [ObjectiveCClass]
    public class TLayerView : NSView
    {
        private const int CIRCLE_COUNT = 3;
        private const float circleRadius = 100;

        private static readonly float[][] colors = new[]
                                                       {
                                                           new[] {0.5f, 0.0f, 0.5f, 1},
                                                           new[] {1.0f, 0.7f, 0.0f, 1},
                                                           new[] {0.0f, 0.5f, 0.0f, 1}
                                                       };

        private static readonly Random r = new Random();

        private NSMutableArray circles;
        private float shadowRadius;
        private CGSize shadowOffset;
        private bool useTLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TLayerView"/> class.
        /// </summary>
        public TLayerView() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="TLayerView"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public TLayerView(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("initWithFrame:")]
        public override Id InitWithFrame(NSRect frame)
        {
            Id self = ObjectiveCRuntime.SendMessageSuper<Id>(this, Class.Get(typeof (TLayerView)), "initWithFrame:", frame);
            if (self == null)
            {
                return null;
            }

            this.useTLayer = false;
            this.circles = new NSMutableArray(CIRCLE_COUNT);

            for (int k = 0; k < CIRCLE_COUNT; k++)
            {
                NSColor color = NSColor.ColorWithCalibratedRedGreenBlueAlpha(colors[k][0], colors[k][1], colors[k][2], colors[k][3]);
                Circle circle = new Circle();
                circle.Color = color;
                circle.Radius = circleRadius;
                circle.Center = MakeRandomPointInRect(this.Bounds);
                this.circles.AddObject(circle);
                circle.Release();
            }

            this.RegisterForDraggedTypes(NSArray.ArrayWithObject(NSPasteboard.NSColorPboardType));
            this.NeedsDisplay = true;

            return self;
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this.circles.Release();
            ObjectiveCRuntime.SendMessageSuper(this, Class.Get(typeof (TLayerView)), "dealloc");
        }

        public float ShadowRadius
        {
            [ObjectiveCMessage("shadowRadius")]
            get { return this.shadowRadius; }
            [ObjectiveCMessage("setShadowRadius:")]
            set
            {
                if (this.shadowRadius != value)
                {
                    this.shadowRadius = value;
                    this.NeedsDisplay = true;
                }
            }
        }

        public CGSize ShadowOffset
        {
            [ObjectiveCMessage("shadowOffset")]
            get { return this.shadowOffset; }
            [ObjectiveCMessage("setShadowOffset:")]
            set
            {
                if (CGSize.CGSizeEqualToSize(this.shadowOffset, value) == 0)
                {
                    this.shadowOffset = value;
                    this.NeedsDisplay = true;
                }
            }
        }

        public bool UsesTransparencyLayers
        {
            [ObjectiveCMessage("usesTransparencyLayers")]
            get { return this.useTLayer; }
            [ObjectiveCMessage("setUsesTransparencyLayers:")]
            set
            {
                if (this.useTLayer != value)
                {
                    this.useTLayer = value;
                    this.NeedsDisplay = true;
                }
            }
        }

        public override bool IsOpaque
        {
            [ObjectiveCMessage("isOpaque")]
            get { return true; }
        }

        [ObjectiveCMessage("acceptsFirstMouse:")]
        public override bool AcceptsFirstMouse(NSEvent evnt)
        {
            return true;
        }

        [ObjectiveCMessage("boundsForCircle:")]
        public NSRect BoundsForCircle(Circle circle)
        {
            float dx = 2*Math.Abs(this.shadowOffset.width) + 2*this.shadowRadius;
            float dy = 2*Math.Abs(this.shadowOffset.height) + 2*this.shadowRadius;
            return NSRect.NSInsetRect(circle.Bounds, -dx, -dy);
        }

        [ObjectiveCMessage("dragCircleAtIndex:withEvent:")]
        public void DragCircleAtIndexWithEvent(uint index, NSEvent evnt)
        {
            Circle circle = this.circles.ObjectAtIndex(index).CastTo<Circle>();
            circle.Retain();
            this.circles.RemoveObjectAtIndex(index);
            this.circles.AddObject(circle);
            circle.Release();

            this.SetNeedsDisplayInRect(this.BoundsForCircle(circle));

            NSEventMask mask = NSEventMask.NSLeftMouseDraggedMask | NSEventMask.NSLeftMouseUpMask;
            NSPoint start = evnt.LocationInView(this);

            while (true)
            {
                evnt = this.Window.NextEventMatchingMask(mask);
                if (evnt.Type == NSEventType.NSLeftMouseUp)
                {
                    break;
                }

                this.SetNeedsDisplayInRect(this.BoundsForCircle(circle));

                NSPoint center = circle.Center;
                NSPoint point = evnt.LocationInView(this);
                center.x += point.x - start.x;
                center.y += point.y - start.y;
                circle.Center = center;

                this.SetNeedsDisplayInRect(this.BoundsForCircle(circle));

                start = point;
            }
        }

        [ObjectiveCMessage("indexOfCircleAtPoint:")]
        public int IndexOfCircleAtPoint(NSPoint point)
        {
            uint count = this.circles.Count;
            for (uint k = 0; k < count; k++)
            {
                Circle circle = this.circles.ObjectAtIndex(count - 1 - k).CastTo<Circle>();
                NSPoint center = circle.Center;
                float radius = circle.Radius;
                float dx = point.x - center.x;
                float dy = point.y - center.y;
                if (dx*dx + dy*dy < radius*radius)
                {
                    return (int) (count - 1 - k);
                }
            }
            return -1;
        }

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent evnt)
        {
            NSPoint point = evnt.LocationInView(this);
            int index = this.IndexOfCircleAtPoint(point);
            if (index >= 0)
            {
                this.DragCircleAtIndexWithEvent((uint) index, evnt);
            }
        }

        [ObjectiveCMessage("setFrame:")]
        public void SetFrame(NSRect frame)
        {
            ObjectiveCRuntime.SendMessageSuper(this, Class.Get(typeof (TLayerView)), "setFrame:", frame);
            this.NeedsDisplay = true;
        }

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect rect)
        {
            IntPtr context = NSGraphicsContext.CurrentContext.GraphicsPort;

            CGContext.SetRGBFillColor(context, 0.7f, 0.7f, 0.9f, 1f);
            CGContext.FillRect(context, ConvertToCGRect(rect));

            CGContext.SetShadow(context, this.shadowOffset, this.shadowRadius);

            if (this.useTLayer)
            {
                CGContext.BeginTransparencyLayer(context, null);
            }

            uint count = this.circles.Count;
            for (uint k = 0; k < count; k++)
            {
                Circle circle = this.circles.ObjectAtIndex(k).CastTo<Circle>();
                NSRect bounds = this.BoundsForCircle(circle);
                if (NSRect.NSIntersectsRect(bounds, rect))
                {
                    circle.Draw();
                }
            }

            if (this.useTLayer)
            {
                CGContext.EndTransparencyLayer(context);
            }
        }

        [ObjectiveCMessage("draggingEntered:")]
        public NSDragOperation DraggingEntered(INSDraggingInfo sender)
        {
            /* Since we have only registered for NSColorPboardType drags, this is
             * actually unneeded. If you were to register for any other drag types,
             * though, this code would be necessary. */
            if ((sender.DraggingSourceOperationMask & NSDragOperation.NSDragOperationGeneric) != 0)
            {
                NSPasteboard pasteboard = sender.DraggingPasteboard;
                if (pasteboard.Types.ContainsObject(NSPasteboard.NSColorPboardType))
                {
                    return NSDragOperation.NSDragOperationGeneric;
                }
            }

            return NSDragOperation.NSDragOperationNone;
        }

        [ObjectiveCMessage("performDragOperation:")]
        public bool PerformDragOperation(INSDraggingInfo sender)
        {
            NSPoint point = this.ConvertPointFromView(sender.DraggingLocation, null);
            int index = this.IndexOfCircleAtPoint(point);

            if (index >= 0)
            {
                /* The current drag location is inside the bounds of a circle so we
            	 * accept the drop and move on to concludeDragOperation:. */
                return true;
            }
            return false;
        }

        [ObjectiveCMessage("concludeDragOperation:")]
        public void ConcludeDragOperation(INSDraggingInfo sender)
        {
            NSColor color = NSColor.ColorFromPasteboard(sender.DraggingPasteboard);
            NSPoint point = this.ConvertPointFromView(sender.DraggingLocation, null);
            int index = this.IndexOfCircleAtPoint(point);

            if (index >= 0)
            {
                Circle circle = this.circles.ObjectAtIndex((uint) index).CastTo<Circle>();
                circle.Color = color;
                this.SetNeedsDisplayInRect(this.BoundsForCircle(circle));
            }
        }

        private static NSPoint MakeRandomPointInRect(NSRect rect)
        {
            NSPoint p;
            p.x = NSRect.NSMinX(rect) + NSRect.NSWidth(rect)*((float) r.NextDouble());
            p.y = NSRect.NSMinY(rect) + NSRect.NSHeight(rect)*((float) r.NextDouble());
            return p;
        }

        private static CGRect ConvertToCGRect(NSRect rect)
        {
            return CGRect.CGRectMake(rect.origin.x, rect.origin.y, rect.size.width, rect.size.height);
        }
    }
}