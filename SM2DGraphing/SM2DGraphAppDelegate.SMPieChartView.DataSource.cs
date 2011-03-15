// 
// Monobjc : a .NET/Objective-C bridge
// Copyright (C) 2007-2009  Laurent Etiemble
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.SM2DGraphView;

namespace Monobjc.Samples.SM2DGraphing
{
    partial class SM2DGraphAppDelegate
    {
        [ObjectiveCMessage("numberOfSlicesInPieChartView:")]
        public uint numberOfSlicesInPieChartView(SMPieChartView inPieChartView)
        {
            uint result;

            if (inPieChartView == this._sm_hardDrive)
            {
                result = 7;
            }
            else
            {
                result = 5;
            }
            return result;
        }

        [ObjectiveCMessage("pieChartView:dataForSliceIndex:")]
        public double pieChartViewDataForSliceIndex(SMPieChartView inPieChartView, uint inSliceIndex)
        {
            double result = 0.0;

            if (inPieChartView == this._sm_timeChart)
            {
                switch (inSliceIndex)
                {
                    case 0:
                        result = 2.0;
                        break;
                    case 1:
                        result = 3.0;
                        break;
                    case 2:
                        result = 4.0;
                        break;
                    case 3:
                        result = 5.0;
                        break;
                    case 4:
                        result = 7.0;
                        break;
                }
            }

            return result;
        }

        [ObjectiveCMessage("pieChartViewArrayOfSliceData:")]
        public NSArray pieChartViewArrayOfSliceData(SMPieChartView inPieChartView)
        {
            NSArray result = null;

            if (inPieChartView == this._sm_hardDrive)
            {
                result = NSArray.ArrayWithObjects(NSNumber.NumberWithInt(10), NSNumber.NumberWithInt(5),
                                                  NSNumber.NumberWithInt(10), NSNumber.NumberWithInt(5),
                                                  NSNumber.NumberWithInt(10), NSNumber.NumberWithInt(5),
                                                  NSNumber.NumberWithInt(10), null);
            }

            return result;
        }

        [ObjectiveCMessage("pieChartView:attributesForSliceIndex:")]
        public NSDictionary pieChartViewAttributesForSliceIndex(SMPieChartView inPieChartView, uint inSliceIndex)
        {
            NSDictionary result;
            NSColor tempColor;

            switch (inSliceIndex%7)
            {
                default:
                case 0:
                    tempColor = NSColor.BlackColor;
                    break;
                case 1:
                    tempColor = NSColor.RedColor;
                    break;
                case 2:
                    tempColor = NSColor.GreenColor;
                    break;
                case 3:
                    tempColor = NSColor.BlueColor;
                    break;
                case 4:
                    tempColor = NSColor.YellowColor;
                    break;
                case 5:
                    tempColor = NSColor.CyanColor;
                    break;
                case 6:
                    tempColor = NSColor.MagentaColor;
                    break;
            }

            if (inPieChartView != this._sm_clickedPie)
            {
                // Make it transparent.
                tempColor = tempColor.ColorWithAlphaComponent(0.4f);
            }

            if (this._sm_clickedSlice.IntValue == inSliceIndex && inPieChartView == this._sm_clickedPie)
            {
                result = NSDictionary.DictionaryWithObjectsAndKeys(tempColor, NSAttributedString_AppKitAdditions.NSBackgroundColorAttributeName,
                                                                   NSColor.WhiteColor, NSAttributedString_AppKitAdditions.NSForegroundColorAttributeName,
                                                                   null);
            }
            else
            {
                result = NSDictionary.DictionaryWithObjectForKey(tempColor, NSAttributedString_AppKitAdditions.NSBackgroundColorAttributeName);
            }

            return result;
        }

        [ObjectiveCMessage("numberOfExplodedPartsInPieChartView:")]
        public uint numberOfExplodedPartsInPieChartView(SMPieChartView inPieChartView)
        {
            uint result = 0;

            if (inPieChartView == this._sm_hardDrive)
            {
                result = 2;
            }

            return result;
        }

        [ObjectiveCMessage("pieChartView:rangeOfExplodedPartIndex:")]
        public NSRange pieChartViewRangeOfExplodedPartIndex(SMPieChartView inPieChartView, uint inIndex)
        {
            NSRange result = NSRange.NSZeroRange;

            if (inPieChartView == this._sm_hardDrive)
            {
                switch (inIndex)
                {
                    case 0:
                        result.location = 1;
                        result.length = 3;
                        break;
                    case 1:
                        result.location = 6;
                        result.length = 1;
                        break;
                }
            }

            return result;
        }
    }
}