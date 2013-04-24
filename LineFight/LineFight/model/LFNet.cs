using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace LineFight.model {
	public class LFNet : UniversalNetwork.NetCore {
		Profile profile;
		public LFNet(Profile prof)
		{
			profile = prof;
		}
		public void connect(string host, int port, string username, string password, bool isReconnect)
		{

		}

		protected bool preProcessData(object package, Socket client)
		{
			return base.preProcessData(package, client);
		}
	}
}
