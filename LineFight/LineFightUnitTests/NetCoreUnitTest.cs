using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniversalNetwork;

namespace LineFightUnitTests
{
    [TestClass]
    public class NetCoreUnitTest : UniversalNetwork.NetCore
    {

        // A tesztekhez szükséges adattagok
        NetCore testClient; // a NetCoreUnitTest maga egy szerver lesz, ez lesz a kliens


        /// <summary>
        /// Konstruktor a tesztesetekhez szükséges adattagok incializálásához.
        /// </summary>
        public NetCoreUnitTest()
        {
            testClient = new NetCore();
        }

        /// <summary>
        /// Nem létező kapcsolat tesztelése.
        /// </summary>
        [TestMethod]
        public void Test_NC_001_hasConnection()
        {
            Assert.AreEqual(false, hasConnection());
        }

        /// <summary>
        /// Nem létező kapcsolat esetén a kliens státusz tesztelése.
        /// </summary>
        [TestMethod]
        public void Test_NC_002_isClient()
        {
            Assert.AreEqual(false, isClient());
        }

        /// <summary>
        /// Nem létező kapcsolat esetén a szerver státusz tesztelése.
        /// </summary>
        [TestMethod]
        public void Test_NC_003_isServer()
        {
            Assert.AreEqual(false, isServer());
        }

        /// <summary>
        /// A getMyIP függvény tesztelése.
        /// </summary>
        [TestMethod]
        public void Test_NC_004_getMyIp()
        {
            // lekérjük az ip-címet "manuálisan"
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            //kiértékelés
            Assert.AreEqual(localIP, getMyIp());
        }

        /// <summary>
        /// Nem létező kapcsolat esetén a getClientNames függvény tesztelése.
        /// </summary>
        [TestMethod]
        public void Test_NC_005_getClientNames()
        {
            Assert.AreEqual(0, getClientNames(false).Length);
        }

        /// <summary>
        /// Szerver indítása, isServer tesztelése létező kapcsolat esetén.
        /// </summary>
        [TestMethod]
        public void Test_NC_006_openServer_isServer_hasConnection()
        {
            openServer("test_server");
            Assert.AreEqual(true, isServer());
            Assert.AreEqual(true, hasConnection());
        }

        /// <summary>
        /// Szerver mód esetén az isClient függvény tesztelése.
        /// </summary>
        [TestMethod]
        public void Test_NC_007_isClient()
        {
            Assert.AreEqual(false, isClient());
        }

        /// <summary>
        /// A kliens oldali connect és isClient eljárás tesztelése
        /// </summary>
        [TestMethod]
        public void Test_NC_008_connect_isClient()
        {
            testClient.connect(getMyIp(), 33555, "test_client", "test_password");
            Assert.AreEqual(true, testClient.isClient());
            Assert.AreEqual(false, testClient.isServer());
        }

        /// <summary>
        /// Létező kliens kapcsolat esetén az isServer tesztelése.
        /// </summary>
        [TestMethod]
        public void Test_NC_009_isServer()
        {
            Assert.AreEqual(false, testClient.isServer());
        }

        /// <summary>
        /// Létező kliens kapcsolat esetén az isServer tesztelése.
        /// </summary>
        [TestMethod]
        public void Test_NC_010_hasConnection()
        {
            Assert.AreEqual(false, testClient.hasConnection());
        }

    }
}
