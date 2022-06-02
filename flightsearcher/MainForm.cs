using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Flurl.Util;

namespace flightsearcher
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			Title = "FlightSearcher";
			MinimumSize = new Size(500, 500);

			StackLayout stack = new StackLayout();
			
			
			TextBox callsign = new TextBox();
			Button button = new Button();
			button.Text = "Click me!";
			button.Click +=  async (s,e) => {
				string test = await new APIRequest().Request("https://data-cloud.flightradar24.com/zones/fcgi/feed.js?faa=1&satellite=1&mlat=1&flarm=1&adsb=1&gnd=1&air=1&vehicles=1&estimated=1&maxage=14400&gliders=1&callsign=" + callsign.Text +  "&pk=&stats=1");
				
			};
			
			stack.Items.Add(callsign);
			stack.Items.Add(button);
			
			Content = stack;

			// create a few commands that can be used for the menu and toolbar
			var clickMe = new Command { MenuText = "Click Me!", ToolBarText = "Click Me!" };
			clickMe.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");

			var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
			quitCommand.Executed += (sender, e) => Application.Instance.Quit();

			var aboutCommand = new Command { MenuText = "About" };
			aboutCommand.Executed += (sender, e) =>
			{
				AboutDialog dialog = new AboutDialog();
				dialog.Version = "1.0";
				dialog.Copyright = "\u00A9 Luca-Miguel Christiansen 2022";
				dialog.ProgramName = "FlightSearcher";
				dialog.ShowDialog(this);
			};

			// create menu
			Menu = new MenuBar
			{
				QuitItem = quitCommand,
				AboutItem = aboutCommand
			};

			// create toolbar			
			ToolBar = new ToolBar { Items = { clickMe } };
		}
	}
}
