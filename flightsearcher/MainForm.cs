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
			MinimumSize = new Size(200, 200);

			StackLayout stack = new StackLayout();
			
			
			ListBox listBox = new ListBox();
			listBox.Size = new Size(50, 50);
			listBox.Items.Add("Item 1");
			Button button = new Button();
			button.Text = "Click me!";
			button.Click +=  async (s,e) => {
				List<Airline> test = await new APIRequest().Request<Airline>("https://www.flightradar24.com/_json/airlines.php");
				// 
			};
			
			stack.Items.Add(listBox);
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
