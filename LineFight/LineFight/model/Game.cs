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
    enum Facing {Up, Down, Left, Right};

    class Game
    {
        private WriteableBitmap Arena;
        private Facing Facing;
        private bool Lost;
        private DispatcherTimer Mover;
        private Color MyColor;
        private LFNet Network;
        private Color OpponentColor;
        private Facing OpponentFacing;
        private Point Position;
        private int Speed;
        private bool Win;

        public WriteableBitmap getArena()
        {
            return Arena;
        }

        public bool IsEnded()
        {
        
            return false;
        }

        public bool IsWin()
        {
            return false;
        }

        public void Mover_Tick(object o, EventArgs e)
        {
        
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
            //Arena.GetBitmapContext();
        }

        public void draw()
        {
            //Arena.GetBitmapContext();
            Arena.Clear(Colors.Aquamarine);
            Arena.SetPixel(5, 5, Colors.Blue);
            Arena.SetPixel(495, 5, Colors.Red);
            Arena.DrawEllipse(200, 200, 400, 400, Colors.Black);
            
        }

        public void TurnLeft()
        {
        
        }

        public void TurnRight()
        {
        
        }
    }
}
