using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace UniversalLobby.model {
	public class LFNet : UniversalNetwork.NetCore {
		private Profile profile;
		private Profile opponentProfile;

		private long ping;

		[Serializable]
		class TimePack {
			public long Time { get; set; }

			public TimePack(long time) {
				this.Time = time;
			}
		}

		public LFNet(Profile prof):base()
		{
			profile = prof;
			opponentProfile = null;

			this.NetClientEvent += LFNet_NetClientEvent;
		}

		public Profile getProfile() {
			return this.profile;
		}

		public Profile getOpponentProfile() {
			return this.opponentProfile;
		}

		public long getPingInTicks() {
			return ping;
		}

		void LFNet_NetClientEvent(object sender, UniversalNetwork.NetClientEvent e) {
			if (e.ev == UniversalNetwork.ClientEventType.connected) {
				if (getClientNames().Length > 1) {
					kick(e.username);
					return;
				}

				send(profile);
			} else if (e.ev == UniversalNetwork.ClientEventType.disconnected) {
				if (getClientNames().Length == 0) {
					opponentProfile = null;
				}
			}
		}

		public void connect(string host, int port, string username, string password)
		{
			base.connect(host, port, username, password);
		}

		public override void disconnect() {
			base.disconnect();
			opponentProfile = null;
		}

		protected override bool preProcessData(object package, Socket client)
		{
			bool l = base.preProcessData(package, client);
			if (l) {
				if (package is Profile) {
					this.opponentProfile = (Profile)package;
					l = false;

					if (isClient()) {
						send(profile);
					} else {
						send(new TimePack(DateTime.Now.Ticks));
					}
				} else if (package is TimePack) {
					l = false;
					if (isClient()) {
						send(package);
					} else {
						ping = (DateTime.Now.Ticks - ((TimePack)package).Time) / 2;
						send("START");
					}
				}

			}
			return l;
		}
	}
}
