using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Text;
using Mono.Security;
using Monobjc.Foundation;

namespace Monobjc.Samples.TutorialMacAppStore
{
	public static class ReceiptChecker
	{
		/// <summary>
		/// The bundle identifier. Must match the one found in the Info.plist file.
		/// </summary>
		private static String IDENTIFIER = "Monobjc.Samples.TutorialMacAppStore";
		
		/// <summary>
		/// The bundle version. Must match the one found in the Info.plist file.
		/// </summary>
		private static String VERSION = "1.0.0";

		/// <summary>
		/// Exit code to trigger the receipt retrieval.
		/// </summary>
		private static int EXIT_CODE = 173;
		
		/// <summary>
		/// Date pattern for RFC 3339.
		/// </summary>
		private static String[] RFC_3339_PATTERNS = new String[]{
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ssK",
		};
		
		/// <summary>
		/// Perform a receipt check.
		/// </summary>
		internal static void Check ()
		{
			CheckBundleIdentifier ();
			CheckBundleVersion ();
			
			String path = Path.Combine (NSBundle.MainBundle.BundlePath, "Contents", "_MASReceipt", "receipt");
			if (!File.Exists (path)) {
				Environment.Exit (EXIT_CODE);
				return;
			}
			
			Receipt receipt = DecodeReceipt (path);
			CheckReceiptIdentifier (receipt);
			CheckReceiptVersion (receipt);
			CheckReceiptHash (receipt);
		}
		
		/// <summary>
		/// Extract PKCS#7 envelop from the receipt file and check its signature.
		/// </summary>
		private static Receipt DecodeReceipt (String path)
		{
			byte[] bytes = File.ReadAllBytes (path);
			SignedCms cms = new SignedCms ();

			cms.Decode (bytes);
			cms.CheckSignature (true);
			
			ContentInfo ci = cms.ContentInfo;
			ASN1 asn1Content = new ASN1 (ci.Content);
			
			Receipt receipt = new Receipt ();
			Decode (asn1Content, receipt);
			return receipt;
		}
		
		/// <summary>
		/// Decode the specified ASN.1 set and extract specific keys.
		/// </summary>
		private static void Decode (ASN1 asn1Set, Receipt receipt)
		{
			try {
				for (int i = 0; i < asn1Set.Count; i++) {
					ASN1 element = asn1Set [i];
				
					ASN1 asn1Type = element [0];
					ASN1 asn1Value = element [2];
				
					ReceiptAttributeType type = (ReceiptAttributeType)ToInt32 (asn1Type);
					switch (type) {
					case ReceiptAttributeType.BundleIdentifier:
					case ReceiptAttributeType.ApplicationVersion:
					case ReceiptAttributeType.OpaqueValue:
					case ReceiptAttributeType.SHA1Hash:
						{
							receipt [type] = asn1Value.Value;
							break;
						}
					case ReceiptAttributeType.InAppPurchase:
						IList<Receipt> purchases;
						if (receipt.ContainsKey (type)) {
							purchases = (IList<Receipt>)receipt [type];
						} else {
							purchases = new List<Receipt> ();
							receipt [type] = purchases;
						}
					
						ASN1 asn1InApp = new ASN1 (asn1Value.Value);
						Receipt purchase = new Receipt ();
						Decode (asn1InApp, purchase);
						purchases.Add (purchase);
					
						break;
					case ReceiptAttributeType.InAppQuantity:
						{
							ASN1 asn1Integer = new ASN1 (asn1Value.Value);
							receipt [type] = ToInt32 (asn1Integer);
							break;
						}
					case ReceiptAttributeType.InAppProductIdentifier:
					case ReceiptAttributeType.InAppTransactionIdentifier:
					case ReceiptAttributeType.InAppOriginalTransactionIdentifier:
						{
							ASN1 asn1String = new ASN1 (asn1Value.Value);
							receipt [type] = ToString (asn1String);
							break;
						}
					case ReceiptAttributeType.InAppPurchaseDate:
					case ReceiptAttributeType.InAppOriginalPurchaseDate:
						{
							ASN1 asn1String = new ASN1 (asn1Value.Value);
							String stringValue = ToString (asn1String);
							DateTime result;
							if (DateTime.TryParseExact (stringValue, RFC_3339_PATTERNS, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal, out result)) {
								receipt [type] = DateTime.SpecifyKind (result, DateTimeKind.Utc);
							}
							break;
						}
					default:
						break;
					}
#if DEBUG		
				// For debug purpose only
				if (receipt.ContainsKey(type))	{
					Console.WriteLine("receipt[{0}] = {1}", type, receipt[type].ToString());
				}
#endif
				}
			} catch (Exception e) {
				Environment.Exit (EXIT_CODE);
				return;
			}
		}
		
		/// <summary>
		/// Convert ASN.1 data to integer.
		/// </summary>
		private static int ToInt32 (ASN1 asn)
		{
			if (asn == null) {
				throw new ArgumentNullException ("asn1");
			}
			if (asn.Tag != 0x02) {
				throw new FormatException ("Only integer can be converted");
			}

			int x = 0;
			for (int i = 0; i < asn.Value.Length; i++) {
				x = (x << 8) + asn.Value [i];
			}
			return x;
		}
		
