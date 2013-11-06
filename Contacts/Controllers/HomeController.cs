using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Contacts.Domain;
using Contacts.Services;
using Contacts.Utilities;

namespace Contacts.Controllers
{
    public class HomeController : Controller
    {
        private static ContactsService _contactService = new ContactsService();

        #region Views

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Contacts ASP.NET MVC application";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        #endregion

        #region Get methods

        [Authorize]
        [HttpPost]
        public JsonResult GetAllGroups()
        {
            var result = new ServiceResponse<Object>();

            try
            {
                result.Result = GetAllGroupsResponse();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GetAllGroups: " + ex.Message);

                result.Message = ex.Message;
            }

            return JsonResponse(result);
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetContact(int contactId)
        {
            var result = new ServiceResponse<Object>();

            try
            {
                result.Result = GetContactResponse(contactId);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("GetContact: " + ex.Message);

                result.Message = ex.Message;
            }

            return JsonResponse(result);
        }

        #endregion

        #region Add/Remove contact

        [Authorize]
        [HttpPost]
        public JsonResult AddContact(string firstName, string middleName, string lastName, string email, string phone,
                                     int contactGroupId)
        {
            var result = new ServiceResponse<Object>();

            var contact = new Contact
                {
                    FirstName = firstName,
                    MiddleName = middleName,
                    LastName = lastName,
                    Email = email,
                    Phone = phone,
                    ContactGroupId = contactGroupId
                };

            try
            {
                _contactService.AddContact(contact);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("AddContact: " + ex.Message);

                result.Message = ex.Message;
            }

            result.Result = GetAllGroupsResponse();

            return JsonResponse(result);
        }

        [Authorize]
        [HttpPost]
        public JsonResult RemoveContact(int contactId)
        {
            var result = new ServiceResponse<Object>();

            var contact = _contactService.GetContactById(contactId);

            try
            {
                _contactService.RemoveContact(contact);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("RemoveContact: " + ex.Message);

                result.Message = ex.Message;
            }

            result.Result = GetAllGroupsResponse();

            return JsonResponse(result);
        }

        #endregion

        #region Add/Remove to/from group

        [Authorize]
        [HttpPost]
        public JsonResult RemoveContactFromGroup(int contactId)
        {
            var result = new ServiceResponse<Object>();

            Contact contact = null;

            try
            {
                contact = _contactService.GetContactById(contactId);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("RemoveContactFromGroup: " + ex.Message);

                result.Message = "A contact does not exist";
            }

            if (contact != null)
            {
                try
                {
                    _contactService.RemoveContactFromGroup(contact);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("RemoveContactFromGroup: " + ex.Message);

                    result.Message = ex.Message;
                }
            }

            result.Result = GetAllGroupsResponse();

            return JsonResponse(result);
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddContactToGroup(int contactId, int contactGroupId)
        {
            var result = new ServiceResponse<Object>();

            Contact contact = null;
            ContactGroup contactGroup = null;

            try
            {
                contact = _contactService.GetContactById(contactId);
                contactGroup = _contactService.GetGroupById(contactGroupId);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("AddContactToGroup: " + ex.Message);

                result.Message = "A contact or group does not exist";
            }

            if (contact != null && contactGroup != null)
            {
                try
                {
                    _contactService.AddContactToGroup(contact, contactGroup);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("AddContactToGroup: " + ex.Message);

                    result.Message = ex.Message;
                }
            }

            result.Result = GetAllGroupsResponse();

            return JsonResponse(result);
        }

        #endregion

        #region Private methods

        private Object GetAllGroupsResponse()
        {
            var groups = _contactService.GetAllGroups().OrderBy(g => g.Name).Select(g => new
                {
                    id = g.Id,
                    name = g.Name,
                    contacts = _contactService.GetContactsByGroup(g).OrderBy(p => p.FullName).
                                               Select(p => new {id = p.Id, name = p.FullName})
                                              .ToList()
                }).ToList();

            var data = new
                {
                    groups
                };

            return data;
        }

        private Object GetContactResponse(int contactId)
        {
            var contact = _contactService.GetContactById(contactId);

            var data = new
                {
                    name = contact.FullName,
                    email = contact.Email,
                    phone = contact.Phone,
                    group = _contactService.GetGroupById(contact.ContactGroupId).Name
                };

            return data;
        }

        private JsonResult JsonResponse(ServiceResponse<Object> serviceResponse)
        {
            var data = new
                {
                    result = serviceResponse.Result,
                    message = serviceResponse.Message
                };

            return Json(data);
        }

        #endregion
    }
}