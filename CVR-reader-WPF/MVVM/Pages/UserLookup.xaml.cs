using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            //get userID
            var rawUserIDlookup = await client.GetStringAsync("https://api.compensationvr.tk/api/accounts/" + SearchID + "/id");

            //parse out userID
            JObject UserIDBag = JObject.Parse(rawUserIDlookup);


            //convert searched username to numberic id
            var parsedUserID = UserIDBag["id"].ToString();

            //unholy sins (get the funny data)
            var rawUserData = await client.GetStringAsync("https://api.compensationvr.tk/api/accounts/" + parsedUserID + "/public");
            JObject UserDataBag = JObject.Parse(rawUserData);

            SearchedUsername.Text = UserDataBag["username"].ToString();
            SearchedNickname.Text = "(" + UserDataBag["nickname"].ToString() + ")";

            if (UserDataBag["bio"] != null)
            {
                SearchedBio.Text =  UserDataBag["bio"].ToString();
            }
            else
            {
                SearchedBio.Text = "No bio set";
            }

        }

    }
}
