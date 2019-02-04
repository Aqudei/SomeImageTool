using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Xceed.Words.NET;

namespace ImgDiffTool.ViewModels
{
    sealed class ImgDiffViewModel : Screen
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private string _tifFolder;
        private string _jpegFolder;
        private string _signatureFolder;
        private BitmapImage _image1;
        private BitmapImage _image2;

        private int _tifIndex;
        private string[] _tiffs;
        private string _issueFolder;
        private string _borderFolder;
        private string _filename1;
        private string _filename2;


        public BitmapImage Image1
        {
            get => _image1;
            set => Set(ref _image1, value);
        }

        public BitmapImage Image2
        {
            get => _image2;
            set => Set(ref _image2, value);
        }

        public ImgDiffViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
        }

        protected override void OnViewReady(object view)
        {
            _tifFolder = Properties.Settings.Default.TIFFolder;
            _jpegFolder = Properties.Settings.Default.JPEGFolder;
            _signatureFolder = Properties.Settings.Default.SignatureFolder;
            _issueFolder = Properties.Settings.Default.IssueFolder;
            _borderFolder = Properties.Settings.Default.BorderFolder;

            _tiffs = Directory.GetFiles(_tifFolder, "*.tif", SearchOption.TopDirectoryOnly);
            Execute.OnUIThreadAsync(async () => await UpdateDisplay());
        }

        //public bool CanNext { get; set; }
        //public bool CanPrevious { get; set; }

        private async Task UpdateDisplay()
        {
            if (_tifIndex >= _tiffs.Length || _tifIndex < 0)
                return;

            try
            {
                await Execute.OnUIThreadAsync(() =>
                 {
                     var imagePath = _tiffs[_tifIndex];
                     Filename1 = Path.Combine(_jpegFolder, Path.ChangeExtension(Path.GetFileName(imagePath), ".jpg"));
                     Image1 = Filename1.ToBitmapImage();

                     Filename2 = imagePath;
                     Image2 = Filename2.ToBitmapImage();
                 });
            }
            catch
            {
                // ignored
            }
        }

        public string Filename2
        {
            get => _filename2;
            set => Set(ref _filename2, value);
        }

        public string Filename1
        {
            get => _filename1;
            set => Set(ref _filename1, value);
        }

        public async Task Next()
        {
            _tifIndex++;
            if (_tifIndex == _tiffs.Length)
                _tifIndex--;

            await UpdateDisplay();
        }

        public async Task Previous()
        {
            _tifIndex--;
            if (_tifIndex < 0)
                _tifIndex = 0;

            await UpdateDisplay();
        }

        private async Task CopyFiles(string tifSource, string destination)
        {
            if (!Directory.Exists(destination))
            {
                throw new DirectoryNotFoundException($"{destination} was not found");
            }

            var basename = Path.GetFileName(tifSource);
            var jpeg = Path.Combine(_jpegFolder, Path.ChangeExtension(basename, ".jpg"));
            var eps = Path.Combine(_jpegFolder, Path.ChangeExtension(basename, ".eps"));

            var jpegDestiny = Path.Combine(destination, Path.GetFileName(jpeg));
            var epsDestiny = Path.Combine(destination, Path.GetFileName(eps));

            var tifDestiny = Path.Combine(destination, Path.GetFileName(tifSource));

            if (File.Exists(jpeg))
            {
                File.Copy(jpeg, jpegDestiny, true);
            }

            if (File.Exists(eps))
            {
                File.Copy(eps, epsDestiny, true);
            }

            File.Copy(tifSource, tifDestiny, true);

            await Next();

            if (File.Exists(jpeg))
                File.Delete(jpeg);

            if (File.Exists(eps))
                File.Delete(eps);

            if (File.Exists(tifSource))
                File.Delete(tifSource);
        }

        public async Task MoveSignature()
        {
            try
            {
                if (_tiffs.Length > 0 && _tifIndex < _tiffs.Length)
                {
                    await CopyFiles(_tiffs[_tifIndex], _signatureFolder);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                await _dialogCoordinator.ShowMessageAsync(this, "Something went wrong", e.Message);
            }
        }

        public async Task MoveIssue()
        {
            try
            {
                if (_tiffs.Length > 0 && _tifIndex < _tiffs.Length)
                {
                    await CopyFiles(_tiffs[_tifIndex], _issueFolder);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                await _dialogCoordinator.ShowMessageAsync(this, "Something went wrong", e.Message);
            }
        }

        public async Task MoveBorder()
        {
            try
            {
                if (_tiffs.Length > 0 && _tifIndex < _tiffs.Length)
                {
                    await CopyFiles(_tiffs[_tifIndex], _borderFolder);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                await _dialogCoordinator.ShowMessageAsync(this, "Something went wrong", e.Message);
            }
        }
    }
}
