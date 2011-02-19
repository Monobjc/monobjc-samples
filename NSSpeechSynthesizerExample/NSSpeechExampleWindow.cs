using System;
using Monobjc.AppKit;
using Monobjc.Foundation;

namespace Monobjc.Samples.NSSpeechSynthesizerExample
{
    [ObjectiveCClass]
    public partial class NSSpeechExampleWindow : NSWindow
    {
        private const UInt32 kNumOfFixedMenuItemsInVoicePopup = 2;

        private uint _offsetToSpokenText;
        private NSRange _orgSelectionRange;

        private NSSpeechSynthesizer _speechSynthesizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NSSpeechExampleWindow"/> class.
        /// </summary>
        public NSSpeechExampleWindow() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NSSpeechExampleWindow"/> class.
        /// </summary>
        /// <param name="nativeObject">The native object.</param>
        public NSSpeechExampleWindow(IntPtr nativeObject)
            : base(nativeObject) {}

        /// <summary>
        /// qsdqsd
        /// </summary>
        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            this._speechSynthesizer = new NSSpeechSynthesizer();
            this._speechSynthesizer.SetDelegate(d =>
                                                    {
                                                        d.SpeechSynthesizerWillSpeakPhoneme += this.SpeechSynthesizerWillSpeakPhoneme;
                                                        d.SpeechSynthesizerDidFinishSpeaking += this.SpeechSynthesizerDidFinishSpeaking;
                                                        d.SpeechSynthesizerWillSpeakWordOfString += this.SpeechSynthesizerWillSpeakWordOfString;
                                                    });
            this._characterView.SetExpression(SpeakingCharacterView.kCharacterExpressionIdentifierIdle);
            this.GetSpeechVoices();
        }

        partial void SpeakTextButtonSelected(Id sender)
        {
            this.StartSpeakingTextViewToURL(null);
        }

        public void SpeechSynthesizerWillSpeakPhoneme(NSSpeechSynthesizer sender, short phonemeOpcode)
        {
            this._characterView.SetExpressionForPhoneme(NSNumber.NumberWithShort(phonemeOpcode));
        }

        public void SpeechSynthesizerDidFinishSpeaking(NSSpeechSynthesizer sender, bool finishedSpeaking)
        {
            this._textToSpeechExampleTextView.SelectedRange = this._orgSelectionRange; // Set selection length to zero.
            this._textToSpeechExampleSpeakButton.Title = FoundationFramework.NSLocalizedString("Start Speaking", "Speaking button name (start)");
            this._saveButton.Title = FoundationFramework.NSLocalizedString("Save As File...", "Save button title");
            this._textToSpeechExampleSpeakButton.IsEnabled = true;
            this._saveButton.IsEnabled = true;
            this._voicePop.IsEnabled = true;
        }

        public void SpeechSynthesizerWillSpeakWordOfString(NSSpeechSynthesizer sender, NSRange characterRange, NSString str)
        {
            UInt32 selectionPosition = characterRange.location + this._offsetToSpokenText;
            UInt32 wordLength = characterRange.length;

            this._textToSpeechExampleTextView.ScrollRangeToVisible(NSRange.NSMakeRange(selectionPosition, wordLength));
            this._textToSpeechExampleTextView.SelectedRange = NSRange.NSMakeRange(selectionPosition, wordLength);
            this._textToSpeechExampleTextView.Display();
        }

        partial void SavetButtonSelected(Id sender)
        {
            if (this._speechSynthesizer.IsSpeaking)
            {
                this._speechSynthesizer.StopSpeaking();
            }
            else
            {
                NSSavePanel theSavePanel = NSSavePanel.SavePanel;
                theSavePanel.Prompt = FoundationFramework.NSLocalizedString("Save", "Save button name");
                if (theSavePanel.RunModalForDirectoryFile(null, FoundationFramework.NSLocalizedString("Synthesized Speech.aiff", "Default save filename")) == NSPanel.NSOKButton)
                {
                    NSURL selectedFileURL = theSavePanel.URL;
                    this.StartSpeakingTextViewToURL(selectedFileURL);
                }
            }
        }

        [ObjectiveCMessage("startSpeakingTextViewToURL:")]
        public void StartSpeakingTextViewToURL(NSURL url)
        {
            if (this._speechSynthesizer.IsSpeaking)
            {
                this._speechSynthesizer.StopSpeaking();
            }
            else
            {
                // Grab the selection substring, or if no selection then grab entire text.
                this._orgSelectionRange = this._textToSpeechExampleTextView.SelectedRange;

                NSString theViewText;
                if (this._orgSelectionRange.length == 0)
                {
                    theViewText = this._textToSpeechExampleTextView.String;
                    this._offsetToSpokenText = 0;
                }
                else
                {
                    theViewText = this._textToSpeechExampleTextView.String.SubstringWithRange(this._orgSelectionRange);
                    this._offsetToSpokenText = this._orgSelectionRange.location;
                }

                if (this._voicePop.IndexOfSelectedItem == 0)
                {
                    // Pass NULL as the voice to use the system voice.
                    this._speechSynthesizer.Voice = null;
                }
                else
                {
                    this._speechSynthesizer.Voice = NSSpeechSynthesizer.AvailableVoices.ObjectAtIndex((uint) this._voicePop.IndexOfSelectedItem - kNumOfFixedMenuItemsInVoicePopup).CastTo<NSString>();
                }

                if (url != null)
                {
                    this._speechSynthesizer.StartSpeakingStringToURL(theViewText, url);
                    this._textToSpeechExampleSpeakButton.IsEnabled = false;
                    this._saveButton.Title = FoundationFramework.NSLocalizedString("Stop Saving", "Save file button name (stop)");
                }
                else
                {
                    this._speechSynthesizer.StartSpeakingString(theViewText);
                    this._textToSpeechExampleSpeakButton.Title = FoundationFramework.NSLocalizedString("Stop Speaking", "Speaking button name (stop)");
                    this._saveButton.IsEnabled = false;
                }
                this._voicePop.IsEnabled = false;
            }
        }

        public void GetSpeechVoices()
        {
            // Delete any items in the voice menu
            while (this._voicePop.NumberOfItems > kNumOfFixedMenuItemsInVoicePopup)
            {
                this._voicePop.RemoveItemAtIndex(this._voicePop.NumberOfItems - 1);
            }

            NSArray voices = NSSpeechSynthesizer.AvailableVoices;
            foreach (NSString identifier in voices.GetEnumerator<NSString>())
            {
                NSDictionary dictionaryOfVoiceAttributes = NSSpeechSynthesizer.AttributesForVoice(identifier);
                NSString voiceDisplayName = dictionaryOfVoiceAttributes[NSSpeechSynthesizer.NSVoiceName].CastTo<NSString>();
                this._voicePop.AddItemWithTitle(voiceDisplayName);
            }
        }
    }
}