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

public partial class PluginWindow: Gtk.Widget
	{
	[UI] Gtk.Button ApplyButton;
	[UI] Gtk.Button CancelButton;
	[UI] Gtk.CheckButton NasaPluginButton;
	[UI] Gtk.CheckButton EuroExoplanetPlguinButton;
	//[UI] Gtk.Label listPluginLabel;


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
		CancelButton.Clicked += CancelButton_Clicked;
		NasaPluginButton.Clicked += NasaPluginButton_Clicked;
		EuroExoplanetPlguinButton.Clicked += EuroExoplanetPlguinButton_Clicked;


	}

	void EuroExoplanetPlguinButton_Clicked (object sender, EventArgs e)
	{
		
	}

	void NasaPluginButton_Clicked (object sender, EventArgs e)
	{
		
	}

	void CancelButton_Clicked (object sender, EventArgs e)
	{
		
	}

	void ApplyButton_Clicked (object sender, EventArgs e)
	{
		
	}



}

}


