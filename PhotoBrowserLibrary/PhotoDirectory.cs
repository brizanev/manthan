using System;
using System.IO;
using Codefresh.PhotoBrowserLibrary.Collections;

namespace Codefresh.PhotoBrowserLibrary
{
	/// <summary>
	/// Object that represents a single directory.
	/// </summary>
	[Serializable]
	public class PhotoDirectory : PhotoObjectBase
	{

		private string name;
		private string virtualPath;
		private PhotoDirectories dirs;
		private Photos photos;

		/// <summary>
		/// Initializes a new PhotoDirectory object. Used when inserting new PhotoDirect objects 
		/// into the database.
		/// </summary>
		/// <param name="token">A reference to the current SessionToken object.</param>
		/// <param name="name">The name of the directory.</param>
		/// <param name="virtualPath">The virtual path of the directory.</param>
		internal PhotoDirectory(SessionToken token, string name, string virtualPath) : base(token)
		{
			this.name = name;
			this.virtualPath = virtualPath;
		}

		/// <summary>
		/// Initializes a new PhotoDirectory object. Used when retrieving PhotoDirectory objects 
		/// from the database.
		/// </summary>
		/// <param name="token">A reference to the current SessionToken object.</param>
		/// <param name="id">The unqiue ID of the directory.</param>
		/// <param name="name">The name of the directory.</param>
		/// <param name="virtualPath">The virtual path of the directory.</param>
		internal PhotoDirectory(SessionToken token, int id, string name, string virtualPath) : this(token, name, virtualPath)
		{
			this.SetId(id);
		}

		/// <value>The directory name.</value>
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <value>The virtual path of the directory.</value>
		public string VirtualPath
		{
			get
			{
				return virtualPath;
			}
		}

		/// <value>The full virtual path of the directory.</value>
		public string FullVirtualPath
		{
			get
			{
				return VirtualPath + Path.DirectorySeparatorChar + Name;
			}
		}

		/// <summary>
		/// Retrieves a collection of directories contained in the directory.
		/// </summary>
		/// <returns>A collection of PhotoDirectory objects.</returns>
		public PhotoDirectories GetDirectories()
		{
			if (dirs == null)
				dirs = DirectoryUtilities.GetDirectories(this, token, token.MapPath(FullVirtualPath));
			
			return dirs;

		}

		/// <summary>
		/// Retrieves a collection of photos contained in the directory.
		/// </summary>
		/// <returns>A collection of Photo objects.</returns>
		public Photos GetPhotos()
		{

			if (photos == null)
				photos = PhotoUtilities.GetPhotos(token, this);

			return photos;

		}
	}

}
