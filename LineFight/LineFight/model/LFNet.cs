﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace LineFight.model {
	public class LFNet : UniversalNetwork.NetCore {
		Profile profile;
		public LFNet(Profile prof):base()
		{
			profile.Username = prof.Username;
			profile.Avatar = prof.Avatar;
		}
		public void connect(string host, int port, string username, string password, bool isReconnect)
		{
			base.connect(host, port, username, password, isReconnect);
		}

		protected bool preProcessData(object package, Socket client)
		{
			return base.preProcessData(package, client);
		}
	}
}
