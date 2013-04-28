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
using LineFight.model;
using UniversalNetwork;

namespace LineFight.gui {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Lobby : Window {
		LFNet _lfnet;
		Profile _profile;
		public Lobby() {
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (_profile == null)
			{
				ProfileWindow profile = new ProfileWindow();
				profile.ShowDialog();
			}
		}

		private void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			_lfnet.connect(txtHost.Text, Convert.ToInt32(txtPort.Text), _profile.Username, pwPassword.Password, false);
		}

		private void btnCreateGame_Click(object sender, RoutedEventArgs e)
		{
			GameWindow g = new GameWindow();
			g.Show();
		}

		private void btnDisconnect_Click(object sender, RoutedEventArgs e)
		{
			_lfnet.disconnect();
		}

		private void btnProfile_Click(object sender, RoutedEventArgs e)
		{
			ProfileWindow profile = new ProfileWindow();
			profile.ShowDialog();
		}

		private void pwPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{

		}

		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void NetClientEventHandler(object sender, NetClientEvent e)
		{

		}

		private void NetCoreEventHandler(object sender, NetCoreError e)
		{

		}

		private void NetPackageReceiveHandler(object sender, PackageReceived e)
		{

		}
	}
}
