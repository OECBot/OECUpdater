using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace OECGUI
{
	public class DashboardForm : Gtk.Widget
	{
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

		public DashboardForm (Builder builder, IntPtr handle) : base (handle)
		{
			//revealer = new Widget (builder.GetObject ("revealer1").Handle);

			CssProvider provider = new CssProvider ();
			provider.LoadFromPath ("test.css");
			ApplyCss (this, provider, uint.MaxValue);
			builder.Autoconnect (this);
			//DeleteEvent += OnDeleteEvent;
			//Destroyed += new EventHandler (OnDestroy);

			//ShowAll ();
		}
	}
}

