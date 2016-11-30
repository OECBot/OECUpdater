using System;
using Gtk;
using OECLib.Utilities;
using UI = Gtk.Builder.ObjectAttribute;
using OECGUI;

public partial class SettingsWindow: Gtk.Window
{
	[UI] Gtk.Button saveButton;
	[UI] Gtk.Button cancelButton;
	[UI] Gtk.Entry usernameField;
	[UI] Gtk.Entry passwordField;
	[UI] Gtk.Entry timeField;

	SettingsManager manager;

	public static SettingsWindow Create ()
	{
		Gtk.Builder builder = new Gtk.Builder (null, "OECGUI.SettingsForm.glade", null);

		SettingsWindow m = new SettingsWindow (builder, builder.GetObject ("SettingsForm").Handle);
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
		
	public SettingsWindow (Builder builder, IntPtr handle) : base (handle)
	{
		CssProvider provider = new CssProvider ();
		provider.LoadFromPath ("test.css");
		ApplyCss (this, provider, uint.MaxValue);
		builder.Autoconnect (this);
		saveButton.Clicked += SaveButton_Clicked;
		cancelButton.Clicked += CancelButton_Clicked;
	}

	public void InitializeSettingsManager (SettingsManager manager) {
		this.manager = manager;
		loadSettings ();
	}

	void loadSettings() {
		usernameField.Text = manager.GetSetting ("username");
		passwordField.Text = manager.GetSetting ("password");
		timeField.Text = manager.GetSetting ("time");
	}

	void saveSettings() {
		//TODO: don't store plaintext password, that or do't commit settings.ini
		manager.ChangeSetting ("username", usernameField.Text);
		manager.ChangeSetting ("password", passwordField.Text);

		if (timeField.Text.Split (':').Length == 2) {
			manager.ChangeSetting ("time", timeField.Text);
		} else {
			MessageDialog md = new MessageDialog (this.Handle);
			md.Text = "Time parameter not valid, using old setting";
			md.Show ();
		}
	}

	protected void SaveButton_Clicked (object sender, EventArgs e)
	{
		saveSettings ();
		manager.SaveSettingsToFile ();
		this.Close ();
	}

	public void CancelButton_Clicked (object sender, EventArgs e)
	{
		this.Close ();
	}
}
