using System;
using System.IO;
using System.Data;
using System.Data.OleDb;
using Codefresh.PhotoBrowserLibrary.Collections;
using System.Data.SqlClient;

namespace Codefresh.PhotoBrowserLibrary
{
	/// <summary>
	/// Main entry point class for the PhotoBrowserLibrary component.
	/// </summary>
	public class DirectoryBrowser
	{
	
		//private const string DATABASE_NAME = "PhotosServer.mdb";

		private SessionToken token;
		private PhotoDirectories dirs;

		/// <summary>
		/// Initializes a new instance of the DirectoryBrowser class.
		/// </summary>
		/// <param name="photosPhysicalPath">The physical location of the root directory containing the photos.</param>
		public DirectoryBrowser(string photosPhysicalPath)
		{
			token = new SessionToken(photosPhysicalPath, OpenDatabaseConnection(photosPhysicalPath));
		}

		/// <summary>
		/// Opens a connection to the photo browser's Access database.
		/// </summary>
		/// <param name="photosPhysicalPath">The full physical path to the database.</param>
		/// <returns>A refrence to a IDbConnection objact.</returns>
		private IDbConnection OpenDatabaseConnection(string photosPhysicalPath)
		{

			//string dbLocation = photosPhysicalPath + Path.DirectorySeparatorChar + DATABASE_NAME;
			//string cs = @"PROVIDER=MICROSOFT.JET.OLEDB.4.0;DATA SOURCE=" + dbLocation;
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString();
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();

            //    // Call the overload that takes a connection in place of the connection string
            //    //return ExecuteDataset(connection, commandType, commandText, commandParameters);
            //}

            IDbConnection conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;

            //IDbConnection conn = new OleDbConnection(cs);
            //conn.Open();
            //return conn;

        }

        /// <summary>
        /// Returns the details of the sub-directories under the root photos folder.
        /// </summary>
        /// <returns>A collection of PhotoDirectory objects.</returns>
        public PhotoDirectories GetDirectories()
		{

			if (dirs == null)
				dirs = DirectoryUtilities.GetDirectories(null, token, token.PhotosRootFullPath);
		
			return dirs;
		
		}


	}
}
