using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.AppList
{
    [ObjectiveCCategory("NSRunningApplication")]
    public static class NSRunningApplication_BooleanPropertiesAsStrings
    {
        [ObjectiveCMessage("activeString")]
        public static NSString ActiveString(this NSRunningApplication runningApplication)
        {
            return runningApplication.IsActive ? "Of Course" : "Not At All";
        }

        [ObjectiveCMessage("hiddenString")]
        public static NSString HiddenString(this NSRunningApplication runningApplication)
        {
            return runningApplication.IsHidden ? "Well Yes" : "In Fact Not";
        }
    }
}