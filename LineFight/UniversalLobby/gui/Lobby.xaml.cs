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

using UniversalNetwork;
using UniversalLobby.model;
using UniversalLobby.iface;

namespace UniversalLobby.gui
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Lobby : Window
	{
		LFNet _lfnet;
		Profile _profile;
		iface.IUGame game;

		public Lobby(iface.IUGame game)
		{
			this.game = game;
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_profile = new Profile("", null);
			ProfileWindow profile = new ProfileWindow(_profile);
			profile.ShowDialog();

			_lfnet = new LFNet(_profile);
			_lfnet.NetClientEvent += NetClientEventHandler;
			_lfnet.NetError += NetCoreEventHandler;
			_lfnet.ReceiveObservers += NetPackageReceiveHandler;
		}

		private void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			if (!String.IsNullOrEmpty(txtHost.Text) && !String.IsNullOrEmpty(txtPort.Text))
			{
				btnProfile.IsEnabled = btnConnect.IsEnabled = false;
				btnCreateGame.IsEnabled = false;
				btnDisconnect.IsEnabled = true;
				_lfnet.connect(txtHost.Text, Int32.Parse(txtPort.Text), _profile.Username, pwPassword.Password);
			}
			else
			{
				MessageBox.Show("A host és port kitöltése kötelező!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void btnCreateGame_Click(object sender, RoutedEventArgs e)
		{
			btnProfile.IsEnabled = btnConnect.IsEnabled = false;
			btnCreateGame.IsEnabled = false;
			btnDisconnect.IsEnabled = true;
			_lfnet.openServer(_profile.Username);
		}

		private void btnDisconnect_Click(object sender, RoutedEventArgs e)
		{
			_lfnet.disconnect();
			btnProfile.IsEnabled = btnConnect.IsEnabled = true;
			btnCreateGame.IsEnabled = true;
			btnDisconnect.IsEnabled = false;
		}

		private void btnProfile_Click(object sender, RoutedEventArgs e)
		{
			ProfileWindow profile = new ProfileWindow(_profile);
			profile.ShowDialog();
		}

		private void pwPassword_PasswordChanged(object sender, RoutedEventArgs e)
		{
			_lfnet.setPassword(pwPassword.Password);
		}

		private void btnExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void NetClientEventHandler(object sender, NetClientEvent e)
		{
			this.Dispatcher.Invoke((Action)(() => {
				if (e.ev == ClientEventType.disconnected)
				{
					MessageBox.Show("Kapcsolat megszakadt.");
					btnProfile.IsEnabled = btnConnect.IsEnabled = true;
					btnCreateGame.IsEnabled = true;
					btnDisconnect.IsEnabled = false;
					_lfnet.disconnect();
				}
			}));
		}

		private void NetCoreEventHandler(object sender, NetCoreError e)
		{
			this.Dispatcher.Invoke((Action)(() => {
				MessageBox.Show(e.error, "Hiba!", MessageBoxButton.OK);
				if (_lfnet.hasConnection()) {
					_lfnet.disconnect();
				}
				btnProfile.IsEnabled = btnConnect.IsEnabled = true;
				btnCreateGame.IsEnabled = true;
				btnDisconnect.IsEnabled = false;
			}));
		}

		private void NetPackageReceiveHandler(object sender, PackageReceived e)
		{
			this.Dispatcher.Invoke((Action)(() => {
				if (e.pack is string) {
					if ((string)e.pack == "START") {
						if (_lfnet.isClient()) {
							_lfnet.send("START");
						}
						game.initialize(_lfnet, _profile);
						game.run();
					}
				}
			}));
		}
	}
}
