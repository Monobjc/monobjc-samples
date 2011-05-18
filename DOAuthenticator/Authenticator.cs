using System;
using Monobjc;
using Monobjc.Foundation;

namespace Monobjc.Samples.DOAuthenticator
{
	[ObjectiveCClass]
	public partial class Authenticator : NSObject
	{
		public static readonly Class AuthenticatorClass = Class.Get (typeof(Authenticator));

		public Authenticator ()
		{
		}

		public Authenticator (IntPtr nativePointer) : base(nativePointer)
		{
		}

		[ObjectiveCMessage("connection:shouldMakeNewConnection:")]
		public virtual bool ConnectionShouldMakeNewConnection (NSConnection parentConnection, NSConnection newConnnection)
		{
			// A non-authentication related delegate method.  Make sure all
			// child (per-client) connections get the same delegate.
			newConnnection.Delegate = parentConnection.Delegate;
			return true;
		}

		[ObjectiveCMessage("authenticationDataForComponents:")]
		public virtual NSData AuthenticationDataForComponents (NSArray components)
		{
			NSUInteger idx1;
			int idx2;
			NSUInteger count = components.Count;
			byte checksum = 0;
			
			// Compute authentication data for the components in the
			// given array.  There are two types of components, NSPorts
			// and NSDatas.  You should ignore a component of a type
			// which you don't understand.
			
			// Here, we compute a trivial 1 byte checksum over all the
			// bytes in the NSData objects in the array.
			for (idx1 = 0; idx1 < count; idx1++) {
				NSObject item = components.ObjectAtIndex (idx1).CastTo<NSObject> ();
				if (!item.IsKindOfClass (NSData.NSDataClass)) {
					continue;
				}
				NSData data = item.CastTo<NSData> ();
				byte[] buffer = data.GetBuffer ();
				for (idx2 = 0; idx2 < buffer.Length; idx2++) {
					checksum ^= buffer[idx2];
				}
			}
			
			return new NSData (new byte[] { checksum }).Autorelease<NSData> ();
		}

		[ObjectiveCMessage("authenticateComponents:withData:")]
		public virtual bool AuthenticateComponentsWithData (NSArray components, NSData authenticationData)
		{
			// Verify the authentication data against the components.  A good
			// authenticator would have a way of verifying the signature without
			// recomputing it.  We don't, in this example, so just recompute.
			NSData recomputedSignature = this.AuthenticationDataForComponents (components);
			
			// If the two NSDatas are not equal, authentication failure!
			if (!recomputedSignature.IsEqual (authenticationData)) {
				Console.WriteLine ("received signature {0} doesn't match computed signature {1}", authenticationData, recomputedSignature);
				return false;
			}
			return true;
		}
	}
}
