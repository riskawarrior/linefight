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

namespace LineFight.gui {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Lobby : Window {
		public Lobby() {
			InitializeComponent();
		}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GameWindow g = new GameWindow();
            g.Show();
        }

	}
}
