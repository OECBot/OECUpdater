using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace OECGUI
{
	public class ExceptionDialog : Dialog
	{
		[UI] TextView textview1;
		[UI] Label label1;
		[UI] Button button1;

		public ExceptionDialog (Builder builder, IntPtr handle, String message, String stackTrace, params object[] args) : base(handle)
		{
			builder.Autoconnect (this);
			this.Title = "Error Occured";
			label1.Text = String.Format (message, args);
			textview1.Buffer.Text = stackTrace;
			button1.Clicked += Close_Clicked;
			ShowAll();
		}

		protected void Close_Clicked(object sender, EventArgs args) {
			this.Destroy ();
		}

		public static ExceptionDialog Create(String format, String stackTrace, params object[] args) {
			Gtk.Builder builder = new Gtk.Builder (null, "OECGUI.ExceptionDialog.glade", null);

			ExceptionDialog m = new ExceptionDialog (builder, builder.GetObject ("dialog1").Handle, format, stackTrace, args);
			return m;
		}
	}
}

