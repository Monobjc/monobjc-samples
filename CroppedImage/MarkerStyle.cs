namespace Monobjc.Samples.CroppedImage
{
    public enum MarkerStyle
    {
        PlainMarkerStyle, // Simple rectangle
        FinderMarkerStyle, // Rectangle bordered with a solid color, and its interior is tinted.
        IPhotoMarkerStyle, // Area outside the rectangle is tinted.
        LassoMarkerStyle // Path is stroked, and filled with a tint.
    }
}