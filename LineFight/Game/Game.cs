using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game {
	class Game {
		[STAThread]
		public static void Main(string[] args) {
			UniversalLobby.iface.IUGame lineFight = new LineFight.gui.GameWindow();
			UniversalLobby.gui.Lobby lobby = new UniversalLobby.gui.Lobby(lineFight);
			lobby.ShowDialog();
		}
	}
}
