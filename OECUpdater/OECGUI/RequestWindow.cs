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
	public partial class RequestWindow : Gtk.Widget
	{
		[UI] Gtk.Button AcceptButton;
		[UI] Gtk.Button RejectButton;
		[UI] Gtk.TreeView RequestTreeView;
		[UI] Gtk.TextView textview1;
		//	CallBackServer callback;

		private RepositoryManager rm;
		private string currentRow = null;
		//private Gtk.RowActivatedArgs arg = null;
		private TreeIter currIter;
		private static IReadOnlyList<PullRequest> pullRequestList;
		private static Task<Repository> repo;
		private Gtk.ListStore requestListStore;

		public static RequestWindow Create (RepositoryManager manager) {
			

			Gtk.Builder builder = new Gtk.Builder(null, "OECGUI.PullRequestWindow.glade", null);
			RequestWindow m = new RequestWindow (builder, builder.GetObject ("RequestWindow").Handle, manager);
			return m;
		}

		public RequestWindow (Builder builder, IntPtr handle, RepositoryManager manager): base (handle)
		{

			builder.Autoconnect (this);
			DeleteEvent += OnDeleteEvent;
			AcceptButton.Clicked += AcceptButtonClicked;
			RejectButton.Clicked += RejectButtonClicked;
			RequestTreeView.RowActivated += RowClicked;
			this.rm = manager;

			createStores ();
			renderColumns ();

			//ShowAll ();
		}

		private async void createStores(){
			requestListStore = new Gtk.ListStore (typeof(string), typeof(string), 
				typeof(string), typeof(string), typeof(string), typeof(string));
			RequestTreeView.Model = requestListStore;

			await getAllPullRequest();

			foreach (PullRequest pr in pullRequestList)
			{
				requestListStore.AppendValues (pr.Title.ToString(), pr.Body, pr.User.Login.ToString(), 
						pr.CreatedAt.ToString(), pr.State.ToString(), pr.Number.ToString());
				
			}
		}

		public async Task getAllPullRequest()
		{
			try
			{
				pullRequestList = await rm.getAllPullRequests();
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
			Gtk.Application.Quit ();
			a.RetVal = true;
		}

		void AcceptButtonClicked (object sender, EventArgs e)
		{
			Console.WriteLine ("Accept button clicked");

			try{
				if(currentRow != null){
					mergePullRequest (Int32.Parse(currentRow));
				}
			} catch (Exception ex){
				Console.WriteLine(ex);
			}
		}

		async void mergePullRequest(int prNum){
			try{
				await rm.MergePullRequest("master", prNum);
				removeRow();
			}catch (Exception e){
				Console.WriteLine (e);
			}
		}

		void RejectButtonClicked (object sender, EventArgs e)
		{
			Console.WriteLine ("Reject button clicked");
			try{
				if(currentRow != null){
					closePullRequest(Int32.Parse(currentRow));
				}
			} catch (Exception ex){
				Console.WriteLine(ex);
			}
		}

		async void closePullRequest(int prNum){
			try{
				await rm.closePullRequest(prNum);
				removeRow();
			}catch (Exception e){
				Console.WriteLine (e);
			}
		}

		void RowClicked(object sender, Gtk.RowActivatedArgs args){
			//arg = args;
			var model = RequestTreeView.Model;
			TreeIter iter;
			model.GetIter (out iter, args.Path);
			currIter = iter;

			var title = model.GetValue (iter, 0);
			var body = model.GetValue (iter, 1);
			var user = model.GetValue (iter, 2);
			var createdAt = model.GetValue (iter, 3);
			var status = model.GetValue (iter, 4);
			var number = model.GetValue (iter, 5);

			currentRow = number.ToString();
			//Console.WriteLine ("requestID: " + requestID + "\ttitle: " + title);

			textview1.Buffer.Text = "Title: " + title + "\nNumber: " + number + "\nUser: " + user + 
				"\nCreated: " + createdAt + "\n\nComments:\n\n" + body;
		}


		void removeRow(){
			Console.WriteLine ("in removeRow");
			try {
//				if(arg != null){
				var model = RequestTreeView.Model;
//					Console.WriteLine ("in removeRow 2");
//					TreeIter iter;
//					Console.WriteLine ("in removeRow 3");
//					model.GetIter (out iter, arg.Path);
				var title = model.GetValue (currIter, 0);
				var body = model.GetValue (currIter, 1);
				var user = model.GetValue (currIter, 2);
				var createdAt = model.GetValue (currIter, 3);
				var status = model.GetValue (currIter, 4);
				var number = model.GetValue (currIter, 5);
				Console.WriteLine ("Title: " + title + "\nNumber: " + number + "\nUser: " + user + 
					"\nCreated: " + createdAt + "\n\nComments:\n\n" + body);
				requestListStore.Remove(ref currIter);
				Console.WriteLine ("in removeRow 5");
				textview1.Buffer.Text = "";
//				}


			} catch(Exception ex){
				Console.WriteLine (ex);
			}
		}

	}
}

