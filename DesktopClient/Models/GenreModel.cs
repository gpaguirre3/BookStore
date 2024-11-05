using PresentationDesktop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopClient.Models
{
    public partial class GenreModel : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _name = "";

        public GenreModel() { }

        public GenreModel(int id, string name)
        {
            _id = id;
            _name = name;
        }

        public GenreModel(Genre model)
        {
            _id = model.Id;
            _name = model.Name;
        }
    }
}
