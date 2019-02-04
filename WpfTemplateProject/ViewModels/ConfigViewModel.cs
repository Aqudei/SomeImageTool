using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ImgDiffTool.ViewModels
{
    sealed class ConfigViewModel : Screen
    {
        private string _tifFolder;
        private string _jpegFolder;
        private string _destinationFolder;

        public string TIFFolder
        {
            get => _tifFolder;
            set
            {
                Set(ref _tifFolder, value);
                NotifyOfPropertyChange(nameof(CanApply));
            }
        }

        public string JPEGFolder
        {
            get => _jpegFolder;
            set
            {
                Set(ref _jpegFolder, value);
                NotifyOfPropertyChange(nameof(CanApply));
            }
        }

        public string DestinationFolder
        {
            get => _destinationFolder;
            set
            {
                Set(ref _destinationFolder, value);
                NotifyOfPropertyChange(nameof(CanApply));
            }
        }

        public bool CanApply => !string.IsNullOrEmpty(TIFFolder) &&
                                !string.IsNullOrEmpty(JPEGFolder) &&
                                !string.IsNullOrEmpty(DestinationFolder);

        public ConfigViewModel()
        {
            TIFFolder = Properties.Settings.Default.TIFFolder;
            JPEGFolder = Properties.Settings.Default.JPEGFolder;
            DestinationFolder = Properties.Settings.Default.DestinationFolder;
        }

        public void Apply()
        {
            Properties.Settings.Default.TIFFolder = TIFFolder;
            Properties.Settings.Default.JPEGFolder = JPEGFolder;
            Properties.Settings.Default.DestinationFolder = DestinationFolder;
            Properties.Settings.Default.Save();
            TryClose(true);
        }

        public void Quit()
        {
            TryClose(false);
        }

        private readonly CommonOpenFileDialog _commonOpenFileDialog = new CommonOpenFileDialog
        {
            IsFolderPicker = true,
            Multiselect = false,
            EnsurePathExists = true
        };

        public void BrowseTIF()
        {
            var dialogResult = _commonOpenFileDialog.ShowDialog();
            if (dialogResult != CommonFileDialogResult.Ok) return;
            TIFFolder = _commonOpenFileDialog.FileName;
        }

        public void BrowseDestination()
        {
            var dialogResult = _commonOpenFileDialog.ShowDialog();
            if (dialogResult != CommonFileDialogResult.Ok) return;
            DestinationFolder = _commonOpenFileDialog.FileName;
        }

        public void BrowseJPEG()
        {
            var dialogResult = _commonOpenFileDialog.ShowDialog();
            if (dialogResult != CommonFileDialogResult.Ok) return;
            JPEGFolder = _commonOpenFileDialog.FileName;
        }
    }
}
