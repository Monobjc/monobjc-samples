using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.CroppedImage
{
    public class CropMarker : IDisposable
    {
        protected NSView target;
        protected bool selecting, dragging, resizing;
        protected NSRect selectedRect;
        protected NSPoint lastLocation;
        protected NSColor fillColor, strokeColor;
        protected SelectionTrackingMode trackingMode;
        protected NSBezierPath selectedPath;

        public CropMarker(NSView view)
        {
            this.target = view;
            this.setColor(NSColor.BlueColor);
            this.selectedPath = NSBezierPath.BezierPath;
            this.selectedPath.Retain();
            this.selectedRect = NSRect.NSZeroRect;
        }

        public void setColor(NSColor aColor)
        {
            this.setStrokeColor(aColor);
            this.setFillColor(this.strokeColor.ColorWithAlphaComponent(0.2f));
        }

        public virtual void drawCropMarker()
        {
            this.strokeColor.Set();
            AppKitFramework.NSFrameRect(this.selectedRect);
        }

        public virtual void startMovingAtPoint(NSPoint where)
        {
            this.trackingMode = SelectionTrackingMode.TrackMoving;
            this.lastLocation = where;
        }

        public virtual void startSelectingAtPoint(NSPoint where)
        {
            this.trackingMode = SelectionTrackingMode.TrackSelecting;
            this.lastLocation = where;
        }

        public virtual void continueMovingAtPoint(NSPoint where)
        {
            this.selectedRect.origin.x += where.x - this.lastLocation.x;
            this.selectedRect.origin.y += where.y - this.lastLocation.y;
            this.lastLocation = where;
        }

        public virtual void stopMovingAtPoint(NSPoint where)
        {
            this.continueMovingAtPoint(where);
            this.trackingMode = SelectionTrackingMode.TrackNone;
        }

        public virtual void continueSelectingAtPoint(NSPoint where)
        {
            this.selectedRect = RectFromPoints(this.lastLocation, where);
        }

        public virtual void stopSelectingAtPoint(NSPoint where)
        {
            this.selectedRect = RectFromPoints(this.lastLocation, where);
            this.trackingMode = SelectionTrackingMode.TrackNone;
        }

        public virtual void mouseDown(NSEvent theEvent)
        {
            this.lastLocation = this.target.ConvertPointFromView(theEvent.LocationInWindow, null);
            if (NSRect.NSPointInRect(this.lastLocation, this.selectedRect))
            {
                this.startMovingAtPoint(this.lastLocation);
                return;
            }
            this.startSelectingAtPoint(this.lastLocation);
        }

        public virtual void mouseUp(NSEvent theEvent)
        {
            switch (this.trackingMode)
            {
                case SelectionTrackingMode.TrackSelecting:
                    this.stopSelectingAtPoint(this.target.ConvertPointFromView(theEvent.LocationInWindow, null));
                    break;

                case SelectionTrackingMode.TrackMoving:
                    this.stopMovingAtPoint(this.target.ConvertPointFromView(theEvent.LocationInWindow, null));
                    break;

                default:
                    Console.WriteLine("Bad tracking mode in [CropMarker mouseUp]");
                    break;
            }
        }

        public virtual void mouseDragged(NSEvent theEvent)
        {
            switch (this.trackingMode)
            {
                case SelectionTrackingMode.TrackSelecting:
                    this.continueSelectingAtPoint(this.target.ConvertPointFromView(theEvent.LocationInWindow, null));
                    break;

                case SelectionTrackingMode.TrackMoving:
                    this.continueMovingAtPoint(this.target.ConvertPointFromView(theEvent.LocationInWindow, null));
                    break;

                default:
                    Console.WriteLine("Bad tracking mode in [CropMarker mouseDragged]");
                    break;
            }
        }

        public void setFillColor(NSColor color)
        {
            this.fillColor.SafeRelease();
            this.fillColor = color;
            this.fillColor.SafeRetain();
        }

        public void setStrokeColor(NSColor color)
        {
            this.strokeColor.SafeRelease();
            this.strokeColor = color;
            this.strokeColor.SafeRetain();
        }

        public virtual NSBezierPath SelectedPath
        {
            get { return NSBezierPath.BezierPathWithRect(this.selectedRect); }
        }

        public virtual NSRect SelectedRect
        {
            get { return this.selectedRect; }
            set { this.selectedRect = value; }
        }

        public void setSelectedRectOrigin(NSPoint where)
        {
            this.selectedRect.origin = where;
        }

        public void setSelectedRectSize(NSSize size)
        {
            this.selectedRect.size = size;
        }

        public void moveSelectedRectBy(NSSize delta)
        {
            this.selectedRect.origin.x += delta.width;
            this.selectedRect.origin.y += delta.height;
        }

        public virtual void Dispose()
        {
            this.fillColor.SafeRelease();
            this.strokeColor.SafeRelease();
        }

        public static NSRect RectFromPoints(NSPoint p1, NSPoint p2) // Given two corners, make an NSRect.
        {
            return NSRect.NSMakeRect(Math.Min(p1.x, p2.x),
                                     Math.Min(p1.y, p2.y),
                                     Math.Abs(p1.x - p2.x),
                                     Math.Abs(p1.y - p2.y));
        }
    }
}