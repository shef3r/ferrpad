using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AppUIBasics.TabViewPages
{
    public sealed partial class MyTabContentControl : UserControl
    {
        public MyTabContentControl()
        {
            this.InitializeComponent();
            
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
    }
}
