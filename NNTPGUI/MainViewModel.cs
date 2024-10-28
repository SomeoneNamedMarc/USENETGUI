using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NNTPGUI
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private IUseNet _useNet;
        private string? _selectedNewsGroup;
        private string? _selectedArticle;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<string>? NewsGroupsList { get; set; }
        public ObservableCollection<string>? ArticlesList { get; set; }
        public ObservableCollection<string>? ArticleContent { get; set; }

        public MainViewModel(IUseNet useNet)
        {
            NewsGroupsList = new ObservableCollection<string>();
            ArticlesList = new ObservableCollection<string>();
            ArticleContent = new ObservableCollection<string>();
            _useNet = useNet;
        }

        public async Task InitializeAsync(string DNS, int Port, string Username, string Password)
        {
            bool connected = await _useNet.ConnectAsync(DNS, Port, Username, Password);
            if (connected)
            {
                await LoadNewsGroupsAsync();
            }
            else
            {
                Console.WriteLine("Failed to connect to the NNTP server.");
            }
        }

        public string SelectedNewsGroup
        {
            get => _selectedNewsGroup;
            set
            {
                _selectedNewsGroup = value;
                OnPropertyChanged(nameof(SelectedNewsGroup));
                LoadArticlesAsync();
            }
        }
        public string SelectedArticle
        {
            get => _selectedArticle;
            set
            {
                _selectedArticle = value;
                OnPropertyChanged(nameof(SelectedArticle));
                LoadArticleContentAsync();
            }
        }

        private async Task LoadArticlesAsync()
        {
            ArticlesList = await _useNet.RetrieveArticleListAsync(SelectedNewsGroup);
            OnPropertyChanged(nameof(ArticlesList));

            ArticleContent.Clear();
            ArticleContent = new ObservableCollection<string>();
            OnPropertyChanged(nameof(ArticleContent));
        }

        private async Task LoadNewsGroupsAsync()
        {
            NewsGroupsList = await _useNet.RetrieveGroupListAsync();
            OnPropertyChanged(nameof(NewsGroupsList));
        }

        private async Task LoadArticleContentAsync()
        {
            ArticleContent = await _useNet.RetrieveArticleAsync(SelectedArticle);
            OnPropertyChanged(nameof(ArticleContent));
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
