using System;

namespace Contacts.Domain
{
	public sealed class Contact
	{
		public Int32 Id { get; set; }
		public String Email { get; set; }
		public String FirstName { get; set; }
		public String MiddleName { get; set; }
        	public String LastName { get; set; }
		public String Phone { get; set; }
		public Int32 ContactGroupId { get; set; }

		public string FullName
		{
			get
			{
				return string.Format("{0} {1}{2}",
				                     FirstName,
				                     !string.IsNullOrEmpty(MiddleName) ? MiddleName + " " : string.Empty,
				                     LastName);
			}
		}

		public override string ToString()
		{
			return FullName;
		}
	}
}
