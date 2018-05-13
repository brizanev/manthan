using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Codefresh.PhotoBrowserLibrary.Collections
{
	
	#region IComparer implementation

	/// <summary>
	/// Class used to sort the Comments collection.
	/// </summary>
	public class CommentComparer: IComparer
	{

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, 
		/// equal to or greater than the other.
		/// </summary>
		/// <param name="x">First object to compare. </param>
		/// <param name="y">Second object to compare. </param>
		/// <returns>Less than zero if x is less than y, zero if x is equal to y or greater than zero if y is greater than x.</returns>
		public int Compare(object x, object y)
		{

			CommentKey dirX = (CommentKey) x;
			CommentKey dirY = (CommentKey) y;

			int result = dirX.DateAdded.CompareTo(dirY.DateAdded);
			if (result == 0)
				return dirX.Id.CompareTo(dirY.Id);
			else
				return result;

		}

	}

	#endregion]

	#region KeyBase implementation

	/// <summary>
	/// The key to the Comments collectionn.
	/// </summary>
	[Serializable]
	internal class CommentKey : KeyBase
	{

		private string name;
		private DateTime dateAdded;

		/// <summary>
		/// Initailizes a new CommentKey.
		/// </summary>
		/// <param name="id">The ID value of the Comment.</param>
		/// <param name="name">The Name value of the Comment.</param>
		/// <param name="dateAdded">The DateAdded value of the Comment.</param>
		public CommentKey(int id, string name, DateTime dateAdded) : base(id)
		{
			this.name = name;
			this.dateAdded = dateAdded;
		}

		/// <value>The Name value of the Comment.</value>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <value>The DateAdded value of the Comment.</value>
		public DateTime DateAdded
		{
			get
			{
				return dateAdded;
			}
		}

	}
	
	#endregion

	/// <summary>
	/// A collection class holding a number of Comment objects. The collection is sorted on
	/// the date added value, then by name.
	/// </summary>
	[Serializable]
	public class Comments: PhotoCollectionBase
	{

		/// <summary>
		/// Creates the comparer required by this collection class.
		/// </summary>
		/// <returns>An object that implements IComparer.</returns>
		protected override IComparer CreateComparer()
		{
			return new CommentComparer();
		}

		/// <summary>
		/// Adds a Comment object to the collection.
		/// </summary>
		/// <param name="comment">The Comment object to add.</param>
		public void Add(Comment comment)
		{
			KeyBase key = CreateKey(comment);
			Add(key, comment);
		}

		/// <summary>
		/// Creates the collection key based on a Comment object.
		/// </summary>
		/// <param name="obj">The Comment object who's collection key we want.</param>
		/// <returns>A CommentKey object.</returns>
		protected override KeyBase CreateKey(PhotoObjectBase obj)
		{
			Comment comment = (Comment) obj;
			return new CommentKey(comment.Id, comment.Name, comment.DateAdded);
		}

		/// <summary>
		/// Removes a Comment object from the collection.
		/// </summary>
		/// <param name="comment">The Comment object to delete.</param>
		public void Remove(Comment comment)
		{
			Remove(CreateKey(comment));
		}

	}

}
