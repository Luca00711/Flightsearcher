using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using flightsearcher.API;
using flightsearcher.Models;

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
			Button button = new Button();
			GridView grid = new GridView();
			DropDown airlines = new DropDown();
			airlines.LoadComplete += async (sender, e) =>
			{
				airlines.DataStore = await Utils.Utils.GetAirlines();
				airlines.ItemTextBinding = new PropertyBinding<string>("Name");
				airlines.ItemKeyBinding = new PropertyBinding<string>("ICAO");
			};
			airlines.KeyUp += async (sender, e) =>
			{
				try
				{
					airlines.SelectedValue = airlines.DataStore.First(x =>
					{

						var t = x as Airline;
						Console.WriteLine(e.Key.ToString());
						return t.Name.StartsWith(e.Key.ToString());
					});
				}
				catch (Exception)
				{
					return;
				}
			};
			ProgressBar progress = new ProgressBar();
			progress.Indeterminate = true;
			button.Text = "Suchen";
			button.Click +=  async (s,e) => {
				progress.Visible = true;
				Airline selectedAirline = airlines.SelectedValue as Airline;
				List<Flight> test = await APIRequest.Request($"https://data-cloud.flightradar24.com/zones/fcgi/feed.js/?airline={selectedAirline.ICAO}&type=A320", selectedAirline.ICAO);
				grid.DataStore = test;
				grid.Width = Bounds.Width;
				grid.Height = 150;
				progress.Visible = false;
				grid.Visible = true;
			};
			stack.Items.Add(airlines);
			stack.Items.Add(button);
			stack.Items.Add(progress);
			stack.Items.Add(grid);
			grid.Visible = false;
			progress.Visible = false;
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("fnac"), HeaderText = "Flight number and Callsign" });
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("departure"), HeaderText = "Departure" });
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("arrival"), HeaderText = "Arrival" });
			grid.Columns.Add(new GridColumn { DataCell = new TextBoxCell("flightduration"), HeaderText = "Flight Duration" });
			grid.CellClick +=  (s, e)  => {
				Flight flight = e.Item as Flight;
				ContextMenu menu = new ContextMenu();
				MenuItem route = new Command().CreateMenuItem();
				route.Text = "Show route";
				route.Click += async (sender, a) => {
					Dialog dialog = new Dialog();
					dialog.Title = "Route";
					StackLayout layout = new StackLayout();
					dialog.Content = layout;
					layout.Items.Add(new Label() { Text = flight.departure, TextAlignment = TextAlignment.Center, Width = dialog.Width});
					layout.Items.Add(new Label() { Text = flight.arrival, TextAlignment = TextAlignment.Center, Width = dialog.Width});
					dialog.ShowModal(this);
				};
				MenuItem livery = new Command().CreateMenuItem();
				livery.Text = "Show livery";
				livery.Click += async (sender, a) => {
					Dialog dialog = new Dialog();
					dialog.Title = "Livery";
					StackLayout layout = new StackLayout();
					layout.Items.Add(new ImageView() {Image = await Utils.Utils.GetPhoto(flight?.registration.ToString())});
					dialog.Content = layout;
					await dialog.ShowModalAsync(this);
				};
				menu.Items.Add(route);
				menu.Items.Add(livery);

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
