using System;
using System.Globalization;

namespace Codefresh.PhotoBrowserLibrary
{
	/// <summary>
	/// The superclass for all photo library entity objects..
	/// </summary>
	[Serializable]
	public abstract class PhotoObjectBase
	{

		private int id = -1;
		protected SessionToken token;

		/// <summary>
		/// Initializes a new PhotoObjectBase object.
		/// </summary>
		protected PhotoObjectBase()
		{
		}

		/// <summary>
		/// Initializes a new PhotoObjectBase object.
		/// </summary>
		/// <param name="token">A reference to the current SessionToken object.</param>
		protected PhotoObjectBase(SessionToken token) :  this()
		{
			this.token = token;
		}

		/// <summary>
		/// Sets the ID of the object.
		/// </summary>
		/// <param name="id">The ID of the object.</param>
		internal void SetId(int id)
		{
			this.id = id;
		}

		/// <value>The unqiue ID of the object.</value>
		public int Id
		{
			get
			{
				return id;
			}
		}

	}

}
