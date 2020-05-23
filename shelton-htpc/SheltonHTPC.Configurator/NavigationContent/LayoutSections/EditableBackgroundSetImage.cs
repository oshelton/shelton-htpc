using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using ReactiveUI;
using SheltonHTPC.Common.Utils;
using SheltonHTPC.Dtos.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPFAspects.Core;
using WPFAspects.Utils;

namespace SheltonHTPC.NavigationContent.LayoutSections
{
    public sealed class EditableBackgroundSetImage : Model
    {
        public EditableBackgroundSetImage(BackgroundSetImageDto dto, int orderIndex)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            OrderIndex = orderIndex;
            _UHDImagePath = dto.UHDImagePath;
            _HDImagePath = dto.HDImagePath;
            _FullImagePath = dto.FullImagePath;

            _PreviewTask = Task.Run(async () =>
            {
                var repo = LayoutDataManager.Repo;

                using (var previewStream = new MemoryStream())
                {
                    repo.Database.FileStorage.Download(dto.HDImagePath, previewStream);
                    previewStream.Seek(0, SeekOrigin.Begin);

                    BitmapImage previewImage = new BitmapImage();
                    previewImage.BeginInit();
                    previewImage.StreamSource = previewStream;
                    previewImage.CacheOption = BitmapCacheOption.OnLoad;
                    previewImage.EndInit();
                    previewImage.Freeze();

                    await DispatcherHelper.InvokeAsyncIfNecessary(() =>
                    {
                        PreviewImage = previewImage;
                        IsLoadingPreview = false;
                    }).ConfigureAwait(false);
                }
            });

            Initialize();
        }

        public EditableBackgroundSetImage(int orderIndex) 
        {
            IsNew = true;
            OrderIndex = orderIndex;
            Initialize();
        }

