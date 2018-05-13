using System;
using System.IO;

namespace Codefresh.PhotoBrowserLibrary
{
	/// <summary>
	/// Class that represents an individual photo comment.
	/// </summary>
	[Serializable]
	public class Comment : PhotoObjectBase
	{
		private string name;
		private string comment;
		private DateTime dateAdded;

		/// <summary>
		/// Initialises a new Comment object. Used by the PhotoBrowser web control
		/// when adding a new comment.
		/// </summary>
		/// <param name="name">The name of the person entering the comment.</param>
		/// <param name="comment">The comment itself.</param>
		public Comment(string name, string comment)
		{
			this.name = name;
			this.comment = comment;
			this.dateAdded = DateTime.Now;
		}

		/// <summary>
		/// Initialises a new Comment object. Used by CommentDB when retrieving a list
		/// of comments of the database. 
		/// </summary>
		/// <param name="token">A reference to the current SessionToken object.</param>
		/// <param name="name">The name of the person entering the comment.</param>
		/// <param name="comment">The comment itself.</param>
		/// <param name="dateAdded">The date that the comment was added.</param>
		internal Comment(SessionToken token, int id, string name, string comment, DateTime dateAdded) : this(name, comment)
		{
			// override the default value
			this.dateAdded = dateAdded;
			this.SetId(id);
		}

		/// <value>The date the comment was added to the photo.</value>
		public DateTime DateAdded
		{
			get
			{
				return dateAdded;
			}
		}

		/// <value>The name of the person who entered the comment.</value>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <value>The comment text itself.</value>
		public string CommentText
		{
			get
			{
				return comment;
			}
		}

	}
}
