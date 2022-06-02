using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using flightsearcher.API;
using flightsearcher.Models;
using Flurl.Util;

namespace flightsearcher
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			Title = "FlightSearcher";
			MinimumSize = new Size(500, 300);
			StackLayout stack = new StackLayout();
			stack.Orientation = Orientation.Horizontal;
			stack.Spacing = Bounds.Width / 10;
			TextBox callSign = new TextBox();
			Button button = new Button();
			GridView grid = new GridView();
			button.Text = "Suchen";
			button.Click +=  async (s,e) => {
				List<Flight> test = await APIRequest.Request("https://data-cloud.flightradar24.com/zones/fcgi/feed.js?faa=1&satellite=1&bounds=58.213%2C31.832%2C-23.618%2C38.828&mlat=1&flarm=1&adsb=1&gnd=1&air=1&vehicles=1&estimated=1&maxage=14400&gliders=1&callsign=" + callSign.Text +  "&pk=&stats=1&type=A320");
				grid.DataStore = test;
				grid.Width = Bounds.Width;
				stack.Orientation = Orientation.Vertical;
				grid.Visible = true;
			};
			stack.Items.Add(callSign);
			stack.Items.Add(button);
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("fnac"), HeaderText = "Flight number and Callsign" });
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("airport.origin.code.icao"), HeaderText = "Departure" });
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("airport.destination.code.icao"), HeaderText = "Arrival" });
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("flightduration"), HeaderText = "Flight Duration" });
			grid.Visible = false;
			stack.Items.Add(grid);
			Content = stack;
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
			Menu = new MenuBar
			{
				QuitItem = quitCommand,
				AboutItem = aboutCommand
			};
			ToolBar = new ToolBar { Items = {  } };
		}
	}
}
