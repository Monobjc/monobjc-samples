using System;
using Monobjc.AddressBook;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.AddressBookCocoa
{
    [ObjectiveCClass]
    public partial class MyObject : NSObject
    {
        public MyObject() {}

        public MyObject(IntPtr nativePointer)
            : base(nativePointer) {}

		partial void AddElba(Id sender)
        {
            NSMutableDictionary homeAddr, workAddr;
            ABMutableMultiValue multiValue;

            // Get the address book - there is only one.
            ABAddressBook ab = ABAddressBook.SharedAddressBook;

            // Create a record.
            ABPerson person = new ABPerson();

            // Set value in record for first name property.
            person.SetValueForProperty((NSString) "Able", AddressBookFramework.kABFirstNameProperty);
            // Set value in record for last name property.
            person.SetValueForProperty((NSString) "Elba", AddressBookFramework.kABLastNameProperty);

            // kABAddressProperty is a multiValue.
            // It's values, such as kABAddressHomeLabel, have in turn keys, 
            // such as kABAddressStreetKey.
            // Create and populate a NSDictionary with some kABAddressHomeLabel keys.
            homeAddr = new NSMutableDictionary();
            homeAddr[AddressBookFramework.kABAddressStreetKey] = (NSString) "123 Home Dr.";
            homeAddr[AddressBookFramework.kABAddressCityKey] = (NSString) "Home City";
            homeAddr[AddressBookFramework.kABAddressStateKey] = (NSString) "CA";
            homeAddr[AddressBookFramework.kABAddressZIPKey] = (NSString) "94110";
            homeAddr[AddressBookFramework.kABAddressCountryKey] = (NSString) "United States";

            // Create and populate a NSDictionary with some kABAddressWorkLabel keys.
            workAddr = new NSMutableDictionary();
            workAddr[AddressBookFramework.kABAddressStreetKey] = (NSString) "123 Home Dr.";
            workAddr[AddressBookFramework.kABAddressCityKey] = (NSString) "Home City";
            workAddr[AddressBookFramework.kABAddressStateKey] = (NSString) "CA";
            workAddr[AddressBookFramework.kABAddressZIPKey] = (NSString) "94110";
            workAddr[AddressBookFramework.kABAddressCountryKey] = (NSString) "United States";

            // Create an ABMultivalue and add the kABAddressHomeLabel and 
            // kABAddressWorkLabel NSDictionaries
            multiValue = new ABMutableMultiValue();
            multiValue.AddValueWithLabel(homeAddr, AddressBookFramework.kABAddressHomeLabel);
            multiValue.AddValueWithLabel(workAddr, AddressBookFramework.kABAddressWorkLabel);

            // Set value in record for kABAddressProperty.
            person.SetValueForProperty(multiValue, AddressBookFramework.kABAddressProperty);
            multiValue.Release();

            // kABPhoneProperty is a multivalue.
            // Create and populate a multiValue.
            multiValue = new ABMutableMultiValue();
            multiValue.AddValueWithLabel((NSString) "408-974-0000", AddressBookFramework.kABPhoneWorkLabel);
            multiValue.AddValueWithLabel((NSString) "408-974-1111", AddressBookFramework.kABPhoneHomeLabel);
            multiValue.AddValueWithLabel((NSString) "408-974-2222", AddressBookFramework.kABPhoneMobileLabel);
            multiValue.AddValueWithLabel((NSString) "408-974-3333", AddressBookFramework.kABPhoneMainLabel);
            multiValue.AddValueWithLabel((NSString) "408-974-4444", AddressBookFramework.kABPhoneHomeFAXLabel);
            multiValue.AddValueWithLabel((NSString) "408-974-5555", AddressBookFramework.kABPhoneWorkFAXLabel);
            multiValue.AddValueWithLabel((NSString) "408-974-6666", AddressBookFramework.kABPhonePagerLabel);

            // Set value in record for kABPhoneProperty.
            person.SetValueForProperty(multiValue, AddressBookFramework.kABPhoneProperty);
            multiValue.Release();

            // Add record to the Address Book
            if (ab.AddRecord(person))
            {
                // Save the Address Book
                if (ab.Save())
                {
                    Console.WriteLine("Success");
                }
            }
            person.Release();
        }

        partial void FindElba(Id sender)
        {
            //ObjectiveCRuntime.ShowStatistics();

            ABMultiValue multiValue;
            ABSearchElement find;
            NSArray results;
            ABRecord firstRecord;
            uint index = 0;

            // Get the address book; there is only one.
            ABAddressBook ab = ABAddressBook.SharedAddressBook;

            // Create a search element
            find = ABPerson.SearchElementForPropertyLabelKeyValueComparison(AddressBookFramework.kABLastNameProperty,
                                                                            null,
                                                                            null,
                                                                            (NSString) "Elba",
                                                                            ABSearchComparison.kABEqual);

            // Run a search
            results = ab.RecordsMatchingSearchElement(find);

            // How many records matched?
            if (results.Count > 0)
            {
                // Fill in the matching records UI
                this.matchingRecords.IntValue = (int) results.Count;

                // Get the first record
                firstRecord = results[0].CastTo<ABRecord>();

                // Get the entry for the kABFirstNameProperty and fill in the first name UI
                this.firstName.StringValue = firstRecord.ValueForProperty(AddressBookFramework.kABFirstNameProperty).CastTo<NSString>();

                // Create a multiValue and populate it with the items in kABAddressProperty
                multiValue = firstRecord.ValueForProperty(AddressBookFramework.kABAddressProperty).CastTo<ABMultiValue>();

                // Get an index into a multiValue value for the kABAddressHomeLabel label
                if (FindFirstMatch(multiValue, AddressBookFramework.kABAddressHomeLabel, ref index))
                {
                    // kABAddressHomeLabel is a NSDictionary
                    NSDictionary dict = multiValue.ValueAtIndex(index).CastTo<NSDictionary>();
                    this.streetAddr.StringValue = dict[AddressBookFramework.kABAddressStreetKey].CastTo<NSString>();
                }

                // Create a multiValue and populate it with the items in the kABPhoneProperty
                multiValue = firstRecord.ValueForProperty(AddressBookFramework.kABPhoneProperty).CastTo<ABMultiValue>();

                // Get an index into multiValue for the kABPhoneWorkFAXLabel label
                if (FindFirstMatch(multiValue, AddressBookFramework.kABPhoneWorkFAXLabel, ref index))
                {
                    this.workFaxPhone.StringValue = multiValue.ValueAtIndex(index).CastTo<NSString>();
                }
            }
        }

        private static bool FindFirstMatch(ABMultiValue multiValue, NSString label, ref uint index)
        {
            uint mvCount;

            mvCount = multiValue.Count;
            if (mvCount > 0)
            {
                for (uint x = 0; x < mvCount; x++)
                {
                    NSString text = multiValue.LabelAtIndex(x);
                    NSComparisonResult result = text.Compare(label);

                    if (result == NSComparisonResult.NSOrderedSame)
                    {
                        index = x;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}