using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections;
using squishyWARE.WebComponents.squishyTREE;
using Codefresh.PhotoBrowserLibrary;
using Codefresh.PhotoBrowserLibrary.Collections;

namespace Codefresh.PhotosBrowser
{

	/// <summary>
	///	This control renders the entire photo album browser. The control is comprised of
	///	a treeview, used to navigate between the albums, and a content pane that adjusts
	///	itself depending on whether an album or individual photo is being displayed.
	/// </summary>
	public class PhotoBrowser : System.Web.UI.UserControl
	{

		// Flag used to specify whether to display the navigation hyperlinks
		// at the bottom of the individual photo page
		private const bool DISPLAY_BOTTOM_IMAGE_NAVIGATION = true;

		protected System.Web.UI.WebControls.Label lblPreviousImage;
		protected System.Web.UI.WebControls.Label lblNextImage;
		protected squishyWARE.WebComponents.squishyTREE.TreeView tvwMain;
		protected System.Web.UI.WebControls.Panel pnlPhotoGridContents;
		protected System.Web.UI.WebControls.HyperLink hylPreviousPage1;
		protected System.Web.UI.WebControls.PlaceHolder plhPageLinks1;
		protected System.Web.UI.WebControls.HyperLink hylNextPage1;
		protected System.Web.UI.WebControls.Table tblPhotos;
		protected System.Web.UI.WebControls.HyperLink hylPreviousPage2;
		protected System.Web.UI.WebControls.PlaceHolder plhPageLinks2;
		protected System.Web.UI.WebControls.HyperLink hylNextPage2;
		protected System.Web.UI.WebControls.Panel pnlPhotoContents;
		protected System.Web.UI.WebControls.HyperLink hlkPreviousImage;
		protected System.Web.UI.WebControls.HyperLink hlkReturnToThumbnails1;
		protected System.Web.UI.WebControls.HyperLink hlkNextImage;
		protected System.Web.UI.WebControls.Image imgPhoto;
		protected System.Web.UI.WebControls.Button btnAddComment;
		protected System.Web.UI.WebControls.TextBox txtCommentText;
		protected System.Web.UI.WebControls.TextBox txtCommentName;
		protected System.Web.UI.WebControls.Literal litPhotoComments;
		protected System.Web.UI.WebControls.Panel pnlComments;
		protected System.Web.UI.WebControls.Panel pnlImageNavigationBottom;
		protected System.Web.UI.WebControls.HyperLink hlkReturnToThumbnails2;
		protected System.Web.UI.WebControls.HyperLink hlkNextImageName;
		protected System.Web.UI.WebControls.HyperLink hlkNextImagePhoto;
		protected System.Web.UI.WebControls.HyperLink hlkPreviousImageName;
		protected System.Web.UI.WebControls.HyperLink hlkPreviousImagePhoto;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator1;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator2;

		// Values that dictate the number of photos displayed when viewing an album
		private int colsPerPage = 3;
		protected System.Web.UI.WebControls.Label lblViewedCount;
		private int rowsPerPage = 3;

		private void Page_Load(object sender, System.EventArgs e)
		{

			HttpRequest request = Context.Request;

			tvwMain.WindowsLafImageBase = @"PhotoBrowserRes\Treeview\";
			tvwMain.TreeOutputStyle = TreeOutputStyle.WindowsLookAndFeel;
			tvwMain.NodeDisplayStyle = NodeDisplayStyle.Standard;

			// We only get a post back if the user is navigating via the treeview control.
			// If we've not got a postback then we need to display correct state of the 
			// control.
			if(!IsPostBack)
			{

				DirectoryBrowser dirBrowser = (DirectoryBrowser) Session["DirectoryBrowser"];
				if (dirBrowser == null || request.Params["method"] == null)
				{

					string photosPath = request.ApplicationPath + Path.DirectorySeparatorChar;
					string mappedPhotosPath = request.MapPath(photosPath) + "photos";

					dirBrowser = new DirectoryBrowser(mappedPhotosPath);

				}

				Hashtable directoryLookup = (Hashtable) Session["DirectoryLookup"];
				if (directoryLookup == null)
					directoryLookup = new Hashtable();

				DisplayTreeview(dirBrowser, directoryLookup);

				// Must be called after DisplayTreeview
				ProcessMethod(request, directoryLookup);

				// expand out the current directory node
				if (request.Params["directoryID"] != null)
				{				
					int directoryID = Int32.Parse(request.Params["directoryID"]);
                    squishyWARE.WebComponents.squishyTREE.TreeNode node = tvwMain.FindTreeNode(directoryID.ToString());
					ExpandNode(node);
				}
				
				Session.Add("DirectoryBrowser", dirBrowser);
				Session.Add("DirectoryLookup", directoryLookup);

			}

			pnlImageNavigationBottom.Visible = DISPLAY_BOTTOM_IMAGE_NAVIGATION;

		}

