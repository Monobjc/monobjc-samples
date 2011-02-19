using System;
using System.Net;
using System.Runtime.InteropServices;
using Monobjc.Foundation;

namespace Monobjc.Samples.CustomFormatter
{
    [ObjectiveCClass]
    public class IPFormatter : NSFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IPFormatter"/> class.
        /// </summary>
        public IPFormatter() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="IPFormatter"/> class.
        /// </summary>
        /// <param name="nativeObject">The native object.</param>
        public IPFormatter(IntPtr nativeObject)
            : base(nativeObject) {}

        [ObjectiveCMessage("stringForObjectValue:")]
        public override NSString StringForObjectValue(Id anObject)
        {
            if (anObject == null)
            {
                return NSString.String;
            }
            // This example is simple as the object is a NSString
            // For a real formatter, you should convert the object to a string.
            return anObject.CastAs<NSString>();
        }

        [ObjectiveCMessage("getObjectValue:forString:errorDescription:")]
        public bool GetObjectValueForStringErrorDescription(IntPtr anObject, NSString str, IntPtr error)
        {
            // First, we convert the NSString to a String to parse it more easily
            String stringToParse = str;

            // If we cannot parse the string correctly into an IPAddress, then reject it
            IPAddress address;
            if (!IPAddress.TryParse(stringToParse, out address))
            {
                if (anObject != IntPtr.Zero)
                {
                    Marshal.WriteIntPtr(anObject, IntPtr.Zero);
                }
                if (error != IntPtr.Zero)
                {
                    // Marshal back the error message (the NSString is autoreleased)
                    NSString errorMessage = "Invalid IPv4 Address";
                    Marshal.WriteIntPtr(error, errorMessage.NativePointer);
                }
                return false;
            }

            // Marshal back the object (the NSString is autoreleased)
            NSString obj = address.ToString();
            if (anObject != IntPtr.Zero)
            {
                Marshal.WriteIntPtr(anObject, obj.NativePointer);
            }
            if (error != IntPtr.Zero)
            {
                Marshal.WriteIntPtr(error, IntPtr.Zero);
            }
            return true;
        }

        [ObjectiveCMessage("attributedStringForObjectValue:withDefaultAttributes:")]
        public override NSAttributedString AttributedStringForObjectValueWithDefaultAttributes(Id anObject, NSDictionary attributes)
        {
            NSAttributedString str = new NSAttributedString(this.StringForObjectValue(anObject), attributes);
            return str.Autorelease<NSAttributedString>();
        }
    }
}