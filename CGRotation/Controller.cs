using System;
using Monobjc.Foundation;
using Monobjc.AppKit;
using Monobjc.CoreServices;
using Monobjc.ApplicationServices;

namespace Monobjc.Samples.CGRotation
{
    [ObjectiveCClass]
    public partial class Controller : NSObject
    {
        private static readonly Class ControllerClass = Class.Get(typeof (Controller));

        private float rotation;
        private float scaleX;
        private float scaleY;
        private float translateX;
        private float translateY;
        private bool preserveAspectRatio;
        private NSMutableArray openImageIOSupportedTypes;

        public Controller() {}

        public Controller(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("init")]
        public override Id Init()
        {
            this.NativePointer = this.SendMessageSuper<IntPtr>(ControllerClass, "init");

            this.rotation = 0.0f;
            this.scaleX = 1.0f;
            this.scaleY = 1.0f;
            this.translateX = 0.0f;
            this.translateY = 0.0f;
            this.preserveAspectRatio = false;

            return this;
        }

        [ObjectiveCMessage("awakeFromNib")]
        public void AwakeFromNib()
        {
            this.openImageIOSupportedTypes = null;
            NSURL url = NSURL.FileURLWithPath(NSBundle.MainBundle.PathForImageResource("demo.png"));
            if (url != null)
            {
                this.imageView.Image = ImageUtils.IICreateImage(url);
            }
            this.imageView.Window.Center();
            this.PreserveAspectRatio = false;
            this.ResetTransformations();
        }

        [ObjectiveCMessage("dealloc")]
        public override void Dealloc()
        {
            this.openImageIOSupportedTypes.SafeRelease();
            this.SendMessageSuper(ControllerClass, "dealloc");
        }

        [ObjectiveCMessage("extensionsForUTI:")]
        public NSArray ExtensionsForUTI(NSString uti)
        {
            // If anything goes wrong, we'll return nil, otherwise this will be the array of extensions for this image type.
            NSArray extensions = null;
            // Only get extensions for UTIs that are images (i.e. conforms to public.image aka kUTTypeImage)
            // This excludes PDF support that ImageIO advertises, but won't actually use.
            if (UTType.ConformsTo(uti, UTType.kUTTypeImage))
            {
                // Copy the declaration for the UTI (if it exists)
                NSDictionary declaration = UTType.CopyDeclaration(uti);
                if (declaration != null)
                {
                    // Grab the tags for this UTI, which includes extensions, OSTypes and MIME types.
                    Id specifications = declaration.ValueForKey(UTType.kUTTypeTagSpecificationKey);
                    if (specifications != null)
                    {
                        NSDictionary tags = specifications.CastTo<NSDictionary>();

                        // We are interested specifically in the extensions that this UTI uses
                        Id filenameExtensions = tags.ValueForKey(UTType.kUTTagClassFilenameExtension);
                        if (filenameExtensions != null)
                        {
                            // It is valid for a UTI to export either an Array (of Strings) representing multiple tags,
                            // or a String representing a single tag.
                            if (filenameExtensions.SendMessage<bool>("isKindOfClass:", NSString.NSStringClass))
                            {
                                // If a string was exported, then wrap it up in an array.
                                extensions = NSArray.ArrayWithObject(filenameExtensions);
                            }
                            else if (filenameExtensions.SendMessage<bool>("isKindOfClass:", NSArray.NSArrayClass))
                            {
                                // If an array was exported, then just return that array.
                                extensions = filenameExtensions.CastTo<NSArray>().Copy<NSArray>();
                                extensions.Autorelease();
                            }
                        }
                    }
                    declaration.Release();
                }
            }
            return extensions;
        }

        [ObjectiveCMessage("createOpenTypesArray")]
        public void CreateOpenTypesArray()
        {
            NSArray imageIOUTIs = CGImageSource.CopyTypeIdentifiers();
            uint i;
            uint count = imageIOUTIs.Count;
            this.openImageIOSupportedTypes = new NSMutableArray(count);
            for (i = 0; i < count; ++i)
            {
                this.openImageIOSupportedTypes.AddObjectsFromArray(this.ExtensionsForUTI(imageIOUTIs.ObjectAtIndex(i).CastTo<NSString>()));
            }
            imageIOUTIs.Release();
        }