		/// <summary>
		/// Configures the control to display the correct content, depending on the current
		/// URL.
		/// </summary>
		/// <param name="request">The current HttpRequest object.</param>
		/// <param name="directoryLookup">A cache of all of the PhotoDirectory objects.</param>
		private void ProcessMethod(HttpRequest request, Hashtable directoryLookup)
		{

			string method = request.Params["method"];

			switch (method)
			{
				case ("gotoPage"):
					GotoPage(request);
					break;

				case ("displayPhoto"):
					int photoID = Int32.Parse(request.Params["photoID"]);
					int directoryID = Int32.Parse(request.Params["directoryID"]);

					PhotoDirectory dir = (PhotoDirectory) directoryLookup[directoryID];						
					DisplayPhoto(dir, photoID);
						
					break;
			}

		}

		/// <value>The number of photos displayed on an album page.</value>
		public int PhotosPerPage
		{
			get
			{
				return rowsPerPage * colsPerPage;
			}
		}

		/// <summary>
		/// Creates an HTML string representing a collection of comments.
		/// </summary>
		/// <param name="comments">A collection of Comment objects.</param>
		/// <returns>An HTML string</returns>
		private string CreateCommentsString(Comments comments)
		{

			StringBuilder buff = new StringBuilder();
			foreach (Comment comment in comments)
			{

				// Prints out comment in the form
				// nick (10/20/2004) - This is rubbish
				buff.Append(String.Format("{0} ({1}) - {2}<BR>", HttpUtility.HtmlEncode(comment.Name),
							comment.DateAdded.ToString("dd/MM/yy"),
							HttpUtility.HtmlEncode(comment.CommentText)));
			}

			return buff.ToString().Replace("'", "");
		
		}

		#region Treeview Control Methods

		/// <summary>
		/// Displays the album treeview control.
		/// </summary>
		/// <param name="dirBrowser">A reference to the cached DirectoryBrowser object.</param>
		/// <param name="directoryLookup">A cache of all the PhotoDirectory objects.</param>
		private void DisplayTreeview(DirectoryBrowser dirBrowser, Hashtable directoryLookup)
		{

			PhotoDirectories dirs = dirBrowser.GetDirectories();

			tvwMain.Controls.Clear();
			addDirectoryNodes(directoryLookup, dirBrowser, dirs, null);

		}

		/// <summary>
		/// Expands a given treeview node. Recursively walks up the node's parent's
		/// nodes, expanding these as well.
		/// </summary>
		/// <param name="node">The node to expand.</param>
		private void ExpandNode(squishyWARE.WebComponents.squishyTREE.TreeNode node)
		{

			node.IsExpanded = true;
			if (node.ParentNode != null)
				ExpandNode(node.ParentNode);

		}

