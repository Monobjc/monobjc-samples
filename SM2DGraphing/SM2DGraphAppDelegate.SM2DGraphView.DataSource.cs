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
        [ObjectiveCMessage("numberOfLinesInTwoDGraphView:")]
        public uint numberOfLinesInTwoDGraphView(SM2DGraphView.SM2DGraphView inGraphView)
        {
            if (inGraphView == this._sm_trigGraph || inGraphView == this._sm_barGraph)
            {
                return 2;
            }
            return 4;
        }

        [ObjectiveCMessage("twoDGraphView:dataForLineIndex:")]
        public NSArray twoDGraphViewDataForLineIndex(SM2DGraphView.SM2DGraphView inGraphView, uint inLineIndex)
        {
            NSMutableArray result = null;

            if (inGraphView == this._sm_barGraph)
            {
                if (inLineIndex == 0)
                {
                    // The bars themselves.
                    result = NSMutableArray.Array;
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.0f, 6.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.0f, 9.5f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(2.0f, 7.0f)));
                }
                else if (inLineIndex == 1)
                {
                    // The line.
                    result = NSMutableArray.Array;
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.0f, 4.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.5f, 4.5f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(2.0f, 6.0f)));
                }
            }
            else if (inGraphView == this._sm_costTimeGraph)
            {
                if (inLineIndex == 0)
                {
                    result = NSMutableArray.Array;
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.0f, 0.1f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.5f, 4.2f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.0f, 12.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.5f, 7.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(2.0f, 17.0f)));
                }
                else if (inLineIndex == 1)
                {
                    result = NSMutableArray.Array;
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.0f, 3.5f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.25f, 15.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.5f, 10.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.75f, 12.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.0f, 19.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.25f, 17.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.5f, 24.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.75f, 30.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(2.0f, 31.0f)));
                }
                else if (inLineIndex == 2)
                {
                    result = NSMutableArray.Array;
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.0f, 1.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.5f, 9.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.0f, 18.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.5f, 40.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(2.0f, 70.0f)));
                }
                else if (inLineIndex == 3)
                {
                    result = NSMutableArray.Array;
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.2f, 40.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.55f, 38.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(0.75f, 36.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.0f, 45.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.3f, 60.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.5f, 56.0f)));
                    result.AddObject(NSPoint.NSStringFromPoint(NSPoint.NSMakePoint(1.8f, 50.0f)));
                }
            }

            return result;
        }

        [ObjectiveCMessage("twoDGraphView:dataObjectForLineIndex:")]
        public NSData twoDGraphViewDataObjectForLineIndex(SM2DGraphView.SM2DGraphView inGraphView, uint inLineIndex)
        {
            NSData result = null;

            if (inGraphView == this._sm_trigGraph)
            {
                if (inLineIndex == 0)
                {
                    result = this._sm_data_sin;
                }
                else if (inLineIndex == 1)
                {
                    result = this._sm_data_cos;
                }
            }

            return result;
        }

        [ObjectiveCMessage("twoDGraphView:maximumValueForLineIndex:forAxis:")]
        public double twoDGraphViewMaximumValueForLineIndexForAxis(SM2DGraphView.SM2DGraphView inGraphView, uint inLineIndex, SM2DGraphAxis inAxis)
        {
            if (inGraphView == this._sm_barGraph)
            {
                if (inAxis == SM2DGraphAxis.kSM2DGraph_Axis_X)
                {
                    return 2.0;
                }
                return 10.0;
            }
            if (inGraphView == this._sm_costTimeGraph)
            {
                if (inAxis == SM2DGraphAxis.kSM2DGraph_Axis_X)
                {
                    return 2.0;
                }
                return 75.0;
            }
            if (inGraphView == this._sm_trigGraph)
            {
                if (inAxis == SM2DGraphAxis.kSM2DGraph_Axis_X)
                {
                    return 360.0;
                }
                return 2.0;
            }

            return 0.0;
        }

        [ObjectiveCMessage("twoDGraphView:minimumValueForLineIndex:forAxis:")]
        public double twoDGraphViewMinimumValueForLineIndexForAxis(SM2DGraphView.SM2DGraphView inGraphView, uint inLineIndex, SM2DGraphAxis inAxis)
        {
            double result;

            //if ( inGraphView == _sm_barGraph )
            //{
            //    if ( inAxis == kSM2DGraph_Axis_Y )
            //        result = -2.0;
            //    else
            //        result = 0.0;
            //}
            //else 
            if (inGraphView == this._sm_trigGraph)
            {
                if (inAxis == SM2DGraphAxis.kSM2DGraph_Axis_X)
                {
                    result = -360.0;
                }
                else
                {
                    result = -2.0;
                }
            }
            else
            {
                result = 0.0;
            }

            return result;
        }

        [ObjectiveCMessage("twoDGraphView:attributesForLineIndex:")]
        public NSDictionary twoDGraphViewAttributesForLineIndex(SM2DGraphView.SM2DGraphView inGraphView, uint inLineIndex)
        {
            NSDictionary result = null;

            if (inGraphView == this._sm_trigGraph && inLineIndex == 1)
            {
                // Make the cosine line red and don't anti-alias it.
                result = NSDictionary.DictionaryWithObjectsAndKeys(NSColor.RedColor, NSAttributedString_AppKitAdditions.NSForegroundColorAttributeName,
                                                                   NSNumber.NumberWithBool(true), SM2DGraphView.SM2DGraphView.SM2DGraphDontAntialiasAttributeName,
                                                                   null);
            }
            else if (inGraphView == this._sm_barGraph && inLineIndex == 0)
            {
                // Make this a bar graph.
                // We could make it blue here if every bar was blue.
                // However, we use the delegate method below to make one of the bars orange, one brown, and the rest blue.
                result = NSDictionary.DictionaryWithObjectsAndKeys(NSNumber.NumberWithBool(true), SM2DGraphView.SM2DGraphView.SM2DGraphBarStyleAttributeName,
                                                                   null);
            }
            else if (inGraphView == this._sm_costTimeGraph && inLineIndex == 3)
            {
                // Make this a black symbols only (scatter plot).
                result = NSDictionary.DictionaryWithObjectsAndKeys(NSNumber.NumberWithInt((int) SM2DGraphSymbolType.kSM2DGraph_Symbol_Diamond), SM2DGraphView.SM2DGraphView.SM2DGraphLineSymbolAttributeName,
                                                                   NSColor.BlackColor, NSAttributedString_AppKitAdditions.NSForegroundColorAttributeName,
                                                                   NSNumber.NumberWithInt((int) SM2DGraphLineWidth.kSM2DGraph_Width_None), SM2DGraphView.SM2DGraphView.SM2DGraphLineWidthAttributeName,
                                                                   null);
            }

            return result;
        }
    }
}