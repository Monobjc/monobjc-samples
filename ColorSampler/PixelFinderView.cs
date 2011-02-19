using System;
using System.Globalization;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.ColorSampler
{
    [ObjectiveCClass]
    public class PixelFinderView : NSImageView
    {
		[ObjectiveCIVar]
		public virtual NSColorWell colorWell {
			get {
				return this.GetInstanceVariable <NSColorWell>("colorWell");
			}
			set {
				this.SetInstanceVariable <NSColorWell>("colorWell", value);
			}
		}

		[ObjectiveCIVar]
		public virtual NSColorWell redWell {
			get {
				return this.GetInstanceVariable <NSColorWell>("redWell");
			}
			set {
				this.SetInstanceVariable <NSColorWell>("redWell", value);
			}
		}

		[ObjectiveCIVar]
		public virtual NSColorWell greenWell {
			get {
				return this.GetInstanceVariable <NSColorWell>("greenWell");
			}
			set {
				this.SetInstanceVariable <NSColorWell>("greenWell", value);
			}
		}

		[ObjectiveCIVar]
		public virtual NSColorWell blueWell {
			get {
				return this.GetInstanceVariable <NSColorWell>("blueWell");
			}
			set {
				this.SetInstanceVariable <NSColorWell>("blueWell", value);
			}
		}

		[ObjectiveCIVar]
		public virtual NSImageView magnifiedImageView {
			get {
				return this.GetInstanceVariable <NSImageView>("magnifiedImageView");
			}
			set {
				this.SetInstanceVariable <NSImageView>("magnifiedImageView", value);
			}
		}

		[ObjectiveCIVar]
		public virtual NSTextField reportText {
			get {
				return this.GetInstanceVariable <NSTextField>("reportText");
			}
			set {
				this.SetInstanceVariable <NSTextField>("reportText", value);
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelFinderView"/> class.
        /// </summary>
        public PixelFinderView() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelFinderView"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public PixelFinderView(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("mouseDown:")]
        public override void MouseDown(NSEvent theEvent)
        {
            this.MouseDragged(theEvent);
        }

        [ObjectiveCMessage("mouseDragged:")]
        public override void MouseDragged(NSEvent theEvent)
        {
            NSPoint where = this.ConvertPointFromView(theEvent.LocationInWindow, null);

            if (!NSRect.NSContainsRect(this.Bounds, NSRect.NSMakeRect(where.x, where.y, 1.0f, 1.0f)))
            {
                return;
            }

            NSColor pixelColor;

            float red, green, blue;

            this.LockFocus(); // NSReadPixel pulls data out of the current focused graphics context, so -lockFocus is necessary here.
            pixelColor = AppKitFramework.NSReadPixel(where);
            this.UnlockFocus(); // always balance -lockFocus with an -unlockFocus.

            red = pixelColor.RedComponent;
            green = pixelColor.GreenComponent;
            blue = pixelColor.BlueComponent;

            this.colorWell.Color = pixelColor;

            this.redWell.Color = NSColor.ColorWithCalibratedRedGreenBlueAlpha(red, 0, 0, 1);
            this.greenWell.Color = NSColor.ColorWithCalibratedRedGreenBlueAlpha(0, green, 0, 1);
            this.blueWell.Color = NSColor.ColorWithCalibratedRedGreenBlueAlpha(0, 0, blue, 1);

            this.magnifiedImageView.Image = this.Snapshot(NSRect.NSMakeRect(where.x - 5.0f, where.y - 5.0f, 10f, 10f));
            this.reportText.StringValue = String.Format(CultureInfo.CurrentCulture, "At: ({0},{1}) R={2:0.00} G={3:0.00} B={4:0.00}", where.x, where.y, red, green, blue);
        }

        [ObjectiveCMessage("mouseUp:")]
        public override void MouseUp(NSEvent theEvent)
        {
            this.MouseDragged(theEvent);
        }

        [ObjectiveCMessage("snapshot")]
        public NSImage Snapshot()
        {
            return this.Snapshot(this.Bounds);
        }

        [ObjectiveCMessage("snapshot:")]
        public NSImage Snapshot(NSRect sourceRect)
        {
            NSImage snapshot = new NSImage(sourceRect.size);

            this.LockFocus();
            NSBitmapImageRep rep = new NSBitmapImageRep(sourceRect).SafeAutorelease();
            this.UnlockFocus();

            snapshot.AddRepresentation(rep);
            snapshot.Autorelease();
            return snapshot;
        }
    }
}