		/// <summary>
		/// Recursively adds a collection of directories to the treeview.
		/// </summary>
		/// <param name="directoryLookup">A cache of all the PhotoDirectory objects.</param>
		/// <param name="dirBrowser">A reference to a DirectoryBrowser object.</param>
		/// <param name="dirs">A collection of PhotoDirectory objects.</param>
		/// <param name="parentNode">A reference to the parent tree node object, or null if at the root of the tree view.</param>
		private void addDirectoryNodes(Hashtable directoryLookup, DirectoryBrowser dirBrowser, 
									   PhotoDirectories dirs, squishyWARE.WebComponents.squishyTREE.TreeNode parentNode)
		{


			IEnumerator enumerator = dirs.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PhotoDirectory dir = (PhotoDirectory) enumerator.Current;

                squishyWARE.WebComponents.squishyTREE.TreeNode node;
				if (parentNode == null)
					node = tvwMain.AddNode(HttpUtility.HtmlEncode(dir.Name), dir.Id.ToString());
				else
					node = parentNode.AddNode(HttpUtility.HtmlEncode(dir.Name), dir.Id.ToString());

				addDirectoryNodes(directoryLookup, dirBrowser, dir.GetDirectories(), node);

				if (!directoryLookup.Contains(dir.Id))
					directoryLookup.Add(dir.Id, dir);

			}

		}

		private void tvwMain_SelectedNodeChanged(object sender, System.EventArgs e)
		{

			pnlComments.Visible = false;

			TreeViewNodeClickEventArgs args = (TreeViewNodeClickEventArgs) e;
            squishyWARE.WebComponents.squishyTREE.TreeNode node = args.Node;			

			// Retrieve the relevant PhotoDirectory object from the lookup, keyed on the 
			// unique ID
			Hashtable directoryLookup = (Hashtable) Context.Session["DirectoryLookup"];
			PhotoDirectory dir = (PhotoDirectory) directoryLookup[Int32.Parse(node.Key)];

			GeneratePhotosTable(dir, 1);

			Session.Add("CurrentDirectory", dir);

		}
	
#endregion

		#region Photo Album Display

		#region Photo Album Table Generation

		/// <summary>
		/// Populates the photo album with photos.
		/// </summary>
		/// <param name="dir">A reference to the PhotoDirectory representing the album to display.</param>
		/// <param name="pageNumber">The page number of the album to display.</param>
		private void GeneratePhotosTable(PhotoDirectory dir, int pageNumber)
		{

			tblPhotos.Rows.Clear();

			Photos photos = dir.GetPhotos();

			if (photos.Count > 0)
			{

				pnlPhotoGridContents.Visible = true;
				pnlPhotoContents.Visible = false;

				int photosPerPage = rowsPerPage * colsPerPage;
				int totalNoOfPages = (int) photos.Count / photosPerPage;
				int photosOnLastPage = photos.Count % photosPerPage;
				if (photosOnLastPage > 0) totalNoOfPages++;

				if (pageNumber > totalNoOfPages)
					throw new Exception("Illegal page number requested");
			
				// figure out which photos we want to display
				int startIndex = (pageNumber == 1 ? 0 : (photosPerPage * (pageNumber - 1)));
				int endIndex = (pageNumber == totalNoOfPages ? photos.Count - 1 : startIndex + photosPerPage - 1);

				System.Diagnostics.Debug.Assert(startIndex < endIndex);

				Photo[] photosArray = new Photo[photos.Count];
				photos.CopyTo(photosArray, 0);

				// loop through the required photos and output the correct table cells
				int rowCounter = 0;
				TableRow row = null;
				for (int i = startIndex; i <= endIndex; i++)
				{

					if (rowCounter == 0)
					{
						row = new TableRow();
						row.Width = Unit.Percentage(100);
						tblPhotos.Rows.Add(row);
						rowCounter++;
					}

					Photo photo = photosArray[i];

					TableCell cell = createCell(dir, photo);
					cell.Width = Unit.Percentage(100 / colsPerPage);
					row.Cells.Add(cell);
						
					if (row.Cells.Count == colsPerPage)
						rowCounter = 0;
				}

				ConfigurePageNavigationArrows(dir, pageNumber, totalNoOfPages);
				ConfigurePageLinks(dir, pageNumber, totalNoOfPages);

			}
			else
			{
				pnlPhotoGridContents.Visible = false;
				pnlPhotoContents.Visible = false;
			}

			Session.Add("CurrentPageNumber", pageNumber);
			Session.Add("CurrentPhotos", photos);
		
		}
	
