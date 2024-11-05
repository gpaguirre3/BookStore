using PresentationDesktop;

namespace DesktopClient.Models
{
    public class AuthorModel
    {
        private int _id;
        private string _firstname = "";
        private string _lastname = "";
        private string _pseudonym = "";
        private DateTimeOffset? _birthdate;
        private string _displayName;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Firstname
        {
            get { return _firstname; }
            set { _firstname = value; }
        }

        public string Lastname
        {
            get { return _lastname; }
            set { _lastname = value; }
        }

        public string Pseudonym
        {
            get { return _pseudonym; }
            set { _pseudonym = value; }
        }

        public DateTimeOffset? Birthdate
        {
            get { return _birthdate; }
            set { _birthdate = value; }
        }

        public string DisplayName
        {
            get { return _pseudonym != null && _pseudonym != "" ? _pseudonym : $"{_firstname} {_lastname}"; }
        }

        public AuthorModel() {}

        public AuthorModel(int id, string firstname, string lastname, string pseudonym, DateTime birthdate)
        {
            _id = id;
            _firstname = firstname;
            _lastname = lastname;
            _pseudonym = pseudonym;
            _birthdate = birthdate;
        }

        public AuthorModel(Author model)
        {
            _id = model.Id;
            _firstname = model.Firstname;
            _lastname = model.Lastname;
            _pseudonym = model.Pseudonym;
            _birthdate = model.Birthdate;
        }

        public string CompleteDisplayName
        {
            get
            {
                var values = new List<string>
                {
                    _id.ToString() + " -",
                    _firstname,
                    _lastname,
                    _pseudonym,
                };

                    return string.Join(" ", values.Where(v => !string.IsNullOrEmpty(v)));
            }
        }

        public override string ToString() => CompleteDisplayName;
    }
}
