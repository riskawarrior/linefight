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
    }
}
