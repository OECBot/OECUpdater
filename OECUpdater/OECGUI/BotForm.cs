using System;
using System.Collections.Generic;
using OECLib.Interface;
using Gtk;
using OECLib.GitHub;
using OECLib.Utilities;
using OECLib;
using UI = Gtk.Builder.ObjectAttribute;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OECGUI
{
	public class BotForm: Gtk.Widget
	{
		[UI] Button startButton;
		[UI] Button stopButton;
		[UI] Button clearButton;
		[UI] Button forceButton;
		[UI] Button scheduleButton;
		[UI] TextView textview1;
		[UI] ScrolledWindow scrolledwindow1;
		[UI] Entry entry1;
		[UI] Entry entry2;

		public static OECBot bot;
		private Thread botThread;
		public DashboardForm dashboard;

		public static BotForm Create ()
		{
			Gtk.Builder builder = new Gtk.Builder (null, "OECGUI.BotForm.glade", null);


			BotForm m = new BotForm (builder, builder.GetObject ("BotForm").Handle);

			return m;

		}

		public void ApplyCss (Widget widget, CssProvider provider, uint priority)
		{
			widget.StyleContext.AddProvider (provider, priority);
			var container = widget as Container;
			if (container != null) {
				foreach (var child in container.Children) {
					ApplyCss (child, provider, priority);
				}
			}
		}

		public BotForm (Builder builder, IntPtr handle) : base (handle)
		{
			//revealer = new Widget (builder.GetObject ("revealer1").Handle);

			CssProvider provider = new CssProvider ();
			provider.LoadFromPath ("test.css");
			ApplyCss (this, provider, uint.MaxValue);
			builder.Autoconnect (this);
			//DeleteEvent += OnDeleteEvent;
			//Destroyed += new EventHandler (OnDestroy);


			Console.SetOut (new ControlWriter (textview1, scrolledwindow1));


			List<IPlugin> plugins = new List<IPlugin> ();
			var ps = Serializer.plugins.Values;
			foreach (IPlugin plugin in ps) {
				plugins.Add (plugin);
			}
			Console.WriteLine ("Connected to repository: {0}/{1}", MainWindow.manager.repo.Owner.Login, MainWindow.manager.repo.Name);
			bot = new OECBot (plugins, MainWindow.manager.repo);

			startButton.Clicked += Start_Clicked;
			stopButton.Clicked += Stop_Clicked;
			clearButton.Clicked += Clear_Clicked;
			forceButton.Clicked += Force_Clicked;
			scheduleButton.Clicked += Schedule_Clicked;
			//ShowAll ();
		}

		protected void Schedule_Clicked(object sender, EventArgs args) {
			bot.checkTime = DateTime.Today.AddHours (double.Parse (entry1.Text)).AddMinutes(double.Parse(entry2.Text));
		}

		protected void Start_Clicked(object sender, EventArgs args)
		{
			if (botThread != null) {
				if (bot.On) {
					return;
				}
			}
			botThread = new Thread(new ThreadStart(startRun));
			botThread.Start ();
		}

		protected void Stop_Clicked(object sender, EventArgs args)
		{
			if (botThread != null) {
				if (bot.On) {
					bot.Stop ();
					botThread.Join ();
					botThread = null;
				}
			}
		}

		protected void Clear_Clicked(object sender, EventArgs args)
		{
			textview1.Buffer.Clear ();
		}

		protected void Force_Clicked(object sender, EventArgs args)
		{
			botThread = new Thread(new ThreadStart(forceRun));
			botThread.Start ();

		}

		protected void OnDestroy (object sender, EventArgs args)
		{
			Console.WriteLine ("Closed");
			Application.Quit ();
		}
			
		public void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Console.WriteLine ("ON DELETE: STOP BOT THREAD");

			if (botThread != null) {
				bot.Stop ();
				botThread.Join ();
			}

			Application.Quit ();
			a.RetVal = false;
		}

		private async void startRun() {
			await bot.Start ();
			Gtk.Application.Invoke(delegate{dashboard.updateTreeList (DateTime.Now.ToString ("yyyy-MM-dd hh:mm"), bot.updateCount, bot.total);});
		}

		private void forceRun() {
			bot.forceRun ();
			Gtk.Application.Invoke(delegate{dashboard.updateTreeList (DateTime.Now.ToString ("yyyy-MM-dd hh:mm"), bot.updateCount, bot.total);});
		}


	
	}

	public class ControlWriter : TextWriter
	{
		private TextView textbox;
		private ScrolledWindow scroll;

		public ControlWriter(TextView textbox, ScrolledWindow scroll)
		{
			this.textbox = textbox;
			this.scroll = scroll;
		}

		public override void Write(char value)
		{
			Gtk.Application.Invoke (delegate{mWrite (value);});
		}

		public void mWrite(char value)
		{
			textbox.Buffer.Text += value;
			var adj = scroll.Vadjustment;
			adj.Value = adj.Upper - adj.PageSize;
		}

		public override void Write(string value)
		{
			Gtk.Application.Invoke (delegate{mWrite (value);});
		}

		public void mWrite(string value)
		{
			textbox.Buffer.Text += value;
			var adj = scroll.Vadjustment;
			adj.Value = adj.Upper - adj.PageSize;
		}

		public override Encoding Encoding
		{
			get { return Encoding.ASCII; }
		}
	}
}
