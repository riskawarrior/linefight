using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalLobby.iface
{
	public interface IUGame
	{
		void initialize(model.LFNet network, model.Profile profile);
		void run();
	}
}
