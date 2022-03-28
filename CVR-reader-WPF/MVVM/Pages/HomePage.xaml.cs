using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

namespace CVR_reader_WPF.MVVM.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            loadFeedImagesAsync();
        }

        private async Task loadFeedImagesAsync()
        {
            //setup client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");

            try
            {
                //get feed
                var rawPhotoFeed = await client.GetStringAsync("https://api.compensationvr.tk/api/social/imgfeed?offset=0&count=5&reverse");

                JArray parsedPhotoFeed = JArray.Parse(rawPhotoFeed);
                string? firstItem = parsedPhotoFeed[0]["_id"].ToString();

                FeedIntroPhoto.Source = new BitmapImage(new Uri("https://api.compensationvr.tk/img/" + firstItem));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
