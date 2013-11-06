namespace Contacts.Utilities
{
	public class ServiceResponse<T>
	{
		public string Message { get; set; }
		public T Result { get; set; }
	}
}