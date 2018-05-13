using System;
using System.Text;
using System.Web.UI;

namespace squishyWARE.WebComponents.squishyTREE
{
	/// <summary>
	/// Summary description for WindowsLookAndFeelRenderingAgent.
	/// </summary>
	public class WindowsLookAndFeelRenderingAgent : StandardRenderingAgent
	{
		private bool first = true;
		public WindowsLookAndFeelRenderingAgent(TreeView tvw) : base(tvw) {}

		public override void RenderNodeStart(TreeNode node, HtmlTextWriter output)
		{
			output.Write("<tr><td><nobr>");
		}

		public override void RenderNodeEnd(TreeNode node, HtmlTextWriter output)
		{
			output.Write("</nobr></td></tr>");
		}

		public override void RenderTreeStart(HtmlTextWriter output)
		{
			output.WriteBeginTag("table");
			output.WriteAttribute("cellpadding", "0");
			output.WriteAttribute("cellspacing", "0");
			output.WriteAttribute("border", "0");
			output.Write(HtmlTextWriter.TagRightChar);
		}
		public override void RenderTreeEnd(HtmlTextWriter output)
		{
			output.WriteEndTag("table");
		}

		private bool ParentHasSibling(TreeNode source, int indentDiff)
		{
			Control ctl = source;
			for(int i = 0; i < indentDiff; i++)
			{
				ctl = ctl.Parent;
			}
			if(ctl is TreeNode)
			{
				return (((TreeNode) ctl).NextSibling() != null);
			}
			else
			{
				return false;
			}
		}
		private bool IsFirst()
		{
			if(first)
			{
				this.first = false;
				return true;
			}
			return false;
		}
		public override void RenderImageLink(TreeNode node, HtmlTextWriter output)
		{
			int indent = node.Indent; //0-based

			StringBuilder sb = new StringBuilder();
			while(indent > 0) //each indent level means one image
			{
				bool hasSibling, parentHasSibling, isTop;

				hasSibling = node.NextSibling() != null;
				isTop = node.Parent is TreeView;

				if(node.Parent is TreeNode)
					parentHasSibling = ParentHasSibling(node, node.Indent - indent);
				else
				{
					parentHasSibling = false;
				}

				if(node.Indent == indent) //first item in the indent
				{
					if(node.HasControls()) //there are children here
					{	
						string anchorStart, anchorEnd;

						anchorStart = "<a href=\"" +
							this.TreeView.Page.GetPostBackClientHyperlink(this.TreeView, node.UniqueID) +
							"\">";
						anchorEnd = "</a>";


						if(hasSibling) //down dots
						{
							if(node.IsExpanded) //minus image
							{
								if(IsFirst())
									sb.Insert(0, anchorStart + "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "topexpandedsibling.gif' border='0'>" + anchorEnd);
								else
									sb.Insert(0, anchorStart + "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "middleexpandedsibling.gif' border='0'>" + anchorEnd);
							}
							else //plus image
							{
								if(IsFirst())
									sb.Insert(0, anchorStart + "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "topcollapsedsibling.gif' border='0'>" + anchorEnd);
								else
									sb.Insert(0, anchorStart + "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "middlecollapsedsibling.gif' border='0'>" + anchorEnd);
							}
						}
						else //no down dots
						{
							if(node.IsExpanded) //minus image
							{
								if(IsFirst())
									sb.Insert(0, anchorStart + "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "topexpandednosibling.gif' border='0'>" + anchorEnd);
								else
									sb.Insert(0, anchorStart + "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "middleexpandednosibling.gif' border='0'>" + anchorEnd);
							}
							else //plus image
							{
								if(IsFirst())
									sb.Insert(0, anchorStart + "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "topcollapsednosibling.gif' border='0'>" + anchorEnd);
								else
									sb.Insert(0, anchorStart + "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "middlecollapsednosibling.gif' border='0'>" + anchorEnd);
							}
						}
					}
					else // no children
					{
						if(hasSibling)
						{
							sb.Insert(0, "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "middlesiblingnochildren.gif' border='0'>");
						}
						else
						{
							sb.Insert(0, "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "bottomnosiblingnochildren.gif' border='0'>");
						}
					}
				}
				else // prior item in the indent
				{
					if(parentHasSibling)
						sb.Insert(0, "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "vertbardots.gif' border='0'>");
					else
						sb.Insert(0, "<img align='top' src='" + this.TreeView.WindowsLafImageBase + "clear.gif' border='0'>");
				}
				indent--;
			}
			output.Write(sb.ToString());
		}
		public override void RenderNodeText(TreeNode node, HtmlTextWriter output)
		{
			bool useLink = node.Controls.Count == 0 &&
				this.TreeView.NodeDisplayStyle == NodeDisplayStyle.LeafNodesNoLink;
			if(!useLink)
			{
				//name the anchor, in case you need to jump
				output.Write("<a name='" + node.Key + "'>&nbsp;</a>");
				output.WriteBeginTag("a");
				output.WriteAttribute("href", this.TreeView.Page.GetPostBackClientHyperlink(this.TreeView, node.UniqueID), false);
				output.WriteAttribute("class", this.TreeView.CssClass);
				output.Write(HtmlTextWriter.TagRightChar);

				if(node.IsExpanded)
				{
					output.Write("<img src='" + this.TreeView.WindowsLafImageBase + "openfolder.gif' border='0'>");
				}
				else
				{
					output.Write("<img src='" + this.TreeView.WindowsLafImageBase + "closedfolder.gif' border='0'>");
				}
			}
			output.Write("&nbsp;");
			output.Write(node.Text);
			if(!useLink)
			{
				output.WriteEndTag("a");
			}
		}
	}
}
