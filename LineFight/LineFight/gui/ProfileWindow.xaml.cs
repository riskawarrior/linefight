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
using Microsoft.Win32;

namespace LineFight.gui
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        Profile profile;
        OpenFileDialog _fileDialog;
        BitmapImage _image;
        public ProfileWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            profile = new Profile(txtUserName.Text, _image) ;
            this.Close();
        }

        private void btnOpenAvatar_Click(object sender, RoutedEventArgs e)
        {
            _fileDialog = new OpenFileDialog();

            if (_fileDialog.ShowDialog() == true)
            {
                _image = new BitmapImage(new Uri(_fileDialog.FileName, UriKind.RelativeOrAbsolute));
                imgAvatar.Source = _image;
            }
        }

        private void txtUserName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
