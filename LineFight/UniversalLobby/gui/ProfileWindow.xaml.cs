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
			ClosingWindow(true);
		}

		private bool ClosingWindow(bool useClose)
		{
			bool yesorno=false;
			if (txtUserName.Text == "")
			{
				var answer = MessageBox.Show("Biztos, hogy ki akarsz lépni?", "Kilépés", MessageBoxButton.OKCancel, MessageBoxImage.Question);
				if (answer == MessageBoxResult.OK)
				{
					Application.Current.Shutdown();
				}
				else
				{
					yesorno = true;
				}
			}
			else
			{
				_profile.Username = txtUserName.Text;
				_profile.Avatar = _image;
				if (useClose) this.Close();
			}
			return yesorno;
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

		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = ClosingWindow(false);
		}
	}
}
