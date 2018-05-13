using System;
using System.Web.UI;

namespace squishyWARE.WebComponents.squishyTREE
{
	public abstract class RenderingAgent
	{
		private TreeView tvw;

		protected RenderingAgent(TreeView tvw) { this.tvw = tvw; }

		protected TreeView TreeView
		{
			get { return tvw; }
		}

		public abstract void RenderNodeStart(TreeNode node, HtmlTextWriter output);
		public abstract void RenderNodeEnd(TreeNode node, HtmlTextWriter output);
		public abstract void RenderImageLink(TreeNode node, HtmlTextWriter output);
		public abstract void RenderCheckbox(TreeNode node, HtmlTextWriter output);
		public abstract void RenderNodeText(TreeNode node, HtmlTextWriter output);
		public abstract void RenderTreeStart(HtmlTextWriter output);
		public abstract void RenderTreeEnd(HtmlTextWriter output);
	}
}
