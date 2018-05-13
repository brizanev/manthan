using System;
using System.IO;
using System.Collections;
using Codefresh.PhotoBrowserLibrary.Collections;
using Codefresh.PhotoBrowserLibrary.DataAccessLayer;

namespace Codefresh.PhotoBrowserLibrary
{
	/// <summary>
	/// Utility class that privides methods for dealing with PhotoDirectory objects.
	/// </summary>
	public sealed class DirectoryUtilities
	{

		private DirectoryUtilities()
		{
		}

		/// <summary>
		/// Returns the details of the sub-directories contained within a given path.
		/// </summary>
		/// <param name="parent">The parent directory, or null it at the root.</param>
		/// <param name="token">A reference to the current directory browser session token object.</param>
		/// <param name="path">The physical path of the directory in question.</param>
		/// <returns>A collection of PhotoDirectory objects.</returns>
		public static PhotoDirectories GetDirectories(PhotoDirectory parent, SessionToken token, string path)
		{

			PhotoDirectoryDB db = new PhotoDirectoryDB(token.DBConnection);

			// If the directory doesn't exist then a directory must have been deleted
			if (!Directory.Exists(path))
			{

				// extract out the name and virtual path from the full physical path
				path = path.Substring(token.PhotosRootFullPath.Length,
									  path.Length - token.PhotosRootFullPath.Length);
				string name = path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
				string virtualPath = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
				
				db.Delete(name, virtualPath);

				return new PhotoDirectories();
			}
			else
			{

				// Get a list of directories from the database
				PhotoDirectories results = db.GetDirectories(token, GetVirtualPath(token, path));

				PhotoDirectories originalResults = (PhotoDirectories) results.Clone();

				// Get a list of directories from the root path from the file system
				DirectoryInfo dirInfo = new DirectoryInfo(path);
				DirectoryInfo[] dirInfos = dirInfo.GetDirectories();

				// Only need to update the database if the number of directories in the database are 
				// different to those on the file system.
				// TODO: Currently won't detect changes in directory names. Ideas to enable this 
				//       as still be fast?
				if (originalResults.Count != dirInfos.Length)
				{

					foreach (DirectoryInfo dir in dirInfos)
					{
				
						// Ignore thumbnail directories
						if (!dir.Name.Equals(ThumbnailUtilities.ThumbnailDirectory))
						{			

							string virtualPath = GetVirtualPath(token, dir.FullName);
					
							// Do we have a PhotoDirectory for this directory already? If not then we need to
							// add it to the database
							if (!results.Contains(virtualPath))
							{	

								string parentVirtualPath = virtualPath.Substring(0, virtualPath.Length - dir.Name.Length - 1);

								PhotoDirectory photoDirectory = new PhotoDirectory(token, dir.Name, parentVirtualPath);

								// insert this directory into the database
								db.Insert(parent, photoDirectory);

								results.Add(photoDirectory);

							}

							originalResults.Remove(virtualPath);

						}

					}

					// Are there any PhotoDirectory objects in the original result set that aren't in
					// the list of directories from the file system? If so, delete 'em
					foreach (PhotoDirectory photoDirectory in originalResults)
					{
						db.Delete(photoDirectory);

						PhotoDB photoDB = new PhotoDB(token.DBConnection);
						photoDB.DeleteDirectoryPhotos(photoDirectory);

						results.Remove(photoDirectory);
					}

				}
				
				return results;

			}

		}

		/// <summary>
		/// Returns the virtual path for a directory.
		/// </summary>
		/// <param name="token">A reference to the current directory browser session token object.</param>
		/// <param name="fullDirectoryPath">The phyiscal full path of the directory.</param>
		/// <returns>The virtual path of the directory as a string.</returns>
		private static string GetVirtualPath(SessionToken token, string fullDirectoryPath)
		{

			return fullDirectoryPath.Substring(token.PhotosRootFullPath.Length,
											   fullDirectoryPath.Length - token.PhotosRootFullPath.Length);

		}

	}
}
