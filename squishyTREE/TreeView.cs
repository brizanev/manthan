using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Xml;

namespace squishyWARE.WebComponents.squishyTREE
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable()]
	public enum NodeDisplayStyle
	{
		/// <summary>
		/// Every node is a link
		/// </summary>
		Standard = 1,
		/// <summary>
		/// Leaf nodes are not linked
		/// </summary>
		LeafNodesNoLink
	}
	[Serializable()]
	public enum TreeOutputStyle
	{
		/// <summary>
		/// Nodes uses standard div tags to render output
		/// </summary>
		Standard = 1,
		/// <summary>
		/// Nodes use tables to render output
		/// </summary>
		/// <remarks>This option supports adding additional data to each node</remarks>
		Tabular = 2,
		/// <summary>
		/// Like Standard, however this output style dictates the folder images, adds 
		/// tree lines, and uses +/- images.
		/// </summary>
		WindowsLookAndFeel = 3
	}
	public class TreeViewNodeClickEventArgs : System.EventArgs
	{
		private TreeNode node;

		public TreeViewNodeClickEventArgs(TreeNode n)
		{
			this.node = n;
		}
		public TreeNode Node
		{
			get
			{
				return this.node;
			}
		}
	}
	/// <summary>
	/// Summary description for TreeView.
	/// </summary>
	[
	DefaultEvent("SelectedNodeChanged"),
	DefaultProperty("Text"), 
	ToolboxData("<{0}:TreeView runat=server></{0}:TreeView>"),
	Designer(typeof(Design.TreeDesigner))
	]
	public sealed class TreeView : Control, INamingContainer, IPostBackEventHandler
	{
		internal ArrayList headers = new ArrayList();
		private bool scrolling = false;
		private string collapsedImage = "";
		private string expandedImage = "";
		private string nonFolderImage = "";
		private string cssClass = "";
		private Unit width;
		private Unit height;
		private bool forceInheritedChecks = true;
		private NodeDisplayStyle nodeDisplayStyle = NodeDisplayStyle.Standard;
		private TreeOutputStyle outputStyle = TreeOutputStyle.Standard;
		private RenderingAgent renderingAgent;
		private string windowsLafImageBase = "./";

		#region tabular display fields
		private Color headerBackColor = Color.DarkGray;
		private Color headerForeColor = Color.White;
		private Color dataBackColor = Color.LightGray;
		private Color dataForeColor = Color.Black;
		private int tableCellpadding = 3;
		private int tableCellspacing = 1;
		private int tableBorder = 0;
		private Color tableBackColor = Color.Black;
		private string tableHeaderHorizAlign = "center";
		#endregion

		/// <summary>
		/// The event which indicates when a node has changed.
		/// </summary>
		public event EventHandler SelectedNodeChanged;
		/// <summary>
		/// The event which indicates when a node has been checked.
		/// </summary>
		public event EventHandler TreeNodeChecked;
	
		/// <summary>
		/// Create a new TreeView
		/// </summary>
		public TreeView() : base()
		{
			this.EnableViewState = true;
			this.renderingAgent = new StandardRenderingAgent(this);
		}
		/// <summary>
		/// Add a header to the TreeView. This is only usable in Tabular display format
		/// </summary>
		/// <param name="displayName">The display value of the header</param>
		/// <param name="formatString">The format string; leave blank to not format</param>
		/// <param name="valueType">The type of value stored in this data column</param>
		/// <param name="tagValueName">Used to retrieve the value for this header. When using <see cref="XmlBind"/>, this is the attribute name to retrieve</param>
		/// <param name="dataHorizAlign">The horizontal alignment of data column</param>
		public void AddHeader(string displayName, string formatString, Type valueType, string tagValueName,
			string dataHorizAlign)
		{
			object[] o = new object[] 
				{ 
					displayName, formatString, valueType, tagValueName, dataHorizAlign
				};
			this.headers.Add(o);
		}
		/// <summary>
		/// Remove all headers from this tree view
		/// </summary>
		public void ClearHeaders()
		{
			this.headers.Clear();
		}
		/// <summary>
		/// Find a given treenode based off its key value
		/// </summary>
		/// <param name="key"></param>
		/// <returns>The specified TreeNode or null if not found</returns>
		public TreeNode FindTreeNode(string key)
		{
			//search the top level first
			foreach(TreeNode n in this.Controls)
			{
				if(n.Key == key)
				{
					return n;
				}
			}
			//search each TreeNode for the correct node
			foreach(TreeNode n in this.Controls)
			{
				TreeNode found = n.FindTreeNode(key);
				if(found != null)
					return found;
			}
			//couldn't find it
			return null;
		}
		/// <summary>
		/// Set the model for this TreeView, and automatically call the FillData
		/// method of the ITreeViewModel passed in.
		/// </summary>
		/// <param name="m"></param>
		public void SetDataModel(ITreeViewModel m)
		{
			m.FillData(this);
		}
		/// <summary>
		/// Guarantee that the __doPostBack function is defined
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			this.Page.GetPostBackEventReference(this);
		}

		/// <summary>
		/// The Width of the TreeView
		/// </summary>
		[Category("Appearance")]
		public Unit Width
		{
			get { return this.width; }
			set { this.width = value; }
		}
		/// <summary>
		/// The Height of the TreeView
		/// </summary>
		[Category("Appearance")]
		public Unit Height
		{
			get { return this.height; }
			set { this.height = value; }
		}
		/// <summary>
		/// The CssClass for the TreeNodes
		/// </summary>
		[Category("Appearance")]
		[Description("The CssClass for the TreeNodes")]
		public string CssClass
		{
			get { return this.cssClass; }
			set { this.cssClass = value; }
		}
		/// <summary>
		/// The output style of the TreeView
		/// </summary>
		[Category("Display")]
		[Description("The output style of the TreeView")]
		public TreeOutputStyle TreeOutputStyle
		{
			get
			{
				return this.outputStyle;
			}
			set
			{
				this.outputStyle = value;
				switch(this.outputStyle)
				{
					case TreeOutputStyle.Standard:
						this.renderingAgent = new StandardRenderingAgent(this);
						break;
					case TreeOutputStyle.Tabular:
						this.renderingAgent = new TabularRenderingAgent(this);
						break;
					case TreeOutputStyle.WindowsLookAndFeel:
						this.renderingAgent = new WindowsLookAndFeelRenderingAgent(this);
						break;
					default:
						throw new ArgumentException("Invalid TreeOutputStyle");
				}
			}
		}
		/// <summary>
		/// How nodes display themselves as links or not links
		/// </summary>
		[Category("Display")]
		[Description("How nodes display themselves as links or not links")]
		public NodeDisplayStyle NodeDisplayStyle
		{
			get
			{
				return this.nodeDisplayStyle;
			}
			set
			{
				this.nodeDisplayStyle = value;
			}
		}
		[Category("Behavior")]
		[Description("When set to true, checking a parent causes all its children to check, and checkng a child causes all of its parents to check")]
		public bool ForceInheritedChecks
		{
			get
			{
				return forceInheritedChecks;
			}
			set
			{
				forceInheritedChecks = value;
			}
		}
		[Category("Node Appearance")]
		[Description("The image that appears on nodes that contain no children")]
		public string NonFolderImage
		{
			get
			{
				return this.nonFolderImage;
			}
			set
			{
				this.nonFolderImage = value;
			}
		}
		[Category("Node Appearance")]
		[Description("The image that appears on expanded nodes that contain children")]
		public string ExpandedImage
		{
			get
			{
				return this.expandedImage;
			}
			set
			{
				this.expandedImage = value;
			}
		}
		[Category("Node Appearance")]
		[Description("The image that appears on collapsed nodes that contain children")]
		public string CollapsedImage
		{
			get
			{
				return this.collapsedImage;
			}
			set
			{
				this.collapsedImage = value;
			}
		}
		[Category("Tree Appearance")]
		[Description("Adds an overflow:auto style to the div tag. This only works with Standard style")]
		public bool Scrolling
		{
			get
			{
				return this.scrolling;
			}
			set
			{
				this.scrolling = value;
			}
		}
		[Category("Tabular Appearance")]
		[Description("The horizontal align of the header table")]
		public string TableHeaderHorizAlign
		{
			get { return this.tableHeaderHorizAlign; }
			set { this.tableHeaderHorizAlign = value; }
		}
		[Category("Tabular Appearance")]
		[Description("The back color for the table")]
		public Color TableBackColor
		{
			get	{ return this.tableBackColor; }
			set { this.tableBackColor = value; }
		}
		[Category("Tabular Appearance")]
		[Description("The cellpadding of the table")]
		public int TableCellpadding
		{
			get	{ return this.tableCellpadding; }
			set { this.tableCellpadding = value; }
		}
		[Category("Tabular Appearance")]
		[Description("The cellspacing of the table")]
		public int TableCellspacing
		{
			get	{ return this.tableCellspacing; }
			set { this.tableCellspacing = value; }
		}
		[Category("Tabular Appearance")]
		[Description("The border size of the table")]
		public int TableBorder
		{
			get	{ return this.tableBorder; }
			set { this.tableBorder = value; }
		}
		[Category("Tabular Appearance")]
		[Description("The back color for header text")]
		public Color HeaderBackColor
		{
			get
			{
				return this.headerBackColor;
			}
			set
			{
				this.headerBackColor = value;
			}
		}
		[Category("Tabular Appearance")]
		[Description("The fore color for header text")]
		public Color HeaderForeColor
		{
			get
			{
				return this.headerForeColor;
			}
			set
			{
				this.headerForeColor = value;
			}
		}
		[Category("Tabular Appearance")]
		[Description("The back color for data value text")]
		public Color DataBackColor
		{
			get
			{
				return this.dataBackColor;
			}
			set
			{
				this.dataBackColor = value;
			}
		}
		[Category("Tabular Appearance")]
		[Description("The fore color for data value text")]
		public Color DataForeColor
		{
			get
			{
				return this.dataForeColor;
			}
			set
			{
				this.dataForeColor = value;
			}
		}

		[Category("Windows Look And Feel Appearance")]
		[Description("The root, resolved URL where the images for the TreeView will reside. You may use ~ in this to indicate your VRoot.")]
		public string WindowsLafImageBase
		{
			get
			{
				string wlaf = this.ResolveUrl(this.windowsLafImageBase);
				if(!wlaf.EndsWith("/"))
				{
					wlaf += "/";
				}
				return wlaf;
			}
			set
			{
				this.windowsLafImageBase = value;
			}
		}

		[Browsable(false)]
		public RenderingAgent RenderingAgent
		{
			get { return this.renderingAgent; }
		}

		/// <summary>
		/// Handle the click of a node
		/// </summary>
		/// <param name="eventArgument"></param>
		public void RaisePostBackEvent(String eventArgument)
		{
			bool checkBoxClicked = false;
			if(eventArgument.IndexOf("checkbox") > -1)
			{
				eventArgument = eventArgument.Replace(":checkbox", "");
				checkBoxClicked = true;
			}
						
			TreeNode selectedNode = (TreeNode) this.Page.FindControl(eventArgument);
			if(!checkBoxClicked)
			{
				selectedNode.IsExpanded = !selectedNode.IsExpanded;

				if(SelectedNodeChanged != null)
				{
					SelectedNodeChanged(this, new TreeViewNodeClickEventArgs(selectedNode));
				}
			}
			else
			{
				//check the checkboxes
				selectedNode.Check();
				//force the parent to track its children's checked state. if any child in the 
				//hierarchy is checked, then all the parents of this node need to also
				//be checked
				if(selectedNode.Parent is TreeNode)
				{
					((TreeNode)selectedNode.Parent).trackCheckedChildren();
				}
				//raise the event
				if(TreeNodeChecked != null)
				{
					TreeNodeChecked(this, new TreeViewNodeClickEventArgs(selectedNode));
				}
			}
		}
		/// <summary>
		/// Bind the TreeView to an XML document.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="textAttribute"></param>
		/// <param name="keyAttribute"></param>
		/// <param name="checkAttribute">The name of the checkbox attribute that contains a true/false value indicating whether to show or not show the checkbox</param>
		/// <param name="checkDefaultsAttribute">The name of the checkbox attribute that contains a true/false value indicating whether to check or not check the checkbox</param>
		public void XmlBind(System.Xml.XmlDocument doc, string textAttribute, string keyAttribute, string checkAttribute, string checkDefaultsAttribute)
		{
			foreach(XmlNode node in doc.DocumentElement.ChildNodes)
			{
				this.addChildNode(node, textAttribute, keyAttribute, checkAttribute, checkDefaultsAttribute);
			}
		}
		/// <summary>
		/// Bind an XML document without checkboxes.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="textAttribute"></param>
		/// <param name="keyAttribute"></param>
		public void XmlBind(System.Xml.XmlDocument doc, string textAttribute, string keyAttribute)
		{
			this.XmlBind(doc, textAttribute, keyAttribute, "", "");
		}
		/// <summary>
		/// Bind the TreeView to a DataSet
		/// </summary>
		/// <param name="data">The DataSet to bind to</param>
		/// <param name="dataTable">The DataTable to bind to</param>
		/// <param name="textColumn"></param>
		/// <param name="keyColumn"></param>
		/// <param name="checkColumn">Whether or not to show a checkbox</param>
		/// <param name="checkDefaultsColumn">Whether or not to default the checkbox to checked</param>
		public void DataSetBind(DataSet data, string dataTable, string textColumn, string keyColumn, string checkColumn, string checkDefaultsColumn)
		{
			foreach(DataRow row in data.Tables[dataTable].Rows)
			{
				this.AddChildNode(row, textColumn, keyColumn, checkColumn, checkDefaultsColumn, null);
			}
		}
		/// <summary>
		/// Binds a DataSet without checkboxes
		/// </summary>
		/// <param name="data">The DataSet to bind to</param>
		/// <param name="dataTable">The DataTable to bind to</param>
		/// <param name="textColumn"></param>
		/// <param name="keyColumn"></param>
		public void DataSetBind(DataSet data, string dataTable, string textColumn, string keyColumn)
		{
			DataSetBind(data, dataTable, textColumn, keyColumn, "", "");
		}
		/// <summary>
		/// Expand all TreeNodes in the TreeView
		/// </summary>
		public void ExpandAll()
		{
			foreach(Control c in this.Controls)
			{
				TreeNode n = c as TreeNode;
				if(n != null)
					n.ExpandAll();
			}
		}
		/// <summary>
		/// Collapse all TreeNodes in the TreeView
		/// </summary>
		public void CollapseAll()
		{
			foreach(Control c in this.Controls)
			{
				TreeNode n = c as TreeNode;
				if(n != null)
					n.CollapseAll();
			}
		}


		private void addChildNode(XmlNode node, string textAttribute, string keyAttribute, string checkAttribute, string checkDefaultsAttribute)
		{
			this.addChildNode(node, textAttribute, keyAttribute, null, checkAttribute, checkDefaultsAttribute);
		}
		private void AddChildNode(DataRow row, string textColumn, string keyColumn, string checkColumn, 
			string checkDefaultsColumn, TreeNode node)
		{
			string text;
			string key;
			bool checkbox = false;
			bool isChecked = false;

			text = row[textColumn].ToString();
			key = row[keyColumn].ToString();
			if(checkColumn != "")
			{
				if(row[checkColumn] != DBNull.Value)
				{
					checkbox = Convert.ToBoolean(row[checkColumn]);
				}
				if(row[checkDefaultsColumn] != DBNull.Value)
				{
					isChecked = Convert.ToBoolean(row[checkDefaultsColumn]);
				}
			}

			TreeNode n;
			if(node == null)
			{//add to the treeview
				n = this.AddNode(text, key, checkbox);
			}
			else
			{//add to the node
				n = node.AddNode(text, key, checkbox);
			}
			//now add the tagged values
			if(this.headers.Count != 0)
			{
				foreach(object[] o in this.headers)
				{
					//only add the tagged value if this row contains the column in question
					if(row.Table.Columns.Contains(o[0].ToString()))
					{
						//o[0] is the column name
						n.AddTaggedValue(o[0].ToString(), row[o[0].ToString()].ToString());
					}
				}
			}
			//now look for child rows
			if(row.Table.ChildRelations.Count > 0)
			{
				foreach(DataRelation r in row.Table.ChildRelations)
				{
					DataRow[] childRows = row.GetChildRows(r);
					foreach(DataRow childRow in childRows)
					{
						this.AddChildNode(childRow, textColumn, keyColumn, checkColumn, checkDefaultsColumn,
							n);
					}
				}
			}
			if(checkbox)
			{
				if(!this.Page.IsPostBack)
				{
					n.Check(isChecked);
				}
			}
		}
		private void addChildNode(XmlNode node, string textAttribute, string keyAttribute, TreeNode tNode, string checkAttribute, string checkDefaultsAttribute)
		{
			string text;
			string key = "";
			bool checkbox = false;
			bool isChecked = false;

			text = node.Attributes[textAttribute].Value;
			if(node.Attributes[keyAttribute] != null)
			{
				key = node.Attributes[keyAttribute].Value;
			}
			if(node.Attributes[checkAttribute] != null)
			{
				checkbox = Convert.ToBoolean(node.Attributes[checkAttribute].Value);
			}
			if(node.Attributes[checkDefaultsAttribute] != null)
			{
				isChecked = Convert.ToBoolean(node.Attributes[checkDefaultsAttribute].Value);
			}

			TreeNode n;
			if(tNode == null)
			{//add to the TreeView
				n = this.AddNode(text, key, checkbox);
			}
			else
			{//add to the TreeNode
				n = tNode.AddNode(text, key, checkbox);
			}
			//get the tagged values
			XmlNodeList taggedValues = node.SelectNodes("taggedValue");
			int tagCount = 0;
			foreach(XmlNode tg in taggedValues)
			{
				tagCount++;
				string tagName = tg.Attributes["tagName"].Value;
				string tagValue = tg.Attributes["tagValue"].Value;

				n.AddTaggedValue(tagName, tagValue);
			}
			if(node.ChildNodes.Count - tagCount > 0)
			{
				//add the child nodes of this node
				foreach(XmlNode child in node.ChildNodes)
				{
					if(child.Name != "taggedValue")
					{
						this.addChildNode(child, textAttribute, keyAttribute, n, checkAttribute, checkDefaultsAttribute);
					}
				}
			}
			if(checkDefaultsAttribute != "" && node.Attributes[checkDefaultsAttribute] != null &&
				node.Attributes[checkDefaultsAttribute].Value.Trim() != "")
			{
				if(!this.Page.IsPostBack)
				{
					n.Check(isChecked);
				}
			}
		}
		/// <summary>
		/// Add a new TreeNode to the TreeView
		/// </summary>
		/// <param name="text"></param>
		/// <param name="key"></param>
		/// <param name="showCheckbox"></param>
		/// <returns>The added TreeNode</returns>
		public TreeNode AddNode(string text, string key, bool showCheckbox)
		{
			TreeNode n = new TreeNode(text, 1, key, showCheckbox);
			this.Controls.Add(n);
			return n;
		}
		/// <summary>
		/// Add a new TreeNode to the TreeView
		/// </summary>
		/// <param name="text"></param>
		/// <param name="key"></param>
		/// <returns>The added TreeNode</returns>
		public TreeNode AddNode(string text, string key)
		{
			return this.AddNode(text, key, false);
		}
		/// <summary>
		/// Render this control.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			this.RenderingAgent.RenderTreeStart(writer);
			base.Render(writer);
			this.RenderingAgent.RenderTreeEnd(writer);
		}

		/// <summary>
		/// Turn the treeview into XML
		/// </summary>
		/// <returns></returns>
		protected override object SaveViewState()
		{
			//turn the treeview into XML
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, System.Text.Encoding.Default);

			writer.WriteStartElement("treeview", "");
			//write out all the child nodes
			foreach(TreeNode n in this.Controls)
			{
				n.WriteXml(writer);
			}
			writer.WriteEndElement();
			writer.Flush();
			//writing the XML is done, read it back out
			stream.Position = 0;
			System.IO.StreamReader reader = new System.IO.StreamReader(stream);
			string xml = reader.ReadToEnd();
			writer.Close();
			stream.Close();
			return xml;
		}
		/// <summary>
		/// Load the TreeView from the view state XML
		/// </summary>
		/// <param name="savedState"></param>
		protected override void LoadViewState(object savedState)
		{
			//load the treeview back from XML
			if(savedState != null)
			{
				string xml = savedState.ToString();
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml);
				//xmlbind
				this.XmlBind(
					doc,
					TreeView.textAttribute,
					TreeView.keyAttribute,
					TreeView.showChkAttribute,
					TreeView.chkAttribute
					);
			}
		}

		internal const string textAttribute = "t";
		internal const string keyAttribute = "k";
		internal const string chkAttribute = "c";
		internal const string showChkAttribute = "s";
	}
}
