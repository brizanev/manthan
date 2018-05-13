using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Codefresh.PhotoBrowserLibrary.Collections
{

	// TODO Can make less visible?
	[Serializable]
	public abstract class KeyBase
	{

		private int id;

		/// <summary>
		/// Initializes a new KeyBase.
		/// </summary>
		/// <param name="id"></param>
		public KeyBase(int id)
		{
			this.id = id;
		}

		/// <value>The ID value of the object.</value>
		public int Id
		{
			get
			{
				return id;
			}
		}

	}	

	/// <summary>
	/// Superclass for all photo collection classes. All collection classes are implemented
	/// as an encapsulated SorterList.
	/// </summary>
	[Serializable]
	public abstract class PhotoCollectionBase
	{

		private SortedList innerList;
		protected Hashtable keyMap;

		/// <summary>
		/// Initializes a new PhotoCollectionBase.
		/// </summary>
		public PhotoCollectionBase()
		{
			innerList = new SortedList(CreateComparer());
			keyMap = new Hashtable();
		}

		/// <summary>
		/// Adds a new object to the collection.
		/// </summary>
		/// <param name="key">The key of the object.</param>
		/// <param name="obj">The object to add</param>
		protected void Add(KeyBase key, PhotoObjectBase obj)
		{
			innerList.Add(key, obj);

			// Keep a map of the keys to the IDs
			keyMap.Add(obj.Id, key);
		}

		/// <summary>
		/// Removes an object from the collection.
		/// </summary>
		/// <param name="key">The key of the object to remove</param>
		protected void Remove(KeyBase key)
		{
			innerList.Remove(key);

			// Remove it from the key map as well
			keyMap.Remove(key.Id);
		
		}

		/// <value>The number of items in the collection.</value>
		public int Count
		{
			get
			{
				return innerList.Count;
			}
		}

		/// <summary>
		/// Gets the value at the specified index of the PhotosCollectionBase.
		/// </summary>
		/// <param name="index">The zero-based index of the value to get. </param>
		/// <returns>The value at the specified index of the PhotosCollectionBase.</returns>
		public PhotoObjectBase GetByIndex(int index)
		{
			return (PhotoObjectBase) innerList.GetByIndex(index);
		}

		/// <summary>
		/// Copies the PhotosCollectionBase elements to a one-dimensional Array instance at the specified index.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the DictionaryEntry objects copied from PhotosCollectionBase. The Array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins. </param>
		public void CopyTo(Array array, int index)
		{
			ArrayList sorted = new ArrayList(innerList.Values);
			sorted.CopyTo(array, index);
		}
	
		/// <summary>
		/// Returns an IEnumerator that can iterate through the PhotosCollectionBase. Note that the 
		/// returned enumerator object is actually the enumerator for a sorted copy of the class.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return innerList.Values.GetEnumerator();
		}

		/// <summary>
		/// Creates a shallow copy of the PhotosCollectionBase.
		/// </summary>
		/// <returns>A shallow copy of the PhotosCollectionBase.</returns>
		public PhotoCollectionBase Clone()
		{
			BinaryFormatter bFormatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			bFormatter.Serialize(stream, this);
			stream.Seek(0, SeekOrigin.Begin);
			return (PhotoCollectionBase) bFormatter.Deserialize(stream);
		}

		/// <value>The Keys collection from the internal SortedList.</value>
		protected ICollection Keys
		{
			get
			{
				return innerList.Keys;
			}
		}

		/// <summary>
		/// Returns the zero-based index of the specified key in the PhotosCollectionBase.
		/// </summary>
		/// <param name="key">The key to locate in the PhotosCollectionBase. </param>
		/// <returns>The zero-based index of key, if key is found in the PhotosCollectionBase; otherwise, -1.</returns>
		public int IndexOf(PhotoObjectBase obj)
		{
			KeyBase key = CreateKey(obj);
			return innerList.IndexOfKey(key);
		}

		/// <summary>
		/// The indexer for the PhotoCollectionBase collection class.
		/// </summary>
		public PhotoObjectBase this[int id]
		{
			get
			{ 
				// Use the kay map to retrieve the KeyBase object based upon the
				// ID of the PhotoObjectBase
				KeyBase key = (KeyBase) keyMap[id];
				return (PhotoObjectBase) innerList[key];
			}
		}

		/// <summary>
		/// Creates a KeyBase object that is used to store PhotoObjectBase objects in
		/// the collection.
		/// </summary>
		/// <param name="obj">The object to create the key for.</param>
		/// <returns>A KeyBase object.</returns>
		protected abstract KeyBase CreateKey(PhotoObjectBase obj);

		/// <summary>
		/// Returns the comparater object to use when sorting the PhotosCollectionBase.
		/// </summary>
		/// <returns>An object than implements IComparer.</returns>
		protected abstract IComparer CreateComparer();

	}
}
