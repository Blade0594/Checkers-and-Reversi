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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rand = new Random(DateTime.Now.Millisecond);
        public MainWindow()
        {
            InitializeComponent();
            double random = rand.NextDouble();
            int difficulty = 0;
            if (random < 0.25)
                difficulty = 1;
            if (random > 0.25 && random < 0.5)
                difficulty = 2;
            if (random > 0.5 && random < 0.75)
                difficulty = 3;
            if (random > 0.75)
                difficulty = 4;
            switch (difficulty)
            {
                case 1: CheckersWindow.SearchDepth = 2; break;
                case 2: CheckersWindow.SearchDepth = 9; break;
                case 3: CheckersWindow.SearchDepth = 8; break;
                case 4: CheckersWindow.SearchDepth = 15; break;

            }
        }

        private void Reversi_Click(object sender, RoutedEventArgs e)
        {
            ReversiWindow.SearchDepth = 6;
            ReversiWindow reversi = new ReversiWindow();
            reversi.ShowDialog();
        }

        private void checkersButton_Click(object sender, RoutedEventArgs e)
        {
           
            CheckersWindow checkers = new CheckersWindow();
            checkers.ShowDialog();
        }

        private void SearchDepthCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem combo = SearchDepthCombo.Items[SearchDepthCombo.SelectedIndex] as ComboBoxItem;
            if (combo != null)
            {
                switch (combo.Tag.ToString()[0])
                {
                    case 'E':
                        ReversiWindow.SearchDepth = 2;
                        CheckersWindow.SearchDepth = 6;
                        break;
                    case 'M':
                        ReversiWindow.SearchDepth = 4;
                        CheckersWindow.SearchDepth = 6;
                        break;
                    case 'H':
                        ReversiWindow.SearchDepth = 6;
                        CheckersWindow.SearchDepth = 9;
                        break;
                    case 'A':
                        CheckersWindow.SearchDepth=8;
                        ReversiWindow.SearchDepth =4;
                        break;
                    case 'P':
                        CheckersWindow.SearchDepth = 15;
                        ReversiWindow.SearchDepth = 4;
                        break;
                }
            }
        }

        private void WhiteBlackPlayer_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (radio != null)
            {
                if (radio.Name == "WhitePlayer")
                {
                    ReversiWindow.ComputerPlayer = 1;
                    CheckersWindow.ComputerPlayer = 1;
                }
                else
                {
                    ReversiWindow.ComputerPlayer = -1;
                    CheckersWindow.ComputerPlayer = -1;
                }
            
            }

        }

       
    }
}