        [ObjectiveCMessage("openDocument:")]
        public void openDocument(Id sender)
        {
            NSOpenPanel panel = NSOpenPanel.OpenPanel;
            panel.AllowsMultipleSelection = false;
            panel.ResolvesAliases = true;
            panel.TreatsFilePackagesAsDirectories = true;
            panel.Message = "Please choose an image file.";

            this.CreateOpenTypesArray();

            panel.BeginSheetForDirectoryFileTypesModalForWindowModalDelegateDidEndSelectorContextInfo(null,
                                                                                                      null,
                                                                                                      this.openImageIOSupportedTypes,
                                                                                                      this.imageView.Window,
                                                                                                      this.OpenImageDidEndReturnCodeContextInfo,
                                                                                                      IntPtr.Zero);
        }

        public void OpenImageDidEndReturnCodeContextInfo(NSOpenPanel panel, NSInteger returnCode, IntPtr contextInfo)
        {
            if (returnCode == NSPanel.NSOKButton)
            {
                if (panel.Filenames.Count > 0)
                {
                    ImageInfo image = ImageUtils.IICreateImage(NSURL.FileURLWithPath((panel.Filenames.ObjectAtIndex(0).CastTo<NSString>())));
                    this.imageView.Image = image;
                    this.Reset(null);
                }
            }
        }

        partial void SaveDocumentAs(Id sender)
        {
            NSSavePanel panel = NSSavePanel.SavePanel;
            panel.CanSelectHiddenExtension = true;
            panel.RequiredFileType = "jpeg";
            panel.AllowsOtherFileTypes = false;
            panel.TreatsFilePackagesAsDirectories = true;
			
            panel.BeginSheetForDirectoryFileModalForWindowModalDelegateDidEndSelectorContextInfo(null,
                                                                                                 "Untitled",
                                                                                                 this.imageView.Window,
                                                                                                 this.SaveImageDidEnd,
                                                                                                 IntPtr.Zero);
        }

        public void SaveImageDidEnd(NSSavePanel panel, NSInteger returnCode, IntPtr contextInfo)
        {
            if (returnCode == NSPanel.NSOKButton)
            {
                NSRect frame = this.imageView.Frame;
                ImageUtils.IISaveImage(this.imageView, panel.URL, (uint) Math.Ceiling(frame.size.width), (uint) Math.Ceiling(frame.size.height));
            }
        }

        partial void ChangeScaleX(Id sender)
        {
            NSStepper stepper = sender.CastTo<NSStepper>();
            float value = this.ScaleX + stepper.FloatValue;
            this.ScaleX = value;
            stepper.FloatValue = 0.0f;
        }

        partial void ChangeScaleY(Id sender)
        {
            NSStepper stepper = sender.CastTo<NSStepper>();
            float value = this.ScaleY + stepper.FloatValue;
            this.ScaleY = value;
            stepper.FloatValue = 0.0f;
        }

        partial void ChangeTranslateX(Id sender)
        {
            NSStepper stepper = sender.CastTo<NSStepper>();
            float value = this.TranslateX + stepper.FloatValue;
            this.TranslateX = value;
            stepper.FloatValue = 0.0f;
        }

        partial void ChangeTranslateY(Id sender)
        {
            NSStepper stepper = sender.CastTo<NSStepper>();
            float value = this.TranslateY + stepper.FloatValue;
            this.TranslateY = value;
            stepper.FloatValue = 0.0f;
        }

        partial void Reset(Id sender)
        {
            this.ResetTransformations();
            this.imageView.NeedsDisplay = true;
        }

        [ObjectiveCMessage("setScaleX:andY:")]
        public void SetScaleXandY(float x, float y)
        {
            this.ScaleX = x;
            this.imageView.SetScaleX(this.ScaleX);
            this.ScaleY = y;
            if (this.preserveAspectRatio)
            {
                this.imageView.SetScaleY(this.ScaleX);
            }
            else
            {
                this.imageView.SetScaleY(this.ScaleY);
            }
            this.imageView.NeedsDisplay = true;
        }

