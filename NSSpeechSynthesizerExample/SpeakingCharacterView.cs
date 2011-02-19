using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.NSSpeechSynthesizerExample
{
    [ObjectiveCClass]
    public class SpeakingCharacterView : NSView
    {
        // Expression Identifiers
        public static readonly NSString kCharacterExpressionIdentifierSleep = NSString.NSPinnedString("ExpressionIdentifierSleep");
        public static readonly NSString kCharacterExpressionIdentifierIdle = NSString.NSPinnedString("ExpressionIdentifierIdle");
        public static readonly NSString kCharacterExpressionIdentifierConsonant = NSString.NSPinnedString("ExpressionIdentifierConsonant");
        public static readonly NSString kCharacterExpressionIdentifierVowel = NSString.NSPinnedString("ExpressionIdentifierVowel");

        // Frame dictionary keys
        private static readonly NSString kCharacterExpressionFrameDurationKey = NSString.NSPinnedString("FrameDuration");
        private static readonly NSString kCharacterExpressionFrameImageFileNameKey = NSString.NSPinnedString("FrameImageFileName");

        private static readonly Class SpeakingCharacterViewClass = Class.Get(typeof (SpeakingCharacterView));

        private NSString _currentExpression;
        private NSTimer _idleStartTimer;
        private NSTimer _expressionFrameTimer;
        private uint _curFrameIndex;
        private NSArray _curFrameArray;
        private NSImage _curFrameImage;
        private NSDictionary _characterDescription;
        private NSMutableDictionary _imageCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeakingCharacterView"/> class.
        /// </summary>
        public SpeakingCharacterView() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeakingCharacterView"/> class.
        /// </summary>
        /// <param name="nativeObject">The native object.</param>
        public SpeakingCharacterView(IntPtr nativeObject)
            : base(nativeObject) {}

        [ObjectiveCMessage("initWithFrame:")]
        public override Id InitWithFrame(NSRect frame)
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(SpeakingCharacterViewClass, "initWithFrame:", frame);

            this.LoadChacaterByName("Buster");
            this.SetExpression(kCharacterExpressionIdentifierSleep);

            this.RegisterForDraggedTypes(NSImage.ImagePasteboardTypes);

            return this;
        }

        [ObjectiveCMessage("drawRect:")]
        public override void DrawRect(NSRect rect)
        {
            NSPoint thePointToDraw;
            NSSize sourceSize = this._curFrameImage.Size;
            NSSize destSize = rect.size;

            if (destSize.width >= sourceSize.width)
            {
                thePointToDraw.x = (destSize.width - sourceSize.width)/2;
            }
            else
            {
                thePointToDraw.x = 0;
            }

            if (destSize.height >= sourceSize.height)
            {
                thePointToDraw.y = (destSize.height - sourceSize.height)/2;
            }
            else
            {
                thePointToDraw.y = 0;
            }

            this._curFrameImage.CompositeToPointOperationFraction(thePointToDraw, NSCompositingOperation.NSCompositeSourceOver, 1.0f);
        }

        [ObjectiveCMessage("setExpressionForPhoneme:")]
        public void SetExpressionForPhoneme(NSNumber phoneme)
        {
            int phonemeValue = phoneme.ShortValue;

            if (phonemeValue == 0 || phonemeValue == 1)
            {
                this.SetExpression(kCharacterExpressionIdentifierIdle);
            }
            else if (phonemeValue >= 2 && phonemeValue <= 17)
            {
                this.SetExpression(kCharacterExpressionIdentifierVowel);
            }
            else
            {
                this.SetExpression(kCharacterExpressionIdentifierConsonant);
            }
        }

        [ObjectiveCMessage("setExpression:")]
        public void SetExpression(NSString expression)
        {
            // Set up to begin animating the frames
            if (this._expressionFrameTimer != null)
            {
                this._expressionFrameTimer.Invalidate();
                this._expressionFrameTimer = null;
            }
            this._currentExpression.SafeRelease();
            this._currentExpression = expression.SafeRetain();
            this._curFrameArray = this._characterDescription[this._currentExpression].CastTo<NSArray>();
            this._curFrameIndex = 0;
            this.AnimateNextExpressionFrame();

            // If the expression we just set is NOT the idle or sleep expression, then set up the idle start timer.
            if (! (expression.IsEqualToString(kCharacterExpressionIdentifierIdle) ||
                   expression.IsEqualToString(kCharacterExpressionIdentifierSleep)))
            {
                if (this._idleStartTimer != null)
                {
                    this._idleStartTimer.Invalidate();
                }
                this._idleStartTimer = NSTimer.ScheduledTimerWithTimeIntervalTargetSelectorUserInfoRepeats(0.5f,
                                                                                                           this,
                                                                                                           ObjectiveCRuntime.Selector("startIdleExpression"),
                                                                                                           null,
                                                                                                           false);
            }
            else
            {
                if (this._idleStartTimer != null)
                {
                    this._idleStartTimer.Invalidate();
                    this._idleStartTimer = null;
                }
            }
        }

        [ObjectiveCMessage("animateNextExpressionFrame")]
        public void AnimateNextExpressionFrame()
        {
            this._expressionFrameTimer = null;

            NSDictionary frameDictionary = this._curFrameArray.ObjectAtIndex(this._curFrameIndex).CastTo<NSDictionary>();

            // Grab image and force draw.  Use cache to reduce disk hits
            NSString frameImageName = frameDictionary[kCharacterExpressionFrameImageFileNameKey].CastTo<NSString>();
            Id imageName = this._imageCache[frameImageName];
            if (imageName != null)
            {
                this._curFrameImage = imageName.CastTo<NSImage>();
            }
            else
            {
                this._curFrameImage = new NSImage(NSBundle.MainBundle.PathForResourceOfType(frameImageName, NSString.Empty));
                this._imageCache[frameImageName] = this._curFrameImage;
                this._curFrameImage.Release();
            }
            this.Display();

            // If there is more than one frame, then schedule drawing of the next and increment our frame index.
            if (this._curFrameArray.Count > 1)
            {
                this._curFrameIndex++;
                this._curFrameIndex %= this._curFrameArray.Count;
                this._expressionFrameTimer = NSTimer.ScheduledTimerWithTimeIntervalTargetSelectorUserInfoRepeats(frameDictionary[kCharacterExpressionFrameDurationKey].CastTo<NSNumber>().FloatValue,
                                                                                                                 this,
                                                                                                                 ObjectiveCRuntime.Selector("animateNextExpressionFrame"),
                                                                                                                 null,
                                                                                                                 false);
            }
        }

        [ObjectiveCMessage("startIdleExpression")]
        public void StartIdleExpression()
        {
            this._idleStartTimer = null;
            this.SetExpression(kCharacterExpressionIdentifierIdle);
        }

        public void LoadChacaterByName(String name)
        {
            this._imageCache.SafeRelease();
            this._characterDescription.SafeRelease();

            this._imageCache = new NSMutableDictionary();
            this._characterDescription = new NSDictionary(NSBundle.MainBundle.PathForResourceOfType(name, "plist"));
        }
    }
}