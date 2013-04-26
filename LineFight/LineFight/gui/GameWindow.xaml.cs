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
            this.Close();
        }

        private void CountDown_Tick()
        { 
        
        }

        public GameWindow()
        {
            InitializeComponent();

            //playerNamelb.Content = MyProfile.Username;
            //opponentNamelb.Content = OpponentProfile.Username;
            /*if (MyProfile.Avatar != null)
            {
                playerImg.Source = MyProfile.Avatar;
            }*/
            NewGame();
            

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Down && Controller.getFacing() == Facing.Left) || (e.Key == Key.Left && Controller.getFacing() == Facing.Up) || (e.Key == Key.Up && Controller.getFacing() == Facing.Right) || (e.Key == Key.Right && Controller.getFacing() == Facing.Down))
            {
                Controller.TurnLeft();
            }
            else if ((e.Key == Key.Down && Controller.getFacing() == Facing.Right) || (e.Key == Key.Left && Controller.getFacing() == Facing.Down) || (e.Key == Key.Up && Controller.getFacing() == Facing.Left) || (e.Key == Key.Right && Controller.getFacing() == Facing.Up))
            {
                Controller.TurnRight();
            }
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
        }

        private void Refresher_Tick(object o, EventArgs e)
        { 
            
        }

        private void newGameBtn_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void gameImg_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
    }
}
