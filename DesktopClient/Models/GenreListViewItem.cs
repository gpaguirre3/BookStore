using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopClient.Models
{
    public partial class GenreListViewItem : ObservableObject
    {
        [ObservableProperty]
        public GenreModel _genre = new GenreModel();

        [ObservableProperty]
        public bool _isSelected = false;
    }
}
