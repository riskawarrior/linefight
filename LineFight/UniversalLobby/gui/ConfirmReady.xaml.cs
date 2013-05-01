using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using UniversalLobby.model;

namespace UniversalLobby.gui {
	/// <summary>
	/// Interaction logic for ConfirmReady.xaml
	/// </summary>
	public partial class ConfirmReady : Window {
		private LFNet network;
		private int Remaining;
		private DispatcherTimer Timer;

		public bool DialogResult { get; set; }

		public ConfirmReady(LFNet network) {
			InitializeComponent();

			this.network = network;
			Remaining = 300;
			DialogResult = false;
			Timer.Interval = new TimeSpan(1000000);
			Timer.Tick += Timer_Tick;
			Timer.IsEnabled = true;
		}

		private void Timer_Tick(object sender, EventArgs e) {
			Remaining -= 1;

		}
	}
}
