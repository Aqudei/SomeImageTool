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
        private string _destinationFolder;
        private BitmapImage _image1;
        private BitmapImage _image2;

        private int _tifIndex;
        private string[] _tiffs;


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
            _destinationFolder = Properties.Settings.Default.DestinationFolder;

            _tiffs = Directory.GetFiles(_tifFolder, "*.tif", SearchOption.TopDirectoryOnly);
            Execute.OnUIThreadAsync(async () => await UpdateDisplay());
        }

        //public bool CanNext { get; set; }
        //public bool CanPrevious { get; set; }

        private async Task UpdateDisplay()
        {
            if (_tifIndex >= _tiffs.Length || _tifIndex < 0)
                return;

            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Please Wait", "Reading Image File");
            controller.SetIndeterminate();

            try
            {
                var imagePath = _tiffs[_tifIndex];
                Image1 = imagePath.ToBitmapImage();
                Image2 = Path.Combine(_jpegFolder, Path.ChangeExtension(Path.GetFileName(imagePath), ".jpg")).ToBitmapImage();
            }
            finally
            {
                await controller.CloseAsync();
            }
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

        public async Task Move()
        {
            if (_tiffs.Length > 0 && _tifIndex < _tiffs.Length)
            {
                var basename = Path.GetFileName(_tiffs[_tifIndex]);

                var jpeg = Path.Combine(_jpegFolder, Path.ChangeExtension(basename, ".jpg"));
                var eps = Path.Combine(_jpegFolder, Path.ChangeExtension(basename, ".eps"));
                var jpegDestiny = Path.Combine(_destinationFolder, Path.GetFileName(jpeg));
                var epsDestiny = Path.Combine(_destinationFolder, Path.GetFileName(eps));

                if (File.Exists(jpeg))
                {
                    File.Copy(jpeg, jpegDestiny, true);
                }

                if (File.Exists(eps))
                {
                    File.Copy(eps, epsDestiny, true);
                }

                await Next();

                if (File.Exists(jpeg))
                    File.Delete(jpeg);

                if (File.Exists(eps))
                    File.Delete(eps);
            }
        }
    }
}
