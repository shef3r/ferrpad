using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Core;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using System.Windows.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AppUIBasics.TabViewPages
{
    public sealed partial class MyTabContentControl : UserControl
    {
        public MyTabContentControl()
        {
            this.InitializeComponent();
            var fonts = Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();
            fontscombo.ItemsSource = fonts;
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            Object value = localSettings.Values["FirstLaunchFinished"];

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
                desc.Children.Add(new TextBlock() { Text = "the rich text document editor for Windows 10 and 11", FontSize = 20, FontFamily = new FontFamily("Segoe UI"), HorizontalAlignment = HorizontalAlignment.Center });
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
            text.Margin = new Thickness(0, commands.ActualHeight, 0, bottombar.ActualHeight);
        }

        private void text_TextChanged(object sender, RoutedEventArgs e)
        {
            string textextract = null;
            text.TextDocument.GetText( Windows.UI.Text.TextGetOptions.None, out textextract);
            char[] delimiters = new char[] { ' ', '\r', '\n' };
            wordcount.Text = textextract.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length.ToString();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (((RichEditBox)text).TextDocument.CanUndo())
            {
                ((RichEditBox)text).TextDocument.Undo();
            }
            
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (((RichEditBox)text).TextDocument.CanRedo())
            {
                ((RichEditBox)text).TextDocument.Redo();
            }
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            if (text.TextDocument.CanCopy())
            {
                var range = text.TextDocument.GetRange(text.TextDocument.Selection.StartPosition, text.TextDocument.Selection.EndPosition);
                range.Cut();
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (text.TextDocument.CanCopy())
            {
                var range = text.TextDocument.GetRange(text.TextDocument.Selection.StartPosition, text.TextDocument.Selection.EndPosition);
                range.Copy();
            }
        }

        private async void Paste_ClickAsync(object sender, RoutedEventArgs e)
        {
            NotImplemented("pasting");
            Paste.IsEnabled = false;
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

        private void Bold_Click(object sender, RoutedEventArgs e)
        {
            var range = text.TextDocument.GetRange(text.TextDocument.Selection.StartPosition, text.TextDocument.Selection.EndPosition);
            if (Bold.IsChecked == true)
            {
                range.CharacterFormat.Bold = FormatEffect.On;
            }
            else
            {
                range.CharacterFormat.Bold = FormatEffect.Off;
            }
        }

        private void Italic_Click(object sender, RoutedEventArgs e)
        {
            var range = text.TextDocument.GetRange(text.TextDocument.Selection.StartPosition, text.TextDocument.Selection.EndPosition);
            if (Italic.IsChecked == true)
            {
                range.CharacterFormat.Italic = FormatEffect.On;
            }
            else
            {
                range.CharacterFormat.Italic = FormatEffect.Off;
            }
        }

        private void Underline_Click(object sender, RoutedEventArgs e)
        {
            var range = text.TextDocument.GetRange(text.TextDocument.Selection.StartPosition, text.TextDocument.Selection.EndPosition);
            if (Underline.IsChecked == true)
            {
                range.CharacterFormat.Underline = UnderlineType.Single;
            }
            else
            {
                range.CharacterFormat.Underline = UnderlineType.None;
            }
        }

        private void text_SelectionChanged(object sender, RoutedEventArgs e)
        {
            checkstyles();

        }

        private void checkstyles()
        {
            var range = text.TextDocument.GetRange(text.TextDocument.Selection.StartPosition, text.TextDocument.Selection.EndPosition + 1);
            if (range.Text == "\n" || range.Text == "\t" || range.Text == @" ")
            {
                Underline.IsEnabled = false;
                Bold.IsEnabled = false;
                Italic.IsEnabled = false;
                Underline.IsChecked = false;
                Bold.IsChecked = false;
                Italic.IsChecked = false;
                range.CharacterFormat.Bold = FormatEffect.Off;
                range.CharacterFormat.Italic = FormatEffect.Off;
                range.CharacterFormat.Underline = UnderlineType.None;
            }
            else
            {
                Underline.IsEnabled = true;
                Bold.IsEnabled = true;
                Italic.IsEnabled = true;
                Underline.IsChecked = false;
                Bold.IsChecked = false;
                Italic.IsChecked = false;

            }
            if (range.CharacterFormat.Underline == UnderlineType.Single)
            {
                Underline.IsChecked = true;
            }
            else
            {
                Underline.IsChecked = false;
            }
            if (range.CharacterFormat.Bold == FormatEffect.On)
            {
                Bold.IsChecked = true;
            }
            else
            {
                Bold.IsChecked = false;
            }
            if (range.CharacterFormat.Italic == FormatEffect.On)
            {
                Italic.IsChecked = true;
            }
            else
            {
                Italic.IsChecked = false;
            }
        }
    }
}
