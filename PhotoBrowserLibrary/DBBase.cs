using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Codefresh.PhotoBrowserLibrary.DataAccessLayer
{

	/// <summary>
	/// Base class for data access layer classes. Provides generic database handling
	/// routines etc.
	/// </summary>
	internal abstract class DBBase
	{

		protected IDbConnection conn;

		/// <summary>
		/// Initializes a new instance of the DBBase class.
		/// </summary>
		/// <param name="conn">A reference to an open database connection object.</param>
		public DBBase(IDbConnection conn)
		{
			this.conn = conn;
		}

		/// <summary>
		/// Returns a new IDbCommend object.
		/// </summary>
		/// <returns>An IDbComment object.</returns>
		protected IDbCommand GetCommand()
		{			

			IDbCommand cmd = conn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			return cmd;

		}

		/// <summary>
		/// Returns a database command parameter object.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="type">The data type of the parameter.</param>
		/// <param name="obj">The value of the parameter.</param>
		/// <returns>A IDbDataParameter object.</returns>
		public IDbDataParameter CreateParam(string name, DbType type, object obj)
		{
			OleDbParameter param = new OleDbParameter();
			param.Direction = ParameterDirection.Input;
			param.DbType = type;
			param.Value = obj;
			param.ParameterName = name;

			return param;

		}

		/// <summary>
		/// Returns a database command long parameter object.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="val">The value of the parameter.</param>
		/// <returns>A IDbDataParameter object.</returns>
		protected IDbDataParameter CreateLongParam(string name, long val)
		{

			return CreateParam(name, DbType.Int64, val);

		}

		/// <summary>
		/// Returns a database command date and time parameter object.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="val">The value of the parameter.</param>
		/// <returns>A IDbDataParameter object.</returns>
		protected IDbDataParameter CreateDateTimeParam(string name, DateTime val)
		{

			if (val == DateTime.MinValue)
				return CreateParam(name, DbType.DateTime, DBNull.Value);
			else
				return CreateParam(name, DbType.DateTime, val);

		}

		/// <summary>
		/// Returns a database command date parameter object.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="val">The value of the parameter.</param>
		/// <returns>A IDbDataParameter object.</returns>
		protected IDbDataParameter CreateDateParam(string name, DateTime val)
		{

			if (val == DateTime.MinValue)
				return CreateParam(name, DbType.Date, DBNull.Value);
			else
				return CreateParam(name, DbType.Date, val);

		}

		/// <summary>
		/// Returns a database command long int object.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="val">The value of the parameter.</param>
		/// <returns>A IDbDataParameter object.</returns>
		protected IDbDataParameter CreateIntParam(string name, int val)
		{

			return CreateParam(name, DbType.Int32, val);

		}

		/// <summary>
		/// Returns a database command string parameter object.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="val">The value of the parameter.</param>
		/// <returns>A IDbDataParameter object.</returns>
		protected IDbDataParameter CreateStringParam(string name, string val)
		{

			return CreateParam(name, DbType.String, val);

		}

		/// <summary>
		/// Deletes an object from the databse.   
		/// </summary>
		/// <param name="obj" type="PhotosServer.PhotosObjectBase">The object to delete.</param>
		internal abstract void Delete(PhotoObjectBase obj);

		/// <summary>
		/// Retrieves the AutoNumber value for a newly inserted row and updates the input object
		/// accordingly.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <returns>The retrieved AutoNumber value.</returns>
		protected int GetIdentityValue(PhotoObjectBase obj)
		{

			IDbCommand identCmd = GetCommand();
			identCmd.CommandText = "SELECT @@IDENTITY";

			int id = (int) identCmd.ExecuteScalar();

			obj.SetId(id);

			return id;

		}

	}
}
