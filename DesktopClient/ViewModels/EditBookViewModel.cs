using DesktopClient.Models;
using DesktopClient.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using PresentationDesktop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DesktopClient.ViewModels
{
    public partial class EditBookViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private INavigationParams _navigationParams;
        private IServiceProvider _serviceProvider;
        private ISnackbarService _snackbarService;
        private BookModel _book;

        [ObservableProperty]
        private Visibility _openedFolderPathVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility _openedPicturePathVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private string _openedPicturePath = string.Empty;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _isbn = string.Empty;

        [ObservableProperty]
        private string _year = string.Empty;

        [ObservableProperty]
        private string _publisher = string.Empty;

        [ObservableProperty]
        private string _price = "0,0";

        [ObservableProperty]
        private ObservableCollection<AuthorModel> _authors = new ObservableCollection<AuthorModel>();

        [ObservableProperty]
        private bool _isAuthorSuggestionSelected = false;

        [ObservableProperty]
        private AuthorModel _selectedAuthor = new AuthorModel();

        [ObservableProperty]
        private string _authorSuggestEnteredText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<GenreListViewItem> _genres = new ObservableCollection<GenreListViewItem>();

        [ObservableProperty]
        private BitmapImage _previewImage = new BitmapImage();

        private BookstoreApiService _bookstoreApiService;

        public EditBookViewModel(
            IServiceProvider serviceProvider, 
            INavigationParams navigationParams,
            ISnackbarService snackbarService,
            BookstoreApiService bookstoreApiService)
        {
            _serviceProvider = serviceProvider;
            _navigationParams = navigationParams;
            _snackbarService = snackbarService;
            _bookstoreApiService = bookstoreApiService;
        }

        public void OnNavigatedFrom() {}

        public void OnNavigatedTo() 
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        private void InitializeViewModel()
        {
            Task.Run(() => LoadData());
            _isInitialized = true;
        }

        private async Task LoadData()
        {
            try
            {
                await LoadAuthors();
                await LoadGenres();
                LoadBookContext();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _snackbarService.Show(
                        "Error",
                        ex.Message,
                        ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.ErrorCircle24),
                        TimeSpan.FromSeconds(15)
                    );
                });
            }
        }

        private async Task LoadAuthors()
        {
            var authors = await _bookstoreApiService.GetAuthorsAsync();

            Application.Current.Dispatcher.Invoke((Delegate)(() =>
            {
                Authors.Clear();

                foreach (var author in authors)
                {
                    Authors.Add(new AuthorModel(author));
                }
            }));
        }

        private async Task LoadGenres()
        {
            var genres = await _bookstoreApiService.GetGenresAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Genres.Clear();

                foreach (var genre in genres)
                {
                    GenreListViewItem item = new();
                    item.Genre = new GenreModel(genre);
                    item.IsSelected = false;

                    Genres.Add(item);
                }
            });
        }

        private void LoadBookContext()
        {
            _book = _navigationParams.Get<BookModel>("BookToUpdate");

            Application.Current.Dispatcher.Invoke(() =>
            {
                Title = _book.Title;
                Isbn = _book.Isbn;
                Year = _book.Year.ToString();
                Publisher = _book.Publisher;
                Price = _book.Price.ToString();

                if (_book.Author != null && _book.Author.Id != 0)
                {
                    SelectedAuthor = Authors.First(author => author.Id == _book.Author.Id);
                    IsAuthorSuggestionSelected = true;
                    AuthorSuggestEnteredText = SelectedAuthor.CompleteDisplayName;
                }

                if (_book.Image != null)
                {
                    Uri uri = new(_book.ImageUrl);
                    string filename = uri.Segments.Last();

                    OpenedPicturePath = "server://" + filename;
                    OpenedPicturePathVisibility = Visibility.Visible;
                    PreviewImage = new BitmapImage(uri);
                }

                Genres.ToList().ForEach(genre =>
                {
                    genre.IsSelected = _book.Genres.Any(bookGenre => bookGenre.Id == genre.Genre.Id);
                });
            });
        }

        [RelayCommand]
        public void OnOpenPicture()
        {
            OpenedPicturePathVisibility = Visibility.Collapsed;

            OpenFileDialog openFileDialog =
                new()
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    Filter = "Image files (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png|All files (*.*)|*.*"
                };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            if (!File.Exists(openFileDialog.FileName))
            {
                return;
            }

            OpenedPicturePath = openFileDialog.FileName;
            OpenedPicturePathVisibility = Visibility.Visible;
            PreviewImage = new BitmapImage(new Uri(openFileDialog.FileName));
        }

        [RelayCommand]
        public void SubmitForm()
        {
            if (Title.Trim() == string.Empty)
            {
                _snackbarService.Show(
                    "Error",
                    "El título del libro es requerido!",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );

                return;
            }

            if (Year.Trim() == string.Empty)
            {
                _snackbarService.Show(
                    "Error",
                    "El año del libro es requerido!",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );

                return;
            }

            if (SelectedAuthor == null || SelectedAuthor.Id == 0)
            {
                _snackbarService.Show(
                    "Error",
                    "El autor del libro es requerido!",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );

                return;
            }

            var selectedGenres = Genres.Where(item => item.IsSelected).ToList();

            if (selectedGenres.Count == 0)
            {
                _snackbarService.Show(
                    "Error",
                    "Al menos un género es requerido",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );

                return;
            }

            double priceValue = Double.TryParse(Price, out double price) ? price : 0;

            if (priceValue <= 0)
            {
                _snackbarService.Show(
                    "Error",
                    "El precio del libro es requerido!",
                    ControlAppearance.Danger,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );

                return;
            }

            try
            {
                Task.Run(() => SaveBook());
            }
            catch (Exception ex)
            {
                _snackbarService.Show(
                    "Error",
                    "Ha ocurrido un error al agregar el libro!",
                    ControlAppearance.Caution,
                    new SymbolIcon(SymbolRegular.Book24),
                    TimeSpan.FromSeconds(2)
                );
            }
        }

        [RelayCommand]
        public void AuthorSuggestionChosen(AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var author = args.SelectedItem as AuthorModel;

            if (author != null)
            {
                SelectedAuthor = author;
                IsAuthorSuggestionSelected = true;
            }
        }

        [RelayCommand]
        public void ClearAuthorSelected()
        {
            SelectedAuthor = new AuthorModel();
            IsAuthorSuggestionSelected = false;
            AuthorSuggestEnteredText = string.Empty;
        }

        private async void SaveBook()
        {
            double priceValue = Double.TryParse(Price, out double price) ? price : 0;
            int yearValue = Int32.TryParse(Year, out int year) ? year : 0;
            var bookModel = new BookRequestModel();
            bookModel.Id = _book.Id;
            bookModel.Title = Title;
            bookModel.Year = yearValue;
            bookModel.Price = priceValue;

            if (Isbn != null && Isbn.Trim() != string.Empty)
            {
                bookModel.Isbn = Isbn;
            }

            if (Publisher != null && Publisher.Trim() != string.Empty)
            {
                bookModel.Publisher = Publisher;
            }


            if (OpenedPicturePath.Trim() != string.Empty && !OpenedPicturePath.StartsWith("server://"))
            {
                byte[] imageBytes = File.ReadAllBytes(OpenedPicturePath);
                bookModel.Image = imageBytes;
            }

            bookModel.Author = new AuthorRequestModel()
            {
                Id = SelectedAuthor.Id
            };

            var selectedGenres = Genres.Where(item => item.IsSelected).ToList();
            bookModel.Genres = selectedGenres.Select(genre => new GenreRequestModel() { Id = genre.Genre.Id }).ToArray();

            try
            {
                await _bookstoreApiService.UpdateBookAsync(bookModel);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Title = string.Empty;
                    Isbn = string.Empty;
                    Publisher = string.Empty;
                    Year = string.Empty;
                    Price = string.Empty;
                    OpenedPicturePath = string.Empty;
                    OpenedPicturePathVisibility = Visibility.Collapsed;
                    SelectedAuthor = new AuthorModel();
                    IsAuthorSuggestionSelected = false;
                    AuthorSuggestEnteredText = string.Empty;
                    Genres.ToList().ForEach(genre => genre.IsSelected = true);
                    PreviewImage = new BitmapImage();

                    var navigationWindow = (_serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow)!;
                    _ = navigationWindow.Navigate(typeof(DesktopClient.Views.Pages.BooksListPage));

                    _snackbarService.Show(
                        "Éxito",
                        "Libro editado correctamente!",
                        ControlAppearance.Success,
                        new SymbolIcon(SymbolRegular.Checkmark24),
                        TimeSpan.FromSeconds(2)
                    );
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _snackbarService.Show(
                        "Error",
                        ex.Message,
                        ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.ErrorCircle24),
                        TimeSpan.FromSeconds(15)
                    );
                });
            }
        }
    }
}
