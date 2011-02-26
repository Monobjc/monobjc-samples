using System;
using Monobjc.AppKit;
using Monobjc.DiscRecording;
using Monobjc.DiscRecordingUI;
using Monobjc.Foundation;

namespace Monobjc.Samples.ContentBurn
{
    [ObjectiveCClass]
    public partial class AppController : NSObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppController"/> class.
        /// </summary>
        public AppController() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AppController"/> class.
        /// </summary>
        /// <param name="nativePointer">The native pointer.</param>
        public AppController(IntPtr nativePointer)
            : base(nativePointer) {}

        [ObjectiveCMessage("applicationDidFinishLaunching:")]
        public void ApplicationDidFinishLaunching(NSNotification notification)
        {
            // Gets the DRBurn icon and assign it another name
            NSImage icon = NSImage.ImageNamed(DiscRecordingUIFramework.DRBurnIcon);
            icon.SetName("OldDRBurnIcon");

            // Gets the Monobjc icon and assign it as default icon
            icon = new NSImage(NSBundle.MainBundle.PathForImageResource("Monobjc.icns"));
            icon.SetName(DiscRecordingUIFramework.DRBurnIcon);
        }

        public NSArray SelectAudioFiles()
        {
            NSOpenPanel op = NSOpenPanel.OpenPanel;

            op.CanChooseDirectories = false;
            op.CanChooseFiles = true;
            op.AllowsMultipleSelection = true;

            op.Title = "Select audio files to burn";
            op.Prompt = "Select";

            if (op.RunModalForTypes(null) == NSPanel.NSOKButton)
            {
                return op.Filenames;
            }
            return null;
        }

        public NSArray SelectDataFiles()
        {
            NSOpenPanel op = NSOpenPanel.OpenPanel;

            op.CanChooseDirectories = true;
            op.CanChooseFiles = true;
            op.AllowsMultipleSelection = true;

            op.Title = "Select files and folder to burn";
            op.Prompt = "Select";

            if (op.RunModalForTypes(null) == NSPanel.NSOKButton)
            {
                return op.Filenames;
            }
            return null;
        }

        public void BurnLayoutWithDescription(Id layout, NSString desc)
        {
            DRBurnSetupPanel bsp = DRBurnSetupPanel.SetupPanel;

            if (bsp.RunSetupPanel() == NSPanel.NSOKButton)
            {
                DRBurnProgressPanel bpp = DRBurnProgressPanel.ProgressPanel;

                bpp.Description = desc;
                bpp.BeginProgressPanelForBurnLayout(bsp.BurnObject, layout);
            }
        }

        partial void CreateAudioDisc(Id sender)
        {
            NSArray files = this.SelectAudioFiles();
            if (files != null)
            {
                NSMutableArray trackList = new NSMutableArray(files.Count);
                foreach (NSString filepath in files.GetEnumerator<NSString>())
                {
                    trackList.AddObject(DRTrack.TrackForAudioFile(filepath));
                }
                this.BurnLayoutWithDescription(trackList, "Burning audio disc");
            }
        }

        partial void CreateDataDisc(Id sender)
        {
            NSArray files = this.SelectDataFiles();
            if (files != null)
            {
                DRFolder discRoot = DRFolder.VirtualFolderWithName("My Stuff");
                foreach (NSString filepath in files.GetEnumerator<NSString>())
                {
                    bool isDir = false;

                    if (NSFileManager.DefaultManager.FileExistsAtPathIsDirectory(filepath, out isDir))
                    {
                        DRFSObject fsObj;

                        if (isDir)
                        {
                            fsObj = DRFolder.FolderWithPath(filepath);
                        }
                        else
                        {
                            fsObj = DRFile.FileWithPath(filepath);
                        }

                        discRoot.AddChild(fsObj);
                    }
                }

                discRoot.ExplicitFilesystemMask = DRFilesystemInclusionMask.DRFilesystemInclusionMaskHFSPlus;
                this.BurnLayoutWithDescription(DRTrack.TrackForRootFolder(discRoot), "Burning data disc");
            }
        }
    }
}