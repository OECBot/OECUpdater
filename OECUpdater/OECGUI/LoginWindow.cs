using System;
using Gtk;
using OECLib.GitHub;
using OECLib.Utilities;
using UI = Gtk.Builder.ObjectAttribute;
using Octokit;

public partial class LoginWindow: Gtk.Window
{
	[UI] Gtk.Button loginButton;
	[UI] Gtk.Button OauthButton;
	[UI] Gtk.Entry entry1;
	[UI] Gtk.Entry entry2;
	CallBackServer callback;

	public static LoginWindow Create () {
		Gtk.Builder builder = new Gtk.Builder(null, "OECGUI.Test.glade", null);

		LoginWindow m = new LoginWindow (builder, builder.GetObject ("MainWindow").Handle);
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

	public LoginWindow (Builder builder, IntPtr handle): base (handle)
	{
		CssProvider provider = new CssProvider();
		provider.LoadFromPath ("../../test.css");
		ApplyCss (this, provider, uint.MaxValue);
		builder.Autoconnect (this);
		DeleteEvent += OnDeleteEvent;
		loginButton.Clicked += LoginButton_Clicked;
		OauthButton.Clicked += OauthLogin;
		//ShowAll ();
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

	protected void Login() {
		String uname = entry1.Text;
		String password = entry2.Text;
		Session session = new Session (uname, password);

	}

	protected void OauthLogin(object sender, EventArgs e) {
		RegisterCallBack ();
	}

	protected async void RegisterCallBack() {
		try {
				
			if (callback != null) {
				callback.Stop();
				callback = null;
			}
			GitHubClient g = new GitHubClient(new ProductHeaderValue("SpazioApp"));

			var request = new OauthLoginRequest(Session.clientId)
			{
				Scopes = { "user", "notifications", "repo" }
			};

			String oauthLoginUrl = g.Oauth.GetGitHubLoginUrl(request).ToString();
			System.Diagnostics.Process.Start(oauthLoginUrl);
			callback = new CallBackServer ("127.0.0.1", 4567, g);
			Session session = await callback.Start ();
			User cur = await session.client.User.Current ();
			MessageDialog md = new MessageDialog(this, 
				DialogFlags.DestroyWithParent, MessageType.Info, 
				ButtonsType.Close, "Succesfully authenticated as: "+cur.Login);
			md.Run();
			md.Destroy();
		}
		catch (Exception ex) {
			MessageDialog me = new MessageDialog (this, 
				                   DialogFlags.DestroyWithParent, MessageType.Info, 
				                   ButtonsType.Close, ex.Message);
			me.Run ();
			me.Destroy ();
		}
	}
}
