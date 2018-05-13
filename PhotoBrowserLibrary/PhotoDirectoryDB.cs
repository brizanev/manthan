using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using Codefresh.PhotoBrowserLibrary;
using Codefresh.PhotoBrowserLibrary.Collections;

namespace Codefresh.PhotoBrowserLibrary.DataAccessLayer
{
	/// <summary>
	/// Data access class for PhotoDirectory objects.
	/// </summary>
	internal class PhotoDirectoryDB : DBBase
	{

		/// <summary>
		/// Intialises a new PhotoDirectoryDB object.
		/// </summary>
		/// <param name="conn">A reference to a currently open database connection object.</param>
		public PhotoDirectoryDB(IDbConnection conn) : base(conn)
		{
		}

		/// <summary>
		/// Retrieves a list of sub-directories stored in the PhotosServer databse.
		/// </summary>
		/// <param name="token">A reference to the current directory browser session token object.</param>
		/// <param name="virtualPath">The virtual path to the parent directory.</param>
		/// <returns>A collection of PhotoDirectory objects</returns>
		public PhotoDirectories GetDirectories(SessionToken token, string virtualPath)
		{

			IDbCommand cmd = GetCommand();
			//cmd.CommandText = "SELECT ID, Name from tblDirectories where VirtualPath = ? " +
            cmd.CommandText = "SELECT ID, Name from tblDirectories where VirtualPath = '" + virtualPath + "' AND IsDeleted <> 'Y' ORDER BY Name";
			//cmd.Parameters.Add(CreateStringParam("VirtualPath", virtualPath));

			PhotoDirectories results = new PhotoDirectories();

			using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.Default)) 
			{

				while(reader.Read())
				{
					PhotoDirectory photoDir = new PhotoDirectory(token,
																 reader.GetInt32(0),
																 reader.GetString(1), 
																 virtualPath);

					results.Add(photoDir);
				}
			}

			return results;

		}

		/// <summary>
		/// Inserts a new PhotoDirectory in to the database.   
		/// </summary>
		/// <param name="parent">The parent directory object, or null if at the root.</param>
		/// <param name="dir">The PhotoDirectory object to insert.</param>
		/// <returns>The ID of the newly inserted PhotoDirectory.</returns>
		internal int Insert(PhotoDirectory parent, PhotoDirectory photoDirectory)
		{

			IDbCommand cmd = GetCommand();

			if (parent == null)
			{
                //cmd.CommandText = "INSERT INTO tblDirectories (ParentID, Name, VirtualPath) VALUES (NULL, ?, ?)";
                cmd.CommandText = "INSERT INTO tblDirectories (ParentID, Name, VirtualPath) VALUES (NULL, '" + photoDirectory.Name + "', '"+ photoDirectory.VirtualPath + "')";
            }
            else
			{
                //cmd.CommandText = "INSERT INTO tblDirectories (ParentID, Name, VirtualPath) VALUES (?, ?, ?)";
                //cmd.Parameters.Add(CreateIntParam("ParentID", parent.Id));
                cmd.CommandText = "INSERT INTO tblDirectories (ParentID, Name, VirtualPath) VALUES ("+ parent.Id + ",  '" + photoDirectory.Name + "', '" + photoDirectory.VirtualPath + "')";
            }

   //         cmd.Parameters.Add(CreateStringParam("Name", photoDirectory.Name)); 
			//cmd.Parameters.Add(CreateStringParam("VirtualPath", photoDirectory.VirtualPath)); 

			cmd.ExecuteNonQuery();

            // Get the AutoNumber identity column values and update the input parameter object
            //return GetIdentityValue(photoDirectory);
            cmd.CommandText = "SELECT MAX(ID) from tblDirectories";
            int id = (int)cmd.ExecuteScalar();
            photoDirectory.SetId(id);
            return id;
        }

        /// <summary>
        /// Deletes a directory based on its name and virtual path. Note that Jet will take care
        /// of deleting any sub-directories via cascading deletes.
        /// </summary>
        /// <param name="name">The name of the directory to delete.</param>
        /// <param name="virtualPath">The virtual path of the directory to delete.</param>
        internal void Delete(string name, string virtualPath)
		{

			IDbCommand cmd = GetCommand();
			cmd.CommandText = "DELETE FROM tblDirectories WHERE Name = '" + name +"' AND VirtualPath = '"+ virtualPath +"'";

			//cmd.Parameters.Add(CreateStringParam("Name", name)); 
			//cmd.Parameters.Add(CreateStringParam("VirtualPath", virtualPath)); 

			cmd.ExecuteNonQuery();
		
		}

		/// <summary>
		/// Deletes a direectory from the database. Note that the associated photos will
		/// be deleted by the Jet engine.
		/// </summary>
		/// <param name="obj">A PhotoDirectory object representing the photo to be deleted.</param>
		internal override void Delete(PhotoObjectBase obj)
		{

			PhotoDirectory photoDirectory = (PhotoDirectory) obj;

			IDbCommand cmd = GetCommand();
			cmd.CommandText = "DELETE FROM tblDirectories WHERE ID = " + photoDirectory.Id;

			//cmd.Parameters.Add(CreateIntParam("ID", photoDirectory.Id)); 

			int rowsAffected = cmd.ExecuteNonQuery();

			if (rowsAffected != 1) throw new Exception("Attempted to delete a PhotoDirectory record that did not exit");

		}

	}
}
