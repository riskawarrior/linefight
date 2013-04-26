using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using UniversalNetwork;

namespace LineFight.model
{
    enum Facing { Up=0, Right=1, Down=2, Left=3 };

    class Game
    {
        private WriteableBitmap Arena;
        private Facing Facing = Facing.Down;
        private bool Lost = false;
        private DispatcherTimer Mover;
        private Color MyColor = Colors.Blue;
        private LFNet Network;
        private Color OpponentColor = Colors.Red;
        private Facing OpponentFacing = Facing.Down;
        private Point Position;
        private int Speed = 10;
        private bool Win = false;

        public WriteableBitmap getArena()
        {
            return Arena;
        }

        public Facing getFacing()
        {
            return Facing;
        }

        public bool IsEnded()
        {
            return Win || Lost;
        }

        public bool IsWin()
        {
            return Win;
        }

        public void Mover_Tick(object o, EventArgs e)
        {
            Point newPosition = Position;
            switch(Facing){
                case Facing.Down:
                    {
                        newPosition.Y++;
                        break;
                    }
                case Facing.Left:
                    {
                        newPosition.X--;
                        break;
                    }
                case Facing.Up:
                    {
                        newPosition.Y--;
                        break;
                    }
                case Facing.Right:
                    {
                        newPosition.X++;
                        break;
                    }
            }
            if (newPosition.X >= 0.0 && newPosition.Y >= 0.0 && newPosition.X<=500 && newPosition.Y<=500 && Arena.GetPixel(Convert.ToInt32(newPosition.X), Convert.ToInt32(newPosition.Y)) == Colors.Black)
            {
                Arena.DrawLine(Convert.ToInt32(Position.X.ToString()), Convert.ToInt32(Position.Y.ToString()), Convert.ToInt32(newPosition.X.ToString()), Convert.ToInt32(newPosition.Y.ToString()), MyColor);
                Position = newPosition;
            }
            else
            {
                Mover.Stop();
                Lost = true;
                MessageBox.Show("You lose!", "Game over");
            }
        }

        private void NetClientEventHandler(object o, NetClientEvent e)
        {
        
        }

        private void NetPackageReceiveHandler(object o, PackageReceived pr)
        { 
        
        }

        private void SendFacing(Facing f)
        {
            
        }

        public void Start()
        {
            Arena = BitmapFactory.New(500, 500);
            Arena.Clear(Colors.Black);
            Arena.SetPixel(5, 5, MyColor);
            Arena.SetPixel(495, 5, OpponentColor);
            Position = new Point(5,5);
            Mover = new DispatcherTimer();
            Mover.Interval = new TimeSpan(Speed);
            Mover.Tick += new EventHandler(Mover_Tick);            
            Mover.Start();
        }

        public void TurnLeft()
        {
            int f = (int)Facing;
            f=(--f+4)%4;
            Facing = (Facing)f;
            SendFacing(Facing);
        }

        public void TurnRight()
        {
            int f = (int)Facing;
            f=++f%4;
            Facing = (Facing)f;
            SendFacing(Facing);
        }
    }
}
