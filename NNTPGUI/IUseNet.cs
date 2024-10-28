using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NNTPGUI
{
    public interface IUseNet
    {
        Task<bool> ConnectAsync(String DNS, int Port, string Username, string Password);
        Task Disconnect();
        Task<ObservableCollection<String>> RetrieveArticleAsync(string ArticleID);
        Task<ObservableCollection<String>> RetrieveArticleListAsync(string Site);
        Task<ObservableCollection<String>> RetrieveGroupListAsync();
        bool IsConnected();
    }
}
