using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using OECLib.GitHub;
using System.Collections.Generic;

namespace OECGUI
{
	public class DashboardForm : Gtk.Widget
	{
		[UI] Button button1;
		[UI] Button button2;
		[UI] Label label1;
		[UI] TreeView historyTree;
		private ListStore updateList;

		public static DashboardForm Create ()
		{
			Gtk.Builder builder = new Gtk.Builder (null, "OECGUI.DashboardForm.glade", null);

			DashboardForm m = new DashboardForm (builder, builder.GetObject ("dashboardForm").Handle);

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

		public void updateTreeList(String date, int count, int total)
		{
			updateList.AppendValues (date, ""+total, ""+count);


		}

		public DashboardForm (Builder builder, IntPtr handle) : base (handle)
		{
			CssProvider provider = new CssProvider ();
			provider.LoadFromPath ("test.css");
			ApplyCss (this, provider, uint.MaxValue);
			builder.Autoconnect (this);
			label1.StyleContext.AddProvider (provider, uint.MaxValue);
			setManagedSystems();
			setupTree ();
			//ShowAll ();
		}

		private void setupTree() {
			Gtk.TreeViewColumn timeCol = new Gtk.TreeViewColumn ();
			timeCol.Title = "Update Time";
			historyTree.AppendColumn (timeCol);

			Gtk.CellRendererText timeCell = new Gtk.CellRendererText ();
			timeCol.PackStart (timeCell, true);
			timeCol.AddAttribute (timeCell, "text", 0);


			Gtk.TreeViewColumn totalCol = new Gtk.TreeViewColumn ();
			totalCol.Title = "Total updates found";
			historyTree.AppendColumn (totalCol);

			Gtk.CellRendererText totalCell = new Gtk.CellRendererText ();
			totalCol.PackStart (totalCell, true);
			totalCol.AddAttribute (totalCell, "text", 1);


			Gtk.TreeViewColumn uCountCol = new Gtk.TreeViewColumn ();
			uCountCol.Title = "Systems updated";
			historyTree.AppendColumn (uCountCol);

			Gtk.CellRendererText uCountCell = new Gtk.CellRendererText ();
			uCountCol.PackStart (uCountCell, true);
			uCountCol.AddAttribute (uCountCell, "text", 2);

			updateList = new ListStore (typeof(string), typeof(string), typeof(string));
			historyTree.Model = updateList;
		}

		private async void setManagedSystems() {
			int count = await MainWindow.manager.getFileCount ("systems/");

			button2.Label = String.Format ("Managing {0}\nsystems in {1}", count, MainWindow.manager.repo.Name);
		}
	}
}

