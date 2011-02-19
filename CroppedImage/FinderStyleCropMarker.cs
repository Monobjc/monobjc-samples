using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.CroppedImage
{
    public class FinderStyleCropMarker : CropMarker
    {
        public FinderStyleCropMarker(NSView view)
            : base(view) {}

        public override void drawCropMarker()
        {
            if (!NSRect.NSIsEmptyRect(this.SelectedRect))
            {
                this.fillColor.Set();
                AppKitFramework.NSRectFillUsingOperation(this.SelectedRect, NSCompositingOperation.NSCompositeSourceOver);
                this.strokeColor.Set();
                AppKitFramework.NSFrameRect(this.SelectedRect);
            }
        }
    }
}