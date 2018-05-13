using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Codefresh.PhotoBrowserLibrary.Collections
{

	#region IComparer implementation

	/// <summary>
	/// Class used to sort the Photo collection.
	/// </summary>
	[Serializable]
	public class PhotoComparer: IComparer
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

			PhotoKey dirX = (PhotoKey) x;
			PhotoKey dirY = (PhotoKey) y;

			int result = dirX.DateTaken.CompareTo(dirY.DateTaken);
			if (result == 0)
				return dirX.Name.CompareTo(dirY.Name);
			else
				return result;


		}
	}

	#endregion

	#region KeyBase implementation

	/// <summary>
	/// The key to the Photos collectionn.
	/// </summary>
	[Serializable]
	internal class PhotoKey : KeyBase
	{

		public DateTime dateTaken;
		public string name;

		/// <summary>
		/// Initailizes a new PhotoKey.
		/// </summary>
		/// <param name="id">The ID value of the Photo.</param>
		/// <param name="dateTaken">The DateTaken value of the Photo.</param>
		/// <param name="name">The Name value of the Photo.</param>
		public PhotoKey(int id, DateTime dateTaken, string name) : base(id)
		{
			this.dateTaken = dateTaken;
			this.name = name;
		}

		/// <value>The Name value of the Photo.</value>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <value>The DateTaken value of the Photo.</value>
		public DateTime DateTaken
		{
			get
			{
				return dateTaken;
			}
		}

	}

	#endregion

	/// <summary>
	/// Collection class for Photo objects. The collection is sorted by 
	/// the directory name. 
	/// </summary>
	[Serializable]
	public class Photos : PhotoCollectionBase
	{

		/// <summary>
		/// Creates the comparer required by this collection class.
		/// </summary>
		/// <returns>An object that implements IComparer.</returns>
		protected override IComparer CreateComparer()
		{
			return new PhotoComparer();
		}

		/// <summary>
		/// Adds a Photo object to the collection.
		/// </summary>
		/// <param name="photo">The Photo object to add.</param>
		public void Add(Photo photo)
		{
			KeyBase key = CreateKey(photo);
			Add(key, photo);
		}

		/// <summary>
		/// Creates the collection key based on a Photo object.
		/// </summary>
		/// <param name="obj">The Photo object who's collection key we want.</param>
		/// <returns>A PhotoKey object.</returns>
		protected override KeyBase CreateKey(PhotoObjectBase obj)
		{
			Photo photo = (Photo) obj;
			return new PhotoKey(photo.Id, photo.DateTaken, photo.Name);
		}

		/// <summary>
		/// Removes a Photo object from the collection.
		/// </summary>
		/// <param name="photo">The Photo object to remove.</param>
		public void Remove(Photo photo)
		{
			Remove(CreateKey(photo));
		}

		/// <summary>
		/// Removes a Photo object from the collection, based on its full virtual path.
		/// </summary>
		/// <param name="fullVirtualPath">The Photo object to remove.</param>
		public void Remove(string fullVirtualPath)
		{
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{

				Photo dir = (Photo) enumerator.Current;
				if (dir.FullVirtualPath.Equals(fullVirtualPath))
				{
					Remove(dir);
					break;
				}
			}
		}

		/// <summary>
		/// Returns a value indicating whether a Photo with the specified full virtual path
		/// exists in the collection,
		/// </summary>
		/// <param name="fullVirtualPath">The full virtual path of the photo in question.</param>
		/// <returns>true if a Photo with the full virtual path exists in the collection, else false.</returns>
		public bool Contains(string fullVirtualPath)
		{
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				Photo photo = (Photo) enumerator.Current;
				if (photo.FullVirtualPath.Equals(fullVirtualPath))
				{
					return true;
				}
			}

			return false;
			
		}

	}
}