        private void Initialize()
        {
            _DirtyTracker = new DirtyTracker(this);

            this.WhenAnyValue(x => x._DirtyTracker.IsDirty, x => x.IsNew)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => IsDirty = x.Item1 || x.Item2);
        }

        private bool _CanMoveLeft;
        /// <summary>
        /// Whether or not this background set can be moved up.
        /// </summary>
        public bool CanMoveLeft
        {
            get => CheckIsOnMainThread(_CanMoveLeft);
            private set => SetPropertyBackingValue(value, ref _CanMoveLeft);
        }

        private bool _CanMoveRight;
        /// <summary>
        /// Whether or not the image can be moved down.
        /// </summary>
        public bool CanMoveRight
        {
            get => CheckIsOnMainThread(_CanMoveRight);
            private set => SetPropertyBackingValue(value, ref _CanMoveRight);
        }

        private bool _IsDirty = false;
        /// <summary>
        /// Whether or not this image is dirty.
        /// </summary>
        public bool IsDirty
        {
            get => CheckIsOnMainThread(_IsDirty);
            private set => SetPropertyBackingValue(value, ref _IsDirty);
        }

        private bool _IsNew = false;
        /// <summary>
        /// Whether or not this is a newly created Background set image.
        /// </summary>
        public bool IsNew
        {
            get => CheckIsOnMainThread(_IsNew);
            set => SetPropertyBackingValue(value, ref _IsNew);
        }

        private int _OrderIndex = 0;
        /// <summary>
        /// Position of this Background set image in the collection of set images.
        /// </summary>
        public int OrderIndex
        {
            get => CheckIsOnMainThread(_OrderIndex);
            set => SetPropertyBackingValue(value, ref _OrderIndex);
        }

        private BitmapImage _PreviewImage = null;
        /// <summary>
        /// Image source for the thumbnail image; will kick off loading it if it hasn't yet been loaded.
        /// </summary>
        public BitmapImage PreviewImage
        {
            get => _PreviewImage;
            private set => SetPropertyBackingValue(value, ref _PreviewImage);
        }

        private bool _IsLoadingPreview = false;
        /// <summary>
        /// Whether or not the thumbnail is currently being loaded.
        /// </summary>
        public bool IsLoadingPreview
        {
            get => CheckIsOnMainThread(_IsLoadingPreview);
            private set => SetPropertyBackingValue(value, ref _IsLoadingPreview);
        }

        private string _ErrorLoadingThumbnailMessage = null;
        /// <summary>
        /// Potential error message when loading a background's thumbnail fails.
        /// </summary>
        public string ErrorLoadingThumbnailMessage
        {
            get => CheckIsOnMainThread(_ErrorLoadingThumbnailMessage);
            set => SetPropertyBackingValue(value, ref _ErrorLoadingThumbnailMessage);
        }

        private string _SourcePath = null;
        /// <summary>
        /// Path to the input source image.  May or may not come in handy and shouldn't be used for much handy.
        /// </summary>
        public string SourcePath
        {
            get => CheckIsOnMainThread(_SourcePath);
            set
            {
                if (SetPropertyBackingValue(value, ref _SourcePath))
                {
                    var oldTask = _PreviewTask;
                    IsLoadingPreview = true;
                    _PreviewTask = Task.Run(async () =>
                    {
                        if (oldTask is object)
                            await oldTask.ConfigureAwait(true);

                        try
                        {
                            using (var processor = new ImageFactory(true))
                            {
                                processor.Load(value);
                                using (var previewStream = new MemoryStream())
                                {
                                    processor.CropToTVAspectRatio()
                                        .Save(previewStream);

                                    BitmapImage previewImage = new BitmapImage();
                                    previewImage.BeginInit();
                                    previewImage.StreamSource = previewStream;
                                    previewImage.CacheOption = BitmapCacheOption.OnLoad;
                                    previewImage.EndInit();
                                    previewImage.Freeze();

                                    await DispatcherHelper.InvokeAsyncIfNecessary(() =>
                                    {
                                        PreviewImage = previewImage;
                                        IsLoadingPreview = false;
                                    }).ConfigureAwait(false);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            await DispatcherHelper.InvokeAsyncIfNecessary(() => ErrorLoadingThumbnailMessage = $"Error generating thumbnail for file: {value}").ConfigureAwait(false);
                        }
                    });
                }
            }
        }

        private bool _WillBeRemoved = false;
        /// <summary>
        /// Whether or not this image will be removed due to the kind of background set.
        /// </summary>
        public bool WillBeRemoved
        {
            get => CheckIsOnMainThread(_WillBeRemoved);
            private set => SetPropertyBackingValue(value, ref _WillBeRemoved);
        }

        public async Task OnSaved()
        {
            await DispatcherHelper.InvokeAsyncIfNecessary(() =>
            {
                IsNew = false;
                _DirtyTracker.SetInitialState();
            }).ConfigureAwait(false);
        }

        public override HashSet<string> DefaultTrackedProperties { get; } = new HashSet<string>()
        {
            nameof(OrderIndex),
        };

        public void UpdateCanMove(int imageCount)
        {
            UpdateCanMoveLeft();
            UpdateCanMoveRight(imageCount);
        }

        public void UpdateWillBeRemoved(string backgroundSetStyle)
        {
            WillBeRemoved = OrderIndex != 0 && backgroundSetStyle == EditableBackgroundSet.SpanningName;
        }

        public async Task CommitImagesToFileStorage()
        {
            string path = null;
            await DispatcherHelper.InvokeAsyncIfNecessary(() => path = SourcePath).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(path) && !File.Exists(path))
                throw new InvalidOperationException("Source image file does not exist; it cannot be saved.");
            else if (string.IsNullOrEmpty(path))
                return;

            using (var processor = new ImageFactory(true))
            {
                processor.Load(path);
                var repo = LayoutDataManager.Repo;

                RemoveImagesFromStorage();

                Guid id = Guid.NewGuid();
                _UHDImagePath = $"backgrounds/4k/{id}";
                _HDImagePath = $"backgrounds/1080p/{id}";
                _FullImagePath = $"backgrounds/full/{id}";

                using (var croppedStream = new MemoryStream())
                {
                    processor.CropToTVAspectRatio()
                        .Save(croppedStream);

                    if (processor.Image.Height >= ImageFactoryExtensions.UhdHeight)
                    {
                        using (var uhdStream = new MemoryStream())
                        {
                            processor.Load(croppedStream)
                                .ResizeImageToUhd()
                                .Format(new JpegFormat() { Quality = 80 })
                                .Save(uhdStream)
                                .Reset();

                            repo.Database.FileStorage.Upload(_UHDImagePath, path, uhdStream);
                        }
                    }

                    using (var hdStream = new MemoryStream())
                    {
                        processor.Load(croppedStream)
                            .ResizeImageToHd()
                            .Format(new JpegFormat() { Quality = 80 } )
                            .Save(hdStream)
                            .Reset();

                        repo.Database.FileStorage.Upload(_HDImagePath, path, hdStream);
                    }

                    if (_PreviewTask is object)
                        await _PreviewTask.ConfigureAwait(false);

                    using (FileStream fs = new FileStream(path, FileMode.Open))
                    {
                        repo.Database.FileStorage.Upload(_FullImagePath, path, fs);
                    }
                }
            }
        }

        public void RemoveImagesFromStorage()
        {
            if (_FullImagePath is object)
            {
                string uhdPath = _UHDImagePath;
                string hdPath = _HDImagePath;
                string thumbnailPath = _FullImagePath;

                var repo = LayoutDataManager.Repo;
                repo.Database.FileStorage.Delete(thumbnailPath);
                if (uhdPath is object)
                    repo.Database.FileStorage.Delete(uhdPath);
                if (hdPath is object)
                    repo.Database.FileStorage.Delete(hdPath);
            }
        }

        public BackgroundSetImageDto CreateDto()
        {
            return new BackgroundSetImageDto()
            {
                SourcePath = SourcePath,
                FullImagePath = _FullImagePath,
                HDImagePath = _HDImagePath,
                UHDImagePath = _UHDImagePath,
            };
        }

        private void UpdateCanMoveLeft()
        {
            CanMoveLeft = OrderIndex > 0;
        }

        private void UpdateCanMoveRight(int setCount)
        {
            CanMoveRight = OrderIndex < setCount - 1;
        }

        private DirtyTracker _DirtyTracker;

        /// <summary>
        /// Path to the 4k appropriate image to use in FileStorage.  May be null if the source image was not large enough.
        /// </summary>
        private string _UHDImagePath = null;

        /// <summary>
        /// Path to the 1080p appropriate image to use in FileStorage.  Should not be null.
        /// </summary>
        private string _HDImagePath = null;

        /// <summary>
        /// Path to the thumbnail image to use in FileStorage.  Should not be null.  Should have a height of 128 pixels.
        /// </summary>
        private string _FullImagePath = null;

        /// <summary>
        /// Task for thumbnail generation.
        /// </summary>
        private Task _PreviewTask;
    }
}
