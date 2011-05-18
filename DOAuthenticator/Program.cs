using System;
using Monobjc;
using Monobjc.Foundation;

namespace Monobjc.Samples.DOAuthenticator
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
			
			if (args.Length < 1) {
				Console.WriteLine ();
				Console.WriteLine ("usage: DOAuthenticator server    // to run as server");
				Console.WriteLine ("usage: DOAuthenticator client    // to run as client");
				Console.WriteLine ("usage: One of the two arguments must be specified.");
				Environment.Exit (1);
			}
			
			Run(args[0]);
		}			
			
		public static void Run (String arg)
		{
			NSAutoreleasePool pool = new NSAutoreleasePool ();
			
			NSString CONNECTION_NAME = new NSString ("authentication test");
			
			if (arg == "server") {
				// Create a generic NSConnection to use to vend an object over DO.
				NSConnection conn = new NSConnection ();
				
				// Create a generic object to vend over DO; usually this is an object
				// that actually has something interesting to do as a "server".
				NSObject @object = new NSObject ();
				
				// Create an Authenticator object to authenticate messages that come
				// in to the server.  The client and server need to use the same
				// authentication logic, but would not need to use the same class.
				Authenticator authenticator = new Authenticator ();
				
				// Configure the connection
				conn.Delegate = authenticator;
				conn.RootObject = @object;
				
				// Set the name of the root object
				if (!conn.RegisterName (CONNECTION_NAME)) {
					Console.WriteLine ("OAuthenticator server: could not register server. Is one already running ?");
					Environment.Exit (1);
				}
				Console.WriteLine ("OAuthenticator server: started");
				
				// Have the run loop run forever, servicing incoming messages
				NSRunLoop.CurrentRunLoop.Run ();
				
				// Cleanup objects; not really necessary in this case
				authenticator.Release ();
				@object.Release ();
				conn.Release ();
			} else if (arg == "client") {
				// Create an Authenticator object to authenticate messages going
				// to the server.  The client and server need to use the same
				// authentication logic, but would not need to use the same class.
				Authenticator authenticator = new Authenticator ();
				NSDistantObject proxy;
				
				// Lookup the server connection
				NSConnection conn = NSConnection.ConnectionWithRegisteredNameHost (CONNECTION_NAME, null);
				
				if (conn == null) {
					Console.WriteLine ("OAuthenticator client: could not find server. You need to start one on this machine first.");
					Environment.Exit (1);
				}
				
				// Set the authenticator as the NSConnection delegate; all 
				// further messages, including the first one to lookup the root 
				// proxy, will go through the authenticator.
				conn.Delegate = authenticator;
				
				proxy = conn.RootProxy;
				
				if (proxy == null) {
					Console.WriteLine ("OAuthenticator client: could not get proxy. This should not happen.");
					Environment.Exit (1);
				}
				
				// Since this is an example, we don't really care what the "served" 
				// object really does, just that we can message it.  Since it is just
				// an NSObject, send it some NSObject messages.  If these aren't
				// authenticated successfully, an NSFailedAuthenticationException
				// exception is raised.
				Console.WriteLine ("description: {0}", proxy.Description);
				Console.WriteLine ("isKindOfClass NSObject ? {0}", proxy.IsKindOfClass (NSObject.NSObjectClass) ? "YES" : "NO");
				
				Console.WriteLine ("Done. Messages sent successfully.");
			} else {
				Console.WriteLine ("Either server or client must be specified.");
			}
			
			pool.Release ();
		}
	}
}
