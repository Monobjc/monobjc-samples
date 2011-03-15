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
using System;
using System.Runtime.InteropServices;
using Monobjc.AppKit;
using Monobjc.Foundation;
using Monobjc.SM2DGraphView;

namespace Monobjc.Samples.SM2DGraphing
{
    partial class SM2DGraphAppDelegate
    {
        [ObjectiveCMessage("twoDGraphView:labelForTickMarkIndex:forAxis:defaultLabel:")]
        public NSString twoDGraphViewLabelForTickMarkIndexForAxisDefaultLabel(SM2DGraphView.SM2DGraphView inGraphView, uint inTickMarkIndex, SM2DGraphAxis inAxis, NSString inDefault)
        {
            NSString result = inDefault;

            if (inGraphView == this._sm_barGraph && inAxis == SM2DGraphAxis.kSM2DGraph_Axis_X)
            {
                if (inTickMarkIndex == 0)
                {
                    result = "Simplicity";
                }
                else if (inTickMarkIndex == 1)
                {
                    result = "Coolness";
                }
                else if (inTickMarkIndex == 2)
                {
                    result = "Fun";
                }
            }

            return result;
        }

        [ObjectiveCMessage("twoDGraphView:willDisplayBarIndex:forLineIndex:withAttributes:")]
        public void twoDGraphViewWillDisplayBarIndexForLineIndexWithAttributes(SM2DGraphView.SM2DGraphView inGraphView, uint inBarIndex, uint inLineIndex, NSMutableDictionary attr)
        {
            if (inGraphView == this._sm_barGraph)
            {
                if (1 == inBarIndex)
                {
                    // Make the second (zero based index of 1) bar orange.
                    attr.SetObjectForKey(NSColor.OrangeColor.BlendedColorWithFractionOfColor(0.7f, NSColor.BlackColor), NSAttributedString_AppKitAdditions.NSForegroundColorAttributeName);
                }
                else if (2 == inBarIndex)
                {
                    // Make the third bar brown.
                    attr.SetObjectForKey(NSColor.BrownColor, NSAttributedString_AppKitAdditions.NSForegroundColorAttributeName);
                }
                else
                {
                    // Make the rest of the bars blue.
                    attr.SetObjectForKey(NSColor.BlueColor, NSAttributedString_AppKitAdditions.NSForegroundColorAttributeName);
                }
            }
        }

        [ObjectiveCMessage("twoDGraphView:didClickPoint:")]
        public void twoDGraphViewDidClickPoint(SM2DGraphView.SM2DGraphView inGraphView, NSPoint inPoint)
        {
            NSPoint testPoint;

            // TODO: Very ugly, so SM2DGraphing needs a good revamp...
            IntPtr testPointBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NSPoint)));
            uint size = (uint) Marshal.SizeOf(typeof (NSPoint));

            if (inGraphView == this._sm_trigGraph)
            {
                // In this case the line index doesn't matter because both lines have the same scale.
                inPoint = this._sm_trigGraph.ConvertPointFromViewToLineIndex(inPoint, this._sm_trigGraph, 0);

                // Quick search through the data array for the closest point.
                uint index = 0;
                uint max = (this._sm_data_sin.Length/size) - 1;
                uint min = 0;
                while ( max > min )
                {
                    index = ( max + min ) / 2;

                    // TODO: Very ugly, so SM2DGraphing needs a good revamp...
                    this._sm_data_sin.GetBytesRange(testPointBuffer, new NSRange(size * index, size));
                    testPoint = (NSPoint) Marshal.PtrToStructure(testPointBuffer, typeof(NSPoint));

                    if ( inPoint.x < testPoint.x )
                        max = index - 1;
                    else if ( inPoint.x > testPoint.x )
                        min = index + 1;
                    else
                        break;
                }
                if (max == min)
                    index = max;

                // Make a string with data point information.
                this._sm_data_sin.GetBytesRange(testPointBuffer, new NSRange(size * index, size));
                // TODO: Very ugly, so SM2DGraphing needs a good revamp...
                testPoint = (NSPoint)Marshal.PtrToStructure(testPointBuffer, typeof(NSPoint));
                NSString tempString = String.Format("Angle = {0:F3}; sin = {1:F3};", testPoint.x, testPoint.y);

                this._sm_data_cos.GetBytesRange(testPointBuffer, new NSRange(size * index, size));
                // TODO: Very ugly, so SM2DGraphing needs a good revamp...
                testPoint = (NSPoint)Marshal.PtrToStructure(testPointBuffer, typeof(NSPoint));
                tempString += String.Format(" cos = {0:F3}", testPoint.y);

                // Set that string into the text field.
                this._sm_clickInfo.StringValue = tempString;
            }

            // TODO: Very ugly, so SM2DGraphing needs a good revamp...
            Marshal.FreeHGlobal(testPointBuffer);
        }

        [ObjectiveCMessage("twoDGraphView:doneDrawingLineIndex:")]
        public void twoDGraphViewDoneDrawingLineIndex(SM2DGraphView.SM2DGraphView inGraphView, uint inLineIndex)
        {
            // This is just an example of what you could do...
            //if (inGraphView == this._sm_trigGraph)
            //{
            //    Console.WriteLine("We're done drawing the sine/cosine line number " + inLineIndex);
            //}
        }
    }
}