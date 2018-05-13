using System;
using System.Web.UI;

namespace squishyWARE.WebComponents.squishyTREE
{
	public class StandardRenderingAgent : RenderingAgent
	{
		public StandardRenderingAgent(TreeView tvw) : base(tvw) {}

		public override void RenderTreeStart(HtmlTextWriter output)
		{
			output.WriteBeginTag("div");

			string style = "overflow:";
			if(this.TreeView.Scrolling)
				style += "auto";
			else
				style += "none";
			if(!this.TreeView.Width.IsEmpty)
				style += ";width:" + this.TreeView.Width.ToString();
			if(!this.TreeView.Height.IsEmpty)
				style += ";height:" + this.TreeView.Height.ToString();
			output.WriteAttribute("style", style);
			output.Write(HtmlTextWriter.TagRightChar);
		}
		public override void RenderTreeEnd(HtmlTextWriter output)
		{
			output.WriteEndTag("div");
		}

		public override void RenderNodeStart(TreeNode node, HtmlTextWriter output)
		{
			output.WriteBeginTag("div");
			output.WriteAttribute("style", "margin-left:" + (node.Indent * 10) + "px;");
			output.Write(HtmlTextWriter.TagRightChar);
		}
		public override void RenderNodeEnd(TreeNode node, HtmlTextWriter output)
		{
			output.WriteEndTag("div");
		}
		public override void RenderImageLink(TreeNode node, HtmlTextWriter output)
		{
			if(node.IsFolder)
			{
				string script = this.TreeView.Page.GetPostBackClientHyperlink(
					this.TreeView, node.UniqueID);

				if(this.TreeView.ExpandedImage.Trim().Length != 0 &&
					this.TreeView.ExpandedImage.Trim().Length != 0)
				{
					output.WriteBeginTag("a");
					output.WriteAttribute("href", script, false);
					output.Write(HtmlTextWriter.TagRightChar);

					if(node.IsExpanded)
					{
						output.Write("<img src='");
						output.Write(this.TreeView.ExpandedImage);
						output.Write("' border='0'>");
					}
					else
					{
						output.Write("<img src='");
						output.Write(this.TreeView.CollapsedImage);
						output.Write("' border='0'>");
					}
					output.WriteEndTag("a");
				}
			}
			else
			{
				if(this.TreeView.NonFolderImage.Trim().Length != 0)
				{
					output.Write("<img src='");
					output.Write(this.TreeView.NonFolderImage);
					output.Write("'>");
				}
				else
				{
					output.Write("&nbsp;&nbsp;&nbsp;&nbsp;");
				}
			}
		}
		public override void RenderCheckbox(TreeNode node, HtmlTextWriter output)
		{
			if(node.ShowCheckBox)
			{
				string script = this.TreeView.Page.GetPostBackClientEvent(this.TreeView, node.UniqueID + ":checkbox");
				output.Write("<input type='checkbox' onclick=\"this.blur();");
				output.Write(script);
				output.Write("\"");
				if(node.IsChecked)
				{
					output.Write(" checked");
				}
				output.Write(">");
			}
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
			}
			output.Write(node.Text);
			if(!useLink)
			{
				output.WriteEndTag("a");
			}
		}
	}
}
