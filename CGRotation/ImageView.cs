using System;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.ApplicationServices;

namespace Monobjc.Samples.CGRotation
{
    [ObjectiveCClass]
    public class ImageView : NSView
    {
        private static readonly Class ImageViewClass = Class.Get(typeof (ImageView));
        private ImageInfo image;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageView"/> class.
        /// </summary>
        public ImageView() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageView"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public ImageView(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("initWithFrame:")]
        public override Id InitWithFrame(NSRect frame)
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(ImageViewClass, "initWithFrame:", frame);

            return this;
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            ImageUtils.IIRelease(ref this.image);

            this.SendMessageSuper(ImageViewClass, "dealloc");
        }

        public ImageInfo Image
        {
            get { return this.image; }
            set
            {
                if (!Equals(value, this.image))
                {
                    ImageUtils.IIRelease(ref this.image);
                    this.image = value;
                    this.NeedsDisplay = true;
                }
            }
        }

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect rect)
        {
            // Obtain the current context
            IntPtr ctx = NSGraphicsContext.CurrentContext.GraphicsPort;

            // Draw the image in the context
            ImageUtils.IIDrawImageTransformed(ref this.image, ctx, CGRect.CGRectMake(rect.origin.x, rect.origin.y, rect.size.width, rect.size.height));

            // Draw the view border, just a simple stroked rectangle
            CGContext.AddRect(ctx, CGRect.CGRectMake(rect.origin.x, rect.origin.y, rect.size.width, rect.size.height));
            CGContext.SetRGBStrokeColor(ctx, 1.0f, 0.0f, 0.0f, 1.0f);
            CGContext.StrokePath(ctx);
        }

        public void SetRotation(float value)
        {
            this.image.fRotation = value;
        }

        public void SetScaleX(float value)
        {
            this.image.fScaleX = value;
        }

        public void SetScaleY(float value)
        {
            this.image.fScaleY = value;
        }

        public void SetTranslateX(float value)
        {
            this.image.fTranslateX = value;
        }

        public void SetTranslateY(float value)
        {
            this.image.fTranslateY = value;
        }
    }
}