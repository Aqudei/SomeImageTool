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
        private string _signatureFolder;

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

        public string SignatureFolder
        {
            get => _signatureFolder;
            set
            {
                Set(ref _signatureFolder, value);
                NotifyOfPropertyChange(nameof(CanApply));
            }
        }

        public string BorderFolder
        {
            get => _borderFolder;
            set
            {
                Set(ref _borderFolder, value);
                NotifyOfPropertyChange(nameof(CanApply));
            }
        }

        public string IssueFolder
        {
            get => _issueFolder;
            set
            {
                Set(ref _issueFolder, value);
                NotifyOfPropertyChange(nameof(CanApply));
            }
        }

        public bool CanApply => !string.IsNullOrEmpty(TIFFolder) &&
                                !string.IsNullOrEmpty(JPEGFolder) &&
                                !string.IsNullOrEmpty(SignatureFolder) &&
                                !string.IsNullOrEmpty(IssueFolder) &&
                                !string.IsNullOrEmpty(BorderFolder);

        public ConfigViewModel()
        {
            TIFFolder = Properties.Settings.Default.TIFFolder;
            JPEGFolder = Properties.Settings.Default.JPEGFolder;
            SignatureFolder = Properties.Settings.Default.SignatureFolder;
            IssueFolder = Properties.Settings.Default.IssueFolder;
            BorderFolder = Properties.Settings.Default.BorderFolder;

            DisplayName = "Configuration";
        }

        public void Apply()
        {
            Properties.Settings.Default.TIFFolder = TIFFolder;
            Properties.Settings.Default.JPEGFolder = JPEGFolder;
            Properties.Settings.Default.SignatureFolder = SignatureFolder;
            Properties.Settings.Default.IssueFolder = IssueFolder;
            Properties.Settings.Default.BorderFolder = BorderFolder;

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

        private string _borderFolder;
        private string _issueFolder;

        public void BrowseTIF()
        {
            var dialogResult = _commonOpenFileDialog.ShowDialog();
            if (dialogResult != CommonFileDialogResult.Ok) return;
            TIFFolder = _commonOpenFileDialog.FileName;
        }

        public void BrowseSignature()
        {
            var dialogResult = _commonOpenFileDialog.ShowDialog();
            if (dialogResult != CommonFileDialogResult.Ok) return;
            SignatureFolder = _commonOpenFileDialog.FileName;
        }

        public void BrowseIssue()
        {
            var dialogResult = _commonOpenFileDialog.ShowDialog();
            if (dialogResult != CommonFileDialogResult.Ok) return;
            IssueFolder = _commonOpenFileDialog.FileName;
        }

        public void BrowseBorder()
        {
            var dialogResult = _commonOpenFileDialog.ShowDialog();
            if (dialogResult != CommonFileDialogResult.Ok) return;
            BorderFolder = _commonOpenFileDialog.FileName;
        }

        public void BrowseJPEG()
        {
            var dialogResult = _commonOpenFileDialog.ShowDialog();
            if (dialogResult != CommonFileDialogResult.Ok) return;
            JPEGFolder = _commonOpenFileDialog.FileName;
        }
    }
}
