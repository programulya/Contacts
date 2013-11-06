using System;
using System.Collections.Generic;
using Contacts.Domain;

namespace Contacts.Contracts
{
	interface IContactsService
	{
		IList<ContactGroup> GetAllGroups();
		IList<Contact> GetContactsByGroup(ContactGroup contactGroup);
		Contact GetContactById(int contactId);
	    ContactGroup GetGroupById(int contactGroupId);
	    Int32 AddContact(Contact contact);
	    void RemoveContact(Contact contact);
	    void RemoveContactFromGroup(Contact contact);
	    void AddContactToGroup(Contact contact, ContactGroup contactGroup);
	}
}