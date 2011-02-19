using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.CroppedImage
{
    public class IPhotoStyleCropMarker : CropMarker
    {
        public IPhotoStyleCropMarker(NSView view)
            : base(view) {}

        public override void drawCropMarker()
        {
            if (!NSRect.NSIsEmptyRect(this.SelectedRect))
            {
                NSBezierPath cutout = NSBezierPath.BezierPathWithRect(NSRect.NSInsetRect(this.target.Bounds, 5.0f, 5.0f));
                cutout.AppendBezierPathWithRect(this.selectedRect);
                cutout.WindingRule = NSWindingRule.NSEvenOddWindingRule;
                this.fillColor.Set();
                cutout.Fill();

                this.strokeColor.Set();
                AppKitFramework.NSFrameRect(this.SelectedRect);
            }
        }
    }
}