		/// <summary>
		/// Convert ASN.1 data to string.
		/// </summary>
		private static String ToString (ASN1 asn)
		{
			if (asn == null) {
				throw new ArgumentNullException ("asn1");
			}
			if (asn.Tag != 0x0C && asn.Tag != 0x16) {
				throw new FormatException ("Only string can be converted");
			}

			return Encoding.UTF8.GetString (asn.Value);
		}
		
		/// <summary>
		/// Check the bundle identfier found in the Info.plist file
		/// </summary>
		private static void CheckBundleIdentifier ()
		{
			NSDictionary dictionary = NSBundle.MainBundle.InfoDictionary;
			NSString str = dictionary.ValueForKey<NSString> ("CFBundleIdentifier");
			if (!str.IsEqualToString (IDENTIFIER)) {
				Environment.Exit (EXIT_CODE);
				return;
			}
		}
		
		/// <summary>
		/// Check the bundle version found in the Info.plist file
		/// </summary>
		private static void CheckBundleVersion ()
		{
			NSDictionary dictionary = NSBundle.MainBundle.InfoDictionary;
			NSString str = dictionary.ValueForKey<NSString> ("CFBundleShortVersionString");
			if (!str.IsEqualToString (VERSION)) {
				Environment.Exit (EXIT_CODE);
				return;
			}
		}
		
		/// <summary>
		/// Check the bundle identfier found in the receipt file
		/// </summary>
		private static void CheckReceiptIdentifier (Receipt receipt)
		{
			byte[] value = (byte[])receipt [ReceiptAttributeType.BundleIdentifier];
			ASN1 asn1String = new ASN1 (value);
			String str = ToString (asn1String);
			if (str != IDENTIFIER) {
				Environment.Exit (EXIT_CODE);
				return;
			}
		}
		
		/// <summary>
		/// Check the bundle version found in the receipt file.
		/// </summary>
		private static void CheckReceiptVersion (Receipt receipt)
		{
			byte[] value = (byte[])receipt [ReceiptAttributeType.ApplicationVersion];
			ASN1 asn1String = new ASN1 (value);
			String str = ToString (asn1String);
			if (str != VERSION) {
				Environment.Exit (EXIT_CODE);
				return;
			}
		}
		
		/// <summary>
		/// Checks the receipt hash based on local and receipt information.
		/// </summary>
		private static void CheckReceiptHash (Receipt receipt)
		{
			// Retrieve data
			byte[] macAddress = GetMacAddress ();
			byte[] bundleIdentifierData = (byte[])receipt [ReceiptAttributeType.BundleIdentifier];
			byte[] opaqueValueData = (byte[])receipt [ReceiptAttributeType.OpaqueValue];
			byte[] hashValueData = (byte[])receipt [ReceiptAttributeType.SHA1Hash];
			
			// Aggregate the data
			int index = 0;
			byte[] buffer = new byte[macAddress.Length + bundleIdentifierData.Length + opaqueValueData.Length];
			Array.Copy (macAddress, 0, buffer, index, macAddress.Length);
			index += macAddress.Length;
			Array.Copy (opaqueValueData, 0, buffer, index, opaqueValueData.Length);
			index += opaqueValueData.Length;
			Array.Copy (bundleIdentifierData, 0, buffer, index, bundleIdentifierData.Length);
			
			// Perform the hash of composite data
			SHA1 digest = SHA1.Create ();
			byte[] computedHash = digest.ComputeHash (buffer);
			
			// Compare generated hash and static value
			if (computedHash.Length != hashValueData.Length) {
				Environment.Exit (EXIT_CODE);
				return;
			}
			
			for (int i = 0; i < hashValueData.Length; i++) {
				if (computedHash [i] != hashValueData [i]) {
					Environment.Exit (EXIT_CODE);
					return;
				}
			}
			
		}
		
		/// <summary>
		/// Get the mac of the primary interface
		/// </summary>
		private static byte[] GetMacAddress ()
		{
			NetworkInterface mainInterface = NetworkInterface.GetAllNetworkInterfaces ().FirstOrDefault (itf => itf.Name == "en0");
			if (mainInterface == null) {
				Environment.Exit (EXIT_CODE);
				return null;
			}
			PhysicalAddress physicalAddress = mainInterface.GetPhysicalAddress ();
			return physicalAddress.GetAddressBytes ();
		}
		
		/// <summary>
		/// Convenience class.
		/// </summary>
		private class Receipt : Dictionary<ReceiptAttributeType, Object>
		{
		}		
		
		/// <summary>
		/// Receipt attribute type keys.
		/// </summary>
		private enum ReceiptAttributeType
		{
			BundleIdentifier = 2,
			ApplicationVersion = 3,
			OpaqueValue = 4,
			SHA1Hash = 5,
			InAppPurchase = 17,
			InAppQuantity = 1701,
			InAppProductIdentifier = 1702,
			InAppTransactionIdentifier = 1703,
			InAppPurchaseDate = 1704,
			InAppOriginalTransactionIdentifier = 1705,
			InAppOriginalPurchaseDate = 1706,
		}
	}
}
