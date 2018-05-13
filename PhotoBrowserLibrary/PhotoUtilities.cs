using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using Codefresh.PhotoBrowserLibrary.Collections;
using Codefresh.PhotoBrowserLibrary.DataAccessLayer;

namespace Codefresh.PhotoBrowserLibrary
{
	/// <summary>
	/// Utility class that privides methods for dealing with Photo objects.
	/// </summary>
	public sealed class PhotoUtilities
	{
		private PhotoUtilities()
		{
		}

		/// <summary>
		/// Returns the details of the photos contained within a directory.
		/// </summary>
		/// <param name="token">A reference to the current session token object.</param>
		/// <param name="obj">The directory whos photos we want to retrieve.</param>
		/// <returns>A collection of Photo objects.</returns>
		public static Photos GetPhotos(SessionToken token, PhotoDirectory obj)
		{

			PhotoDB db = new PhotoDB(token.DBConnection);

			Photos results = db.GetPhotos(token, obj);
				
			Photos originalResults = (Photos) results.Clone();

			// Get a list of directories from the root path from the file system
			DirectoryInfo dirInfo = new DirectoryInfo(token.MapPath(obj.FullVirtualPath));
			FileInfo[] fileInfos = dirInfo.GetFiles("*.jpg");

			// Only need to update the database if the number of files in the database are 
			// different to those on the file system.
			// TODO: Currently won't detect changes in file names. Ideas to enable this 
			//       and still be fast?
			if (results.Count != fileInfos.Length)
			{

				foreach (FileInfo file in fileInfos)
				{

					string virtualPath = GetVirtualPath(token, file.FullName);

					// Do we have a Photo for this directory already? If not then we need to
					// add it to the database
					if (!results.Contains(virtualPath))
					{								

						string dirVirtualPath = virtualPath.Substring(0, virtualPath.Length - file.Name.Length - 1);
						
						DateTime dateTaken = DateTime.MinValue;
						using (Image image = Image.FromFile(file.FullName))
						{
							try
							{
								PropertyItem date = image.GetPropertyItem((int) KnownEXIFIDCodes.DateTimeOriginal);	
								dateTaken = ParseExifDateTime(date.Value);
							}
							catch (ArgumentException)
							{
								// image does not have a date value
							}
						
						}
						Photo photo = new Photo(token, file.Name, dirVirtualPath, dateTaken, file.Length);

						// insert this fire into the database
						db.Insert(obj, photo);

						results.Add(photo);
					}

					originalResults.Remove(virtualPath);

				}

				// Are there any Photo objects in the original result set that aren't in
				// the list of files from the file system? If so, delete 'em
				foreach (Photo photo in originalResults)
				{

					// Delete the associated thumbnail
					ThumbnailUtilities.DeleteThumbnail(token.MapPath(photo.FullThumbnailVirtualPath));

					db.Delete(photo);
					results.Remove(photo);
				}

			}

			return results;

		}

		/// <summary>
		/// Converts the byte data from an image's EXIF date data into a DateTime object.
		/// </summary>
		/// <param name="data">The data to conert.</param>
		/// <returns>The input byte array converts into a DateTime object.</returns>
		public static DateTime ParseExifDateTime(byte[] data)
		{
			DateTime ret = DateTime.MinValue;
			string date = ParseExifString(data);
			if (date.Length >= 19)
			{
				try
				{
					int year = int.Parse(date.Substring(0, 4), CultureInfo.CurrentCulture);
					int month = int.Parse(date.Substring(5,2), CultureInfo.CurrentCulture);
					int day = int.Parse(date.Substring(8,2), CultureInfo.CurrentCulture);
					int hour = int.Parse(date.Substring(11,2), CultureInfo.CurrentCulture);
					int minute = int.Parse(date.Substring(14,2), CultureInfo.CurrentCulture);
					int second = int.Parse(date.Substring(17,2), CultureInfo.CurrentCulture);
					ret = new DateTime(year, month, day, hour, minute, second);
				}
				catch (FormatException)
				{
				}
			}
			return ret;

		}

		/// <summary>
		/// Converts a EXIF byte array into the string equivalent.
		/// </summary>
		/// <param name="data">The data to convert.</param>
		/// <returns>The input byte array as a string.</returns>
		public static string ParseExifString(byte[] data)
		{
			string parsed = "";
			if (data.Length > 1)
			{
				IntPtr h = Marshal.AllocHGlobal(data.Length);
				int i = 0;
				foreach (byte b in data)
				{
					Marshal.WriteByte(h, i, b);
					i++;
				}
				// qu: can any of these items be non-ansi?
				parsed = Marshal.PtrToStringAnsi(h);
				Marshal.FreeHGlobal(h);
			}
			return parsed;
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
