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
        private int currentImageOffsetID = 0;

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

                var takenByInfoRaw = parsedPhotoFeed[0]["takenBy"].ToString();
                JObject takenObject = JObject.Parse(takenByInfoRaw);

                currentImageOffsetID = 0; //reset offset index


                FeedIntroPhotoAuthor.Text = "from @" + takenObject["username"].ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error loading cover photos: " + e.Message);
            }
        }

    //search buttons
        // Jank Previous Image
        private void JankPreviousImage_Click(object sender, RoutedEventArgs e)
        {
            int loadID;
            try
            {
                loadID = Int32.Parse(FeedIntroPhoto.Source.ToString().Remove(0, 34));
                loadID--;

                if (loadID < 2)
                {
                    loadID = 2;
                }
            }
            catch
            {   //if theres no image loaded
                loadID = 2;
            }
            
            string sentloadID = loadID.ToString();

            loadExactPhotoID(sentloadID);
        }

        
    //back forth buttons 
        //Previous Image
        private async Task AsyncPreviousImage_Click()
        {
            //setup client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");

            try
            {
                //get feed
                currentImageOffsetID++; //boot up offset
                var rawPhotoFeed = await client.GetStringAsync("https://api.compensationvr.tk/api/social/imgfeed?offset="+ currentImageOffsetID + "&count=5&reverse");

                JArray parsedPhotoFeed = JArray.Parse(rawPhotoFeed);
                string? firstItem = parsedPhotoFeed[0]["_id"].ToString();

                FeedIntroPhoto.Source = new BitmapImage(new Uri("https://api.compensationvr.tk/img/" + firstItem));

                var takenByInfoRaw = parsedPhotoFeed[0]["takenBy"].ToString();
                JObject takenObject = JObject.Parse(takenByInfoRaw);


                FeedIntroPhotoAuthor.Text = "from @" + takenObject["username"].ToString();
            }
            catch
            {
                MessageBox.Show("error loading next image");
            }
        }

        //Next Image
        private async Task AsyncNextImage_Click()
        {
            //setup client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");

            if (currentImageOffsetID > 0)
            {
                try
                {

                    //get feed
                    currentImageOffsetID--; //boot up offset
                    var rawPhotoFeed = await client.GetStringAsync("https://api.compensationvr.tk/api/social/imgfeed?offset=" + currentImageOffsetID + "&count=5&reverse");

                    JArray parsedPhotoFeed = JArray.Parse(rawPhotoFeed);
                    string? firstItem = parsedPhotoFeed[0]["_id"].ToString();

                    FeedIntroPhoto.Source = new BitmapImage(new Uri("https://api.compensationvr.tk/img/" + firstItem));

                    var takenByInfoRaw = parsedPhotoFeed[0]["takenBy"].ToString();
                    JObject takenObject = JObject.Parse(takenByInfoRaw);


                    FeedIntroPhotoAuthor.Text = "from @" + takenObject["username"].ToString();
                }
                catch
                {
                    MessageBox.Show("error loading next image");
                }
            }
        }


        //latest image button
        private void LatestImage_Click(object sender, RoutedEventArgs e)
        {
            loadFeedImagesAsync();
        }

        //load image from ID from API
        private async Task loadExactPhotoID(string sentloadID)
        {
            //setup client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");
            
            try
            {
                FeedIntroPhoto.Source = new BitmapImage(new Uri("https://api.compensationvr.tk/img/" + sentloadID));
                FeedIntroPhotoAuthor.Text = "";
            }
            catch
            {
                MessageBox.Show("error loading exact photo id");
            }
        }

        private void PreviousImage_Click(object sender, RoutedEventArgs e)
        {
            AsyncPreviousImage_Click();
        }

        private void NewerImage_Click(object sender, RoutedEventArgs e)
        {
            AsyncNextImage_Click();
        }
    }
}