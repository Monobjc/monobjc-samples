using System;
using Monobjc.AddressBook;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.CocoaPeoplePicker
{
    [ObjectiveCClass]
    public partial class MyPeoplePickerController : NSObject
    {
        public MyPeoplePickerController() {}

        public MyPeoplePickerController(IntPtr nativePointer)
            : base(nativePointer) {}

        partial void GetGroups(Id sender)
        {
            NSArray groups = this.ppView.SelectedGroups;
            Console.WriteLine("getGroups: {0} groups selected", groups.Count);
            uint count = groups.Count;
            for (int index = 0; index < count; index++)
            {
                ABRecord record = groups[index].CastTo<ABRecord>();
                Console.WriteLine("  Group {0}: {1}", index, record.UniqueId);
            }
        }

        partial void GetRecords(Id sender)
        {
            NSArray records = this.ppView.SelectedRecords;
            Console.WriteLine("getRecords: {0} records selected", records.Count);
            uint count = records.Count;
            for (int index = 0; index < count; index++)
            {
                ABRecord record = records[index].CastTo<ABRecord>();
                Console.WriteLine("  Record {0}: {1}", index, record.UniqueId);
            }
        }

        partial void ViewProperty(NSButton sender)
        {
            NSString property = null;
            switch ((int) sender.Tag)
            {
                case 0: // Phone
                    property = AddressBookFramework.kABPhoneProperty;
                    break;
                case 1: // Address
                    property = AddressBookFramework.kABAddressProperty;
                    break;
                case 2: // Email
                    property = AddressBookFramework.kABEmailProperty;
                    break;
                case 3: // AIM
                    property = AddressBookFramework.kABAIMInstantProperty;
                    break;
                case 4: // Homepage
                    property = AddressBookFramework.kABHomePageProperty;
                    break;
                default:
                    break;
            }
            if (sender.State == NSCellStateValue.NSOnState)
            {
                this.ppView.AddProperty(property);
            }
            else
            {
                this.ppView.RemoveProperty(property);
            }
        }

        partial void SetGroupSelection(NSButton sender)
        {
            this.ppView.AllowsGroupSelection = (sender.State == NSCellStateValue.NSOnState);
        }

        partial void SetMultiRecordSelection(NSButton sender)
        {
            this.ppView.AllowsMultipleSelection = (sender.State == NSCellStateValue.NSOnState);
        }

        partial void EditInAB(Id sender)
        {
            this.ppView.EditInAddressBook(sender);
        }

        partial void SelectInAB(Id sender)
        {
            this.ppView.SelectInAddressBook(sender);
        }
    }
}