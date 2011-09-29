using System;
using Monobjc;
using Monobjc.Foundation;

namespace TutorialConsole
{
	internal class Program
	{
		private static void Main (String[] args)
		{
			#region --- Monobjc Generated Code ---
			//
			// DO NOT ALTER OR REMOVE
			//
			ObjectiveCRuntime.LoadFramework("Foundation");
			#endregion
			
			ObjectiveCRuntime.Initialize ();

			Run ();
		}
        
		private static void Run ()
		{
			NSAutoreleasePool pool = new NSAutoreleasePool ();
			
			Console.WriteLine ("Hello World !!!");
			
			pool.Release ();
		}
	}
}
