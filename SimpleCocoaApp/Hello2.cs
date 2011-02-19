using System;
using Monobjc.Foundation;
using Monobjc.AppKit;

namespace Monobjc.Samples.SimpleCocoaApp
{
    [ObjectiveCClass]
    public partial class Hello2 : NSObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hello2"/> class.
        /// </summary>
        public Hello2() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Hello2"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public Hello2(IntPtr nativePointer)
            : base(nativePointer) {}

        partial void Message1(Id sender)
        {
            // Use a standard alert to display the message
            AppKitFramework.NSRunAlertPanel("Message1, Hello2", "Hello, Cocoa!", "OK", null, null);
        }

        partial void Message2(Id sender)
        {
            // Use a standard alert to display the message
            AppKitFramework.NSRunAlertPanel("Message2, Hello2", "Hello again, Cocoa!", "OK", null, null);
        }

        partial void Message3(Id sender)
        {
            // Use a standard alert to display the message
            AppKitFramework.NSRunAlertPanel("Message3, Hello2", this.messageTextField.StringValue, "OK", null, null);
        }
    }
}