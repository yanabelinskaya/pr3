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
using System.Windows.Shapes;

namespace пр3
{
    public partial class Window1 : Window
    {
        private List<string> listenedTracks;
        public Window1(List<string> listenedTracks)
        {
            InitializeComponent();
            this.listenedTracks = listenedTracks;
            PopulateHistoryListBox();
        }

        private void PopulateHistoryListBox()
        {
            foreach(string track in listenedTracks)
            {
                listBox.Items.Add(track);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
