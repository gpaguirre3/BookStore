using DesktopClient.Models;
using Microsoft.Win32;
using PresentationDesktop;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DesktopClient.ViewModels
{
    public partial class AddBookViewModel(
        BookstoreApiService bookstoreApiService,
        ISnackbarService snackbarService) : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private BookstoreApiService _bookstoreApiService = bookstoreApiService;

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

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        public void OnNavigatedFrom() {}

        private void InitializeViewModel()
        {
            Task.Run(() =>
            {
                try
                {
                    LoadAuthors();
                    LoadGenres();
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        snackbarService.Show(
                            "Error",
                            ex.Message,
                            ControlAppearance.Danger,
                            new SymbolIcon(SymbolRegular.ErrorCircle24),
                            TimeSpan.FromSeconds(15)
                        );
                    });
                }
            });
            _isInitialized = true;
        }

        private async void LoadAuthors()
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

        private async void LoadGenres()
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
        }

        [RelayCommand]
        public void SubmitForm()
        {
            if (Title.Trim() == string.Empty)
            {
                snackbarService.Show(
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
                snackbarService.Show(
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
                snackbarService.Show(
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
                snackbarService.Show(
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
                snackbarService.Show(
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
                Task.Run(() => AddBook());
            }
            catch (Exception ex)
            {
                snackbarService.Show(
                    "Error",
                    "Ha ocurrido un error al agregar el libro:\n\n"
                    + ex.Message,
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

        private async void AddBook()
        {
            double priceValue = Double.TryParse(Price, out double price) ? price : 0;
            int yearValue = Int32.TryParse(Year, out int year) ? year : 0;
            var bookModel = new BookRequestModel();
            bookModel.Title = Title;
            bookModel.Year = yearValue;
            bookModel.Price = priceValue;

            if (Isbn.Trim() != string.Empty)
            {
                bookModel.Isbn = Isbn;
            }

            if (Publisher.Trim() != string.Empty)
            {
                bookModel.Publisher = Publisher;
            }

            if (OpenedPicturePath.Trim() != string.Empty)
            {
                byte[] imageBytes = File.ReadAllBytes(OpenedPicturePath);
                bookModel.Image = imageBytes;
            }

            bookModel.Author = new AuthorRequestModel()
            {
                Id = SelectedAuthor?.Id
            };

            var selectedGenres = Genres.Where(item => item.IsSelected).ToList();
            bookModel.Genres = selectedGenres.Select(genre => new GenreRequestModel() { Id = genre.Genre.Id }).ToArray();

            try
            {
                await _bookstoreApiService.AddBookAsync(bookModel);

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
                Genres.ToList().ForEach(genre => genre.IsSelected = false);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    snackbarService.Show(
                        "Éxito",
                        "Libro agregado correctamente!",
                        ControlAppearance.Success,
                        new SymbolIcon(SymbolRegular.Checkmark24),
                        TimeSpan.FromSeconds(2)
                    );
                });
            }
            catch(Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    snackbarService.Show(
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
