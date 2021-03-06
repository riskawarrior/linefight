﻿using System;
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
		private Profile MyProfile;
		private LFNet Network;
		private Profile OpponentProfile;
		private DispatcherTimer Refresher;
		private int Remaining = 6;
		private String replay;

		public String getReplay()
		{
			return replay;
		}

        public void setReplay(String r)
        {
            replay = r;
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
				Controller = new Game(this);
				CountDown.Stop();
				countDownlb.Visibility = System.Windows.Visibility.Hidden;
				Controller.Start();
				Arena.Source = Controller.getArena();
				NewGame();
                Remaining = 6;
			}
		}

		public LFNet getNetwork()
		{
			return Network;
		}

		public void run() 
		{
            countDownlb.Visibility = System.Windows.Visibility.Visible;
			this.Show();
			CountDown.Start();
		}

		public void initialize(UniversalLobby.model.LFNet network, UniversalLobby.model.Profile profile)
		{
            replay = "None";
            Network = network;
			MyProfile = profile;
			OpponentProfile = Network.getOpponentProfile();
			Refresher = new DispatcherTimer();
			Refresher.Interval = new TimeSpan(100);
			Refresher.Tick += new EventHandler(Refresher_Tick);
			CountDown = new DispatcherTimer();
			CountDown.Interval = new TimeSpan(10000000);
			CountDown.Tick += new EventHandler(CountDown_Tick);
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
				ImgOpponentProfile.Source = OpponentProfile.Avatar;
			}
		}

		public GameWindow()
		{
			InitializeComponent();
			this.Closing += GameWindow_Closing;
		}

        private void GameWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Network.disconnect();
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
                    result = MessageBox.Show("You lost! Wanna play again?", "Game over", MessageBoxButton.YesNo);
				}
				else if (Controller.IsWin())
				{
                    result = MessageBox.Show("You win! Wanna play again?", "Game over", MessageBoxButton.YesNo);
				}
				Refresher.Stop();
                //this.Hide();
                if (result == MessageBoxResult.Yes)
                {
                    Pack p = new Pack(packNames.Replay, "Yes");
                    Network.send(p);

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        if (replay == "Yes")
                        {
                            initialize(Network, MyProfile);
                            run();
                        }
                        else if(replay == "No")
                        {
                            this.Hide();
                        }
                    }));
                }
                else
                {
                    Pack p = new Pack(packNames.Replay, "No");
                    Network.send(p);
                    this.Hide();
                }
			}
		}

		public void opDisconnect()
		{
			MessageBox.Show("Opponent disconnected! You win.", "Game over");
			this.Hide();
		}
	}
}
