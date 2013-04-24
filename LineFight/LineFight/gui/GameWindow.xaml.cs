using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using LineFight.model;
using UniversalNetwork;

namespace LineFight.gui
{
    public partial class GameWindow : Window
    {
        private Image Arena;
        private Button btnAbandon;
        private Game Controller;
        private DispatcherTimer CountDown;
        private Image ImgMyProfile;
        private Image ImgOpponentProfile;
        private Label lblCountdown;
        private Profile MyProfile;
        private LFNet Network;
        private Profile OpponentProfile;
        private DispatcherTimer Refresher;
        private int Remaining;
        private bool Replay;

        private void btnAbandon_Click(object o, RoutedEventArgs e)
        { 
            
        }

        private void CountDown_Tick()
        { 
        
        }

        public GameWindow()
        {
            InitializeComponent();

        }

        private void GameWindow_KeyPress(object o, KeyboardEventArgs e)
        { 
        
        }

        private void NetClientEventHandler(object o, NetClientEvent e)
        { 
            
        }

        public void NetPackageReceiveHandler(object o, PackageReceived pr)
        { 
        
        }

        public void NewGame()
        {
            Controller = new Game();
            Controller.Start();
            gameImg.Source = Controller.getArena();
            Controller.draw();
            //Controller.getArena().GetBitmapContext();
            gameImg.UpdateLayout();
        }

        private void Refresher_Tick(object o, EventArgs e)
        { 
            
        }
    }
}
