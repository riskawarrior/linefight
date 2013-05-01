using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace UniversalLobby.model {
	public class LFNet : UniversalNetwork.NetCore {
		Profile profile;
		public LFNet(Profile prof):base()
		{
			profile = prof;
		}
		public void connect(string host, int port, string username, string password)
		{
			base.connect(host, port, username, password);
		}

		protected bool preProcessData(object package, Socket client)
		{
			return base.preProcessData(package, client);
			//this.send()
		}
	}
}
