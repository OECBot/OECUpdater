using System;
using Gtk;
using OECLib.GitHub;
using OECLib.Utilities;
using UI = Gtk.Builder.ObjectAttribute;
using Octokit;
using OECGUI;

public partial class LoginWindow: Gtk.Window
{
	[UI] Gtk.Button loginButton;
	[UI] Gtk.Button OauthButton;
	[UI] Gtk.Button settingsButton;
	[UI] Gtk.Entry entry1;
	[UI] Gtk.Entry entry2;
	[UI] Gtk.Revealer revealer1;
	[UI] Gtk.Entry entry3;

	public static LoginWindow Create ()
	{
		Gtk.Builder builder = new Gtk.Builder (null, "OECGUI.Test.glade", null);

		LoginWindow m = new LoginWindow (builder, builder.GetObject ("LoginWindow").Handle);
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

	public LoginWindow (Builder builder, IntPtr handle) : base (handle)
	{
		//revealer = new Widget (builder.GetObject ("revealer1").Handle);

		CssProvider provider = new CssProvider ();
		provider.LoadFromPath ("test.css");
		ApplyCss (this, provider, uint.MaxValue);
		builder.Autoconnect (this);
		DeleteEvent += OnDeleteEvent;
		loginButton.Clicked += LoginButton_Clicked;
		OauthButton.Clicked += OauthLogin;
		settingsButton.Clicked += Settings_Clicked;
		//ShowAll ();
	}

	protected void Settings_Clicked (object sender, EventArgs e)
	{
		revealer1.RevealChild = !revealer1.RevealChild;
	}

	void LoginButton_Clicked (object sender, EventArgs e)
	{
		Login ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void Login ()
	{
		String uname = entry1.Text;
		String password = entry2.Text;
		Session session = new Session (uname, password);

		RequestWindow main = RequestWindow.Create (session);
		main.Show ();
		this.Hide ();

	}

	protected void OauthLogin (object sender, EventArgs e)
	{
		RegisterCallBack ();
	}

	protected async void RegisterCallBack ()
	{
		
		Session session = await Session.CreateOauthSession ();
		if (session == null) {
			return;
		}
		try {
			await session.SetCurrentUser ();
			if (Session.server.isCancelled) {
				return;
			}
			MessageDialog md = new MessageDialog (this, 
				                  DialogFlags.DestroyWithParent, MessageType.Info, 
				                  ButtonsType.Close, "Succesfully authenticated as: " + session.current.Login);
			md.Run ();
			md.Destroy ();

			String[] repoInfo = entry3.Text.Split ('/');

			MainWindow main = MainWindow.Create(new RepositoryManager(session, await session.client.Repository.Get(repoInfo[0], repoInfo[1])));
			main.Show();
			this.Hide();
		} catch (Exception ex) {
			MessageDialog md = new MessageDialog (this, 
				                   DialogFlags.DestroyWithParent, MessageType.Info, 
				                   ButtonsType.Close, ex.Message);
			md.Run ();
			md.Destroy ();
		}

	}
}
