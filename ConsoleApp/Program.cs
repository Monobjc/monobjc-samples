using System;
using Monobjc;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.ConsoleApp
{
    /// 
    /// Entry point for the application
    /// 
    internal class Program
    {
        private static void Main(String[] args)
        {
            // Load any required framework
            ObjectiveCRuntime.LoadFramework("Foundation");
            ObjectiveCRuntime.LoadFramework("AppKit");
            // Do the bridge initialization
            ObjectiveCRuntime.Initialize();
 
			// Allocate a pool for memory management
            NSAutoreleasePool pool = new NSAutoreleasePool();

            Console.WriteLine("Username: " + FoundationFramework.NSFullUserName());
            Console.WriteLine("Home Dir: " + FoundationFramework.NSHomeDirectory());
            Console.WriteLine("Screen  : " + NSScreen.MainScreen.Frame);
       
            NSDictionary dict = NSDictionary.DictionaryWithContentsOfFile("/System/Library/Frameworks/AppKit.framework/Resources/version.plist");
            Console.WriteLine("AppKit Version: " + dict[(NSString) "CFBundleVersion"].CastTo<NSString>());

			// Release any used memory
            pool.Release();
        }
    }
}