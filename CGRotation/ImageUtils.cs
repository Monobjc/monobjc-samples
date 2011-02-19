using System;
using System.Runtime.InteropServices;
using Monobjc.Foundation;
using Monobjc.ApplicationServices;
using Monobjc.CoreServices;
using Monobjc.CoreFoundation;

namespace Monobjc.Samples.CGRotation
{
    public static class ImageUtils
    {
        // Create a new image from a file at the given url
        // Returns null if unsuccessful.
        public static ImageInfo IICreateImage(NSURL url)
        {
            ImageInfo ii = new ImageInfo();
            // Try to create an image source to the image passed to us
            IntPtr imageSrc = CGImageSource.CreateWithURL(url, null);
            if (imageSrc != IntPtr.Zero)
            {
                // And if we can, try to obtain the first image available
                IntPtr image = CGImageSource.CreateImageAtIndex(imageSrc, 0, null);
                if (image != IntPtr.Zero)
                {
                    // and if we could, create the ImageInfo struct with default values
                    ii.fRotation = 0.0f;
                    ii.fScaleX = 1.0f;
                    ii.fScaleY = 1.0f;
                    ii.fTranslateX = 0.0f;
                    ii.fTranslateY = 0.0f;
                    // the ImageInfo struct owns this CGImageRef now, so no need for a retain.
                    ii.fImageRef = image;
                    // the ImageInfo struct owns this CFDictionaryRef, so no need for a retain.
                    ii.fProperties = CGImageSource.CopyPropertiesAtIndex(imageSrc, 0, null);
                    // Setup the orientation transformation matrix so that the image will display with the proper orientation
                    IIGetOrientationTransform(ref ii);
                }
                // cleanup the image source
                CFType.CFRelease(imageSrc);
            }
            return ii;
        }

        // Transforms the context based on the orientation of the image.
        // This ensures the image always appears correctly when drawn.
        public static void IIGetOrientationTransform(ref ImageInfo image)
        {
            float w = (uint) CGImage.GetWidth(image.fImageRef);
            float h = (uint) CGImage.GetHeight(image.fImageRef);
            if (image.fProperties != null)
            {
                // The Orientations listed here are mirroed from CGImageProperties.h,
                // listed under the kCGImagePropertyOrientation key.
                switch (IIGetImageOrientation(ref image))
                {
                    case 1:
                        // 1 = 0th row is at the top, and 0th column is on the left.
                        // Orientation Normal
                        image.fOrientation = CGAffineTransform.Make(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);
                        break;

                    case 2:
                        // 2 = 0th row is at the top, and 0th column is on the right.
                        // Flip Horizontal
                        image.fOrientation = CGAffineTransform.Make(-1.0f, 0.0f, 0.0f, 1.0f, w, 0.0f);
                        break;

                    case 3:
                        // 3 = 0th row is at the bottom, and 0th column is on the right.
                        // Rotate 180 degrees
                        image.fOrientation = CGAffineTransform.Make(-1.0f, 0.0f, 0.0f, -1.0f, w, h);
                        break;

                    case 4:
                        // 4 = 0th row is at the bottom, and 0th column is on the left.
                        // Flip Vertical
                        image.fOrientation = CGAffineTransform.Make(1.0f, 0.0f, 0.0f, -1.0f, 0.0f, h);
                        break;

                    case 5:
                        // 5 = 0th row is on the left, and 0th column is the top.
                        // Rotate -90 degrees and Flip Vertical
                        image.fOrientation = CGAffineTransform.Make(0.0f, -1.0f, -1.0f, 0.0f, h, w);
                        break;

                    case 6:
                        // 6 = 0th row is on the right, and 0th column is the top.
                        // Rotate 90 degrees
                        image.fOrientation = CGAffineTransform.Make(0.0f, -1.0f, 1.0f, 0.0f, 0.0f, w);
                        break;

                    case 7:
                        // 7 = 0th row is on the right, and 0th column is the bottom.
                        // Rotate 90 degrees and Flip Vertical
                        image.fOrientation = CGAffineTransform.Make(0.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f);
                        break;

                    case 8:
                        // 8 = 0th row is on the left, and 0th column is the bottom.
                        // Rotate -90 degrees
                        image.fOrientation = CGAffineTransform.Make(0.0f, 1.0f, -1.0f, 0.0f, h, 0.0f);
                        break;
                }
            }
        }

