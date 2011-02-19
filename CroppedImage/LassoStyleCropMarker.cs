using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.CroppedImage
{
    public class LassoStyleCropMarker : CropMarker
    {
        public LassoStyleCropMarker(NSView view)
            : base(view) {}

        public override void drawCropMarker()
        {
            if (this.SelectedPath != null)
            {
                NSBezierPath path = (this.trackingMode == SelectionTrackingMode.TrackSelecting) ? NSBezierPathExtensions.ClosedPath(this.SelectedPath) : this.SelectedPath;

                this.fillColor.Set();
                path.Fill();
                this.strokeColor.Set();
                path.Stroke();
            }
        }

        public override void Dispose()
        {
            this.SelectedPath.SafeRelease();
            base.Dispose();
        }

        public override void mouseDown(NSEvent theEvent)
        {
            this.lastLocation = this.target.ConvertPointFromView(theEvent.LocationInWindow, null);
            if (NSBezierPathExtensions.ClosedPath(this.SelectedPath).ContainsPoint(this.lastLocation))
            {
                this.startMovingAtPoint(this.lastLocation);
                return;
            }
            this.startSelectingAtPoint(this.lastLocation);
        }

        public override NSRect SelectedRect
        {
            get { return (this.selectedPath.IsEmpty ? NSRect.NSZeroRect : this.SelectedPath.Bounds); }
            set { base.SelectedRect = value; }
        }

        public override void startSelectingAtPoint(NSPoint where)
        {
            this.lastLocation = where;
            this.trackingMode = SelectionTrackingMode.TrackSelecting;

            this.SelectedPath.RemoveAllPoints();
            this.SelectedPath.MoveToPoint(where);
        }

        public override NSBezierPath SelectedPath
        {
            get { return this.selectedPath; }
        }

        public override void continueMovingAtPoint(NSPoint where)
        {
            NSAffineTransform transform = NSAffineTransform.Transform;
            transform.TranslateXByYBy(where.x - this.lastLocation.x, where.y - this.lastLocation.y);
            this.SelectedPath.TransformUsingAffineTransform(transform);
            this.lastLocation = where;
        }

        public override void continueSelectingAtPoint(NSPoint where)
        {
            this.SelectedPath.LineToPoint(where);
        }

        public override void stopSelectingAtPoint(NSPoint where)
        {
            this.SelectedPath.LineToPoint(where);
            this.SelectedPath.ClosePath();
            this.trackingMode = SelectionTrackingMode.TrackNone;
        }
    }
}