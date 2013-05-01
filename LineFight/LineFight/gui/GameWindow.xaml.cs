using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

using UniversalNetwork;
using UniversalLobby.model;
using LineFight.model;

namespace LineFight.gui
{
	public partial class GameWindow : Window, UniversalLobby.iface.IUGame
	{
		private Game Controller;
		private DispatcherTimer CountDown;
		private Label lblCountdown;
		private Profile MyProfile;
		private LFNet Network;
		private Profile OpponentProfile;
		private DispatcherTimer Refresher;
		private int Remaining = 6;
		private bool Replay;

		private void btnAbandon_Click(object o, RoutedEventArgs e)
		{
			Refresher.Stop();
			this.Close();
		}

		private void CountDown_Tick(object o, EventArgs e)
		{
			//int Remaining = Convert.ToInt32(countDownlb.Content.ToString());
			if (Remaining > 0)
			{
				Remaining--;
				countDownlb.Content = Remaining;
			}
			else
			{
				CountDown.Stop();
				countDownlb.Visibility = System.Windows.Visibility.Hidden;
				NewGame();
			}
		}

		public void run() {

		}

		public void initialize(UniversalLobby.model.LFNet network, UniversalLobby.model.Profile profile) {

		}

		public GameWindow()
		{
			InitializeComponent();
			Refresher = new DispatcherTimer();
			Refresher.Interval = new TimeSpan(10);
			Refresher.Tick += new EventHandler(Refresher_Tick);
			CountDown = new DispatcherTimer();
			CountDown.Interval = new TimeSpan(10000000);
			CountDown.Tick += new EventHandler(CountDown_Tick);
			CountDown.Start();
			MyProfile = new Profile("AAA", new BitmapImage());
			OpponentProfile = new Profile("BBB", new BitmapImage());
			if (MyProfile.Username == null)
			{
				playerNamelb.Content = "Me";
			}
			else
			{
				playerNamelb.Content = MyProfile.Username;
			}
			if (OpponentProfile.Username == null)
			{
				opponentNamelb.Content = "Opponent";
			}
			else
			{
				opponentNamelb.Content = OpponentProfile.Username;
			}
			if (MyProfile.Avatar != null)
			{
				ImgMyProfile.Source = MyProfile.Avatar;
			}
			if (OpponentProfile.Avatar != null)
			{
				ImgOpponentProfile.Source = MyProfile.Avatar;
			}
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

		public void NewGame()
		{
			Refresher.Start();
			Controller = new Game(this);
			Controller.Start();
			Arena.Source = Controller.getArena();
		}

		private void Refresher_Tick(object o, EventArgs e)
		{
			if (Controller.IsEnded())
			{
				MessageBoxResult result = MessageBoxResult.No;
				if (Controller.IsLost() && Controller.IsWin())
				{
					result = MessageBox.Show("Draw! Wanna play again?", "Game over", MessageBoxButton.YesNo);
				}
				else if (Controller.IsLost())
				{
					result =  MessageBox.Show("You lost! Wanna play again?", "Game over", MessageBoxButton.YesNo);
				}
				else if (Controller.IsWin())
				{
					result = MessageBox.Show("You win! Wanna play again?", "Game over", MessageBoxButton.YesNo);
				}
				Refresher.Stop();
				if (result == MessageBoxResult.Yes /*&& opponentAnswer == "yes"*/)
				{
					NewGame();
				}
				else
				{
					this.Close();
				}
			}
		}

		public void opDisconnect()
		{
			MessageBox.Show("Opponent disconnected! You win.", "Game over");
			this.Close();
		}

		private void newGameBtn_Click(object sender, RoutedEventArgs e)
		{
			NewGame();
		}
	}
}
