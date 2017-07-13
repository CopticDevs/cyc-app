using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Web.Media.SmoothStreaming;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace CYC
{
    public partial class MainPage : PhoneApplicationPage
    {
        enum PivotIndex
        {
            Stream,
            Twitter,
            About
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            NavigationService.Navigate(new Uri("/VideoPage.xaml", UriKind.Relative));
        }

        private void RefreshAppBarBtn_Click(object sender, EventArgs e)
        {
            switch (this.PagePanorama.SelectedIndex)
            {
                case (int)PivotIndex.Twitter:
                    this.RefreshTweets();
                    break;
                case (int)PivotIndex.About:
                default:
                    break;
            }
        }

        private void PagePanorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.RefreshApplicationBar();

            if ((this.PagePanorama.SelectedIndex == (int)PivotIndex.Twitter)
                && (this.twitterListBox.Items.Count == 0))
            {
                this.RefreshTweets();
            }
        }


        /// <summary>
        /// Shows/Hides the progress bar
        /// </summary>
        /// <param name="IsEnabled">Shows the progress bar when set to true</param>
        private void UpdateProgressBar(bool IsEnabled)
        {
            if (IsEnabled)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // show progress bar
                    this.FeedFetchProgress.IsEnabled = true;
                    this.FeedFetchProgress.Visibility = System.Windows.Visibility.Visible;
                    this.FeedFetchOpacity.Visibility = System.Windows.Visibility.Visible;
                });
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // hide progress bar
                    this.FeedFetchProgress.IsEnabled = false;
                    this.FeedFetchProgress.Visibility = System.Windows.Visibility.Collapsed;
                    this.FeedFetchOpacity.Visibility = System.Windows.Visibility.Collapsed;
                });
            }
        }

        private void RefreshTweets()
        {
            this.UpdateProgressBar(true);

            WebClient twitter = new WebClient();

            twitter.DownloadStringCompleted += new DownloadStringCompletedEventHandler(twitter_DownloadStringCompleted);
            twitter.DownloadStringAsync(new System.Uri("https://api.twitter.com/1/statuses/user_timeline.xml?include_rts=true&screen_name=cycnow&count=200"));
        }

        void twitter_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // Showing the exact error message is useful for debugging. In a finalized application, 
                    // output a friendly and applicable string to the user instead. 
                    MessageBox.Show("There was a problem trying to get tweets.  Please hit the refresh button to try again.");
                });
            }
            else
            {
                XElement xmlTweets = XElement.Parse(e.Result);

                twitterListBox.ItemsSource = from tweet in xmlTweets.Descendants("status")
                                             select new TwitterItem
                                             {
                                                 ImageSource = tweet.Element("user").Element("profile_image_url").Value,
                                                 Message = tweet.Element("text").Value,
                                                 UserName = tweet.Element("user").Element("screen_name").Value,
                                                 PublishedTime = DateTime.ParseExact(tweet.Element("created_at").Value, "ddd MMM dd HH:mm:ss zzz yyyy", System.Globalization.CultureInfo.InvariantCulture)
                                             };
            }

            this.UpdateProgressBar(false);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.RefreshApplicationBar();
            //readingsBrowser.Navigate(new Uri("http://cycnow.com/site/readings-of-the-day?tmpl=component", UriKind.Absolute));
        }

        /// <summary>
        /// Update the application bar buttons based on selected pivot item
        /// </summary>
        private void RefreshApplicationBar()
        {
            if ((this.PagePanorama.SelectedIndex == (int)PivotIndex.Twitter))
            {
                // Ensure the button is added
                if (this.ApplicationBar.Buttons.Count == 0)
                {
                    ApplicationBarIconButton refreshButton = new ApplicationBarIconButton(new Uri("/Images/ApplicationBar.Refresh.png", UriKind.Relative));
                    refreshButton.Click += new EventHandler(RefreshAppBarBtn_Click);
                    refreshButton.Text = "refresh";

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this.ApplicationBar.IsVisible = true;
                        this.ApplicationBar.Buttons.Add(refreshButton);
                    });
                }
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.ApplicationBar.IsVisible = false;
                    this.ApplicationBar.Buttons.Clear();
                });
            }
        }

        private void ausEmailHL_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailcomposer = new EmailComposeTask();
            emailcomposer.To = "australia@cycnow.com";
            emailcomposer.Subject = "Australian Donation (sent from WP7 App)";
            emailcomposer.Body = "\n\nSent from Windows Phone 7 CYC app";
            emailcomposer.Show();
        }
    }
}