        // Gets the orientation of the image from the properties dictionary if available
        // If the kCGImagePropertyOrientation is not available or invalid,
        // then 1, the default orientation, is returned.
        public static int IIGetImageOrientation(ref ImageInfo image)
        {
            int result = 1;
            if (image.fProperties != null)
            {
                Id value = image.fProperties.ValueForKey(CGImageProperties.kCGImagePropertyOrientation);
                if (value != null)
                {
                    NSNumber orientation = value.CastTo<NSNumber>();
                    if (orientation != null)
                    {
                        result = orientation.IntValue;
                    }
                }
            }
            return result;
        }

        // Save the given image to a file at the given url.
        // Returns true if successful, false otherwise.
        public static bool IISaveImage(ImageView view, NSURL url, uint width, uint height)
        {
            ImageInfo image = view.Image;
            bool result = false;

            // If there is no image, no destination, or the width/height is 0, then fail early.
            if ((url != null) && (width != 0) && (height != 0))
            {
                // Try to create a jpeg image destination at the url given to us
                IntPtr imageDest = CGImageDestination.CreateWithURL(url, UTType.kUTTypeJPEG, 1, null);
                if (imageDest != IntPtr.Zero)
                {
                    // And if we can, then we can start building our final image.
                    // We begin by creating a CGBitmapContext to host our desintation image.

                    // Allocate enough space to hold our pixels
                    IntPtr imageData = Marshal.AllocHGlobal((int) (Marshal.SizeOf(typeof (UInt32))*width*height));

                    // Create the bitmap context
                    IntPtr bitmapContext = CGBitmapContext.Create(
                        imageData, // image data we just allocated...
                        width, // width
                        height, // height
                        8, // 8 bits per component
                        sizeof (UInt32)*width, // bytes per pixel times number of pixels wide
                        CGImage.GetColorSpace(image.fImageRef), // use the same colorspace as the original image
                        (CGBitmapInfo) CGImageAlphaInfo.kCGImageAlphaPremultipliedFirst); // use premultiplied alpha

                    // Check that all that went well
                    if (bitmapContext != IntPtr.Zero)
                    {
                        // Now, we draw the image to the bitmap context
                        IIDrawImageTransformed(ref image, bitmapContext, CGRect.CGRectMake(0.0f, 0.0f, width, height));

                        // We have now gotten our image data to the bitmap context, and correspondingly
                        // into imageData. If we wanted to, we could look at any of the pixels of the image
                        // and manipulate them in any way that we desire, but for this case, we're just
                        // going to ask ImageIO to write this out to disk.

                        // Obtain a CGImageRef from the bitmap context for ImageIO
                        IntPtr imageIOImage = CGBitmapContext.CreateImage(bitmapContext);

                        // Check if we have additional properties from the original image
                        if (image.fProperties != null)
                        {
                            // If we do, then we want to inspect the orientation property.
                            // If it exists and is not the default orientation, then we
                            // want to replace that orientation in the destination file
                            int orientation = IIGetImageOrientation(ref image);
                            if (orientation != 1)
                            {
                                // If the orientation in the original image was not the default,
                                // then we need to replace that key in a duplicate of that dictionary
                                // and then pass that dictionary to ImageIO when adding the image.
                                NSMutableDictionary prop = new NSMutableDictionary(image.fProperties);
                                orientation = 1;
                                prop.SetValueForKey(NSNumber.NumberWithInt(orientation), CGImageProperties.kCGImagePropertyOrientation);

                                // And add the image with the new properties
                                CGImageDestination.AddImage(imageDest, imageIOImage, prop);

                                // Clean up after ourselves
                                prop.Release();
                            }
                            else
                            {
                                // Otherwise, the image was already in the default orientation and we can just save
                                // it with the original properties.
                                CGImageDestination.AddImage(imageDest, imageIOImage, image.fProperties);
                            }
                        }
                        else
                        {
                            // If we don't, then just add the image without properties
                            CGImageDestination.AddImage(imageDest, imageIOImage, null);
                        }

                        // Release the image and the context, since we are done with both.
                        CGImage.Release(imageIOImage);
                        CGContext.Release(bitmapContext);
                    }

                    // Deallocate the image data
                    Marshal.FreeHGlobal(imageData);

                    // Finalize the image destination
                    result = CGImageDestination.Finalize(imageDest);
                    CFType.CFRelease(imageDest);
                }
            }

            return result;
        }

        // Applies the transformations specified in the ImageInfo struct without drawing the actual image
        public static void IIApplyTransformation(ref ImageInfo image, IntPtr context, CGRect bounds)
        {
            if (context != IntPtr.Zero)
            {
                // Whenever you do multiple CTM changes, you have to be very careful with order.
                // Changing the order of your CTM changes changes the outcome of the drawing operation.
                // For example, if you scale a context by 2.0 along the x-axis, and then translate
                // the context by 10.0 along the x-axis, then you will see your drawing will be
                // in a different position than if you had done the operations in the opposite order.

                // Our intent with this operation is that we want to change the location from which we start drawing
                // (translation), then rotate our axies so that our image appears at an angle (rotation), and finally
                // scale our axies so that our image has a different size (scale).
                // Changing the order of operations will markedly change the results.
                IITranslateContext(ref image, context);
                IIRotateContext(ref image, context, bounds);
                IIScaleContext(ref image, context, bounds);
            }
        }

