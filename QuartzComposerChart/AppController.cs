using System;
using Monobjc.Quartz;
using Monobjc.Foundation;
using Monobjc.AppKit;

/* based on the QuartzComposer "QuartzComposerChart" example provided by Apple Inc. */

namespace Monobjc.Samples.QuartzComposerChart
{
    [ObjectiveCClass]
    public partial class AppController : NSObject
    {
        private static readonly Class AppControllerClass = Class.Get(typeof (AppController));

        private NSMutableArray _data;

        private readonly NSString kDataKey_Label = new NSString("label").Retain<NSString>();
        private readonly NSString kDataKey_Value = new NSString("value").Retain<NSString>();
        private readonly NSString kParameterKey_Data = new NSString("Data").Retain<NSString>();
        private readonly NSString kParameterKey_Scale = new NSString("Scale").Retain<NSString>();
        //readonly NSString kParameterKey_Spacing = new NSString("Spacing").Retain<NSString> (); // unused

        public AppController() {}

        public AppController(IntPtr nativePointer) : base(nativePointer) {}

        [ObjectiveCMessage("init")]
        public override Id Init()
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(AppControllerClass, "init");
            this._data = new NSMutableArray().Retain<NSMutableArray>();

            return this;
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this._data.Release();
            this.SendMessageSuper(AppControllerClass, "dealloc");
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            //Load the composition file into the QCView (because this QCView is bound to a QCPatchController in the nib file, this will actually update the QCPatchController along with all the bindings)
            if (!this.view.LoadCompositionFromFile(NSBundle.MainBundle.PathForResourceOfType(new NSString("Chart"), new NSString("qtz"))))
            {
                Console.WriteLine("Composition loading failed");
                NSApplication.SharedApplication.Terminate(null);
            }

            //Populate data storage
            this.AddObjectsAndKeys("Paolo Alto", 2);
            this.AddObjectsAndKeys("Cupertino", 1);
            this.AddObjectsAndKeys("Menlo Park", 4);
            this.AddObjectsAndKeys("Mountain View", 8);
            this.AddObjectsAndKeys("San Francisco", 7);
            this.AddObjectsAndKeys("Los Altos", 3);

            this.tableView.ReloadData();

            this.UpdateChart();
        }

        private void AddObjectsAndKeys(string value, int data)
        {
            // Hint: adding objects and keys only with AddObject(NSMutableDictionary.DictionaryWithObjectsAndKeys
            // will result in an NSDictionary object, which is not mutable (changeable)
            // furthermore the last paramter of "DictionaryWithObjectsAndKeys" must be NULL otherwise an NSInvalidOperationException will be thrown
            NSDictionary dict = NSDictionary.DictionaryWithObjectsAndKeys(new NSString(value), this.kDataKey_Label, NSNumber.NumberWithInt(data), this.kDataKey_Value, null);
            NSMutableDictionary mutDict = new NSMutableDictionary(dict, true);
            this._data.AddObject(mutDict);
        }

        private void UpdateChart()
        {
            float max, value;

            //Update the data displayed by the chart - it will be converted to a Structure of Structures by Quartz Composer
            this.view.SetValueForInputKey(this._data, this.kParameterKey_Data);

            //Compute the maximum value and set the chart scale accordingly
            max = 0.0f;

            for (uint i = 0; i < this._data.Count; ++i)
            {
                NSDictionary entry = (NSDictionary) this._data.ObjectAtIndex(i);
                value = (entry.ObjectForKey(this.kDataKey_Value).CastTo<NSNumber>()).FloatValue;
                if (value > max)
                {
                    max = value;
                }
            }

            this.view.SetValueForInputKey(NSNumber.NumberWithFloat(
                max > 0.0f ? 1.0f/max : 1.0f),
                                          this.kParameterKey_Scale
                );
        }

        partial void AddEntry(Id sender)
        {
            //Add a new entry to the data storage
            this.AddObjectsAndKeys("Untitled", 0);

            //Notify the NSTableView and update the chart
            this.tableView.ReloadData();
            this.UpdateChart();

            //Automatically select and edit the new entry
            this.tableView.SelectRowIndexesByExtendingSelection(NSIndexSet.IndexSetWithIndex(this._data.Count - 1), false);
            this.tableView.EditColumnRowWithEventSelect(this.tableView.ColumnWithIdentifier(this.kDataKey_Label), (int) this._data.Count - 1, null, true);
        }

        partial void RemoveEntry(Id sender)
        {
            int selectedRow;

            //Make sure we have a valid selected row
            selectedRow = this.tableView.SelectedRow;
            if (selectedRow < 0 || this.tableView.EditedRow == selectedRow)
            {
                return;
            }

            //Remove the currently selected entry from the data storage
            this._data.RemoveObjectAtIndex((uint) selectedRow);

            //Notify the NSTableView and update the chart
            this.tableView.ReloadData();
            this.UpdateChart();
        }

        [ObjectiveCMessage("numberOfRowsInTableView:")]
        public int NumberOfRowsInTableView(NSTableView aTableView)
        {
            //Return the number of entries in the data storage
            return (int) this._data.Count;
        }

        [ObjectiveCMessage("tableView:objectValueForTableColumn:row:")]
        public Id TableViewObjectValueForTableColumnRow(NSTableView aTableView, NSTableColumn aTableColumn, int rowIndex)
        {
            return (this._data.ObjectAtIndex((uint) rowIndex).CastTo<NSDictionary>()).ObjectForKey(aTableColumn.Identifier);
        }

        [ObjectiveCMessage("tableView:setObjectValue:forTableColumn:row:")]
        public void TableViewSetObjectValueForTableColumnRow(NSTableView aTableView, Id anObject, NSTableColumn aTableColumn, int rowIndex)
        {
            (this._data.ObjectAtIndex((uint) rowIndex).CastTo<NSMutableDictionary>()).SetObjectForKey(anObject, aTableColumn.Identifier);

            this.UpdateChart();
        }
    }
}