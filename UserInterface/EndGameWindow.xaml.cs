using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using UserInterface.Utils;
namespace UserInterface
{
    /// <summary>
    /// Interaction logic for EndGame.xaml
    /// </summary>
    public partial class EndGameWindow : Window
    {
        private int valence = 0;
        private int arousal = 0;
        private Image lastValenceImageCliked;
        private Image lastArousalImageClicked;
        CheckersWindow chekersWindow;
        public EndGameWindow(CheckersWindow window)
        {
            InitializeComponent();
            this.chekersWindow = window;
        }

        private void VPValence_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            if (img != null)
            {
                if (lastValenceImageCliked != null)
                    lastValenceImageCliked.Opacity = 0.5;
                valence = int.Parse(img.Tag.ToString());
                lastValenceImageCliked = img;
                img.Opacity = 1;

            }
            else
            {
                MessageBox.Show("Something went wrong");
            }
        }

        private void VPArousal_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            if (img != null)
            {
                if (lastArousalImageClicked != null)
                    lastArousalImageClicked.Opacity = 0.5;
                arousal = int.Parse(img.Tag.ToString());
                lastArousalImageClicked = img;
                img.Opacity = 1;

            }
            else
            {
                MessageBox.Show("Something went wrong");
            }

        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (lastValenceImageCliked == null || lastArousalImageClicked == null)
            {
                MessageBox.Show("Please select a rating for happiness and arousal", "Rate the game", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to submit?", "Submit?", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation,
                                                 MessageBoxResult.Cancel);


            if (result == MessageBoxResult.OK)
            {
                FileStream file;
                StreamWriter writer;
                StringBuilder sb = new StringBuilder();
                sb.Append(chekersWindow.ToString());
                sb.Append(this.valence.ToString() + ' ');
                sb.Append(this.arousal.ToString() + ' ');
                if (commentTextBox.ToString() != string.Empty)
                    sb.Append(commentTextBox.Text + ' ');
                sb.Append(DateTime.Now.ToLongDateString() + ' ');
                sb.Append(DateTime.Now.ToLongTimeString());
                string encrypted = CryptorEngine.EncryptString(sb.ToString());
                try
                {
                    file = new FileStream("PlayerResults.txt", FileMode.Append, FileAccess.Write);
                    writer = new StreamWriter(file);
                    writer.WriteLine(encrypted);
                    writer.Flush();
                    writer.Close();
                    file.Close();
                }
                catch (IOException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            this.Close();

        }

    }
}
