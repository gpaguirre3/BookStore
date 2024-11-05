using PresentationDesktop;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace DesktopClient.Models
{
    public class BookModel : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private int _year;
        private string _isbn;
        private string _publisher;
        private double _price;
        private string _imageUrl;
        private BitmapImage _image;
        private AuthorModel _author;
        private List<GenreModel> _genres = new List<GenreModel>();

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public int Year
        {
            get { return _year; }
            set
            {
                _year = value;
                OnPropertyChanged(nameof(Year));
            }
        }

        public string Isbn
        {
            get { return _isbn; }
            set
            {
                _isbn = value;
                OnPropertyChanged(nameof(Isbn));
            }
        }

        public string Publisher
        {
            get { return _publisher; }
            set
            {
                _publisher = value;
                OnPropertyChanged(nameof(Publisher));
            }
        }

        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                OnPropertyChanged(nameof(ImageUrl));
            }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public AuthorModel Author
        {
            get { return _author; }
            set
            {
                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        public List<GenreModel> Genres
        {
            get { return _genres; }
            set
            {
                _genres = value;
                OnPropertyChanged(nameof(Genres));
            }
        }

        public BookModel(Book model)
        {
            _id = model.Id;
            _title = model.Title;
            _year = model.Year ?? 0;
            _price = model.Price ?? 0;
            _isbn = model.Isbn;
            _publisher = model.Publisher;

            if (model.Image != null && model.Image != "")
            {
                _imageUrl = model.Image;
                _image = new BitmapImage(new Uri(model.Image));
            }
            else
            {
                _imageUrl = "https://i.imgur.com/XVthRcs.png";
                _image = new BitmapImage(new Uri(_imageUrl));
            }

            foreach (var genre in model.Genres)
            {
                _genres.Add(new GenreModel(genre));
            }

            _author = new AuthorModel(model.Author);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
