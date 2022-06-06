using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Mime;
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
			Content = StackContent();	
			var clickMe = new Command {MenuText = "Click Me!", ToolBarText = "Click Me!"};
			clickMe.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");
			var quitCommand = new Command {MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q};
			quitCommand.Executed += (sender, e) => Application.Instance.Quit();
			SizeChanged += OnSizeChanged;
			var aboutCommand = new Command {MenuText = "About"};
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
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			StackLayout stack = (StackLayout) Content;
			if (stack.Items.Count > 0)
			{
				foreach (var item in stack.Items.OfType<StackLayoutItem>())
				{
					if (item.Control.GetType() == typeof(GridView))
					{
						item.Control.Width = Bounds.Width;
					}
				}
			}
		}

		public  StackLayout StackContent()
		{
			StackLayout stack = new StackLayout();
			stack.BackgroundColor = new Color(0.1f, 0.1f, 0.1f);
			TextBox callSign = new TextBox();
			Button button = new Button();
			GridView grid = new GridView();
			ProgressBar progress = new ProgressBar();
			progress.Indeterminate = true;
			button.Text = "Suchen";
			button.Click +=  async (s,e) => {
				progress.Visible = true;
				List<Flight> test = await APIRequest.Request("https://data-cloud.flightradar24.com/zones/fcgi/feed.js?faa=1&satellite=1&bounds=58.213%2C31.832%2C-23.618%2C38.828&mlat=1&flarm=1&adsb=1&gnd=1&air=1&vehicles=1&estimated=1&maxage=14400&gliders=1&callsign=" + callSign.Text +  "&pk=&stats=1");
				grid.DataStore = test;
				grid.Width = Bounds.Width;
				grid.Height = 150;
				progress.Visible = false;
				grid.Visible = true;
			};
			stack.Items.Add(callSign);
			stack.Items.Add(button);
			stack.Items.Add(progress);
			stack.Items.Add(grid);
			grid.Visible = false;
			progress.Visible = false;
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("fnac"), HeaderText = "Flight number and Callsign" });
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("airport.origin.code.icao"), HeaderText = "Departure" });
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("airport.destination.code.icao"), HeaderText = "Arrival" });
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("flightduration"), HeaderText = "Flight Duration" });
			grid.CellClick +=  (s, e)  => {
				Flight flight = e.Item as Flight;
				ContextMenu menu = new ContextMenu();
				MenuItem item = new Command().CreateMenuItem();
				item.Text = "Show livery";
				item.Click += async (sender, a) => {
					Dialog dialog = new Dialog();
					dialog.Title = "Livery";
					StackLayout layout = new StackLayout();
					layout.Items.Add(new ImageView() {Image = await Utils.Utils.GetPhoto(flight?.aircraft.registration.ToString())});
					dialog.Content = layout;
					await dialog.ShowModalAsync(this);
				};
				menu.Items.Add(item);

				if (e.Buttons == MouseButtons.Alternate)
				{
					menu.Show(grid, e.Location);
				}
			};
			grid.Width = Bounds.Width;
			return stack;
		}
	}
}
