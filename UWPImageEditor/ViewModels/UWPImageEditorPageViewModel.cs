using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UWPImageEditor.Controls;
using UWPImageEditor.UWPImageEditorModels;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace UWPImageEditor.ViewModels
{
    class UWPImageEditorPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        private CanvasControl _canvasControl;
        private ScrollViewer _scrollView;
        private double _scale;

        private Stack<IDrawing> _strokes;
        private IDrawing _currentStroke;

        private CanvasBitmap _bitmap;
        private CanvasBitmap _clipboard;
        private ClipboardBorder _clipboardBorder;

        private int _penSize;
        public int PenSize
        {
            get { return _penSize; }
            set { SetProperty(ref _penSize, value); }
        }

        private List<SolidColorBrush> paletteColors = new List<SolidColorBrush>
        {
            new SolidColorBrush(Colors.Black),
            new SolidColorBrush(Colors.White),
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Yellow),
            new SolidColorBrush(Colors.Violet),
            new SolidColorBrush(Colors.Orange)
        };
        public List<SolidColorBrush> PaletteColors
        {
            get { return paletteColors; }
            set { SetProperty(ref paletteColors, value); }
        }

        private SolidColorBrush _penColor;
        public SolidColorBrush PenColor
        {
            get { return _penColor; }
            set { SetProperty(ref _penColor, value); }
        }

        private List<DrawingMode> drawingModes = new List<DrawingMode>
        {
            DrawingMode.Lines,
            DrawingMode.Arrows,
            DrawingMode.Circles,
            DrawingMode.Rectangles,
            DrawingMode.Selection,
            DrawingMode.Moving
        };
        public List<DrawingMode> DrawingModes
        {
            get { return drawingModes; }
            set { SetProperty(ref drawingModes, value); }
        }

        private DrawingMode _currentDrawingMode;
        public DrawingMode CurrentDrawingMode
        {
            get { return _currentDrawingMode; }
            set { SetProperty(ref _currentDrawingMode, value); }
        }

        public ICommand OnDrawCommand => new DelegateCommand<CanvasDrawEventArgs>(OnDraw);
        public ICommand OnManipulationStartedCommand => new DelegateCommand<ManipulationStartedRoutedEventArgs>(OnManipulationStarted);
        public ICommand OnManipulationCompletedCommand => new DelegateCommand<ManipulationCompletedRoutedEventArgs>(OnManipulationCompleted);
        public ICommand OnManipulationDeltaCommand => new DelegateCommand<ManipulationDeltaRoutedEventArgs>(OnManipulationDelta);
        public ICommand OnTappedCommand => new DelegateCommand<TappedRoutedEventArgs>(OnTapped);
        public ICommand OnCreateResourcesCommand => new DelegateCommand<CanvasCreateResourcesEventArgs>(OnCreateResources);
        public ICommand SaveCommand => new DelegateCommand(async () => await SaveImage());
        public ICommand UndoCommand => new DelegateCommand(OnUndo);
        public ICommand CopyCommand => new DelegateCommand(OnCopy);
        public ICommand PasteCommand => new DelegateCommand(OnPaste);

        public UWPImageEditorPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            var frame = Window.Current.Content as Frame;
            _scrollView = VisualHelper.FindVisualChildInsideFrame<ScrollViewer>(frame);
            _canvasControl = (CanvasControl)_scrollView.Content;
            _strokes = new Stack<IDrawing>();

            PenSize = 5;
            PenColor = paletteColors[paletteColors.Count - 1];
            CurrentDrawingMode = DrawingMode.Lines;

            base.OnNavigatedTo(e, viewModelState);
        }

        private async Task SaveImage()
        {
            var result = GetDrawings();
            if (result == null)
                return;

            IRandomAccessStream stream = new InMemoryRandomAccessStream();
            await result.SaveAsync(stream, CanvasBitmapFileFormat.Jpeg);

            using (var reader = new DataReader(stream))
            {
                await reader.LoadAsync((uint)stream.Size);
                var buffer = new byte[(int)stream.Size];
                reader.ReadBytes(buffer);
                IStorageFolder folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("UWPImageEditorImages", CreationCollisionOption.OpenIfExists);
                IStorageFile file = await folder.CreateFileAsync(string.Concat(Guid.NewGuid().ToString(), ".jpg"));

                if (file == null)
                    return;

                await FileIO.WriteBytesAsync(file, buffer);
                await new MessageDialog($"Image successfully saved to: {file.Path}").ShowAsync();
            }
        }

        private void OnUndo()
        {
            if (_strokes == null || !_strokes.Any())
                return;

            _strokes.Pop();
            _canvasControl.Invalidate();
        }

        private void OnPaste()
        {
            if (_clipboard == null)
                return;

            _clipboardBorder = null;
            _currentStroke = new Clipboard(_clipboard);
            CurrentDrawingMode = DrawingMode.Moving;
            _canvasControl.Invalidate();
        }

        private void OnCopy()
        {
            if (_clipboardBorder == null)
                return;

            var device = CanvasDevice.GetSharedDevice();
            var border = _clipboardBorder.ToRect();
            var drawings = CanvasBitmap.CreateFromBytes(device, GetDrawings().GetPixelBytes(),
                                                        (int)(_bitmap.Size.Width * _scale), (int)(_bitmap.Size.Height * _scale),
                                                        _bitmap.Format);
            var clipboardBytes = drawings.GetPixelBytes((int)(border.X), (int)(border.Y),
                                                        (int)(border.Width), (int)(border.Height));
            _clipboard = CanvasBitmap.CreateFromBytes(device, clipboardBytes,
                                                      (int)(border.Width), (int)(border.Height),
                                                      drawings.Format, drawings.Dpi, drawings.AlphaMode);
        }

        private void OnCreateResources(CanvasCreateResourcesEventArgs args) =>
            args.TrackAsyncAction(CreateResourcesAsync(_canvasControl).AsAsyncAction());

        private async Task CreateResourcesAsync(CanvasControl sender)
        {
            StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Images/cat.jpg"));
            var stream = await storageFile.OpenReadAsync();
            _bitmap = await CanvasBitmap.LoadAsync(sender, stream);
        }

        private CanvasRenderTarget GetDrawings()
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget target = new CanvasRenderTarget(device, (float)_canvasControl.ActualWidth, (float)_canvasControl.ActualHeight, 96);
            using (CanvasDrawingSession graphics = target.CreateDrawingSession())
            {
                graphics.Clear(Colors.Transparent);

                if (_bitmap != null)
                    graphics.DrawImage(_bitmap, GetBitmapRect());

                if (_strokes != null && _strokes.Any())
                {
                    var list = _strokes.ToList();
                    list.Reverse();
                    list.ForEach((d) => { d.Draw(graphics); });
                }

                if (_currentStroke != null)
                    _currentStroke.Draw(graphics);
            }
            return target;
        }

        private Rect GetBitmapRect()
        {
            var controlWidth = _canvasControl.ActualWidth;
            var controlHeight = _canvasControl.ActualHeight;
            var imageHeight = _bitmap.Size.Height;
            var imageWidth = _bitmap.Size.Width;

            var hScale = controlHeight / imageHeight;
            var wScale = controlWidth / imageWidth;

            _scale = Math.Min(hScale, wScale);

            var rectHeight = imageHeight * _scale;
            var rectWidth = imageWidth * _scale;

            _canvasControl.Width = rectWidth;
            _canvasControl.Height = rectHeight;

            return new Rect()
            {
                Height = rectHeight,
                Width = rectWidth,
                X = 0,
                Y = 0
            };
        }

        private void OnDraw(CanvasDrawEventArgs args)
        {
            var target = GetDrawings();

            if (target != null)
                args.DrawingSession.DrawImage(target);

            if (_clipboardBorder != null)
                _clipboardBorder.Draw(args.DrawingSession);
        }

        private void OnManipulationStarted(ManipulationStartedRoutedEventArgs args)
        {
            if (_currentStroke != null)
                return;

            if (PenColor == null)
                return;

            if (CurrentDrawingMode == DrawingMode.Selection)
            {
                _currentStroke = null;
                _clipboardBorder = new ClipboardBorder(args.Position);
                return;
            }

            _clipboardBorder = null;

            if (CurrentDrawingMode == DrawingMode.Lines)
                _currentStroke = new Stroke(PenColor.Color, PenSize);
            if (CurrentDrawingMode == DrawingMode.Arrows)
                _currentStroke = new Arrow(PenColor.Color, PenSize, args.Position);
            if (CurrentDrawingMode == DrawingMode.Circles)
                _currentStroke = new Circle(PenColor.Color, PenSize, args.Position);
            if (CurrentDrawingMode == DrawingMode.Rectangles)
                _currentStroke = new Rectangle(PenColor.Color, PenSize, args.Position);
        }

        private void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs args)
        {
            if (_currentStroke == null)
                return;

            if (CurrentDrawingMode == DrawingMode.Moving)
                CurrentDrawingMode = DrawingMode.Selection;

            _strokes.Push(_currentStroke);
            _currentStroke = null;
            _canvasControl.Invalidate();
        }

        private void OnManipulationDelta(ManipulationDeltaRoutedEventArgs args)
        {
            if (_currentStroke != null)
                _currentStroke.AddPoint(args.Position);
            if (_clipboardBorder != null)
                _clipboardBorder.AddPoint(args.Position);
            _canvasControl.Invalidate();
        }

        private void OnTapped(TappedRoutedEventArgs args)
        {
            return;
        }
    }
}
