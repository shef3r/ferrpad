using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace ferrpad
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    
    public sealed partial class MainPage : Page
    {
        StorageFile file {  get; set; }
        public MainPage()
        {

            this.InitializeComponent();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            Object value = localSettings.Values["FirstLaunchFinished"];
            Window.Current.SetTitleBar(ttb);
            if (value == null)
            {
                WelcomeDialog();
                localSettings.Values["FirstLaunchFinished"] = "done";
            }
        }

        private async void WelcomeDialog()
        {
            try
            {
                ContentDialog dialog = new ContentDialog() { PrimaryButtonText = "Let's get started!" };
                StackPanel desc = new StackPanel();
                desc.HorizontalAlignment = HorizontalAlignment.Center;
                desc.Children.Add(new Image() { Source = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/logowtext.png") }, Width = 300, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
                desc.Children.Add(new TextBlock() { Text = "Welcome to ferrpad!", FontSize = 30, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Segoe UI Variable"), HorizontalAlignment = HorizontalAlignment.Center });
                desc.Children.Add(new TextBlock() { Text = "the text document editor for Windows 10 and 11", FontSize = 20, FontFamily = new FontFamily("Segoe UI"), HorizontalAlignment = HorizontalAlignment.Center });
                dialog.Content = desc;
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                showerror(ex.ToString());
            }
        }

        private async void showerror(string error)
        {
            ContentDialog dialogerror = new ContentDialog();
            dialogerror.Title = "Oops, an error occured!";
            dialogerror.PrimaryButtonText = "OK";
            dialogerror.Content = error;
            await dialogerror.ShowAsync();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            text.Margin = new Thickness(0, comcont.ActualHeight, 0, bottombar.ActualHeight);
        }

        private void text_TextChanged(object sender, RoutedEventArgs e)
        {
            string textextract = text.Text;
            char[] delimiters = new char[] { ' ', '\r', '\n' };
            wordcount.Text = textextract.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length.ToString();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (text.CanUndo)
            {
                text.Undo();
            }

        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (text.CanRedo)
            {
                text.Redo();
            }
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            text.CopySelectionToClipboard();
            text.Text.Remove(text.Text.IndexOf(text.SelectedText));
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            text.CopySelectionToClipboard();
        }

        private void Paste_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (text.CanPasteClipboardContent)
            {
                text.PasteFromClipboard();
            }
        }

        private async void NotImplemented(string thing)
        {
            ContentDialog notimplemented = new ContentDialog() { PrimaryButtonText = "Okay ill wait lmaooo", Title = "Not " + thing + " not working :skull:", Content = thing + " doesnt work yet cause idk or am too lazy to implement it 💀", SecondaryButtonText = "Complain" };
            notimplemented.SecondaryButtonClick += Notimplemented_SecondaryButtonClick;
            await notimplemented.ShowAsync();
        }

        private async void Notimplemented_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/shef3r/ferrpad/issues/new"));
        }

        private void text_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".txt");

            file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                ApplicationView appView = ApplicationView.GetForCurrentView();
                appView.Title = file.Name;
                text.Text = await FileIO.ReadTextAsync(file);
                text.ClearUndoRedoHistory();
            }
            else
            {
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            await FileIO.WriteTextAsync(file, text.Text);
        }
    }
}
