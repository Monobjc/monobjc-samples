using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.AnimatingViews
{
    [ObjectiveCClass]
    public partial class SimpleLayoutView : NSView
    {
        private static readonly Class SimpleLayoutViewClass = Class.Get(typeof (SimpleLayoutView));

        private const float SEPARATION = 10.0f;
        private const float BOXWIDTH = 80.0f;
        private const float BOXHEIGHT = 80.0f;

        private LayoutStyle layoutStyle;

        public SimpleLayoutView() {}

        public SimpleLayoutView(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("initialize")]
        public new static void Initialize()
        {
            NSColorPanel.SharedColorPanel.ShowsAlpha = true;
        }

        [ObjectiveCMessage("initWithFrame:")]
        public override Id InitWithFrame(NSRect frame)
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(SimpleLayoutViewClass, "initWithFrame:", frame);
            this.layoutStyle = LayoutStyle.ColumnLayout;
            return this;
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this.SendMessageSuper(SimpleLayoutViewClass, "dealloc");
        }

        [ObjectiveCMessage("setFrameSize:")]
        public override void SetFrameSize(NSSize size)
        {
            this.SendMessageSuper(SimpleLayoutViewClass, "setFrameSize:", size);
            this.Layout();
        }

        partial void AddABox(Id sender)
        {
            this.AddSubview(this.ViewToBeAdded());
            this.Layout();
        }

        partial void RemoveLastBox(Id sender)
        {
            this.Subviews.LastObject.CastTo<NSView>().RemoveFromSuperview();
            this.Layout();
        }

        partial void ChangeLayout(Id sender)
        {
            this.LayoutStyle = (LayoutStyle) sender.SendMessage<int>("selectedTag");
        }

        private NSRect IntegralRect(NSRect rect)
        {
            return this.ConvertRectFromBase(NSRect.NSIntegralRect(this.ConvertRectToBase(rect)));
        }

        private void Layout()
        {
            NSArray subviews = this.Subviews;

            switch (this.LayoutStyle)
            {
                case LayoutStyle.ColumnLayout:
                    {
                        NSPoint curPoint = NSPoint.NSMakePoint(this.Bounds.size.width/2.0f, 0.0f); // Starting point: center bottom of view
                        foreach (NSView subview in subviews)
                        {
                            NSRect frame = NSRect.NSMakeRect(curPoint.x - BOXWIDTH/2.0f, curPoint.y, BOXWIDTH, BOXHEIGHT); // Centered horizontally, stacked higher
                            subview.Animator.Frame = this.IntegralRect(frame);
                            curPoint.y += frame.size.height + SEPARATION; // Next view location; we're stacking higher
                        }
                    }
                    break;
                case LayoutStyle.RowLayout:
                    {
                        NSPoint curPoint = NSPoint.NSMakePoint(0.0f, this.Bounds.size.width/2.0f); // Starting point: center left edge of view
                        foreach (NSView subview in subviews)
                        {
                            NSRect frame = NSRect.NSMakeRect(curPoint.x, curPoint.y - BOXHEIGHT/2.0f, BOXWIDTH, BOXHEIGHT); // Centered vertically, stacked left to right
                            subview.Animator.Frame = this.IntegralRect(frame);
                            curPoint.x += frame.size.width + SEPARATION; // Next view location
                        }
                    }
                    break;
                case LayoutStyle.GridLayout:
                    {
                        int viewsPerSide = Convert.ToInt32(Math.Ceiling(Math.Sqrt((uint)subviews.Count))); // Put the views in a roughly square grid
                        int index = 0;
                        NSPoint curPoint = NSPoint.NSZeroPoint; // Starting at the bottom left corner
                        foreach (NSView subview in subviews)
                        {
                            NSRect frame = NSRect.NSMakeRect(curPoint.x, curPoint.y, BOXWIDTH, BOXHEIGHT);
                            subview.Animator.Frame = this.IntegralRect(frame);
                            curPoint.x += BOXWIDTH + SEPARATION; // Stack them horizontally
                            if ((++index)%viewsPerSide == 0)
                            {
                                // And if we have enough on this row, move up to the next
                                curPoint.x = 0;
                                curPoint.y += BOXHEIGHT + SEPARATION;
                            }
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private NSView ViewToBeAdded()
        {
            NSBox box = new NSBox(NSRect.NSMakeRect(0.0f, 0.0f, BOXWIDTH, BOXHEIGHT));
            box.BoxType = NSBoxType.NSBoxCustom;
            box.BorderType = NSBorderType.NSLineBorder;
            box.TitlePosition = NSTitlePosition.NSNoTitle;
            box.FillColor = this.boxColorField.Color;
            box.Autorelease();
            return box;
        }

        private LayoutStyle LayoutStyle
        {
            get { return this.layoutStyle; }
            set
            {
                if (value != this.layoutStyle)
                {
                    this.layoutStyle = value;
                    this.Layout();
                }
            }
        }
    }
}