        // Draw the image to the given context centered inside the given bounds
        public static void IIDrawImage(ref ImageInfo image, IntPtr context, CGRect bounds)
        {
            CGRect imageRect;

            // Setup the image size so that the image fills it's natural boudaries in the base coordinate system.
            imageRect.size.width = (uint) CGImage.GetWidth(image.fImageRef);
            imageRect.size.height = (uint) CGImage.GetHeight(image.fImageRef);

            // Determine the correct origin of the image such that it is centered in the coordinate system.
            // The exact calculations depends on the image orientation, but the basic idea
            // is that the image is located such that it is positioned so that half the difference
            // between the image's size and the bounds to be drawn is used as it's x/y location.
            if ((image.fProperties == null) || (IIGetImageOrientation(ref image) < 5))
            {
                // For orientations 1-4, the images are unrotated, so the width and height of the base image
                // can be used as the width and height of the coordinate translation calculation.
                imageRect.origin.x = (float) Math.Floor((bounds.size.width - imageRect.size.width)/2.0f);
                imageRect.origin.y = (float) Math.Floor((bounds.size.height - imageRect.size.height)/2.0f);
            }
            else
            {
                // For orientations 5-8, the images are rotated 90 or -90 degrees, so we need to use
                // the image width in place of the height and vice versa.
                imageRect.origin.x = (float) Math.Floor((bounds.size.width - imageRect.size.height)/2.0f);
                imageRect.origin.y = (float) Math.Floor((bounds.size.height - imageRect.size.width)/2.0f);
            }

            // Obtain the orientation matrix for this image
            CGAffineTransform ctm = image.fOrientation;

            // Finally, orient the context so that the image draws naturally.
            CGContext.ConcatCTM(context, ctm);

            // And draw the image.
            CGContext.DrawImage(context, imageRect, image.fImageRef);
        }

        // Rotates the context around the center point of the given bounds
        public static void IIRotateContext(ref ImageInfo image, IntPtr context, CGRect bounds)
        {
            // First we translate the context such that the 0,0 location is at the center of the bounds
            CGContext.TranslateCTM(context, bounds.size.width/2.0f, bounds.size.height/2.0f);

            // Then we rotate the context, converting our angle from degrees to radians
            CGContext.RotateCTM(context, image.fRotation*M_PI/180.0f);

            // Finally we have to restore the center position
            CGContext.TranslateCTM(context, -bounds.size.width/2.0f, -bounds.size.height/2.0f);
        }

        // Scale the context around the center point of the given bounds
        public static void IIScaleContext(ref ImageInfo image, IntPtr context, CGRect bounds)
        {
            // First we translate the context such that the 0,0 location is at the center of the bounds
            CGContext.TranslateCTM(context, bounds.size.width/2.0f, bounds.size.height/2.0f);

            // Next we scale the context to the size that we want
            CGContext.ScaleCTM(context, image.fScaleX, image.fScaleY);

            // Finally we have to restore the center position
            CGContext.TranslateCTM(context, -bounds.size.width/2.0f, -bounds.size.height/2.0f);
        }

        // Translate the context
        public static void IITranslateContext(ref ImageInfo image, IntPtr context)
        {
            // Translation is easy, just translate.
            CGContext.TranslateCTM(context, image.fTranslateX, image.fTranslateY);
        }

        // Draw the image to the given context centered inside the given bounds with
        // the transformation info. The CTM of the context is unchanged after this call
        public static void IIDrawImageTransformed(ref ImageInfo image, IntPtr context, CGRect bounds)
        {
            // We save the current graphics state so as to not disrupt it for the caller.
            CGContext.SaveGState(context);

            // Apply the transformation
            IIApplyTransformation(ref image, context, bounds);

            // Draw the image centered in the context
            IIDrawImage(ref image, context, bounds);

            // Restore our original graphics state.
            CGContext.RestoreGState(context);
        }

        // Release the ImageInfo struct and other associated data
        // you should not refer to the reference after this call
        // This function is null safe.
        public static void IIRelease(ref ImageInfo image)
        {
            CGImage.Release(image.fImageRef);
            image.fProperties.SafeRelease();
        }

        private static readonly float M_PI = 3.141593f;
    }
}