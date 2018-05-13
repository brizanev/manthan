using System;
using System.Drawing;
using System.Reflection;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace squishyWARE.WebComponents.squishyTREE
{
	[DesignTimeVisible(false)]
	public sealed class TreeNode : System.Web.UI.Control, INamingContainer
	{
		private NameValueCollection taggedValues = new NameValueCollection();
		private string text;
		private string key;
		private bool showCheckBox;
		private int indent;
		private TreeView parentTree;
		private TreeNode parentNode;

		internal TreeNode(string text, int indent, string key, bool showCheckbox) : base()
		{
			this.text = text;
			this.indent = indent;
			this.key = key;
			this.showCheckBox = showCheckbox;
		}
		internal TreeNode(string text, int indent, string key) : 
			this(text, indent, key, false) {}

		/// <summary>
		/// Adds a tagged value to this node (used for tabular layout with headers
		/// </summary>
		/// <param name="tagName"></param>
		/// <param name="value"></param>
		public void AddTaggedValue(string tagName, string value)
		{
			this.taggedValues.Add(tagName, value);
		}
		/// <summary>
		/// Write an XML version of this tree node to the given XmlTextWriter
		/// </summary>
		/// <param name="writer"></param>
		internal void WriteXml(System.Xml.XmlTextWriter writer)
		{
			//write the opening element
			writer.WriteStartElement("treenode");
			//write out the attributes
			writer.WriteAttributeString(
				TreeView.textAttribute,
				"",
				this.Text);
			writer.WriteAttributeString(
				TreeView.keyAttribute,
				"",
				this.Key);
			if( this.showCheckBox == false )
			{
				//don't even put the checkbox attribute in
			}
			else
			{
				//write the showcheckbox attribute
				writer.WriteAttributeString(
					TreeView.showChkAttribute,
					"",
					"True");
				//write the is checked attribute
				writer.WriteAttributeString(
					TreeView.chkAttribute,
					"",
					this.IsChecked.ToString());
			}
			//write out any tagged values
			for(int i = 0; i < this.taggedValues.Count; i++)
			{
				writer.WriteStartElement("taggedValue", "");
				writer.WriteAttributeString("", "tagName", "", this.taggedValues.Keys[i]);
				writer.WriteAttributeString("", "tagValue", "", this.taggedValues.GetValues(this.taggedValues.Keys[i])[0]);
				writer.WriteEndElement();
			}
			//write out any child treenodes
			foreach(TreeNode n in this.Controls)
			{
				n.WriteXml(writer);
			}
			writer.WriteEndElement();
		}
		/// <summary>
		/// Find the next TreeNode
		/// </summary>
		/// <returns>The next sibling TreeNode, or null if there are no TreeNode siblings.</returns>
		public TreeNode NextSibling()
		{
			int thisIndex = this.Parent.Controls.IndexOf(this);
			try
			{
				if(this.Parent.Controls[thisIndex + 1] == null ||
					!(this.Parent.Controls[thisIndex + 1] is TreeNode))
				{
					return null;
				}
				else
				{
					return (TreeNode) this.Parent.Controls[thisIndex + 1];
				}
			}
			catch(ArgumentException) { return null; }
		}
		/// <summary>
		/// Expand the whole hierarchy of nodes.
		/// </summary>
		public void ExpandAll()
		{
			this.IsExpanded = true;
			foreach(TreeNode n in this.Controls)
			{
				n.ExpandAll();
			}
		}
		/// <summary>
		/// Collapse the whole hierarchy of nodes.
		/// </summary>
		public void CollapseAll()
		{
			this.IsExpanded = false;
			foreach(TreeNode n in this.Controls)
			{
				n.CollapseAll();
			}
		}
		/// <summary>
		/// Add a new TreeNode child to this node.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="key"></param>
		/// <param name="showCheckbox"></param>
		/// <returns></returns>
		public TreeNode AddNode(string text, string key, bool showCheckbox)
		{
			TreeNode n = new TreeNode(text, this.indent + 1, key, showCheckbox);
			n.parentNode = this;
			this.Controls.Add(n);
			return n;
		}
		/// <summary>
		/// Add a new TreeNode child to this node.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public TreeNode AddNode(string text, string key)
		{
			return this.AddNode(text, key, false);
		}
		/// <summary>
		/// Find a child TreeNode given its key value
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
			//search each child tree node
			foreach(TreeNode n in this.Controls)
			{
				TreeNode found = n.FindTreeNode(key);
				if(found != null)
					return found;
			}
			//couldn't find it
			return null;
		}

		#region Internal Properties (used mostly by renderers)
		internal bool ShowCheckBox
		{
			get { return this.showCheckBox; }
		}

		internal int Indent
		{
			get { return this.indent; }
		}

		internal bool IsFolder
		{
			get { return this.HasControls(); }
		}
		#endregion

		#region public properties
		public NameValueCollection TaggedValues
		{
			get
			{
				return this.taggedValues;
			}
		}
		public string Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}
		public bool IsExpanded
		{
			get
			{
				return Convert.ToBoolean(ViewState["IsExpanded"]);
			}
			set
			{
				ViewState["IsExpanded"] = value;
			}
		}
		public bool IsChecked
		{
			get
			{
				if(ViewState["IsChecked"] == null)
				{
					//default to not checked
					return false;
				}
				else
				{
					return Convert.ToBoolean(ViewState["IsChecked"]);
				}
			}
			set
			{
				ViewState["IsChecked"] = value;
			}
		}

		public string Text 
		{
			get
			{
				return text;
			}

			set
			{
				text = value;
			}
		}
		#endregion

		[Obsolete("", false)]
		private TreeView findTreeView()
		{
			if(this.parentTree == null)
			{
				TreeNode current = this;
				//walk up the tree, find the parent TreeView
				while(current.Parent.GetType() != typeof(TreeView))
				{
					current = (TreeNode) current.Parent;
				}
				this.parentTree = (TreeView) current.Parent;
			}
			return this.parentTree;
		}

		private TreeView FindTreeView()
		{
			if(this.parentTree == null)
			{
				TreeNode current = this;
				//walk up the tree, find the parent TreeView
				while(current.Parent.GetType() != typeof(TreeView))
				{
					current = (TreeNode) current.Parent;
				}
				this.parentTree = (TreeView) current.Parent;
			}
			return this.parentTree;
		}

		internal void Check(bool isChecked)
		{
			this.IsChecked = isChecked;
			if(this.findTreeView().ForceInheritedChecks)
			{
				foreach(TreeNode n in this.Controls)
				{
					n.Check(isChecked);
				}
			}
		}
		/// <summary>
		/// Check the tree node
		/// </summary>
		public void Check()
		{
			Check(!this.IsChecked);
		}
		internal void trackCheckedChildren()
		{
			if(this.findTreeView().ForceInheritedChecks)
			{
				bool hasCheckedChildren = this.hasCheckedChildren();
				if(hasCheckedChildren == true)
					this.IsChecked = true;
				else
					this.IsChecked = false;
				//go up through the hierarchy
				if(this.Parent is TreeNode)
				{
					((TreeNode)this.Parent).trackCheckedChildren();
				}
			}
		}
		private bool hasCheckedChildren()
		{
			bool ret = false;
			foreach(TreeNode n in this.Controls)
			{
				if(n.IsChecked)
				{
					ret = true;
					//stop the loop
					break;
				}
				else
				{
					ret = n.hasCheckedChildren();
					if(ret == true)
						break;
				}
			}
			return ret;
		}

		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{			
			this.FindTreeView().RenderingAgent.RenderNodeStart(this, output);
			this.FindTreeView().RenderingAgent.RenderImageLink(this, output);
			this.FindTreeView().RenderingAgent.RenderCheckbox(this, output);
			this.FindTreeView().RenderingAgent.RenderNodeText(this, output);
			this.FindTreeView().RenderingAgent.RenderNodeEnd(this, output);
			
			if(this.IsFolder && this.IsExpanded && this.Controls.Count > 0)
			{
				//render the child controls
				this.RenderChildren(output);
			}
		}

		public TreeNode ParentNode
		{
			get
			{
				return parentNode;
			}
		}

	}
}