        [ObjectiveCMessage("setTranslateX:andY:")]
        public void setTranslateXandY(float x, float y)
        {
            this.TranslateX = x;
            this.imageView.SetTranslateX(this.TranslateX);
            this.TranslateY = y;
            this.imageView.SetTranslateY(this.TranslateY);
            this.imageView.NeedsDisplay = true;
        }

        [ObjectiveCMessage("resetTransformations")]
        public void ResetTransformations()
        {
            this.Rotation = 0.0f;
            this.SetScaleXandY(1.0f, 1.0f);
            this.setTranslateXandY(0.0f, 0.0f);
        }

        public float Rotation
        {
            [ObjectiveCMessage("rotation")]
            get { return this.rotation; }
            [ObjectiveCMessage("setRotation:")]
            set
            {
                this.WillChangeValueForKey("rotation");
                if (value >= 360.0f)
                {
                    while (value >= 360.0f)
                    {
                        value -= 360.0f;
                    }
                }
                else if (value < 0.0f)
                {
                    while (value < 0.0f)
                    {
                        value += 360.0f;
                    }
                }
                this.rotation = value;
                this.imageView.SetRotation(360.0f - value);
                this.imageView.NeedsDisplay = true;
                this.DidChangeValueForKey("rotation");
            }
        }

        public float ScaleX
        {
            [ObjectiveCMessage("scaleX")]
            get { return this.scaleX; }
            [ObjectiveCMessage("setScaleX:")]
            set
            {
                this.WillChangeValueForKey("scaleX");
                this.imageView.SetScaleX(this.scaleX = value);
                if (this.preserveAspectRatio)
                {
                    this.imageView.SetScaleY(this.ScaleX);
                }
                this.imageView.NeedsDisplay = true;
                this.DidChangeValueForKey("scaleX");
            }
        }

        public float ScaleY
        {
            [ObjectiveCMessage("scaleY")]
            get { return this.scaleY; }
            [ObjectiveCMessage("setScaleY:")]
            set
            {
                this.WillChangeValueForKey("scaleY");
                this.scaleY = value;
                if (!this.preserveAspectRatio)
                {
                    this.imageView.SetScaleY(this.ScaleY);
                    this.imageView.NeedsDisplay = true;
                }
                this.DidChangeValueForKey("scaleY");
            }
        }

        public bool PreserveAspectRatio
        {
            [ObjectiveCMessage("preserveAspectRatio")]
            get { return this.preserveAspectRatio; }
            [ObjectiveCMessage("setPreserveAspectRatio:")]
            set
            {
                this.WillChangeValueForKey("preserveAspectRatio");
                this.preserveAspectRatio = value;
                this.imageView.SetScaleX(this.ScaleX);
                if (this.preserveAspectRatio)
                {
                    this.imageView.SetScaleY(this.ScaleX);
                }
                else
                {
                    this.imageView.SetScaleY(this.ScaleY);
                }
                this.scaleYView.IsHidden = this.preserveAspectRatio;
                this.textScaleYView.IsHidden = this.preserveAspectRatio;
                this.textLabelXView.IsHidden = this.preserveAspectRatio;
                this.textLabelYView.IsHidden = this.preserveAspectRatio;
                this.imageView.NeedsDisplay = true;
                this.DidChangeValueForKey("preserveAspectRatio");
            }
        }

        public float TranslateX
        {
            [ObjectiveCMessage("translateX")]
            get { return this.translateX; }
            [ObjectiveCMessage("setTranslateX:")]
            set
            {
                this.WillChangeValueForKey("translateX");
                this.imageView.SetTranslateX(this.translateX = value);
                this.imageView.NeedsDisplay = true;
                this.DidChangeValueForKey("translateX");
            }
        }

        public float TranslateY
        {
            [ObjectiveCMessage("translateY")]
            get { return this.translateY; }
            [ObjectiveCMessage("setTranslateY:")]
            set
            {
                this.WillChangeValueForKey("translateY");
                this.imageView.SetTranslateY(this.translateY = value);
                this.imageView.NeedsDisplay = true;
                this.DidChangeValueForKey("translateY");
            }
        }
    }
}