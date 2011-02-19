using Monobjc.AppKit;

namespace Monobjc.Samples.CroppedImage
{
    public static class NSBezierPathExtensions
    {
        public static bool IsClosed(NSBezierPath path)
        {
            int elements = path.ElementCount;
            if (elements > 0)
            {
                if (path.ElementAtIndex(path.ElementCount - 1) == NSBezierPathElement.NSClosePathBezierPathElement)
                {
                    return true;
                }
            }
            return false;
        }

        public static NSBezierPath ClosedPath(NSBezierPath path)
        {
            if (!IsClosed(path))
            {
                NSBezierPath result = path.Copy<NSBezierPath>();
                result.ClosePath();
                return result;
            }
            return path;
        }
    }
}