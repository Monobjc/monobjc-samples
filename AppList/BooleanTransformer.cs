using System;
using Monobjc.Foundation;

namespace Monobjc.Samples.AppList
{
    [ObjectiveCClass]
    public class BooleanTransformer : NSObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanTransformer"/> class.
        /// </summary>
        public BooleanTransformer() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanTransformer"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public BooleanTransformer(IntPtr nativePointer) : base(nativePointer) { }

        public static Class TransformedValueClass
        {
            [ObjectiveCMessage("transformedValueClass")]
            get
            {
                return NSString.NSStringClass;
            }
        }

        public static bool AllowsReverseTransformation
        {
            [ObjectiveCMessage("allowsReverseTransformation")]
            get
            {
                return false;
            }
        }

        [ObjectiveCMessage("transformedValue:")]
        public Id transformedValue(Id value)
        {
        	bool b = value.SendMessage<bool>("boolValue");

			return b ? YES : NO;
        }
        
        private static readonly NSString YES = NSString.NSPinnedString("Yes");
        private static readonly NSString NO  = NSString.NSPinnedString("No");
    }
}