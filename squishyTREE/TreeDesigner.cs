using System;
using System.Drawing.Design;
using System.IO;
using System.Web.UI.Design;
using System.Web.UI;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace squishyWARE.WebComponents.squishyTREE.Design
{
	/// <summary>
	/// Summary description for TreeDesigner.
	/// </summary>
	public class TreeDesigner : System.Web.UI.Design.ControlDesigner
	{
		public override string GetDesignTimeHtml() 
		{
			TreeView tvw = (TreeView) this.Component;
			tvw.Controls.Clear();
			tvw.ClearHeaders();
			tvw.AddHeader("DateTime Header", "yyyy-MM-dd", typeof(DateTime), "val1", "center");
			tvw.AddHeader("Double Header (currency)", "c", typeof(double), "val2", "right");
			tvw.AddHeader("Double Header", "n", typeof(double), "val3", "right");
			tvw.AddHeader("String Header", "", typeof(string), "val4", "left");

			TreeNode n1 = tvw.AddNode("Test Node 1", "n1", true);
			n1.AddTaggedValue("val1", "1/1/2001");
			n1.AddTaggedValue("val2", "90873");
			n1.AddTaggedValue("val3", "90873");
			n1.AddTaggedValue("val4", "Hello!");

			TreeNode n2 = tvw.AddNode("Test Node 2", "n2", true);
			n2.AddTaggedValue("val1", "1/1/2001");
			n2.AddTaggedValue("val2", "90873");
			n2.AddTaggedValue("val3", "90873");
			n2.AddTaggedValue("val4", "Hello!");

			n1.AddNode("Sub-node", "s1", false);
			n1.AddNode("Sub-node", "s2", false);

			n2.AddNode("Sub-node", "s3", true);
			n2.AddNode("Sub-node", "s4", true);

			tvw.ExpandAll();
			
			StringWriter sw = new StringWriter();
			HtmlTextWriter tw = new HtmlTextWriter(sw);

			tvw.RenderControl(tw);

			return sw.ToString();
		}
	}
}
