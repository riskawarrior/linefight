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
			_profile = new Profile("",new BitmapImage());
			ProfileWindow profile = new ProfileWindow(_profile);
			profile.ShowDialog();

			_lfnet = new LFNet(_profile);
			_lfnet.NetClientEvent += NetClientEventHandler;
			_lfnet.NetError += NetCoreEventHandler;
			_lfnet.ReceiveObservers += NetPackageReceiveHandler;
		}

		private void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			if (_profile.Username != "")
			{
				if (!String.IsNullOrEmpty(txtHost.Text) && !String.IsNullOrEmpty(txtPort.Text))
				{
					btnConnect.IsEnabled = false;
					btnCreateGame.IsEnabled = false;
					btnDisconnect.IsEnabled = true;
					_lfnet.connect(txtHost.Text, Int32.Parse(txtPort.Text), _profile.Username, pwPassword.Password);
					
				}
				else
				{
					MessageBox.Show("A host és port kitöltése kötelező!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else
			{
				ProfileWindow profile = new ProfileWindow(_profile);
				profile.ShowDialog();
			}
		}

		private void btnCreateGame_Click(object sender, RoutedEventArgs e)
		{
			if (_profile.Username != "")
			{
				btnConnect.IsEnabled = false;
				btnCreateGame.IsEnabled = false;
				btnDisconnect.IsEnabled = true;
				_lfnet.openServer(_profile.Username);
				
			}
			else
			{
				ProfileWindow profile = new ProfileWindow(_profile);
				profile.ShowDialog();
			}
			
		}

		private void btnDisconnect_Click(object sender, RoutedEventArgs e)
		{
			_lfnet.disconnect();
			btnConnect.IsEnabled = true;
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
			if (e.ev == ClientEventType.connected)
			{
				if (_lfnet.isServer() && _lfnet.getClientNames().Length > 1)
				{
					_lfnet.kick(e.username);
				}
				else
				{
					_lfnet.send(_profile);
					game.initialize(_lfnet, _profile);
					game.run();
				}
				
			}
			else if (e.ev == ClientEventType.disconnected)
			{
				_lfnet.disconnect();
				btnConnect.IsEnabled = true;
			}
		}

		private void NetCoreEventHandler(object sender, NetCoreError e)
		{
			MessageBox.Show(e.error, "Hiba!", MessageBoxButton.OK);
			btnConnect.IsEnabled = true;
			btnCreateGame.IsEnabled = true;
			btnDisconnect.IsEnabled = false;
		}

		private void NetPackageReceiveHandler(object sender, PackageReceived e)
		{
			if (((Profile)e.pack).Username != _profile.Username)
			{
				
			}
		}
	}
}
