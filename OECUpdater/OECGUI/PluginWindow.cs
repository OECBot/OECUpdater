using System;
using Gtk;
using OECLib.GitHub;
using OECLib.Utilities;
using System.Threading.Tasks;
using UI = Gtk.Builder.ObjectAttribute;
using OECLib.Interface;
using System.Collections.Generic;

namespace OECGUI
{

	public partial class PluginWindow: Gtk.Widget
		{
		[UI] Gtk.Button ApplyButton;
		[UI] TreeView pluginTree;
		[UI] TextView textview1;
		//[UI] Gtk.Label listPluginLabel;
		private ListStore pluginList;

		public DashboardForm dashboard;

		public static PluginWindow Create () {

			Gtk.Builder builder = new Gtk.Builder(null, "OECGUI.PluginWindow.glade", null);
			PluginWindow m = new PluginWindow (builder, builder.GetObject ("PluginWindow").Handle);
			return m;
		}

		public PluginWindow (Builder builder, IntPtr handle): base (handle)
		{

			builder.Autoconnect (this);

			ApplyButton.Clicked += ApplyButton_Clicked;
			pluginTree.RowActivated += OnSelectRow;

			setupTree ();
			pluginList = new ListStore (typeof(bool), typeof(IPlugin));
			pluginTree.Model = pluginList;
			foreach (IPlugin plugin in Serializer.plugins.Values) {
				pluginList.AppendValues (true, plugin);
			}
			loadFromSettings ();
			ApplyButton_Clicked (null, null);
		}

		private void loadFromSettings() {
			Gtk.TreeIter iter;
			pluginList.GetIterFirst (out iter);
			while (pluginList.IterIsValid (iter)) {
				
				IPlugin plugin = (IPlugin) pluginList.GetValue (iter, 1);
				bool flag = false;

				bool.TryParse(SettingsWindow.manager.GetSetting (plugin.GetName ()), out flag);

				pluginList.SetValue (iter, 0, flag);

				pluginList.IterNext (ref iter);
			}
		}

		private void setupTree() {
			Gtk.TreeViewColumn checkCol = new Gtk.TreeViewColumn ();
			checkCol.Title = "Active";
			pluginTree.AppendColumn (checkCol);

			Gtk.CellRendererToggle checkCell = new CellRendererToggle ();
			checkCol.PackStart (checkCell, true);
			checkCol.AddAttribute (checkCell, "active", 0);

			//checkCol.SetCellDataFunc (checkCell, new TreeCellDataFunc (renderCheckCol));
			checkCell.Toggled += OnToggle;

			Gtk.TreeViewColumn pluginCol = new Gtk.TreeViewColumn ();
			pluginCol.Title = "Plugin Name";
			pluginTree.AppendColumn (pluginCol);

			Gtk.CellRendererText pluginCell = new Gtk.CellRendererText ();
			pluginCol.PackStart (pluginCell, true);
			//pluginCol.AddAttribute (pluginCell, "text", 1);
			pluginCol.SetCellDataFunc (pluginCell, new TreeCellDataFunc (renderPluginCol));


		}

		private void OnSelectRow(object sender, RowActivatedArgs args) {
			TreeIter iter;
			pluginList.GetIter (out iter, args.Path);
			IPlugin plugin = (IPlugin)pluginList.GetValue (iter, 1);
			textview1.Buffer.Text = String.Format("Name: {0}\n\nCreated By: {1}\n\nDescription: {2}", plugin.GetName(), plugin.GetAuthor(), plugin.GetDescription());
		}

		private void OnToggle(object sender, ToggledArgs args) {
			int column = 0;

			Gtk.TreeIter iter;
			if (pluginList.GetIterFromString (out iter, args.Path)) {
				bool val = (bool) pluginList.GetValue (iter, column);
				pluginList.SetValue (iter, column, !val);
			}
		}

		private void renderPluginCol(TreeViewColumn column, CellRenderer cell, ITreeModel model, TreeIter iter) {
			IPlugin plugin = (IPlugin) model.GetValue (iter, 1);
			(cell as Gtk.CellRendererText).Text = plugin.GetName ();
		}

		void ApplyButton_Clicked (object sender, EventArgs e)
		{
			Gtk.TreeIter iter;
			List<IPlugin> plugins = new List<IPlugin> ();
			pluginList.GetIterFirst (out iter);
			while (pluginList.IterIsValid (iter)) {
				bool flag = (bool) pluginList.GetValue (iter, 0);
				IPlugin plugin = (IPlugin) pluginList.GetValue (iter, 1);
				if (flag) {
					plugins.Add (plugin);
				}
				SettingsWindow.manager.ChangeSetting (plugin.GetName(), ""+flag);
				pluginList.IterNext (ref iter);
			}
			BotForm.updatePlugins (plugins);
			SettingsWindow.manager.SaveSettingsToFile ();
		}



	}

}


