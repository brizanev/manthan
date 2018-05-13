using System;
using System.Web.UI;
using System.Drawing;
using System.Reflection;

namespace squishyWARE.WebComponents.squishyTREE
{
	public class TabularRenderingAgent : StandardRenderingAgent
	{
		public TabularRenderingAgent(TreeView tvw) : base(tvw) {}

		public override void RenderTreeStart(HtmlTextWriter output)
		{
			output.Write("<table cellpadding='" + this.TreeView.TableCellpadding.ToString() + 
				"' cellspacing='" + this.TreeView.TableCellspacing.ToString() + "' border='" + 
				this.TreeView.TableBorder.ToString() + "' bgcolor='" + 
				this.TreeView.TableBackColor.Name + "'");
			//write the width
			output.WriteAttribute("width", this.TreeView.Width.ToString());
			
			if(this.TreeView.headers.Count > 0)
			{
				output.WriteFullBeginTag("tr");
				output.WriteBeginTag("td");
				output.WriteAttribute("bgcolor", ColorTranslator.ToHtml(this.TreeView.HeaderBackColor));
				output.WriteAttribute("style", "color:" + ColorTranslator.ToHtml(this.TreeView.HeaderForeColor) + ";");
				output.WriteAttribute("align", this.TreeView.TableHeaderHorizAlign);
				output.Write(HtmlTextWriter.TagRightChar);
				output.Write("&nbsp;");
				output.WriteEndTag("td");
				foreach(object[] o in this.TreeView.headers)
				{
					output.WriteBeginTag("td");
					output.WriteAttribute("bgcolor", ColorTranslator.ToHtml(this.TreeView.HeaderBackColor));
					output.WriteAttribute("style", "color:" + ColorTranslator.ToHtml(this.TreeView.HeaderForeColor) + ";");
					output.WriteAttribute("align", this.TreeView.TableHeaderHorizAlign);
					output.Write(HtmlTextWriter.TagRightChar);
					output.Write(o[0]);
					output.WriteEndTag("td");
				}
				output.WriteEndTag("tr");
			}
		}
		public override void RenderTreeEnd(HtmlTextWriter output)
		{
			output.Write("</table>");
		}

		public override void RenderNodeStart(TreeNode node, HtmlTextWriter output)
		{
			output.WriteFullBeginTag("tr");
			output.WriteBeginTag("td");
			output.WriteAttribute("bgcolor", ColorTranslator.ToHtml(this.TreeView.DataBackColor));
			output.WriteAttribute("style", "color:" + 
				ColorTranslator.ToHtml(this.TreeView.DataForeColor) + ";");
			output.Write(HtmlTextWriter.TagRightChar);
			output.WriteBeginTag("div");
			output.WriteAttribute("style", "margin-left:" + (node.Indent * 10) + "px;");
			output.Write(HtmlTextWriter.TagRightChar);
		}
		public override void RenderNodeEnd(TreeNode node, HtmlTextWriter output)
		{
			output.WriteEndTag("div");
			output.WriteEndTag("td");
			//optionally write out all tagged values
			foreach(object[] o in this.TreeView.headers)
			{
				string displayName;
				string formatString;
				Type valueType;
				string tagValueName;

				displayName = (string) o[0];
				formatString = (string) o[1];
				valueType = (Type) o[2];
				tagValueName = (string) o[3];

				//write out a td for this header
				output.WriteBeginTag("td");
				output.WriteAttribute("bgcolor", ColorTranslator.ToHtml(this.TreeView.DataBackColor));
				output.WriteAttribute("style", "color:" + ColorTranslator.ToHtml(this.TreeView.DataForeColor) + ";");
				output.WriteAttribute("align", o[4].ToString());
				output.Write(HtmlTextWriter.TagRightChar);
				//is there data for this?
				if(node.TaggedValues[tagValueName] != null)
				{
					string outpt = node.TaggedValues[tagValueName];
					if(formatString.Trim() != "" && valueType.FullName != "System.String")
					{
						//create an object of the correct type by Parse()'ing it
						object theValue = valueType.InvokeMember(
							"Parse",
							BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod,
							null,
							null,
							new object[] { outpt }
							);
								
						//format the output
						outpt = (string) valueType.InvokeMember(
							"ToString", 
							BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
							null,
							theValue,
							new object[] { formatString }
							);
					}
					//now write the value
					output.Write(outpt);
				}
				else
				{
					output.Write("&nbsp;");
				}
				output.WriteEndTag("td");
			}
			output.WriteEndTag("tr");
		}
	}
}
