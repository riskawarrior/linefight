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

using UniversalLobby.model;

namespace UniversalLobby.gui
{
	/// <summary>
	/// Interaction logic for ProfileWindow.xaml
	/// </summary>
	public partial class ProfileWindow : Window
	{
		Profile _profile;
		BitmapImage _image;
		
		public ProfileWindow(Profile profile)
		{
			_profile = profile;
			InitializeComponent();
			txtUserName.Text = _profile.Username;
			imgAvatar.Source = _profile.Avatar;
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			if (txtUserName.Text == "")
			{
				MessageBox.Show("The username is required.");
			}
			else
			{
				_profile.Username = txtUserName.Text;
				this.Close();
			}
		}

		private void btnOpenAvatar_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog _fileDialog = new Microsoft.Win32.OpenFileDialog();

			if (_fileDialog.ShowDialog() == true)
			{
				_image = new BitmapImage(new Uri(_fileDialog.FileName, UriKind.RelativeOrAbsolute));
				imgAvatar.Source = _image;
				_profile.Avatar = _image;
			}
		}

		private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
		{
			_profile.Username = txtUserName.Text;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (txtUserName.Text == "") {
				var answer = MessageBox.Show("Username is missing. Are you sure to exit?", "Exiting", MessageBoxButton.OKCancel, MessageBoxImage.Question);
				if (answer == MessageBoxResult.OK) {
					e.Cancel = false;
					Environment.Exit(0);
				} else {
					e.Cancel = true;
				}
			} else {
				_profile.Username = txtUserName.Text;
			}
			
		}
	}
}
