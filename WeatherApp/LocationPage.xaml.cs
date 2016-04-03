using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace WeatherApp
{

    //Converter bool --> opposite bool 
    //(for isEnabled property of textbox, when checkbox is checked)
    public class BoolToOppositeBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            string language)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            throw new NotSupportedException();
        }

        #endregion
    }




    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationPage : Page
    {
        public LocationPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            txtBoxCity.Text = e.Parameter.ToString(); //show city name in textbox
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed; //add action when pressing the back button
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed; //remove the action on leaving page
        }

       
        async void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true; //handled button pressed

            //what to do when going back
            //if all is okay
            if (!(txtBoxCity.Text == String.Empty && chkBoxUseLocation.IsChecked == false))
            {
                Messenger.Default.Send(new NotificationMessage(txtBoxCity.Text));

                if (Frame.CanGoBack)
                    Frame.GoBack();
            }
            else //if city is not set, but the checkbox for using location is not  checked
            {
                var dialog = new MessageDialog("Enter city!");
                await dialog.ShowAsync();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // remove text from textbox if the checkbox is checked
            txtBoxCity.Text = "";
        }

        private async void btnDone_Click(object sender, RoutedEventArgs e) //same as back button
        {
            if (!(txtBoxCity.Text == String.Empty && chkBoxUseLocation.IsChecked == false))
            {
                Messenger.Default.Send(new NotificationMessage(txtBoxCity.Text));

                if (Frame.CanGoBack)
                    Frame.GoBack();
            }
            else
            {
                var dialog = new MessageDialog("Enter city!");
                await dialog.ShowAsync();
            }
        }

        private void TxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //clear textbox when it gets in focus
            txtBoxCity.Text = "";
        }
    }
}
