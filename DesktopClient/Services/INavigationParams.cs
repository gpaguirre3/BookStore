using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopClient.Services
{
    public interface INavigationParams
    {
        void Set(string key, object value);
        T Get<T>(string key);
    }
}
