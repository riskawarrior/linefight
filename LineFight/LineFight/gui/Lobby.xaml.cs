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

		/// <summary>
		/// Tesztelő metódos a ConfirmReady dialógusablakhoz. Ha útban van dobd ki nyugodtan.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void test_confirmopen_Click(object sender, RoutedEventArgs e) {
			LineFight.model.LFNet net = new model.LFNet();

			ConfirmReady window = new ConfirmReady(net);
			window.ShowDialog();
		}
	}
}
