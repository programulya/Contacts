using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Contacts.Contracts;
using Contacts.Domain;

namespace Contacts.Services
{
    public class ContactsService : IContactsService
    {
        #region Data

        private List<ContactGroup> _contactGroups = new List<ContactGroup>
            {
                new ContactGroup {Id = 1, Name = "Unsorted"},
                new ContactGroup {Id = 2, Name = "Best friends"},
                new ContactGroup {Id = 3, Name = "Haters gonna hate"},
                new ContactGroup {Id = 4, Name = "Family"}
            };

        private List<Contact> _contacts = new List<Contact>
            {
                new Contact
                    {
                        Id = 1,
                        Email = "dave@gmail.com",
                        FirstName = "Dave",
                        MiddleName = String.Empty,
                        LastName = "Gahan",
                        Phone = "(093)2696232",
                        ContactGroupId = 1
                    },
                new Contact
                    {
                        Id = 2,
                        Email = "megan@gmail.com",
                        FirstName = "Megan",
                        MiddleName = String.Empty,
                        LastName = "Fox",
                        Phone = "(093)3456232",
                        ContactGroupId = 2
                    },
                new Contact
                    {
                        Id = 3,
                        Email = "santa@gmail.com",
                        FirstName = "Santa",
                        MiddleName = String.Empty,
                        LastName = "Klaus",
                        Phone = "(093)1111111",
                        ContactGroupId = 1
                    },
                new Contact
                    {
                        Id = 4,
                        Email = "elvis@gmail.com",
                        FirstName = "Elvis",
                        MiddleName = String.Empty,
                        LastName = "Presley",
                        Phone = "2222222",
                        ContactGroupId = 4
                    },
                new Contact
                    {
                        Id = 5,
                        Email = "hater@gmail.com",
                        FirstName = "Hater",
                        MiddleName = String.Empty,
                        LastName = "Gonna hate",
                        Phone = "(067)3333333",
                        ContactGroupId = 2
                    },
                new Contact
                    {
                        Id = 6,
                        Email = "homeless@gmail.com",
                        FirstName = "Wall-e",
                        MiddleName = "Forever",
                        LastName = "Alone",
                        Phone = "(093)5673378",
                        ContactGroupId = 3
                    }
            };

        #endregion

        #region IContactsService

        public IList<ContactGroup> GetAllGroups()
        {
            try
            {
                var result = _contactGroups;

                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GetAllGroups: " + ex.Message);

                throw;
            }
        }

        public IList<Contact> GetContactsByGroup(ContactGroup contactGroup)
        {
            try
            {
                var result = _contacts.Where(c => c.ContactGroupId == contactGroup.Id).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GetAllGroups: " + ex.Message);

                throw;
            }
        }

        public Contact GetContactById(int contactId)
        {
            try
            {
                var result = _contacts.FirstOrDefault(c => c.Id == contactId);

                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GetContactById: " + ex.Message);

                throw;
            }
        }

        public ContactGroup GetGroupById(int contactGroupId)
        {
            try
            {
                var result = _contactGroups.FirstOrDefault(cg => cg.Id == contactGroupId);

                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GetGroupById: " + ex.Message);

                throw;
            }
        }

        public Int32 AddContact(Contact contact)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");

            if (_contacts.Any(c => contact.FullName.ToLower().Trim() == c.FullName))
            {
                throw new ArgumentException("A contact with the specified name already exists");
            }

            try
            {
                contact.Id = _contacts.Max(c => c.Id) + 1;

                _contacts.Add(contact);

                return contact.Id;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("AddContact: " + ex.Message);

                throw;
            }
        }

        public void RemoveContact(Contact contact)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");

            try
            {
                _contacts.Remove(contact);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("RemoveContact: " + ex.Message);

                throw;
            }
        }

        public void RemoveContactFromGroup(Contact contact)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");

            try
            {
                var foundContact = _contacts.FirstOrDefault(c => c.Id == contact.Id);
                if (foundContact != null)
                {
                    var contactGroup = _contactGroups.FirstOrDefault(cg => cg.Name == "Unsorted");
                    if (contactGroup != null)
                        foundContact.ContactGroupId = contactGroup.Id;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("RemoveContactFromGroup: " + ex.Message);

                throw;
            }
        }

        public void AddContactToGroup(Contact contact, ContactGroup contactGroup)
        {
            if (contact == null)
                throw new ArgumentNullException("contact");
            if (contactGroup == null)
                throw new ArgumentNullException("contactGroup");

            try
            {
                var foundContact = _contacts.FirstOrDefault(c => c.Id == contact.Id);
                if (foundContact != null)
                    foundContact.ContactGroupId = contactGroup.Id;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("AddContactToGroup: " + ex.Message);

                throw;
            }
        }

        #endregion
    }
}