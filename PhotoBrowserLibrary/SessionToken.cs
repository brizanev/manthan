using System;
using System.Data;
using System.IO;

namespace Codefresh.PhotoBrowserLibrary
{
	/// <summary>
	/// Used by all classes to obtain references to the current database object and the root
	/// photos directory.
	/// </summary>
	[Serializable]
	public class SessionToken
	{

		private string photosRootFullPath;

		[NonSerialized()]
		private IDbConnection conn;

		/// <summary>
		/// Initailizes a new SessionToken object.
		/// </summary>
		/// <param name="photosRootFullPath">The photos root directory.</param>
		/// <param name="connection">A reference to an object database connect.</param>
		public SessionToken(string photosRootFullPath, IDbConnection connection)
		{
			this.photosRootFullPath = photosRootFullPath;
			this.conn = connection;
		}

		/// <value>The photos root directory.</value>
		public string PhotosRootFullPath
		{
			get
			{
				return photosRootFullPath;
			}
		}

		/// <value>The current open database connection.</value>
		public IDbConnection DBConnection
		{
			get
			{
				return conn;
			}
		}
	
		/// <summary>
		/// Maps the virtual directory path to a physical path on the server.
		/// </summary>
		/// <param name="virtualPath">The virtual path to map.</param>
		/// <returns>The physical path on the server specified by virtualPath.</returns>
		public string MapPath(string virtualPath)
		{
			return photosRootFullPath + virtualPath;
		}

	}

}
