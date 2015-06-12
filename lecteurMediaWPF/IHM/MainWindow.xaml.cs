using System;
using System.Collections.Generic;
using System.Linq;
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
using BLL;
using BO;
using System.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace IHM
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MgtVideo mgt = MgtVideo.getInstance();
        private bool userIsDraggingSlider = false;

        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

        }

        private MediaState GetMediaState(MediaElement myMedia)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = hlp.GetValue(myMedia);
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)stateField.GetValue(helperObject);
            return state;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((myPlayer.Source != null) && (myPlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = myPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = myPlayer.Position.TotalSeconds;
            }
        }

        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            Video video = (Video)(sender as Grid).DataContext;
            myPlayer.Visibility = System.Windows.Visibility.Visible;
            myPlayer.Source = new Uri(video.Chemin);
            myPlayer.Play();
            volumeSlider.Value = 100;
            stackPanelControl.Visibility = System.Windows.Visibility.Visible;
            statusBartime.Visibility = System.Windows.Visibility.Visible;
        }

        #region MEDIA

        // Pause the media.
        void OnMouseDownPauseMedia(object sender, MouseButtonEventArgs args)
        {
            // The Pause method pauses the media if it is currently running.
            // The Play method can be used to resume.
            if (GetMediaState(myPlayer) == MediaState.Pause)
            {
                myPlayer.Play();
                image_pause.Visibility = System.Windows.Visibility.Hidden;
                btn_pause.Source = new BitmapImage(new Uri(@"images/Pause_button_24.png", UriKind.Relative));
                
            }
            else
            {
                myPlayer.Pause();
                image_pause.Visibility = System.Windows.Visibility.Visible;
                btn_pause.Source = new BitmapImage(new Uri(@"images/Player_24.png", UriKind.Relative));
            }
        }

        // Stop the media.
        void OnMouseDownStopMedia(object sender, MouseButtonEventArgs args)
        {

            // The Stop method stops and resets the media to be played from
            // the beginning.
            myPlayer.Stop();
            myPlayer.Visibility = System.Windows.Visibility.Hidden;
            stackPanelControl.Visibility = System.Windows.Visibility.Hidden;
            statusBartime.Visibility = System.Windows.Visibility.Hidden;
            image_pause.Visibility = System.Windows.Visibility.Hidden;

        }


        // Change the volume of the media.
        void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            myPlayer.Volume = (int)volumeSlider.Value*10;
        }

        void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> args)
        {

        }

        // When the media playback is finished. Stop() the media to seek to media start.
        private void Element_MediaEnded(object sender, EventArgs e)
        {
            myPlayer.Stop();
            myPlayer.Visibility = System.Windows.Visibility.Hidden;
            stackPanelControl.Visibility = System.Windows.Visibility.Hidden;
            statusBartime.Visibility = System.Windows.Visibility.Hidden;
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            myPlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }

        #endregion

        private void grid_KeyDown_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (myPlayer.Source != null && e.Key == Key.Space) 
            {
               
                if (GetMediaState(myPlayer) == MediaState.Pause)
                    {
                        myPlayer.Play();
                        image_pause.Visibility = System.Windows.Visibility.Hidden;
                        btn_pause.Source = new BitmapImage(new Uri(@"images/Pause_button_24.png", UriKind.Relative));

                    }
                else
                    {
                        myPlayer.Pause();
                        image_pause.Visibility = System.Windows.Visibility.Visible;
                        btn_pause.Source = new BitmapImage(new Uri(@"images/Player_24.png", UriKind.Relative));
                    }  
                
            }
        }
    }
}
