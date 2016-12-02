using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using OECLib.GitHub;
using System.Collections.Generic;
using OECLib.Utilities;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace OECGUI
{
	public class DashboardForm : Gtk.Widget
	{
		[UI] Button button1;
		[UI] Button button2;
		[UI] Button button5;
		[UI] Button button3;
		[UI] Label label1;
		[UI] Label label2;
		[UI] Label label3;
		[UI] Label label4;
		[UI] Label label5;

		[UI] TreeView historyTree;
		private ListStore updateList;
		private SerializableDictionary<String, List<String>> historyValues;
		private int prCount;
		public String gitHubPRAdress = "https://github.com/{0}/{1}/pulls?utf8=✓&q=is%3Apr%20is%3Aopen%20created%3A{2}T{3}..{4}T{5}";

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

		public void updateTreeList(String date, int count, int total, String condition)
		{
			updateList.AppendValues (date, ""+total, ""+count, condition);
			historyValues ["Date"].Add (date);
			historyValues ["Total"].Add ("" + total);
			historyValues ["Updated"].Add ("" + count);
			historyValues ["Condition"].Add (condition);

			saveHistory ();

		}

		public DashboardForm (Builder builder, IntPtr handle) : base (handle)
		{
			CssProvider provider = new CssProvider ();
			provider.LoadFromPath ("test.css");
			ApplyCss (this, provider, uint.MaxValue);
			builder.Autoconnect (this);
			label1.StyleContext.AddProvider (provider, uint.MaxValue);
			label3.StyleContext.AddProvider (provider, uint.MaxValue);
			label4.StyleContext.AddProvider (provider, uint.MaxValue);
			label5.StyleContext.AddProvider (provider, uint.MaxValue);
			label2.StyleContext.AddProvider (provider, uint.MaxValue);
			button1.StyleContext.AddProvider (provider, uint.MaxValue);
			button2.StyleContext.AddProvider (provider, uint.MaxValue);
			button3.StyleContext.AddProvider (provider, uint.MaxValue);
			button5.StyleContext.AddProvider (provider, uint.MaxValue);
			button3.Clicked += Help_Clicked;
			historyTree.RowActivated += rowClicked;

			historyTree.StyleContext.AddProvider (provider, uint.MaxValue);

			label1.Text = String.Format ("Hello, {0}!", MainWindow.manager.session.current.Login);
			setManagedSystems();
			setPluginData ();
			setupTree ();
			//ShowAll ();
		}

		protected void rowClicked(object sender, RowActivatedArgs args) {
			TreeIter iter;
			updateList.GetIter (out iter, args.Path);
			String time = (String) updateList.GetValue (iter, 0);
			String[] fields = time.Split (' ');
			DateTime filter;
			DateTime.TryParseExact (time, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out filter);
			filter = filter.AddMinutes(-30).AddHours(5);
			String address = String.Format (gitHubPRAdress, MainWindow.manager.repo.Owner.Login, MainWindow.manager.repo.Name, filter.ToString("yyyy-MM-dd"), filter.ToString ("HH:mm"), filter.ToString("yyyy-MM-dd"), filter.AddMinutes(32).ToString("HH:mm"));
			System.Diagnostics.Process.Start (address);
		}

		private void renderCol(TreeViewColumn column, CellRenderer cell, ITreeModel model, TreeIter iter) {
			if (model.GetPath(iter).Indices[0] % 2 == 0) {
				
				cell.CellBackground = "#e6e9ef";

			} else {
				cell.CellBackground = "#FFFFFF";
			}
		}

		private void setupTree() {
			Gtk.TreeViewColumn timeCol = new Gtk.TreeViewColumn ();
			timeCol.Title = "Update Time";
			historyTree.AppendColumn (timeCol);

			Gtk.CellRendererText timeCell = new Gtk.CellRendererText ();
			timeCol.PackStart (timeCell, true);
			timeCol.AddAttribute (timeCell, "text", 0);
			timeCol.SetCellDataFunc (timeCell, new TreeCellDataFunc (renderCol));


			Gtk.TreeViewColumn totalCol = new Gtk.TreeViewColumn ();
			totalCol.Title = "Total updates found";
			historyTree.AppendColumn (totalCol);

			Gtk.CellRendererText totalCell = new Gtk.CellRendererText ();
			totalCol.PackStart (totalCell, true);
			totalCol.AddAttribute (totalCell, "text", 1);
			totalCol.SetCellDataFunc (totalCell, new TreeCellDataFunc (renderCol));

			Gtk.TreeViewColumn uCountCol = new Gtk.TreeViewColumn ();
			uCountCol.Title = "Total systems updated";
			historyTree.AppendColumn (uCountCol);

			Gtk.CellRendererText uCountCell = new Gtk.CellRendererText ();
			uCountCol.PackStart (uCountCell, true);
			uCountCol.AddAttribute (uCountCell, "text", 2);
			uCountCol.SetCellDataFunc (uCountCell, new TreeCellDataFunc (renderCol));

			Gtk.TreeViewColumn condiCol = new Gtk.TreeViewColumn ();
			condiCol.Title = "Run Condition";
			historyTree.AppendColumn (condiCol);

			Gtk.CellRendererText condiCell = new Gtk.CellRendererText ();
			condiCol.PackStart (condiCell, true);
			condiCol.AddAttribute (condiCell, "text", 3);
			condiCol.SetCellDataFunc (condiCell, new TreeCellDataFunc (renderCol));


			loadHistory ();
			if (updateList == null) {
				this.updateList = new ListStore (typeof(string), typeof(string), typeof(string), typeof(string));
			}
			historyTree.Model = updateList;
		}


		protected void Help_Clicked(object sender, EventArgs args) {
			System.Diagnostics.Process.Start ("https://github.com/Gazing/SpazioWiki/wiki");
		}

		private async void setManagedSystems() {
			int count = await MainWindow.manager.getFileCount ("systems/");

			label4.Text = String.Format ("{0}", count);
		}

		private void setPluginData() {
			int count = Serializer.plugins.Count;

			label3.Text = String.Format ("{0}", count);
		}

		public void setPRData(int count) {
			prCount = count;
			label5.Text = String.Format ("{0}", count);
		}

		private void saveHistory() {
			XmlSerializer serializer = new XmlSerializer (typeof(SerializableDictionary<String, List<String>>));
			TextWriter writer = new StreamWriter("updatehistory.xml");
			serializer.Serialize (writer, historyValues);
		}

		public void subPRData() {
			setPRData (prCount - 1);
		}

		private void loadHistory() {
			XmlSerializer serializer = new XmlSerializer (typeof(SerializableDictionary<String, List<String>>));
			SerializableDictionary<String, List<String>> dict;
			try {
				TextReader reader = new StreamReader ("updatehistory.xml");
				dict = serializer.Deserialize (reader) as SerializableDictionary<String, List<String>>;
			}
			catch {
				dict = null;
			}
			if (dict == null) {
				this.historyValues = new SerializableDictionary<String, List<String>> ();
				historyValues.Add ("Date", new List<String> ());
				historyValues.Add ("Total", new List<String> ());
				historyValues.Add("Updated", new List<String>());
				historyValues.Add("Condition", new List<String>());
				return;
			}
			this.historyValues = dict;
			this.updateList = new ListStore (typeof(string), typeof(string), typeof(string), typeof(string));
			for (int i = 0; i < historyValues ["Date"].Count; i++) {
				updateList.AppendValues(historyValues["Date"][i], historyValues["Total"][i], historyValues["Updated"][i], historyValues["Condition"][i]);
			}
		}
	}

	/// <summary>
	/// Represents an XML serializable collection of keys and values.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
	[Serializable]
	[XmlRoot("dictionary")]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
	{
		/// <summary>
		/// The default XML tag name for an item.
		/// </summary>
		private const string DefaultItemTag = "item";

		/// <summary>
		/// The default XML tag name for a key.
		/// </summary>
		private const string DefaultKeyTag = "key";

		/// <summary>
		/// The default XML tag name for a value.
		/// </summary>
		private const string DefaultValueTag = "value";

		/// <summary>
		/// The XML serializer for the key type.
		/// </summary>
		private static readonly XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));

		/// <summary>
		/// The XML serializer for the value type.
		/// </summary>
		private static readonly XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="SerializableDictionary&lt;TKey, TValue&gt;"/> class.
		/// </summary>
		public SerializableDictionary()
		{
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="SerializableDictionary&lt;TKey, TValue&gt;"/> class.
		/// </summary>
		/// <param name="info">A
		/// <see cref="T:System.Runtime.Serialization.SerializationInfo"/> object
		/// containing the information required to serialize the
		/// <see cref="T:System.Collections.Generic.Dictionary`2"/>.
		/// </param>
		/// <param name="context">A
		/// <see cref="T:System.Runtime.Serialization.StreamingContext"/> structure
		/// containing the source and destination of the serialized stream
		/// associated with the
		/// <see cref="T:System.Collections.Generic.Dictionary`2"/>.
		/// </param>
		protected SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Gets the XML tag name for an item.
		/// </summary>
		protected virtual string ItemTagName
		{
			get
			{
				return DefaultItemTag;
			}
		}

		/// <summary>
		/// Gets the XML tag name for a key.
		/// </summary>
		protected virtual string KeyTagName
		{
			get
			{
				return DefaultKeyTag;
			}
		}

		/// <summary>
		/// Gets the XML tag name for a value.
		/// </summary>
		protected virtual string ValueTagName
		{
			get
			{
				return DefaultValueTag;
			}
		}

		/// <summary>
		/// Gets the XML schema for the XML serialization.
		/// </summary>
		/// <returns>An XML schema for the serialized object.</returns>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Deserializes the object from XML.
		/// </summary>
		/// <param name="reader">The XML representation of the object.</param>
		public void ReadXml(XmlReader reader)
		{
			var wasEmpty = reader.IsEmptyElement;

			reader.Read();
			if (wasEmpty)
			{
				return;
			}

			try
			{
				while (reader.NodeType != XmlNodeType.EndElement)
				{
					this.ReadItem(reader);
					reader.MoveToContent();
				}
			}
			finally
			{
				reader.ReadEndElement();
			}
		}

		/// <summary>
		/// Serializes this instance to XML.
		/// </summary>
		/// <param name="writer">The XML writer to serialize to.</param>
		public void WriteXml(XmlWriter writer)
		{
			foreach (var keyValuePair in this)
			{
				this.WriteItem(writer, keyValuePair);
			}
		}

		/// <summary>
		/// Deserializes the dictionary item.
		/// </summary>
		/// <param name="reader">The XML representation of the object.</param>
		private void ReadItem(XmlReader reader)
		{
			reader.ReadStartElement(this.ItemTagName);
			try
			{
				this.Add(this.ReadKey(reader), this.ReadValue(reader));
			}
			finally
			{
				reader.ReadEndElement();
			}
		}

		/// <summary>
		/// Deserializes the dictionary item's key.
		/// </summary>
		/// <param name="reader">The XML representation of the object.</param>
		/// <returns>The dictionary item's key.</returns>
		private TKey ReadKey(XmlReader reader)
		{
			reader.ReadStartElement(this.KeyTagName);
			try
			{
				return (TKey)keySerializer.Deserialize(reader);
			}
			finally
			{
				reader.ReadEndElement();
			}
		}

		/// <summary>
		/// Deserializes the dictionary item's value.
		/// </summary>
		/// <param name="reader">The XML representation of the object.</param>
		/// <returns>The dictionary item's value.</returns>
		private TValue ReadValue(XmlReader reader)
		{
			reader.ReadStartElement(this.ValueTagName);
			try
			{
				return (TValue)valueSerializer.Deserialize(reader);
			}
			finally
			{
				reader.ReadEndElement();
			}
		}

		/// <summary>
		/// Serializes the dictionary item.
		/// </summary>
		/// <param name="writer">The XML writer to serialize to.</param>
		/// <param name="keyValuePair">The key/value pair.</param>
		private void WriteItem(XmlWriter writer, KeyValuePair<TKey, TValue> keyValuePair)
		{
			writer.WriteStartElement(this.ItemTagName);
			try
			{
				this.WriteKey(writer, keyValuePair.Key);
				this.WriteValue(writer, keyValuePair.Value);
			}
			finally
			{
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Serializes the dictionary item's key.
		/// </summary>
		/// <param name="writer">The XML writer to serialize to.</param>
		/// <param name="key">The dictionary item's key.</param>
		private void WriteKey(XmlWriter writer, TKey key)
		{
			writer.WriteStartElement(this.KeyTagName);
			try
			{
				keySerializer.Serialize(writer, key);
			}
			finally
			{
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Serializes the dictionary item's value.
		/// </summary>
		/// <param name="writer">The XML writer to serialize to.</param>
		/// <param name="value">The dictionary item's value.</param>
		private void WriteValue(XmlWriter writer, TValue value)
		{
			writer.WriteStartElement(this.ValueTagName);
			try
			{
				valueSerializer.Serialize(writer, value);
			}
			finally
			{
				writer.WriteEndElement();
			}
		}
	}
}

