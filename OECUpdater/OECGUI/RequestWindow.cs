using System;
using Gtk;
using OECLib.GitHub;
using OECLib.Utilities;
using System.Threading.Tasks;
using UI = Gtk.Builder.ObjectAttribute;
using Octokit;
using System.Collections.Generic;

namespace OECGUI
{
	public partial class RequestWindow : Gtk.Window
	{
		[UI] Gtk.Button AcceptButton;
		[UI] Gtk.Button RejectButton;
		[UI] Gtk.TreeView RequestTreeView;
		[UI] Gtk.TextView textview1;
		//	CallBackServer callback;

		static Session session;
		static RepositoryManager rm;
		private static IReadOnlyList<PullRequest> pullRequestList;

		public static RequestWindow Create (Session currSession) {
			session = new Session("OECBot", "UoJ84XJTXphgO4F");
			Task<Repository> repo = session.client.Repository.Get("Gazing", "OECTest");
			repo.Wait();
			rm = new RepositoryManager(session, repo.Result);

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

			createStores ();
			renderColumns ();



			//ShowAll ();
		}

		private async void createStores(){
			Gtk.ListStore requestListStore = new Gtk.ListStore (typeof(string), typeof(string), 
				typeof(string), typeof(string), typeof(string));
			RequestTreeView.Model = requestListStore;

			await getAllPullRequest();

			foreach (PullRequest pr in pullRequestList)
			{
				requestListStore.AppendValues (pr.Title.ToString(), pr.Body, pr.User.Login.ToString(), 
					pr.CreatedAt.ToString(), pr.State.ToString());
			}
		}

		public async static Task getAllPullRequest()
		{
			try
			{
				pullRequestList = await rm.getAllPullRequests();
//				Console.WriteLine(pullRequestList[0].Title);
//				foreach (PullRequest pr in pullRequestList)
//				{
//					Console.WriteLine("Pull-Request: {0} - {1}Created by: {2} on {3}, Status: {4}", pr.Title, pr.Body, pr.User.Login, pr.CreatedAt, pr.State);
//				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}

		private void renderColumns(){
			Gtk.TreeViewColumn requestIDCol = new Gtk.TreeViewColumn ();
			requestIDCol.Title = "Title";
			RequestTreeView.AppendColumn (requestIDCol);

			Gtk.TreeViewColumn titleCol = new Gtk.TreeViewColumn ();
			titleCol.Title = "User";
			RequestTreeView.AppendColumn (titleCol);


			Gtk.CellRendererText requestIDCell = new Gtk.CellRendererText ();
			requestIDCol.PackStart (requestIDCell, true);
			requestIDCol.AddAttribute (requestIDCell, "text", 0);

			Gtk.CellRendererText titleCell = new Gtk.CellRendererText ();
			titleCol.PackStart (titleCell, true);
			titleCol.AddAttribute (titleCell, "text", 2);
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
			var title = model.GetValue (iter, 0);
			var body = model.GetValue (iter, 1);
			var user = model.GetValue (iter, 2);
			var createdAt = model.GetValue (iter, 3);
			var status = model.GetValue (iter, 4);
			//Console.WriteLine ("requestID: " + requestID + "\ttitle: " + title);

			textview1.Buffer.Text = "Title: " + title + "\nUser: " + user + 
				"\nCreated: " + createdAt + "\n\nComments:\n\n" + body;
		}

	}
}

