using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CVR_reader_WPF.MVVM.Pages
{
    /// <summary>
    /// Interaction logic for UserLookup.xaml
    /// </summary>
    public partial class UserLookup : Page
    {
        public UserLookup()
        {
            InitializeComponent();
        }

        //searh user bar enter envoke
        private void OnSearchKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SearchUserAsync();
            }
        }

        //search user
        private async Task SearchUserAsync()
        {
            //set vars
            var SearchID = UserLookupBasic.Text;
            
            //setup client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");

            //get and parse userID
            var rawUserIDlookup = await client.GetStringAsync("https://api.compensationvr.tk/api/accounts/" + SearchID + "/id");
            JObject UserIDBag = JObject.Parse(rawUserIDlookup);


            //convert searched username to numberic id
            var parsedUserID = UserIDBag["id"].ToString();

            var searchedID = parsedUserID;
            
            basicSearchAsync(Convert.ToInt32(searchedID));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        //search for random user
        private void RandomUserSimpleSearch_Click(object sender, RoutedEventArgs e)
        {   
            //setup client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");

            //get amount of registered users
            var accountsRegisteredAmount = client.GetStringAsync("https://api.compensationvr.tk/api/analytics/account-count");

            try
            {
                //get random userID
                Random rnd = new Random();

                int searchedID = rnd.Next(1, Convert.ToInt32(accountsRegisteredAmount.Result));

                basicSearchAsync(searchedID);
            }
            catch
            {
                //too many request error
                MessageBox.Show("please wait a few seconds before searching again");
                return;

            }
        }

        //basic search
        private async void basicSearchAsync(int searchedID)
        {
            //setup client
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# console program");

            try
            {
                //unholy sins (get the funny data)
                var rawUserData = await client.GetStringAsync("https://api.compensationvr.tk/api/accounts/" + searchedID + "/public");
                JObject UserDataBag = JObject.Parse(rawUserData);

                SearchedUsername.Text = UserDataBag["username"].ToString();
                SearchedNickname.Text = $"({UserDataBag["nickname"]})";
                SearchedPronouns.Text = UserDataBag["pronouns"].ToString();

                if (string.IsNullOrEmpty(UserDataBag["bio"].ToString()))
                {
                    SearchedBio.Text = UserDataBag["bio"].ToString();
                }
                else
                {
                    SearchedBio.Text = "No bio currently set.";
                }
            }
            catch{
                MessageBox.Show("No user found with that ID or connection error");
                SearchedUsername.Text = "No user found";
            }
        }
    }
}