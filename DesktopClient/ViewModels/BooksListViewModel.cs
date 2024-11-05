using CommunityToolkit.Mvvm.ComponentModel;
using DesktopClient.Models;
using DesktopClient.Services;
using DesktopClient.Views;
using Microsoft.Extensions.DependencyInjection;
using PresentationDesktop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Navigation;
using Wpf.Ui;
using Wpf.Ui.Controls;
using static System.Reflection.Metadata.BlobBuilder;

namespace DesktopClient.ViewModels
{
    public partial class BooksListViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private IServiceProvider serviceProvider;
        private INavigationParams navigationParams;
        private BookstoreApiService bookstoreApiService;

        [ObservableProperty]
        private List<DataColor> _colors = [];

        [ObservableProperty]
        private ObservableCollection<BookModel> _books = [];

        [ObservableProperty]
        private string _query = string.Empty;

        public BooksListViewModel(
            IServiceProvider serviceProvider, 
            INavigationParams navigationParams,
            BookstoreApiService bookstoreApiService)
        {
            this.serviceProvider = serviceProvider;
            this.navigationParams = navigationParams;
            this.bookstoreApiService = bookstoreApiService;
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }

            try
            {
                Task.Run(() => LoadBooks());
            }
            catch (Exception ex)
            {
                // Log error
                Console.Write(ex.ToString());
            }
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            _isInitialized = true;
        }

        private async void LoadBooks(string? query = null)
        {
            try
            {
                ICollection<Book> books;

                if (query == null || query.Trim().Length == 0)
                {
                    books = await bookstoreApiService.GetBooksAsync();
                }
                else
                {
                    books = await bookstoreApiService.SearchBooksAsync(query);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Books.Clear();

                    foreach (var book in books)
                    {
                        Books.Add(new BookModel(book));
                    }
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnOpenErrorMessageBox(ex.Message).Wait();
                    Application.Current.Shutdown();
                });
            }
        }

        [RelayCommand]
        public void SearchBooks()
        {
            var query = Query.Trim();

            if (query.Length == 0)
            {
                LoadBooks();
                return;
            }

            LoadBooks(query);
        }

        [RelayCommand]
        public void DeleteBook(object sender)
        {
            if (sender is BookModel book)
            {
                Task.Run(async () =>
                {
                    await bookstoreApiService.DeleteBookAsync(book.Id);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                        {
                            Title = "Atención",
                            Content = "Libro eliminado correctamente",
                        };

                        _ = uiMessageBox.ShowDialogAsync();
                    });

                    LoadBooks();
                });
            }
        }

        [RelayCommand]
        public void EditBook(object sender)
        {
            if (sender is BookModel book)
            {
                navigationParams.Set("BookToUpdate", book);
                var navigationWindow = (serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow)!;
                _ = navigationWindow.Navigate(typeof(DesktopClient.Views.Pages.EditBookPage));
            }
        }


        private async Task OnOpenErrorMessageBox(string message)
        {
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "Error",
                Content = "Ocurrió un error al intentar conectarse con el servidor\n\n" + message,
            };

            _ = await uiMessageBox.ShowDialogAsync();
        }
    }
}
