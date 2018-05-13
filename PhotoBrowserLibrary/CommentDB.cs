using System;
using System.Data;
using System.Data.OleDb;
using Codefresh.PhotoBrowserLibrary;
using Codefresh.PhotoBrowserLibrary.Collections;

namespace Codefresh.PhotoBrowserLibrary.DataAccessLayer
{
	/// <summary>
	/// Database access class for Comment objects.
	/// </summary>
	internal class CommentDB : DBBase
	{

		/// <summary>
		/// Intialises a new CommentDB object.
		/// </summary>
		/// <param name="conn">A reference to a currently open database connection.</param>
		public CommentDB(IDbConnection conn) : base(conn)
		{
		}

		/// <summary>
		/// Inserts a new comment into the databse
		/// </summary>
		/// <param name="photo">A reference to the photo that the comment belongs to.</param>
		/// <param name="comment">The comment to insert.</param>
		/// <returns>The ID of the inserted comment.</returns>
		public int Insert(Photo photo, Comment comment)
		{

			IDbCommand cmd = GetCommand();
			cmd.CommandText = "INSERT INTO tblPhotoComments (PhotoFullVirtualPath, Name, Comment, DateAdded) VALUES ('"+ photo.FullVirtualPath + "', '"+ comment.Name + 
                "', '"+ comment.CommentText+"', '"+ comment.DateAdded +"')";

			//cmd.Parameters.Add(CreateStringParam("PhotoFullVirtualPath", photo.FullVirtualPath)); 
			//cmd.Parameters.Add(CreateStringParam("Name", comment.Name)); 
			//cmd.Parameters.Add(CreateStringParam("Comment", comment.CommentText));
			//cmd.Parameters.Add(CreateDateParam("DateAdded", comment.DateAdded)); 

			cmd.ExecuteNonQuery();

            //return GetIdentityValue(comment);
            cmd.CommandText = "SELECT MAX(ID) from tblPhotoComments";
            int id = (int)cmd.ExecuteScalar();
            comment.SetId(id);
            return id;
        }

        /// <summary>
        /// Retrieves the list of comments for a photo.
        /// </summary>
        /// <param name="token">A reference to the current SessionToken object.</param>
        /// <param name="photo">The photo to retrieve the list of comments for.</param>
        /// <returns>A collection of Comment objects.</returns>
        public Comments GetComments(SessionToken token, Photo photo)
		{

			IDbCommand cmd = GetCommand();
			cmd.CommandText = "SELECT ID, Name, Comment, DateAdded FROM tblPhotoComments WHERE PhotoFullVirtualPath = '"+ photo.FullVirtualPath + "'  AND IsDeleted <> 'Y' ORDER BY DateAdded, ID";

			//cmd.Parameters.Add(CreateStringParam("PhotoFullVirtualPath", photo.FullVirtualPath)); 

			Comments results = new Comments();

			using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.Default))
			{

				while(reader.Read()) 
				{

					Comment comment = new Comment(token,
												  reader.GetInt32(0), 
												  reader.IsDBNull(1) ? "" : reader.GetString(1), 
 												  reader.IsDBNull(2) ? "" : reader.GetString(2), 
												  reader.GetDateTime(3));

					results.Add(comment);
	
				}

			}

			return results;

		}

		/// <summary>
		/// Not used as you cannot currently delete comments.
		/// </summary>
		/// <param name="obj">A reference to the comment to delete.</param>
		internal override void Delete(PhotoObjectBase obj)
		{
			throw new NotImplementedException("Cannot delete comment objects");
		}

	}
}
