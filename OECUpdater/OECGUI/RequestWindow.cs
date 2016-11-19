using System;
using Gtk;
using OECLib.GitHub;
using OECLib.Utilities;
using UI = Gtk.Builder.ObjectAttribute;
using Octokit;

namespace OECGUI
{
	public partial class RequestWindow : Gtk.Window
	{
		[UI] Gtk.Button AcceptButton;
		[UI] Gtk.Button RejectButton;
		//	[UI] Gtk.TextView Details;
		//	[UI] Gtk.TreeView RequestList;
		//	CallBackServer callback;

		public static RequestWindow Create () {
			Console.WriteLine ("Before builder");
			Gtk.Builder builder = new Gtk.Builder(null, "OECGUI.PullRequestWindow.glade", null);
			Console.WriteLine ("After builder");
			RequestWindow m = new RequestWindow (builder, builder.GetObject ("RequestWindow").Handle);
			return m;

		}

		public RequestWindow (Builder builder, IntPtr handle): base (handle)
		{
//			CssProvider provider = new CssProvider();
//			provider.LoadFromPath ("../../test.css");
//			ApplyCss (this, provider, uint.MaxValue);
			builder.Autoconnect (this);
			DeleteEvent += OnDeleteEvent;
			AcceptButton.Clicked += AcceptButtonClicked;
			RejectButton.Clicked += RejectButtonClicked;
			//ShowAll ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		void AcceptButtonClicked (object sender, EventArgs e)
		{

		}

		void RejectButtonClicked (object sender, EventArgs e)
		{

		}
	}
}

