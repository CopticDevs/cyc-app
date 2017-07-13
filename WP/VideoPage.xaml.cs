using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Phone.Shell;

namespace CYC
{
    public partial class VideoPage : PhoneApplicationPage
    {
        public VideoPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            video.CurrentStateChanged += new RoutedEventHandler(video_CurrentStateChanged);
            video.PlaybackTrackChanged += new EventHandler<TrackChangedEventArgs>(video_PlaybackTrackChanged);
            video.SmoothStreamingSource = new Uri("http://cycnow.az-streamingserver.com:3333/live/cyc1/Manifest");
        }

        void video_PlaybackTrackChanged(object sender, TrackChangedEventArgs e)
        {
            currentBitrate.Text = "(" + e.NewTrack.Bitrate.ToString() + " bps)";
        }

        void video_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            status.Text = video.CurrentState.ToString();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (video.CurrentState != SmoothStreamingMediaElementState.Closed)
            {
                //This should simply stop the playback
                video.Stop();
            }
            else
            {
                // set stream source
                video.SmoothStreamingSource = new Uri("http://cycnow.az-streamingserver.com:3333/live/cyc1/Manifest");
            }
            //We should also reflect the chang on the play button
            PlayButton.Content = "Play";
            PlayButton.Style = (Style)this.Resources["PlayButtonStyle"];
        }

        private void PlayButton_Loaded(object sender, RoutedEventArgs e)
        {
            switch (video.AutoPlay)
            {
                case false:
                    PlayButton.Content = "Play";
                    PlayButton.Style = (Style)this.Resources["PlayButtonStyle"];
                    break;
                case true:
                    PlayButton.Content = "Pause";
                    PlayButton.Style = (Style)this.Resources["PauseButtonStyle"];
                    break;
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            //Monitor the state of the content to determine the right action to take on this button being clicked
            //and then change the text to reflect the next action
            switch (video.CurrentState)
            {
                case SmoothStreamingMediaElementState.Playing:
                    video.Pause();
                    PlayButton.Content = "Play";
                    PlayButton.Style = (Style)this.Resources["PlayButtonStyle"];
                    break;
                case SmoothStreamingMediaElementState.Stopped:
                case SmoothStreamingMediaElementState.Paused:
                    video.Play();
                    PlayButton.Content = "Pause";
                    PlayButton.Style = (Style)this.Resources["PauseButtonStyle"];
                    break;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            video.SmoothStreamingSource = new Uri("http://cycnow.az-streamingserver.com:3333/live/cyc1/Manifest");

            // Keep the phone awake
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Allow the phone to sleep
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
        }

    }
}