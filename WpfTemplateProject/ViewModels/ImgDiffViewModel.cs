using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ImgDiffTool.Models;
using ImgDiffTool.Properties;
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
        private string _issueFolder;
        private string _borderFolder;
        private string _filename1;
        private string _filename2;
        private string _stretch1 = "Uniform";
        private string _stretch2 = "Uniform";


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

            Stretches.AddRange(Enum.GetNames(typeof(Stretch)));
        }

        protected override async void OnViewReady(object view)
        {
            _tifFolder = Properties.Settings.Default.TIFFolder;
            _jpegFolder = Properties.Settings.Default.JPEGFolder;
            _signatureFolder = Properties.Settings.Default.SignatureFolder;
            _issueFolder = Properties.Settings.Default.IssueFolder;
            _borderFolder = Properties.Settings.Default.BorderFolder;

            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Please wait", "Loading images...");
            try
            {
                controller.SetIndeterminate();
                await LoadImages();
                await UpdateDisplay();
            }
            finally
            {
                await controller.CloseAsync();
            }
        }

        //private async Task LoadImages()
        //{
        //    await Task.Run(async () =>
        //    {
        //        _tiffs = Directory.GetFiles(_tifFolder, "*.tif", SearchOption.TopDirectoryOnly);
        //        Array.Sort(_tiffs);

        //        var lastViewed = Properties.Settings.Default.LastViewedImage;
        //        if (string.IsNullOrWhiteSpace(lastViewed))
        //        {
        //            _tifIndex = 0;
        //        }
        //        else
        //        {
        //            var index = Array.IndexOf(_tiffs, lastViewed);
        //            if (index < 0)
        //            {
        //                _tifIndex = 0;
        //            }
        //            else
        //            {
        //                var result = await _dialogCoordinator.ShowMessageAsync(this,
        //                    "Please choose", $"Do you want to continue viewing from {lastViewed}",
        //                    MessageDialogStyle.AffirmativeAndNegative);
        //                _tifIndex = result == MessageDialogResult.Affirmative ? index : 0;
        //            }
        //        }
        //    });
        //}

        private async Task LoadImages()
        {
            await Task.Run(async () =>
            {
                var result = await _dialogCoordinator.ShowMessageAsync(this, "Please confirm", "Data already exist in the database. Do you want to use this existing data?",
                    MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Negative)
                {
                    using (var db = new ImageDiffContext())
                    {
                        if (db.MyImages.Any())
                        {
                            db.MyImages.RemoveRange(db.MyImages.ToList());
                            db.SaveChanges();
                        }
                    }

                    var myImageIndex = 0;
                    using (var db = new ImageDiffContext())
                    {
                        foreach (var tif in Directory.EnumerateFiles(_tifFolder, "*.tif", SearchOption.TopDirectoryOnly))
                        {
                            db.MyImages.Add(new MyImage
                            {
                                Filename = tif,
                                Order = myImageIndex
                            });
                            myImageIndex++;
                        }

                        db.SaveChanges();
                        _tifIndex = 0;
                    }
                }
                else
                {
                    using (var db = new ImageDiffContext())
                    {
                        var lastTiff = db.MyImages
                            .FirstOrDefault(i => i.Filename == Settings.Default.LastViewedImage);
                        _tifIndex = lastTiff?.Order ?? 0;
                    }
                }
            });
        }

        //public bool CanNext { get; set; }
        //public bool CanPrevious { get; set; }

        private int CountTiffs()
        {
            using (var db = new ImageDiffContext())
            {
                return db.MyImages.Count();
            }
        }

        private async Task UpdateDisplay()
        {
            using (var db = new ImageDiffContext())
            {
                var tiff = db.MyImages.FirstOrDefault(i => i.Order == _tifIndex && !i.Untracked);
                if (tiff == null)
                {
                    return;
                }

                try
                {
                    await Execute.OnUIThreadAsync(() =>
                    {
                        var imagePath = tiff.Filename;
                        Properties.Settings.Default.LastViewedImage = imagePath;
                        Properties.Settings.Default.Save();
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
            using (var db = new ImageDiffContext())
            {
                var tif = db.MyImages.OrderBy(i => i.Order)
                    .FirstOrDefault(i => i.Order > _tifIndex && !i.Untracked);
                if (tif == null)
                    return;

                _tifIndex = tif.Order;
                await UpdateDisplay();
            }
        }

        public IEnumerable<IResult> Previous()
        {
            yield return Task.Run(async () =>
            {
                using (var db = new ImageDiffContext())
                {
                    var tif = db.MyImages.OrderByDescending(i => i.Order)
                        .FirstOrDefault(i => i.Order < _tifIndex && !i.Untracked);
                    if (tif == null)
                        return;

                    _tifIndex = tif.Order;
                    await UpdateDisplay();
                }

            }).AsResult();

        }

        public List<string> Stretches { get; set; } = new List<string>();

        public string Stretch1
        {
            get => _stretch1;
            set => Set(ref _stretch1, value);
        }

        public string Stretch2
        {
            get => _stretch2;
            set => Set(ref _stretch2, value);
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
            {
                //SetUntracked(jpeg);
                File.Delete(jpeg);
            }


            if (File.Exists(eps))
            {
                //SetUntracked(eps);
                File.Delete(eps);
            }

            if (File.Exists(tifSource))
            {
                SetUntracked(tifSource);
                File.Delete(tifSource);
            }

        }

        private void SetUntracked(string tifSource)
        {
            using (var db = new ImageDiffContext())
            {
                var img = db.MyImages.FirstOrDefault(i => i.Filename == tifSource);
                if (img != null)
                {
                    img.Untracked = true;
                }

                db.Entry(img).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        private MyImage GetTiff(int order)
        {
            using (var db = new ImageDiffContext())
            {
                return db.MyImages.FirstOrDefault(i => i.Order == order && !i.Untracked);
            }
        }

        public async Task MoveSignature()
        {
            try
            {
                var tiffsCount = CountTiffs();
                if (tiffsCount > 0 && _tifIndex < tiffsCount)
                {
                    var tiff = GetTiff(_tifIndex);
                    if (tiff == null)
                        return;

                    await CopyFiles(tiff.Filename, _signatureFolder);
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
                var tiffsCount = CountTiffs();
                if (tiffsCount > 0 && _tifIndex < tiffsCount)
                {
                    var tiff = GetTiff(_tifIndex);
                    if (tiff == null)
                        return;
                    await CopyFiles(tiff.Filename, _issueFolder);
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
                var tiffsCount = CountTiffs();
                if (tiffsCount > 0 && _tifIndex < tiffsCount)
                {
                    var tiff = GetTiff(_tifIndex);
                    if (tiff == null)
                        return;
                    await CopyFiles(tiff.Filename, _borderFolder);
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
