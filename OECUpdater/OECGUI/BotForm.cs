﻿using System;
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
		[UI] TextView textview1;
		[UI] ScrolledWindow scrolledwindow1;

		public OECBot bot;
		private Thread botThread;



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
			//DeleteEvent += OnDeleteEvent;
			//Destroyed += new EventHandler (OnDestroy);


			Console.SetOut (new ControlWriter (textview1, scrolledwindow1));

			Serializer.InitPlugins ();
			List<IPlugin> plugins = new List<IPlugin> ();
			var ps = Serializer.plugins.Values;
			foreach (IPlugin plugin in ps) {
				plugins.Add (plugin);
			}
			Console.WriteLine ("Connected to repository: {0}/{1}", manager.repo.Owner.Login, manager.repo.Name);
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
			botThread = new Thread(new ThreadStart(bot.forceRun));
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