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

namespace OECGUI
{
	public class BotForm: Gtk.Widget
	{
		[UI] Button startButton;
		[UI] Button stopButton;
		[UI] Button clearButton;
		[UI] Button forceButton;
		[UI] TextView textview1;

		public OECBot bot;

		public static BotForm Create (RepositoryManager manager)
		{
			Gtk.Builder builder = new Gtk.Builder (null, "OECGUI.BotForm.glade", null);

			BotForm m = new BotForm (builder, builder.GetObject ("BotForm").Handle, manager);

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

		public BotForm (Builder builder, IntPtr handle, RepositoryManager manager) : base (handle)
		{
			//revealer = new Widget (builder.GetObject ("revealer1").Handle);

			CssProvider provider = new CssProvider ();
			provider.LoadFromPath ("test.css");
			ApplyCss (this, provider, uint.MaxValue);
			builder.Autoconnect (this);
			DeleteEvent += OnDeleteEvent;

			Console.SetOut (new ControlWriter (textview1));

			Serializer.InitPlugins ();
			List<IPlugin> plugins = new List<IPlugin> ();
			var ps = Serializer.plugins.Values;
			foreach (IPlugin plugin in ps) {
				plugins.Add (plugin);
			}
			bot = new OECBot (plugins, manager.repo);

			startButton.Clicked += Start_Clicked;
			stopButton.Clicked += Stop_Clicked;
			clearButton.Clicked += Clear_Clicked;
			forceButton.Clicked += Force_Clicked;
			//ShowAll ();
		}

		protected void Start_Clicked(object sender, EventArgs args)
		{
			bot.Start ();
		}

		protected void Stop_Clicked(object sender, EventArgs args)
		{
			bot.Stop ();
		}

		protected void Clear_Clicked(object sender, EventArgs args)
		{
			textview1.Buffer.Clear ();
		}

		protected void Force_Clicked(object sender, EventArgs args)
		{
			bot.runChecks ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}
	}

	public class ControlWriter : TextWriter
	{
		private TextView textbox;
		public ControlWriter(TextView textbox)
		{
			this.textbox = textbox;
		}

		public override void Write(char value)
		{
			textbox.Buffer.Text += value;
		}

		public override void Write(string value)
		{
			textbox.Buffer.Text += value;
		}

		public override Encoding Encoding
		{
			get { return Encoding.ASCII; }
		}
	}
}