		/// <summary>
		/// Returns a TableCell object for each photo in the photo album.
		/// </summary>
		/// <param name="dir">The PhotoDirectory the photo belongs to.</param>
		/// <param name="photo">The photo that is to be displayed in the table cell.</param>
		/// <returns>A TableCell object.</returns>
		private TableCell createCell(PhotoDirectory dir, Photo photo)
		{


			TableCell cell = new TableCell();
			cell.CssClass = "indexThumbCell";
			cell.HorizontalAlign = HorizontalAlign.Left;
			cell.Width = new Unit("33%");

			HyperLink hl = new HyperLink();
			hl.NavigateUrl = Context.Request.Path + "?method=displayPhoto&photoID=" + photo.Id + 
				"&directoryID=" + dir.Id;

			System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
			image.CssClass = "image";
			image.ImageUrl = "photos" + photo.FullThumbnailVirtualPath.Replace(@"\", "/");
			image.BorderStyle = BorderStyle.None;

			Comments comments = photo.GetComments();

			if (comments.Count > 0)
			{

				string cs = String.Format("Comments:<BR>{0}",  CreateCommentsString(comments));
				
				image.Attributes.Add("onmouseover", "return overlib('" + cs + "')");
				image.Attributes.Add("onmouseout", "return nd()");
			}

			hl.Controls.Add(image);

			LiteralControl text = new LiteralControl();
			text.Text = "<br><div class=\"thumbLabel\"><b>" + HttpUtility.HtmlEncode(photo.Name) + "</b></div>";

			hl.Controls.Add(text);
			cell.Controls.Add(hl);

			string dateText;
			if (photo.DateTaken == DateTime.MinValue)
				dateText = photo.FileSizeText;
			else
				dateText = photo.DateTaken.ToString("dd MMM yyyy - hh:mm") + ", " + photo.FileSizeText;

			LiteralControl date = new LiteralControl("<div class=\"thumbDetails\">" + dateText + "</div>");

			string commentsCount = "(" + comments.Count + " comments)";
			
			LiteralControl commentLit = new LiteralControl("<div class=\"thumbComment\">" + commentsCount + "</div><br>");

			cell.Controls.Add(date);
			cell.Controls.Add(commentLit);

			return cell;

		}

		#endregion

		/// <summary>
		/// Displays a photo album page choosen by the user.
		/// </summary>
		/// <param name="request">The current HttpRequest object.</param>
		private void GotoPage(HttpRequest request)
		{

			// Gets the neccessary IDs out of the request
			int pageNumber = Int32.Parse(request.Params["pageNumber"]);
			int directoryID = Int32.Parse(request.Params["directoryID"]);

			// Obtain a reference to the PhotoDirectory object
			Hashtable directoryLookup = (Hashtable) Session["DirectoryLookup"];			
			PhotoDirectory dir = (PhotoDirectory) directoryLookup[directoryID];
			
			// Display the album grid for the selected page
			GeneratePhotosTable(dir, pageNumber);

		}

		/// <summary>
		/// Configures the page hyperlinks displayed at the top and bottom of the photo album grid.
		/// </summary>
		/// <param name="dir">The current photo album directory the user is viewing.</param>
		/// <param name="currentPage">The current page number.</param>
		/// <param name="totalPages">The total number of pages in the album.</param>
		public void ConfigurePageLinks(PhotoDirectory dir, int currentPage, int totalPages)
		{

			plhPageLinks1.Controls.Clear();
			plhPageLinks2.Controls.Clear();

			for (int i = 1; i <= totalPages; i++)
			{
				
				if (i != currentPage)
				{
					HyperLink hl = new HyperLink();
					hl.Text = i.ToString();
					hl.NavigateUrl = Context.Request.Path + "?method=gotoPage&pageNumber=" + i.ToString() +
									 "&directoryID=" + dir.Id;
					hl.CssClass = "pageNavCell";

					HyperLink hl2 = new HyperLink();
					hl2.Text = hl.Text;
					hl2.NavigateUrl = hl.NavigateUrl;
					hl2.CssClass = hl.CssClass;

					plhPageLinks1.Controls.Add(hl);
					plhPageLinks2.Controls.Add(hl2);
				}
				else
				{
					Label label = new Label();
					label.Text = i.ToString();
					label.CssClass = "pageNavCell";

					Label label2 = new Label();
					label2.Text = label.Text;
					label2.CssClass = label.CssClass;

					plhPageLinks1.Controls.Add(label);
					plhPageLinks2.Controls.Add(label2);
				
				}
				plhPageLinks1.Controls.Add(new LiteralControl(" "));
				plhPageLinks2.Controls.Add(new LiteralControl(" "));

			}

		}

		/// <summary>
		/// Configures the images and URL used by the page nagivation arrows diaplayed at the top 
		/// and bottom of the photos album grid.
		/// </summary>
		/// <param name="dir">The current photo album directory the user is viewing.</param>
		/// <param name="currentPage">The current page number.</param>
		/// <param name="totalPages">The total number of pages in the album.</param>
		public void ConfigurePageNavigationArrows(PhotoDirectory dir, int currentPage, int totalPages)
		{

			hylPreviousPage1.Visible = true;
			hylNextPage1.Visible = true;
			hylPreviousPage2.Visible = true;
			hylNextPage2.Visible = true;

			if (currentPage == 1)
			{
				hylPreviousPage1.ImageUrl = "PhotoBrowserRes/previous_disabled.gif";
				hylPreviousPage1.Enabled = false;
			}
			else
			{
				hylPreviousPage1.NavigateUrl = Context.Request.Path + "?method=gotoPage&pageNumber=" + (currentPage - 1) +
											   "&directoryID=" + dir.Id;
				hylPreviousPage1.ImageUrl = "PhotoBrowserRes/previous.gif";
				hylPreviousPage1.Enabled = true;
			}

			if (currentPage == totalPages)
			{
				hylNextPage1.ImageUrl = "PhotoBrowserRes/next_disabled.gif";
				hylNextPage1.Enabled = false;
			}
			else
			{
				hylNextPage1.NavigateUrl = Context.Request.Path + "?method=gotoPage&pageNumber=" + (currentPage + 1) +
										   "&directoryID=" + dir.Id;
				hylNextPage1.ImageUrl = "PhotoBrowserRes/next.gif";
				hylNextPage1.Enabled = true;
			}

			// These are the duplicate controls shown at the bottom of the page so we simply copy
			// the values accross from those at the top
			hylNextPage2.NavigateUrl = hylNextPage1.NavigateUrl;
			hylNextPage2.ImageUrl = hylNextPage1.ImageUrl ;
			hylNextPage2.Enabled = hylNextPage1.Enabled;
			
			hylPreviousPage2.NavigateUrl = hylPreviousPage1.NavigateUrl;
			hylPreviousPage2.ImageUrl = hylPreviousPage1.ImageUrl;
			hylPreviousPage2.Enabled = hylPreviousPage1.Enabled;


		}

#endregion

		#region Individual Photo Display 

		/// <summary>
		/// Displays an individual photo as selected by the user.
		/// </summary>
		/// <param name="dir">The PhotoDirectory the selected photo belongs to.</param>
		/// <param name="id">The ID of the selected photo.</param>
		private void DisplayPhoto(PhotoDirectory dir, int id)
		{

			pnlPhotoContents.Visible = true;
			pnlPhotoGridContents.Visible = false;

			Photos photos = dir.GetPhotos();
			Photo photo = (Photo) photos[id];

			photo.IncrementViewed();

			imgPhoto.ImageUrl = "photos" + photo.FullVirtualPath.Replace(@"\", "/");
			
			int index = photos.IndexOf(photo);
			if (index == 0)
			{
				hlkPreviousImagePhoto.Visible = false;

				hlkPreviousImage.ImageUrl = "PhotoBrowserRes/previous_disabled.gif";

				hlkPreviousImageName.Enabled = false;
				hlkPreviousImageName.Text = "No Previous Image";
			}
			else
			{

				Photo previousPhoto = (Photo) photos.GetByIndex(index - 1);
				
				hlkPreviousImagePhoto.Visible = true;
				hlkPreviousImagePhoto.ImageUrl = "photos" + previousPhoto.FullThumbnailVirtualPath.Replace(@"\", "/");
				hlkPreviousImagePhoto.NavigateUrl = Context.Request.Path + "?method=displayPhoto&photoID=" + previousPhoto.Id +
													"&directoryID=" + dir.Id;

				hlkPreviousImage.ImageUrl = "PhotoBrowserRes/previous.gif";
				hlkPreviousImage.NavigateUrl = hlkPreviousImagePhoto.NavigateUrl;

				hlkPreviousImageName.NavigateUrl = hlkPreviousImage.NavigateUrl;
				hlkPreviousImageName.Enabled = true;
				hlkPreviousImageName.Text = previousPhoto.Name;
			}

			if (index == photos.Count - 1)
			{
				hlkNextImagePhoto.Visible = false;
				
				hlkNextImage.ImageUrl = "PhotoBrowserRes/next_disabled.gif";

				hlkNextImageName.Enabled = false;
				hlkNextImageName.Text = "No Next Image";
			}
			else
			{
				Photo nextPhoto = (Photo) photos.GetByIndex(index + 1);

				hlkNextImagePhoto.Visible = true;
				hlkNextImagePhoto.ImageUrl = "photos" + nextPhoto.FullThumbnailVirtualPath.Replace(@"\", "/");
				hlkNextImagePhoto.NavigateUrl = Context.Request.Path + "?method=displayPhoto&photoID=" + nextPhoto.Id +
												"&directoryID=" + dir.Id;

				hlkNextImage.ImageUrl = "PhotoBrowserRes/next.gif";
				hlkNextImage.NavigateUrl = hlkNextImagePhoto.NavigateUrl;

				hlkNextImageName.NavigateUrl = hlkNextImage.NavigateUrl;
				hlkNextImageName.Enabled = true;
				hlkNextImageName.Text = nextPhoto.Name;
			}

			// Have to figure out which page the current photograph would appear on
			int page = (index / PhotosPerPage) + 1;
			hlkReturnToThumbnails1.NavigateUrl = Context.Request.Path + "?method=gotoPage&pageNumber=" + page +
												 "&directoryID=" + dir.Id;
			hlkReturnToThumbnails2.NavigateUrl = hlkReturnToThumbnails1.NavigateUrl;

			DisplayComments(photo);

			lblViewedCount.Text = String.Format("Image has been viewed {0} times", photo.Viewed);

			Session["CurrentPhoto"] = photo;

		}

		/// <summary>
		/// Displays the comments associated with a photo.
		/// </summary>
		/// <param name="photo">The photo in question.</param>
		public void DisplayComments(Photo photo)
		{

			pnlComments.Visible = true;

			string cs = CreateCommentsString(photo.GetComments()) + "<BR>";
			litPhotoComments.Text = cs;
			litPhotoComments.Visible = true;

		}

		private void btnAddComment_Click(object sender, System.EventArgs e)
		{

			Photo photo = (Photo) Session["CurrentPhoto"];

			Comment comment = new Comment(HttpUtility.HtmlDecode(txtCommentName.Text),
										  HttpUtility.HtmlDecode(txtCommentText.Text));

			photo.AddComment(comment);

			DisplayComments(photo);

			txtCommentName.Text = "";
			txtCommentText.Text = "";

		}

		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{		
			this.tvwMain.SelectedNodeChanged += new System.EventHandler(this.tvwMain_SelectedNodeChanged);
			this.btnAddComment.Click += new System.EventHandler(this.btnAddComment_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}
