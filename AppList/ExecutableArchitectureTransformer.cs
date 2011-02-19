using System;
using Monobjc.Foundation;

namespace Monobjc.Samples.AppList
{
    [ObjectiveCClass]
    public class ExecutableArchitectureTransformer : NSObject
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "ExecutableArchitectureTransformer" /> class.
        /// </summary>
        public ExecutableArchitectureTransformer() {}

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ExecutableArchitectureTransformer" /> class.
        /// </summary>
        /// <param name = "nativePointer">The native pointer.</param>
        public ExecutableArchitectureTransformer(IntPtr nativePointer) : base(nativePointer) {}

        public static Class TransformedValueClass
        {
            [ObjectiveCMessage("transformedValueClass")]
            get { return NSString.NSStringClass; }
        }

        public static bool AllowsReverseTransformation
        {
            [ObjectiveCMessage("allowsReverseTransformation")]
            get { return false; }
        }

        [ObjectiveCMessage("transformedValue:")]
        public Id transformedValue(Id value)
        {
            NSString archStr;

            switch (value.SendMessage<NSMachOArchitecture>("intValue"))
            {
                case NSMachOArchitecture.NSBundleExecutableArchitectureI386:
                    archStr = @"Intel 32-bit";
                    break;
                case NSMachOArchitecture.NSBundleExecutableArchitectureX86_64:
                    archStr = @"Intel 64-bit";
                    break;
                case NSMachOArchitecture.NSBundleExecutableArchitecturePPC:
                    // in case of Rosetta, allow for PPC
                    archStr = @"PPC Translated";
                    break;
                default:
                    archStr = @"unknown";
                    break;
            }

            return archStr;
        }
    }
}