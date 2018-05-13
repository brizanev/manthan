using System;
using System.IO;
using Codefresh.PhotoBrowserLibrary.Collections;
using Codefresh.PhotoBrowserLibrary.DataAccessLayer;

namespace Codefresh.PhotoBrowserLibrary
{
	/// <summary>
	/// Represents a photo on the filesystem. Contains a collection class containing
	/// a list of comments applicable to it. Note that this collection is cached and therefore 
	/// concurrent users will not see each others additions until they end their session.
	/// </summary>
	[Serializable]
	public class Photo : PhotoObjectBase
	{
		private string name;
		private string virtualPath;
		private DateTime dateTaken;
		private long filesize;
		private int viewed;

		private Comments comments;

		/// <summary>
		/// Initializes a new Photo object. Used when inserting new Photo objects into the database.
		/// </summary>
		/// <param name="token">A reference to the current SessionToken object.</param>
		/// <param name="name">The filename of the photo.</param>
		/// <param name="virtualPath">The virtual path of the photo.</param>
		/// <param name="dateTaken">The date the photo was taken.</param>
		/// <param name="filesize">The file size of the photo.</param>
		/// <param name="viewed">The number of times the photo has been viewed.</param>
		internal Photo(SessionToken token, string name, string virtualPath, DateTime dateTaken, 
					   long filesize) : base(token)
		{
			this.name = name;
			this.virtualPath = virtualPath;
			this.dateTaken = dateTaken;
			this.filesize = filesize;

			// Create a thumbnail for this photo
			ThumbnailUtilities.CreateThumbnail(token.MapPath(FullVirtualPath));

		}

		/// <summary>
		/// Initializes a new Photo object. Used when retrieving Photo objects from the database.
		/// </summary>
		/// <param name="token">A reference to the current SessionToken object.</param>
		/// <param name="id">The unique ID of the photo.</param>
		/// <param name="name">The filename of the photo.</param>
		/// <param name="virtualPath">The virtual path of the photo.</param>
		/// <param name="dateTaken">The date the photo was taken.</param>
		/// <param name="filesize">The file size of the photo.</param>
		/// <param name="viewed">The number of times the photo has been viewed.</param>
		internal Photo(SessionToken token, int id, string name, string virtualPath, DateTime dateTaken, 
					   long filesize, int viewed) : base(token)
		{

			// Note that we cannot chain the constructors together in this case because we create
			// the tumbnail in the above one

			this.name = name;
			this.virtualPath = virtualPath;
			SetId(id);
			this.dateTaken = dateTaken;
			this.filesize = filesize;
			this.viewed = viewed;

		}

		/// <value>The date and time the photo was taken.</value>
		public DateTime DateTaken
		{
			get
			{
				return dateTaken;
			}
		}

		/// <value>The filename of the photo.</value>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <value>The virtual path of the photo.</value>
		public string VirtualPath
		{
			get
			{
				return virtualPath;
			}
		}

		/// <value>The full virtual path of the photo.</value>
		public string FullVirtualPath
		{
			get
			{
				return VirtualPath + Path.DirectorySeparatorChar + Name;
			}
		}

		/// <value>The full virtual path of the photo's thumbnail image.</value>
		public string FullThumbnailVirtualPath
		{
			get
			{
				string file = VirtualPath + Path.DirectorySeparatorChar + ThumbnailUtilities.ThumbnailDirectory +
							  Path.DirectorySeparatorChar + name;

				// if the thumbnail doesn't exist then we'll have to create it
				if (!File.Exists(token.MapPath(file)))
					ThumbnailUtilities.CreateThumbnail(token.MapPath(FullVirtualPath));
			
				return file;

			}
		}

		/// <value>The file size of the photo.</value>
		public long FileSize
		{
			get
			{
				return filesize;
			}
		}

		/// <value>The file size of the photo as a string (e.g. 100KB).</value>
		public string FileSizeText
		{
			get
			{
				long kbs = (filesize / 1024);
				if (kbs >= 1)
					return kbs + " KB";
				else
					return filesize + " b";
			}
		}

		/// <value>The number of times the photo has been viewed.</value>
		public int Viewed
		{
			get
			{
				return viewed;
			}
		}

		/// <summary>
		/// Increment the count of the number of times the current photo has been viewed.
		/// </summary>
		public void IncrementViewed()
		{

			viewed++;

			PhotoDB db = new PhotoDB(token.DBConnection);
			db.IncrementViewedCount(this);

		}

		/// <summary>
		/// Retrieves a collection of comments associated with the photo.
		/// </summary>
		/// <returns>A collection of Comment objects.</returns>
		public Comments GetComments()
		{

			if (comments == null)
			{
				CommentDB db = new CommentDB(token.DBConnection);
				comments = db.GetComments(token, this);
			}

			return comments;

		}
	
		/// <summary>
		/// Adds a comment to the photo.
		/// </summary>
		/// <param name="comment">The comment to add.</param>
		public void AddComment(Comment comment)
		{

			CommentDB db = new CommentDB(token.DBConnection);
			db.Insert(this, comment);

			// Add the comment to the current comment cache
			comments.Add(comment);

		}


	}
}
