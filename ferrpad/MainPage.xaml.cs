using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
            Window.Current.SetTitleBar(comcont);
            if (value == null)
            {
                WelcomeDialog();
                localSettings.Values["FirstLaunchFinished"] = "done";
            }
            ApplicationView.GetForCurrentView().Title = "New file";
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
            ContentDialog notimplemented = new ContentDialog() { PrimaryButtonText = "ill keep it to myself lmaooo", Title = "Damn you got it to crash :skull:", Content = "Here's the exception in case u want to report it or smth: " + error, SecondaryButtonText = "Complain" };
            notimplemented.SecondaryButtonClick += Notimplemented_SecondaryButtonClick;
            await notimplemented.ShowAsync();
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
            text.CutSelectionToClipboard();
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

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(".txt");

                file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    ApplicationView appView = ApplicationView.GetForCurrentView();
                    appName.Text = file.Name + " - ferrpad";
                    appView.Title = file.Name;
                    text.Text = await FileIO.ReadTextAsync(file);
                    text.ClearUndoRedoHistory();
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                showerror(ex.Message);
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (file != null)
                {
                    CachedFileManager.DeferUpdates(file);
                    await FileIO.WriteTextAsync(file, text.Text);
                    try { await CachedFileManager.CompleteUpdatesAsync(file); } catch { }
                }
                else
                {
                    var savePicker = new FileSavePicker
                    {
                        SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                    };

                    savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
                    savePicker.SuggestedFileName = "New Document";
                    file = await savePicker.PickSaveFileAsync();

                    if (file != null)
                    {
                        CachedFileManager.DeferUpdates(file);
                        await FileIO.WriteTextAsync(file, text.Text);
                        appName.Text = file.Name + " - ferrpad";
                        ApplicationView.GetForCurrentView().Title = file.Name;
                        try { await CachedFileManager.CompleteUpdatesAsync(file); } catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                showerror(ex.Message);
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            file = null;
            appName.Text = "ferrpad";
            ApplicationView.GetForCurrentView().Title = "New file";
            text.Text = "";
        }
    }
}
