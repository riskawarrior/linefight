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

namespace LineFight.gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Lobby : Window
    {
        LFNet _lfnet;
        Profile _profile;
        public Lobby()
        {
            InitializeComponent();
            _lfnet.NetClientEvent += new NetCore.NetClientEventHandler(NetClientEventHandler);
            _lfnet.NetError += new NetCore.NetCoreErrorHandler(NetCoreEventHandler);
            _lfnet.ReceiveObservers += new NetCore.NetPackageReceiveHandler(NetPackageReceiveHandler);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _profile = new Profile("",new BitmapImage());
            ProfileWindow profile = new ProfileWindow(_profile);
            profile.ShowDialog();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            _lfnet.connect(txtHost.Text, Convert.ToInt32(txtPort.Text), _profile.Username, pwPassword.Password);
        }

        private void btnCreateGame_Click(object sender, RoutedEventArgs e)
        {
            _lfnet.openServer(_profile.Username, Convert.ToInt32(txtPort.Text));
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            _lfnet.disconnect();
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
            //e.ev == ClientEventType.
        }

        private void NetCoreEventHandler(object sender, NetCoreError e)
        {
            MessageBox.Show(e.error, "Hiba!", MessageBoxButton.OK);
        }

        private void NetPackageReceiveHandler(object sender, PackageReceived e)
        {

        }
    }
}
