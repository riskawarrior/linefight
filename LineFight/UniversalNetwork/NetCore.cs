using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace UniversalNetwork {

	/// <summary>
	/// A hálózati kapcsolatokat kezelő magas(abb) szintű osztály
	/// 
	/// A külvilággal eseményeken keresztü tartja a kapcsolatot. Ezek külön szálon futnak, a megfelelő kezelésükről gondoskodni kell.
	/// </summary>
	public class NetCore {
		/// <summary>
		/// Belső kommunikációra használt osztály.
		/// Egy ilyen példányban kerül elküldésre a szervernek a jelszó, illetve a válasz (elutasítva vagy elfogadva) is ebben érkezik meg.
		/// </summary>
		[Serializable]
		private class PasswordPackage {
			public bool? accepted;
			public string message;
			public string username;
			public string password;

			/// <summary>
			/// Konstruktor
			/// Inicializálja az objektumot a megadott paraméterekkel
			/// </summary>
			/// <param name="uname">Csatlakozó kliens neve</param>
			/// <param name="pass">Jelszó</param>
			public PasswordPackage(string uname, string pass = "") {
				accepted = null;
				message = "";
				username = uname;
				password = pass;
			}
		}

		/// <summary>
		/// Utasítás felsorolás belső használatra.
		/// A szerver küldi a kliensnek tájékoztatásul egy esemény folytán.
		/// </summary>
		private enum InternalCommand {
			Kick,
			ServerDisconnecting
		}

		/// <summary>
		/// Delegate a NetPackageReceiveHandler eseménykezelőjéhez
		/// </summary>
		/// <param name="sender">Küldő objektum</param>
		/// <param name="e">Fogadott csomag</param>
		public delegate void NetPackageReceiveHandler(object sender, PackageReceived e);

		/// <summary>
		/// Delegate a NetCoreErrorHandler eseménykezelőjéhez
		/// </summary>
		/// <param name="sender">Küldő objektum</param>
		/// <param name="e">Csomagolt hibaüzenet</param>
		public delegate void NetCoreErrorHandler(object sender, NetCoreError e);

		/// <summary>
		/// Delegate a NetClientEventHandler eseménykezelőjéhez
		/// </summary>
		/// <param name="sender">Küldő objektum</param>
		/// <param name="e">Kliens esemény csomag</param>
		public delegate void NetClientEventHandler(object sender, NetClientEvent e);

		/// <summary>
		/// Eseméykezelő hálózati csomag fogadásához
		/// </summary>
		public event NetPackageReceiveHandler ReceiveObservers;

		/// <summary>
		/// Eseménykezelő a hálózati kapcsolatban bekövetkező nem várt eseményhez
		/// A kivételkezelést hivatott kiváltani
		/// </summary>
		public event NetCoreErrorHandler NetError;

		/// <summary>
		/// Eseménykezelő a kliensek akcióinak külvilág felé történő közléséhez
		/// </summary>
		public event NetClientEventHandler NetClientEvent;

		/// <summary>
		/// Port engedélyezés
		/// </summary>
		private SocketPermission permission;

		/// <summary>
		/// A szerver figyelő socketje és a kliens socket
		/// </summary>
		private Socket listener, client;

		/// <summary>
		/// A kliens socketek listája (csak szervernél)
		/// </summary>
		private List<Socket> clients;

		/// <summary>
		/// A kliensek nickjeit tároló dictionary
		/// Kulcs az egyedi felhasználónév, érték a kliens socket
		/// </summary>
		private Dictionary<string, Socket> clientnames;

		/// <summary>
		/// A klienseket monitorozó szál
		/// <see cref="clientDisposer()"/>
		/// </summary>
		private Thread clientDisposerThread;

		/// <summary>
		/// A saját felhasználónév (szervernél és kliensnél egyaránt)
		/// </summary>
		protected string username;

		/// <summary>
		/// Jelszó (szervernél és kliensnél is eltárolásra kerül)
		/// </summary>
		private string password;

		/// <summary>
		/// Hostname
		/// Kliensek újracsatlakozásához van rá szükség.
		/// </summary>
		private string host;

		/// <summary>
		/// Port szám
		/// Kliensek újracsatlakozásához van rá szükség
		/// </summary>
		private int port;

		/// <summary>
		/// Ez a flag garantálja, hogy csak egyszer történik újracsatlakozási kísérlet a kapcsolat megszakadása után
		/// </summary>
		private bool cancelReconnect = false;

		/// <summary>
		/// Konstruktor
		/// A jelszót üresre állítja.
		/// </summary>
		public NetCore() {
			password = "";
		}

		/// <summary>
		/// Ha van felépített kapcsolat akkor igaz
		/// Szervernél akkor is igaz, ha az fogadja a kliensek kapcsolódását, de jelenleg nincs csatlakozott kliens
		/// </summary>
		/// <returns>Van kapcsolat?</returns>
		public bool hasConnection() {
			return client != null || listener != null;
		}

		/// <summary>
		/// Ha a kapcsolat ezen végpontja a szerver, akkor igaz
		/// Ha nincs kapcsolat, akkor hamis
		/// </summary>
		/// <returns>Szerver?</returns>
		public bool isServer() {
			return listener != null;
		}

		/// <summary>
		/// Ha a kapcsolat ezen végpontja a kliens, akkor igaz
		/// Ha nincs kapcsolat, akkor hamis
		/// </summary>
		/// <returns>Kliens?</returns>
		public bool isClient() {
			return client != null;
		}

		/// <summary>
		/// Beállítja a jelszót
		/// Szervernél a kapcsolatok fogadása közben is meghívható
		/// </summary>
		/// <param name="pass">Új jelszó</param>
		public void setPassword(string pass) {
			password = pass;
		}

		/// <summary>
		/// Lekérdezi a szerver hálózati címét.
		/// Alhálózatnál csak az alhálózaton kapott IP címet adja vissza
		/// </summary>
		/// <returns>IP cím</returns>
		public string getMyIp() {
			IPHostEntry host;
			string localIP = "?";
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList) {
				if (ip.AddressFamily == AddressFamily.InterNetwork) {
					localIP = ip.ToString();
				}
			}
			return localIP;
		}

		/// <summary>
		/// Lekérdezi a csatlakozott kliensek neveit
		/// </summary>
		/// <param name="addMyself">A szerver nevét beleveszi a listába</param>
		/// <returns>Kliensek neveinek tömbje</returns>
		public string[] getClientNames(bool addMyself = false) {
			List<string> result = clientnames.Keys.ToList();
			if (addMyself) {
				result.Add(username);
			}
			result.Sort();
			return result.ToArray();
		}

		/// <summary>
		/// Szerver kapcsolat megnyitása
		/// A szerver a sikeres nyitás után fogadja a kliensek kapcsolódási kísérleteit
		/// Elindítja a kliensek állapotát monitorozó szálat
		/// </summary>
		/// <param name="uname">Szerver felhasználóneve</param>
		/// <param name="port">Hallgatózó port</param>
		public virtual void openServer(string uname = "", int port = 33555) {
			if (hasConnection()) {
				disconnect();
			}

			username = uname;

			try {
				permission = new SocketPermission(
					NetworkAccess.Accept,     // Allowed to accept connections 
					TransportType.Tcp,        // Defines transport types 
					"",                       // The IP addresses of local host 
					port                      // Specifies all ports 
				);
				permission.Demand();

				IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);

				listener = new Socket(
					AddressFamily.InterNetwork,
					SocketType.Stream,
					ProtocolType.Tcp
				);
				listener.Bind(endPoint);
				listener.NoDelay = false;
				listener.Listen(10000);

				clients = new List<Socket>();
				clientnames = new Dictionary<string, Socket>();
				clientDisposerThread = new Thread(new ThreadStart(clientDisposer));
				clientDisposerThread.Start();

				listener.BeginAccept(new AsyncCallback(acceptConnection), listener);
			} catch (Exception ex) {
				listener = null;
				dispatchErrorEvent(new NetCoreError("Listening error: " + ex.Message));
			}
		}

		/// <summary>
		/// Elfogadja a bejövő kliens kapcsolatát
		/// Felkészül az adatfogadásra
		/// </summary>
		/// <param name="ar">Aszinkron paraméter</param>
		private void acceptConnection(IAsyncResult ar) {
			Socket handler = null, listener = null;

			try {
				listener = (Socket)ar.AsyncState;
				handler = listener.EndAccept(ar);
				clients.Add(handler);

				byte[] headerbuffer = new byte[4];
				object[] obj = new object[2];
				obj[0] = headerbuffer;
				obj[1] = handler;
				handler.BeginReceive(headerbuffer, 0, headerbuffer.Length, SocketFlags.None, new AsyncCallback(receiveHeader), obj);
				listener.BeginAccept(new AsyncCallback(acceptConnection), listener);
			} catch (Exception) {
				// client connection error
			}
		}

		/// <summary>
		/// Kapcsolódik a megadott szerverhez
		/// </summary>
		/// <param name="host">Szerver hostneve</param>
		/// <param name="port">Szerver portja</param>
		/// <param name="uname">Kliens felhasználóneve</param>
		/// <param name="password">Jelszó a szerverhez</param>
		/// <param name="isReconnect">Ez újracsatlakozási kísérlet?</param>
		public virtual void connect(string host, int port, string uname, string password = "", bool isReconnect = false) {
			if (hasConnection()) {
				disconnect();
			}

			username = uname;
			this.password = password;
			this.host = host;
			this.port = port;
			cancelReconnect = false;

			try {
				permission = new SocketPermission(
					NetworkAccess.Accept,     // Allowed to accept connections 
					TransportType.Tcp,        // Defines transport types 
					"",                       // The IP addresses of local host 
					port                      // Specifies all ports 
				);
				permission.Demand();

				IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(host), port);

				client = new Socket(
					AddressFamily.InterNetwork,
					SocketType.Stream,
					ProtocolType.Tcp
				);
				client.NoDelay = false;
				client.Connect(endPoint);

				byte[] headerbuffer = new byte[4];
				object[] obj = new object[2];
				obj[0] = headerbuffer;
				obj[1] = client;
				client.BeginReceive(headerbuffer, 0, headerbuffer.Length, SocketFlags.None, new AsyncCallback(receiveHeader), obj);

				send(new PasswordPackage(username, password));
			} catch (Exception ex) {
				client = null;
				if (isReconnect) {
					throw new Exception("Reconnect error!", ex);
				} else {
					dispatchErrorEvent(new NetCoreError("Connection error: " + ex.Message));
				}
			}
		}

		/// <summary>
		/// Kliens kapcsolat esetén megpróbál újracsatlakozni a szerverhez
		/// A kapcsolat megszakadása esetén használatos
		/// </summary>
		/// <param name="onFailMessage">A kapcsolat megszakadásának hibaüzenete</param>
		private void reconnect(string onFailMessage = "") {
			if (isServer()) {
				return;
			}

			if (hasConnection()) {
				disconnect();
			}

			if (cancelReconnect) {
				cancelReconnect = false;
				return;
			}
			Thread.Sleep(1000);	// reconnect waiting
			if (cancelReconnect) {
				cancelReconnect = false;
				return;
			}

			try {
				connect(host, port, username, password, true);
			} catch (Exception) {
				dispatchErrorEvent(new NetCoreError("Connection error: " + onFailMessage + "\nReconnecting failed!"));
			}
		}

		/// <summary>
		/// Adatok fejlécét fogadó metódus
		/// Felkészül a fejlécben kapott (bájt szám) adat fogadására
		/// </summary>
		/// <param name="ar">Aszinkron paraméter</param>
		private void receiveHeader(IAsyncResult ar) {
			object[] obj = new object[2];
			obj = (object[])ar.AsyncState;

			byte[] headerbuffer = (byte[])obj[0];
			Socket handler = (Socket)obj[1];

			try {
				handler.EndReceive(ar);
				int packetsize = BitConverter.ToInt32(headerbuffer, 0);
				if (packetsize == 0) {
					headerbuffer = new byte[4];
					obj = new object[2];
					obj[0] = headerbuffer;
					obj[1] = handler;
					handler.BeginReceive(headerbuffer, 0, headerbuffer.Length, SocketFlags.None, new AsyncCallback(receiveHeader), obj);
				} else {
					byte[] buffer = new byte[packetsize < 8192 ? packetsize : 8192];
					obj = new object[4];
					obj[0] = buffer;
					obj[1] = handler;
					obj[2] = packetsize;
					obj[3] = new byte[packetsize];
					handler.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveData), obj);
				}
			} catch (Exception) {
				if (hasConnection()) {
					if (isClient()) {
						reconnect("Header receive failed! Disconnecting...");
					} else {
						disconnect(handler);
					}
				}
			}
		}

		/// <summary>
		/// Fogadja a fejlécben meghatározott mennyiségű adatot több részletben
		/// Ha a csomag teljes egészében megérkezett, akkor kivált egy eseményt, és mellékeli a csomagot
		/// </summary>
		/// <param name="ar">Aszinkron paraméter</param>
		private void receiveData(IAsyncResult ar) {
			object[] obj = new object[2];
			obj = (object[])ar.AsyncState;

			byte[] buffer = (byte[])obj[0];
			Socket handler = (Socket)obj[1];
			int remaining = (int)obj[2];
			byte[] data = (byte[])obj[3];

			try {
				int received = handler.EndReceive(ar);

				Array.ConstrainedCopy(buffer, 0, data, data.Length - remaining, received);
				remaining -= received;

				if (remaining != 0) {
					buffer = new byte[remaining < 8192 ? remaining : 8192];
					obj = new object[4];
					obj[0] = buffer;
					obj[1] = handler;
					obj[2] = remaining;
					obj[3] = data;
					handler.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveData), obj);
				} else {
					MemoryStream stream = new MemoryStream(data);
					stream.Position = 0;
					BinaryFormatter formatter = new BinaryFormatter();
					object package = formatter.Deserialize(stream);

					byte[] headerbuffer = new byte[4];
					obj = new object[2];
					obj[0] = headerbuffer;
					obj[1] = handler;
					handler.BeginReceive(headerbuffer, 0, headerbuffer.Length, SocketFlags.None, new AsyncCallback(receiveHeader), obj);

					if (preProcessData(package, handler)) {
						dispatchPackageEvent(new PackageReceived(package));
					}
				}
			} catch (Exception) {
				if (hasConnection()) {
					if (isClient()) {
						reconnect("Receive failed! Disconnecting...");
					} else {
						disconnect(handler);
					}
				}
			}
		}

		/// <summary>
		/// A fogadott csomagot elő-feldolgozza
		/// Ha a csomag a NetCore belső kommunikációs csomagja, akkor feldolgozza és válaszol rá:
		/// - Szerver:
		/// -- Jelszó csomagnál ellenőrzi a jelszót
		/// -- Felhasználónév csomagnál ellenőrzi a felhasználónevet
		/// -- Ha minden rendben van felveszi a kliens listába
		/// - Kliens:
		/// -- Megvizsgálja a szerver válaszát, ha szükséges szétkapcsolódik
		/// Hibánál vagy kliens eseménynél kilövi a megfelelő eseményt
		/// Ha a fogadott csomag nem NetCore kommunkiációs csomag, akkor továbbítja egy eventtel
		/// </summary>
		/// <param name="package"></param>
		/// <param name="client"></param>
		/// <returns></returns>
		protected virtual bool preProcessData(object package, Socket client) {
			if (package is PasswordPackage) {
				PasswordPackage ppack = (PasswordPackage)package;
				if (isServer()) {
					if (ppack.password != password) {
						ppack.accepted = false;
						ppack.message = "Wrong password!";
						sendTo((object)ppack, client);
						disconnect(client);
					} else if (clientnames.ContainsKey(ppack.username) || username == ppack.username) {
						ppack.accepted = false;
						ppack.message = "Your username has been taken already!";
						sendTo((object)ppack, client);
						disconnect(client);
					} else {
						ppack.accepted = true;
						clientnames.Add(ppack.username, client);
						dispatchClientEvent(new NetClientEvent(ClientEventType.connected, ppack.username));
						sendTo((object)ppack, client);
					}
				} else {
					if (!(bool)ppack.accepted) {
						cancelReconnect = true;
						disconnect();
						dispatchErrorEvent(new NetCoreError(ppack.message));
					}
				}
				return false;
			} else if (package is InternalCommand) {
				switch ((InternalCommand)package) {
					case InternalCommand.Kick:
						cancelReconnect = true;
						disconnect();
						dispatchErrorEvent(new NetCoreError("You have been kicked!"));
						break;
					case InternalCommand.ServerDisconnecting:
						cancelReconnect = true;
						disconnect();
						dispatchErrorEvent(new NetCoreError("Server closed connection!"));
						break;
				}
				return false;
			}

			return true;
		}

		/// <summary>
		/// Elküldi a csomagot
		/// Kliensnél a szervernek, szervernél minden kliensnek
		/// </summary>
		/// <param name="package">Küldendő <b>serializable</b> csomag</param>
		public virtual void send(object package) {
			if (!hasConnection()) {
				return;
			}

			byte[] data = prepareData(package);

			if (isClient()) {
				try {
					client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(sendCallback), client);
				} catch (Exception) {
					if (hasConnection()) {
						reconnect("Send failed! Disconnecting...");
					}
				}
			} else {
				foreach (Socket sock in clients.ToArray()) {
					try {
						sock.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(sendCallback), sock);
					} catch (Exception) {
						disconnect(sock);
					}
				}
			}
		}

		/// <summary>
		/// Elküldi a csomagot
		/// Csak szervernél: a megadott felhasználónak továbbítja
		/// </summary>
		/// <param name="package">Küldendő <b>serializable</b> csomag</param>
		/// <param name="user">Cél kliens felhasználóneve</param>
		public virtual void sendTo(object package, string user) {
			if (!hasConnection() || isClient() || !clientnames.ContainsKey(user)) {
				return;
			}

			byte[] data = prepareData(package);
			Socket sock = clientnames[user];

			try {
				sock.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(sendCallback), sock);
			} catch (Exception) {
				disconnect(sock);
			}
		}

		/// <summary>
		/// Elküldi a csomagot (belső használat)
		/// A megadott socketre küldi (csak szerver)
		/// </summary>
		/// <param name="package">Küldendő <b>serializable</b> csomag</param>
		/// <param name="sock">Kliens socket</param>
		private void sendTo(object package, Socket sock) {
			if (!hasConnection() || isClient()) {
				return;
			}

			byte[] data = prepareData(package);

			try {
				sock.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(sendCallback), sock);
			} catch (Exception) {
				disconnect(sock);
			}
		}

		/// <summary>
		/// Elküldi a csomagot
		/// Csak szervernél: a megadott felhasználót kivéve mindenkinek elküldi a csomagot
		/// </summary>
		/// <param name="package">Küldendő <b>serializable</b> csomag</param>
		/// <param name="user">A 'kimaradó' felhasználó</param>
		public virtual void sendExclude(object package, string user) {
			if (!hasConnection() || isClient() || !clientnames.ContainsKey(user)) {
				return;
			}

			byte[] data = prepareData(package);
			Socket excl = clientnames[user];

			foreach (Socket sock in clients.ToArray()) {
				try {
					if (sock != excl) {
						sock.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(sendCallback), sock);
					}
				} catch (Exception) {
					disconnect(sock);
				}
			}
		}

		/// <summary>
		/// Felkészíti a kapott adatot küldésre
		/// A szerializálható objektumot bájt tömbbé alakítja
		/// </summary>
		/// <param name="obj"><b>Serializable</b> objektum</param>
		/// <returns>Szerializált objektum</returns>
		private byte[] prepareData(object obj) {
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();

			formatter.Serialize(stream, obj);
			byte[] data = stream.ToArray();
			byte[] header = BitConverter.GetBytes(data.Length);

			byte[] tosend = new byte[data.Length + header.Length];
			header.CopyTo(tosend, 0);
			data.CopyTo(tosend, header.Length);

			return tosend;
		}

		/// <summary>
		/// A küldést lezáró aszinkron metódus
		/// </summary>
		/// <param name="ar">Aszinkron paraméter</param>
		private void sendCallback(IAsyncResult ar) {
			Socket handler = (Socket)ar.AsyncState;
			try {
				handler.EndSend(ar);
			} catch (Exception) {
				if (hasConnection()) {
					if (isClient()) {
						reconnect("Send failed! Disconnecting...");
					} else {
						disconnect(handler);
					}
				}
			}
		}

		/// <summary>
		/// Bontja a kapcsolatot
		/// Kliensnél: lecsatlakozik a szerverről
		/// Szervernél: bontja minden klienssel a kapcsolatot, majd zárja a listenert
		/// </summary>
		public virtual void disconnect() {
			if (!hasConnection()) {
				return;
			}

			if (isServer()) {
				try {
					clientDisposerThread.Abort();
				} catch (Exception) { }
				//listener.Shutdown(SocketShutdown.Both);
				listener.Close();
				foreach (Socket client in clients) {
					if (client.Connected) {
						sendTo(InternalCommand.ServerDisconnecting, client);
						try {
							client.Shutdown(SocketShutdown.Both);
						} catch (Exception) { }
						client.Close();
					}
				}
				listener = null;
			} else if (isClient()) {
				if (client.Connected) {
					try {
						client.Shutdown(SocketShutdown.Both);
					} catch (Exception) { }
					client.Close();
				}
				client = null;
			}
		}

		/// <summary>
		/// Bontja a kapcsolatot a megadott sockettel (szervernél)
		/// </summary>
		/// <param name="client">Kliens socket</param>
		protected virtual void disconnect(Socket client) {
			if (!hasConnection()) {
				return;
			}

			if (isClient()) {
				disconnect();
			} else {
				try {
					client.Shutdown(SocketShutdown.Both);
				} catch (Exception) { }
				client.Close();
			}
		}

		/// <summary>
		/// Másodpercenként átvizsgálja a klienseket
		/// Ha az egyik kliens állapota nem csatlakozott, akkor eltávolítja a kliensek listájából
		/// </summary>
		private void clientDisposer() {
			while ((true)) {
				Thread.Sleep(1000);
				for (int i = 0; i < clients.Count; i++) {
					if (!clients[i].Connected) {
						removeClient(clients[i]);
					}
				}
			}
		}

		/// <summary>
		/// Eltávolítja a klienst a kliensek listáiból
		/// Ha a kliens bekerült a felhasználóneveket tartalmazó dictionaryba (felépített kapcsolat), akkor eltávolítja
		/// Eltávolítja a klienst a socketek listájából
		/// </summary>
		/// <param name="client"></param>
		private void removeClient(Socket client) {
			string user = null;
			foreach (string item in clientnames.Keys.ToArray()) {
				if (clientnames[item] == client) {
					user = item;
				}
			}
			if (user != null) {
				clientnames.Remove(user);
				dispatchClientEvent(new NetClientEvent(ClientEventType.disconnected, user));
			}
			clients.Remove(client);
		}

		/// <summary>
		/// Bontja a kapcsolatot a megadott nevű felhasználóval
		/// </summary>
		/// <param name="username">Felhasználónév</param>
		/// <returns>Kirúgás sikeres?</returns>
		public virtual bool kick(string username) {
			if (clientnames.ContainsKey(username)) {
				sendTo(InternalCommand.Kick, username);
				disconnect(clientnames[username]);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Elküldi a fogadott csomagot egy eventben
		/// </summary>
		/// <param name="e">Csomag</param>
		private void dispatchPackageEvent(PackageReceived e) {
			if (ReceiveObservers != null) {
				ReceiveObservers(this, e);
			}
		}

		/// <summary>
		/// Elküldi a hibaüzenetet egy eventben
		/// </summary>
		/// <param name="e">Hibaüzenet</param>
		private void dispatchErrorEvent(NetCoreError e) {
			if (NetError != null) {
				NetError(this, e);
			}
		}

		/// <summary>
		/// Elküldi a kliens eseményt egy eventben
		/// </summary>
		/// <param name="e">Kliens esemény</param>
		private void dispatchClientEvent(NetClientEvent e) {
			if (NetClientEvent != null) {
				NetClientEvent(this, e);
			}
		}
	}

	/// <summary>
	/// A fogadott csomagot tartalmazó event
	/// </summary>
	public class PackageReceived : EventArgs {
		/// <summary>
		/// Fogadott csomag
		/// </summary>
		public object pack;

		/// <summary>
		/// Konstruktor
		/// Eltárolja a csomagot
		/// </summary>
		/// <param name="pack">Csomag</param>
		public PackageReceived(object pack) {
			this.pack = pack;
		}
	}

	/// <summary>
	/// Hibaüzenetet tartalmazó event
	/// </summary>
	public class NetCoreError : EventArgs {
		/// <summary>
		/// Hibaüzenet szövegesen
		/// </summary>
		public string error;

		/// <summary>
		/// Hibakód
		/// </summary>
		public int code;

		/// <summary>
		/// Konstruktor
		/// Eltárolja a hibaüzenetet és a hibakódot
		/// </summary>
		/// <param name="error">Hibaüzenet</param>
		/// <param name="code">Hibakód</param>
		public NetCoreError(string error, int code = -1) {
			this.error = error;
			this.code = code;
		}
	}

	/// <summary>
	/// Kliens események
	/// Újonnan csatlakozott kliens: connected
	/// Lekapcsolódott kliens: disconnected
	/// </summary>
	public enum ClientEventType {
		connected, disconnected
	}

	/// <summary>
	/// Kliens eseményt tartalmazó event
	/// </summary>
	public class NetClientEvent : EventArgs {
		/// <summary>
		/// Kliens esemény
		/// </summary>
		public ClientEventType ev;

		/// <summary>
		/// Felhasználónév, akihez az esemény kapcsolódik
		/// </summary>
		public string username;

		/// <summary>
		/// Konstruktor
		/// Eltárolja a kliens eseményt és a felhasználónevet, akihez az esemény tartozik
		/// </summary>
		/// <param name="ev">Kliens esemény</param>
		/// <param name="username">Felhasználónév</param>
		public NetClientEvent(ClientEventType ev, string username) {
			this.ev = ev;
			this.username = username;
		}
	}
}
