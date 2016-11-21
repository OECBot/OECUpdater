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
		[UI] Gtk.TreeView RequestTreeView;
		[UI] Gtk.TextView textview1;
		//	CallBackServer callback;

		public static RequestWindow Create () {
			Gtk.Builder builder = new Gtk.Builder(null, "OECGUI.PullRequestWindow.glade", null);
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
			RequestTreeView.RowActivated += RowClicked;

			Gtk.ListStore requestListStore = new Gtk.ListStore (typeof(string), typeof(string), typeof(string));
			RequestTreeView.Model = requestListStore;

			requestListStore.AppendValues ("123", "First", "Comment1");
			requestListStore.AppendValues ("456", "Second", "Comment2");
			requestListStore.AppendValues ("789", "Third", "Comment3");
			requestListStore.AppendValues ("1231", "1First", "Comment4");
			requestListStore.AppendValues ("4561", "1Second", "Comment5");
			requestListStore.AppendValues ("7891", "1Third", "Comment6");
			requestListStore.AppendValues ("1232", "2First", "Comment7");
			requestListStore.AppendValues ("4562", "2Second", "Comment8");
			requestListStore.AppendValues ("7892", "2Third", "Comment9");
			requestListStore.AppendValues ("123", "First", "Comment10");
			requestListStore.AppendValues ("456", "Second", "Comment11");
			requestListStore.AppendValues ("789", "Third", "Comment12");
			requestListStore.AppendValues ("1231", "1First", "Comment13");
			requestListStore.AppendValues ("4561", "1Second", "Comment14");
			requestListStore.AppendValues ("7891", "1Third", "Comment15");
			requestListStore.AppendValues ("1232", "2First", "Comment16");
			requestListStore.AppendValues ("4562", "2Second", "Comment17");
			requestListStore.AppendValues ("7892", "2Third", "Comment18");
			requestListStore.AppendValues ("123", "First", "Comment19");
//			requestListStore.AppendValues ("456", "Second");
//			requestListStore.AppendValues ("789", "Third");
//			requestListStore.AppendValues ("1231", "1First");
//			requestListStore.AppendValues ("4561", "1Second");
//			requestListStore.AppendValues ("7891", "1Third");
//			requestListStore.AppendValues ("1232", "2First");
//			requestListStore.AppendValues ("4562", "2Second");
//			requestListStore.AppendValues ("7892", "2Third");


			Gtk.TreeViewColumn requestIDCol = new Gtk.TreeViewColumn ();
			requestIDCol.Title = "Request ID";
			RequestTreeView.AppendColumn (requestIDCol);

			Gtk.TreeViewColumn titleCol = new Gtk.TreeViewColumn ();
			titleCol.Title = "Title";
			RequestTreeView.AppendColumn (titleCol);


			Gtk.CellRendererText requestIDCell = new Gtk.CellRendererText ();
			requestIDCol.PackStart (requestIDCell, true);
			requestIDCol.AddAttribute (requestIDCell, "text", 0);

			Gtk.CellRendererText titleCell = new Gtk.CellRendererText ();
			titleCol.PackStart (titleCell, true);
			titleCol.AddAttribute (titleCell, "text", 1);


			//ShowAll ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		void AcceptButtonClicked (object sender, EventArgs e)
		{
			Console.WriteLine ("Accept button clicked");
		}

		void RejectButtonClicked (object sender, EventArgs e)
		{
			Console.WriteLine ("Reject button clicked");
		}

		void RowClicked(object sender, Gtk.RowActivatedArgs args){
			var model = RequestTreeView.Model;
			TreeIter iter;
			model.GetIter (out iter, args.Path);
			var requestID = model.GetValue (iter, 0);
			var title = model.GetValue (iter, 1);
			var comment = model.GetValue (iter, 2);
			Console.WriteLine ("requestID: " + requestID + "\ttitle: " + title);

			textview1.Buffer.Text = "System: " + title + "\nRequest ID: " + requestID + "\n\nComments:\n\n" + comment;
		}

	}
}

