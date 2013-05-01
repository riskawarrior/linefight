using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

using LineFight.gui;
using UniversalNetwork;
using UniversalLobby.model;

namespace LineFight.model
{
    enum Facing { Up=0, Right=1, Down=2, Left=3 };
    enum packNames { Facing, End};

    [Serializable]
    class Pack
    {
        public packNames packName { get; set; }
        public object content { get; set; }

        public Pack(packNames p, object o)
        {
            packName = p;
            content = o;
        }
    }

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
        private Point OpponentPosition;
        private int Speed = 10;
        private bool Win = false;
        private int myPosX = 5;
        private int myPosY = 5;
        private int opPosX = 495;
        private int opPosY = 5;
        private GameWindow gameWindow;

        public Game(GameWindow gWindow)
        {
            gameWindow = gWindow;
            //Network.NetClientEvent +=new NetCore.NetClientEventHandler(NetClientEventHandler);
            //Network.ReceiveObservers += new NetCore.NetPackageReceiveHandler(NetPackageReceiveHandler);
        }

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

        public bool IsLost()
        {
            return Lost;
        }

        public void Mover_Tick(object o, EventArgs e)
        {
            Point newPosition = Position;
            switch (Facing)
            {
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
            Point newOpPosition = OpponentPosition;
            switch (OpponentFacing)
            {
                case Facing.Down:
                    {
                        newOpPosition.Y++;
                        break;
                    }
                case Facing.Left:
                    {
                        newOpPosition.X--;
                        break;
                    }
                case Facing.Up:
                    {
                        newOpPosition.Y--;
                        break;
                    }
                case Facing.Right:
                    {
                        newOpPosition.X++;
                        break;
                    }
            }
            bool felt1 = newPosition.X >= 0.0 && newPosition.Y >= 0.0 && newPosition.X <= 500 && newPosition.Y <= 500 && Arena.GetPixel(Convert.ToInt32(newPosition.X), Convert.ToInt32(newPosition.Y)) == Colors.Black;
            bool felt2 = newOpPosition.X >= 0.0 && newOpPosition.Y >= 0.0 && newOpPosition.X <= 500 && newOpPosition.Y <= 500 && Arena.GetPixel(Convert.ToInt32(newOpPosition.X), Convert.ToInt32(newOpPosition.Y)) == Colors.Black;
            if (!felt1 && !felt2)
            {
                Mover.Stop();
                Lost = true;
                Win = true;
                Pack p = new Pack(packNames.End, "Draw");
                //Network.send(p);
            }
            else
            {
                if (felt1)
                {
                    Arena.DrawLine(Convert.ToInt32(Position.X.ToString()), Convert.ToInt32(Position.Y.ToString()), Convert.ToInt32(newPosition.X.ToString()), Convert.ToInt32(newPosition.Y.ToString()), MyColor);
                    Position = newPosition;
                }
                else
                {
                    Mover.Stop();
                    Lost = true;
                    Pack p = new Pack(packNames.End, "Lost");
                    Network.send(p);
                }
                if (felt2)
                {
                    Arena.DrawLine(Convert.ToInt32(OpponentPosition.X.ToString()), Convert.ToInt32(OpponentPosition.Y.ToString()), Convert.ToInt32(newOpPosition.X.ToString()), Convert.ToInt32(newOpPosition.Y.ToString()), OpponentColor);
                    OpponentPosition = newOpPosition;
                }
                else
                {
                    Mover.Stop();
                    Win = true;
                    Pack p = new Pack(packNames.End, "Win");
                    Network.send(p);
                }
            }
        }

        private void NetClientEventHandler(object o, NetClientEvent e)
        {
            if (e.ev == ClientEventType.disconnected)
            {
                gameWindow.opDisconnect();
            }
        }

        private void NetPackageReceiveHandler(object o, PackageReceived pr)
        {
            if (((Pack)pr.pack).packName == packNames.Facing)
            {
                OpponentFacing = (Facing)((Pack)pr.pack).content;
            }
            else if (((Pack)pr.pack).packName == packNames.End)
            {
                if (((Pack)pr.pack).content == "Win")
                {
                    Win = false;
                }
                else if (((Pack)pr.pack).content == "Lost")
                {
                    Lost = false;
                }
                else
                {
                    Lost = true;
                    Win = true;
                }
            }
        }

        private void SendFacing(Facing f)
        {
            Pack p = new Pack(packNames.Facing, f);
            Network.send(p);
        }

        public void Start()
        {

            Win = false;
            Lost = false;
            Arena = BitmapFactory.New(500, 500);
            Arena.Clear(Colors.Black);
            Arena.SetPixel(myPosX, myPosY, MyColor);
            Arena.SetPixel(opPosX, opPosY, OpponentColor);
            Position = new Point(myPosX, myPosY);
            OpponentPosition = new Point(opPosX, opPosY);
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
