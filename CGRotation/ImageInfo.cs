using System;
using System.Runtime.InteropServices;
using Monobjc.Foundation;
using Monobjc.ApplicationServices;

namespace Monobjc.Samples.CGRotation
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ImageInfo
    {
        public float fRotation; // The rotation about the center of the image (degrees)
        public float fScaleX; // The scaling of the image along it's X-axis
        public float fScaleY; // The scaling of the image along it's Y-axis
        public float fTranslateX; // Move the image along the X-axis
        public float fTranslateY; // Move the image along the Y-axis
        public IntPtr fImageRef; // The image itself
        public NSDictionary fProperties; // Image properties
        public CGAffineTransform fOrientation; // Affine transform that ensures the image displays correctly
    }
}