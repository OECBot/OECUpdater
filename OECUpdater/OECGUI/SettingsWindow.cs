using System;
using Gtk;
using OECLib.Utilities;
using UI = Gtk.Builder.ObjectAttribute;
using OECGUI;
using System.Collections.Generic;

public partial class SettingsWindow: Gtk.Window
{
	[UI] Gtk.Button saveButton;
	[UI] Gtk.Entry usernameField;
	[UI] Gtk.Entry passwordField;
	[UI] Gtk.Entry timeField;
	[UI] Calendar calendar1;

	public delegate void UpdateDelegate();
	private List<UpdateDelegate> subscribers;

	public static SettingsManager manager;

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
		this.subscribers = new List<UpdateDelegate> ();
		CssProvider provider = new CssProvider ();
		provider.LoadFromPath ("test.css");
		ApplyCss (this, provider, uint.MaxValue);
		builder.Autoconnect (this);
		saveButton.Clicked += SaveButton_Clicked;

		loadSettings ();
	}

	public void Subscribe(UpdateDelegate func) {
		this.subscribers.Add (func);
	}

	public static void InitializeSettingsManager (SettingsManager manager) {
		SettingsWindow.manager = manager;

	}

	private void loadSettings() {
		usernameField.Text = manager.GetSetting ("username");
		passwordField.Text = manager.GetSetting ("password");
		timeField.Text = manager.GetSetting ("time");
	}

	public void updateCalendar(DateTime date) {
		calendar1.Date = date;
	}

	private void saveSettings() {
		//TODO: don't store plaintext password, that or do't commit settings.ini
		try {
			SettingsWindow.manager.ChangeSetting ("username", usernameField.Text);
			SettingsWindow.manager.ChangeSetting ("password", passwordField.Text);
			if (isValidTime(timeField.Text)) {
				manager.ChangeSetting ("time", timeField.Text);
			} else {
				
				MessageDialog md = new MessageDialog (this.Handle);
				md.Text = "Time parameter not valid, using old setting";
				md.Show ();
			}
			SettingsWindow.manager.ChangeSetting("lastCheckDate", calendar1.GetDate().ToString("yyyy-MM-dd"));

		}
		catch (Exception ex) {
			ExceptionDialog md = ExceptionDialog.Create (ex.Message, ex.StackTrace);
			md.Show ();
		}
	}

	private bool isValidTime(String time) {
		String[] fields = time.Split (':');
		if (fields.Length == 2) {
			double hh;
			double mm;
			if (double.TryParse(fields[0], out hh) && double.TryParse(fields[1], out mm)) {
				if (hh >= 0 && hh <= 23 && mm >= 0 && mm <= 59) {
					return true;
				}
			}

		}
		return false;
	}

	protected void SaveButton_Clicked (object sender, EventArgs e)
	{
		saveSettings ();
		notifySubs ();
		SettingsWindow.manager.SaveSettingsToFile ();
	}

	public void notifySubs() {
		foreach (UpdateDelegate func in subscribers) {
			func ();
		}
	}
}
