using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Codefresh.PhotoBrowserLibrary.Collections
{

	#region IComparer Implementation

	/// <summary>
	/// Class used to sort the PhotoDirectories collection.
	/// </summary>
	[Serializable]
	public class PhotoDirectoryComparer: IComparer
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

			PhotoDirectoryKey dirX = (PhotoDirectoryKey) x;
			PhotoDirectoryKey dirY = (PhotoDirectoryKey) y;

			return dirX.Name.CompareTo(dirY.Name);

		}

	}

	#endregion

	#region KeyBase implementation
	
	/// <summary>
	/// The key to the PhotoDirectories collectionn.
	/// </summary>
	[Serializable]
	internal class PhotoDirectoryKey : KeyBase
	{

		private string name;

		/// <summary>
		/// Initailizes a new PhotoDirectoryKey.
		/// </summary>
		/// <param name="id">The ID value of the PhotoDirectory.</param>
		/// <param name="name">The Name value of the PhotoDirectory.</param>
		public PhotoDirectoryKey(int id, string name) : base(id)
		{
			this.name = name;
		}

		/// <value>The Name value of the photo.</value>
		public string Name
		{
			get
			{
				return name;
			}
		}

	}

	#endregion

	/// <summary>
	/// Collection class for PhotoDirectory objects. The collection
	/// is sorted by the directory name. 
	/// </summary>
	[Serializable]
	public class PhotoDirectories : PhotoCollectionBase
	{

		/// <summary>
		/// Creates the comparer required by this collection class.
		/// </summary>
		/// <returns>An object that implements IComparer.</returns>
		protected override IComparer CreateComparer()
		{
			return new PhotoDirectoryComparer();
		}

		/// <summary>
		/// Adds a PhotoDirectory object to the collection.
		/// </summary>
		/// <param name="photoDirectory">The PhotoDirectory object to add.</param>
		public void Add(PhotoDirectory photoDirectory)
		{
			KeyBase key = CreateKey(photoDirectory);
			Add(key, photoDirectory);
		}

		/// <summary>
		/// Creates the collection key based on a PhotoDirectory object.
		/// </summary>
		/// <param name="obj">The PhotoDirectory object who's collection key we want.</param>
		/// <returns>A PhotoDirectoryKey object.</returns>
		protected override KeyBase CreateKey(PhotoObjectBase obj)
		{
			PhotoDirectory photoDirectory = (PhotoDirectory) obj;
			return new PhotoDirectoryKey(photoDirectory.Id, photoDirectory.Name);
		}

		/// <summary>
		/// Removes a PhotoDirectory object from the collection, based on its full virtual path.
		/// </summary>
		/// <param name="fullVirtualPath">The PhotoDirectory object to remove.</param>
		public void Remove(string fullVirtualPath)
		{
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{

				PhotoDirectory dir = (PhotoDirectory) enumerator.Current;
				if (dir.FullVirtualPath.Equals(fullVirtualPath))
				{
					Remove(dir);
					break;
				}
			}
		}
		
		/// <summary>
		/// Removes a PhotoDirectory object from the collection.
		/// </summary>
		/// <param name="photoDirectory">The PhotoDirectory object to remove.</param>
		public void Remove(PhotoDirectory photoDirectory)
		{
			Remove(CreateKey(photoDirectory));
		}

		/// <summary>
		/// Returns a value indicating whether a PhotoDirectory with the specified full virtual path
		/// exists in the collection,
		/// </summary>
		/// <param name="fullVirtualPath">The full virtual path of the directory in question.</param>
		/// <returns>true if a PhotoDirectory with the full virtual path exists in the collection, else false.</returns>
		public bool Contains(string fullVirtualPath)
		{
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				PhotoDirectory dir = (PhotoDirectory) enumerator.Current;
				if (dir.FullVirtualPath.Equals(fullVirtualPath))
				{
					return true;
				}
			}

			return false;
			
		}

	}
}