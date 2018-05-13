using System;
using System.Data;
using System.Data.OleDb;
using Codefresh.PhotoBrowserLibrary.Collections;

namespace Codefresh.PhotoBrowserLibrary.DataAccessLayer
{
	/// <summary>
	/// Database access class for Photo objects.
	/// </summary>
	internal class PhotoDB : DBBase
	{

		/// <summary>
		/// Intialises a new PhotoDB object.
		/// </summary>
		/// <param name="conn">A reference to a currently open database connection object.</param>
		public PhotoDB(IDbConnection conn) : base(conn)
		{
		}

		/// <summary>
		/// Deletes the database photo records belonging to a particular directory.
		/// </summary>
		/// <param name="dir">A PhotoDirectory object representing the directory who's photos that are to be deleted.</param>
		public void DeleteDirectoryPhotos(PhotoDirectory dir)
		{

			IDbCommand cmd = GetCommand();
			cmd.CommandText = "DELETE FROM tblPhotos WHERE DirectoryID = " + dir.Id;

			//cmd.Parameters.Add(CreateIntParam("DirectoryID", dir.Id)); 

			cmd.ExecuteNonQuery();

		}

		/// <summary>
		/// Retrieves a list of photos that belong to a particular directory.
		/// </summary>
		/// <param name="token">A reference to the current SessionToken object.</param>
		/// <param name="dir">A PhotoDirectory object representing the directory who's photos that are to be retrieved.</param>
		/// <returns></returns>
		public Photos GetPhotos(SessionToken token, PhotoDirectory dir)
		{

			IDbCommand cmd = GetCommand();
			cmd.CommandText = "SELECT ID, Name, VirtualPath, DateTaken, FileSize, ViewedCount FROM tblPhotos WHERE DirectoryID = " + dir.Id + 
                              " AND IsDeleted <> 'Y' ORDER BY DateTaken, Name"; 
			//cmd.Parameters.Add(CreateIntParam("DirectoryID", dir.Id));

			Photos results = new Photos();

			using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.Default))
			{

				while(reader.Read()) 
				{
					Photo photo = new Photo(token,
											reader.GetInt32(0), 
											reader.GetString(1), 
											reader.GetString(2),
											reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3),
											reader.GetInt32(4),
											reader.GetInt32(5));
					results.Add(photo);
	
				}

			}

			return results;

		}

		/// <summary>
		/// Inserts a new photo into the database.
		/// </summary>
		/// <param name="dir">A PhotoDirectory object representing the photo's parent directory.</param>
		/// <param name="photo">The photo object to insert.</param>
		/// <returns>The primary key of the newly inserted photo.</returns>
		internal int Insert(PhotoDirectory dir, Photo photo)
		{
            DateTime dt;
            if (photo.DateTaken == DateTime.MinValue)
                dt = DateTime.Today;
            else
                dt = photo.DateTaken;

			IDbCommand cmd = GetCommand();
            //cmd.CommandText = "INSERT INTO tblPhotos (DirectoryID, Name, VirtualPath, DateTaken, FileSize, ViewedCount) VALUES (?, ?, ?, ?, ?, 0)";
            cmd.CommandText = "INSERT INTO tblPhotos (DirectoryID, Name, VirtualPath, DateTaken, FileSize, ViewedCount) VALUES ("+ dir.Id + ", '"+ photo.Name + "', '"+ photo.VirtualPath + "', '"+ dt + "', '"+ photo.FileSize + "', 0)";

            //cmd.Parameters.Add(CreateIntParam("DirectoryID", dir.Id)); 
			//cmd.Parameters.Add(CreateStringParam("Name", photo.Name)); 
			//cmd.Parameters.Add(CreateStringParam("VirtualPath", photo.VirtualPath)); 
			//cmd.Parameters.Add(CreateDateTimeParam("DateTaken", photo.DateTaken)); 
			//cmd.Parameters.Add(CreateLongParam("FileSize", photo.FileSize)); 

			cmd.ExecuteNonQuery();

            //return GetIdentityValue(photo);
            cmd.CommandText = "SELECT MAX(ID) from tblPhotos";
            int id = (int)cmd.ExecuteScalar();
            photo.SetId(id);
            return id;
        }

        /// <summary>
        /// Deletes a photo from the database. Note that this does not delete any associated comments.
        /// </summary>
        /// <param name="obj">A Photo object representing the photo to be deleted.</param>
        internal override void Delete(PhotoObjectBase obj)
		{

			Photo photo = (Photo) obj;

			IDbCommand cmd = GetCommand();
			cmd.CommandText = "DELETE FROM tblPhotos WHERE ID = " + photo.Id;

			//cmd.Parameters.Add(CreateIntParam("ID", photo.Id)); 

			int rowsAffected = cmd.ExecuteNonQuery();

			if (rowsAffected != 1) throw new Exception("Attempted to delete a Photo record that does not exist");

		}

		/// <summary>
		/// Increments the viewed count on the database for a given photo.
		/// </summary>
		/// <param name="photo">The Photo whos viewed count is to be incremented.</param>
		public void IncrementViewedCount(Photo photo)
		{

			IDbCommand cmd = GetCommand();
			cmd.CommandText = "UPDATE tblPhotos SET ViewedCount = ViewedCount + 1 WHERE ID = " + photo.Id;

			//cmd.Parameters.Add(CreateIntParam("ID", photo.Id)); 

			int rowsAffected = cmd.ExecuteNonQuery();

			if (rowsAffected != 1) throw new Exception("Attempted to increment the viewed count for a non-existant Photo record");
		
		}

	}
}
