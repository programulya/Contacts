$(document).ready(function() {
    contactManager = new contactManagementControl();
});

function contactManagementControl() {
    this.contactsData = {};
    this.activeGroup;

    this.getAllGroupsPath = '/Home/GetAllGroups';
    this.getContactPath = '/Home/GetContact';
    this.addContactPath = '/Home/AddContact';
    this.removeContactPath = '/Home/RemoveContact';
    this.removeContactFromGroupPath = '/Home/RemoveContactFromGroup';
    this.addContactToGroupPath = '/Home/AddContactToGroup';

    this.addContactDialog = $("#addContactDialog");
    this.removeContactDialog = $("#removeContactDialog");
    this.removeContactFromGroupDialog = $("#removeContactFromGroupDialog");
    this.addContactToGroupDialog = $("#addContactToGroupDialog");

    this.regexName = /^[^<>`~!/@\#}$%:;,)(^?{&*=|'+\[\]\\]+$/;
    this.regexEmail = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    this.regexPhone = /^(\+\d)*\s*(\(\d{3}\)\s*)*\d{3}(-{0,1}|\s{0,1})\d{2}(-{0,1}|\s{0,1})\d{2}$/;

    this.removeContactButton = $("#removeContactButton");
    this.addNewContactButton = $("#addNewContactButton");
    this.removeContactFromGroupButton = $("#removeContactFromGroupButton");
    this.addContactToGroupButton = $("#addContactToGroupButton");

    this.currentId = $("#currentId");
    this.currentFullName = $("#currentFullName");
    this.currentEmail = $("#currentEmail");
    this.currentPhone = $("#currentPhone");
    this.currentGroup = $("#currentGroup");

    this.removeContactButton.attr("disabled", "disabled");

    this.requestContactsData();
}

contactManagementControl.prototype = {
    requestContactsData: function() {
        var self = this;
        this.showLoader();

        $.ajax(
            {
                type: "POST",
                url: self.getAllGroupsPath,
                data: JSON.stringify(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(responseData) {
                    if (responseData.message) {
                        self.hideLoader();
                        self.errorHandler(responseData.message);
                    } else {
                        self.contactsData = responseData.result;
                        self.updateContactsView();
                        self.hideLoader();
                    }
                }
            });

        $("#groupsLists .groups-list").change(function() {
            self.activeGroup = $("#groupsLists .groups-list").val();
            self.updateContactsView();
        });

        this.addContactDialog.dialog({
            autoOpen: false,
            width: 360,
            title: "Add New Contact",
            modal: true,
            resizable: false,
            draggable: false,
            buttons: [{
                    id: "addContactOkButton",
                    text: "Ok",
                    click: function() {
                        var group = -1;
                        if (self.activeGroup != 0)
                            group = self.activeGroup;

                        if (self.validateContact(self.addContactDialog) === true) {
                            self.addContact(self.addContactDialog.find("input#firstName").val(),
                                self.addContactDialog.find("input#middleName").val(),
                                self.addContactDialog.find("input#lastName").val(),
                                self.addContactDialog.find("input#email").val(),
                                self.addContactDialog.find("input#phone").val(), group);
                        }
                    }
                }, {
                    id: "addContactCancelButton",
                    text: "Cancel",
                    click: function() {
                        $(this).dialog("close");
                    }
                }],
            open: function() {
                $("#addContactOkButton").focus();
            }
        });

        this.removeContactDialog.dialog({
            autoOpen: false,
            width: 300,
            title: "Remove contact",
            modal: true,
            resizable: false,
            draggable: false,
            buttons: [{
                    id: "removeContactOkButton",
                    text: "Ok",
                    click: function() {
                        self.removeContact(self.currentId.val());
                        $(this).dialog("close");
                        self.removeContactButton.attr("disabled", "disabled");
                    }
                }, {
                    id: "removeContactCancelButton",
                    text: "Cancel",
                    click: function() {
                        $(this).dialog("close");
                    }
                }],
            open: function() {
                $("#removeContactCancelButton").focus();
            }
        });

        this.removeContactFromGroupDialog.dialog({
            autoOpen: false,
            width: 300,
            title: "Remove Contact From Group",
            modal: true,
            resizable: false,
            draggable: false,
            buttons: [{
                    id: "removeContactFromGroupOkButton",
                    text: "Ok",
                    click: function() {
                        self.removeContactFromGroup(self.currentId.val());
                        $(this).dialog("close");
                    }
                }, {
                    id: "removeContactFromGroupCancelButton",
                    text: "Cancel",
                    click: function() {
                        $(this).dialog("close");
                    }
                }],
            open: function() {
                $("#removeContactFromGroupCancelButton").focus();
            }
        });

        this.addContactToGroupDialog.dialog({
            autoOpen: false,
            width: 300,
            title: "Add Contact To Group",
            modal: true,
            resizable: false,
            draggable: false,
            buttons: [{
                    id: "addContactToGroupOkButton",
                    text: "Ok",
                    click: function() {
                        self.addContactToGroup(self.currentId.val(), self.activeGroup);
                        $(this).dialog("close");
                    }
                }, {
                    id: "addContactToGroupCancelButton",
                    text: "Cancel",
                    click: function() {
                        $(this).dialog("close");
                    }
                }],
            open: function() {
                $("#addContactToGroupCancelButton").focus();
            }
        });

        self.addNewContactButton.click(function() {
            self.addContactDialog.find("input").val("");
            self.addContactDialog.find(".error").text("");
            self.addContactDialog.dialog("open");
        });

        self.removeContactButton.click(function() {
            self.removeContactDialog.dialog("open");
        });

        self.removeContactFromGroupButton.click(function() {
            self.removeContactFromGroupDialog.dialog("open");
        });

        self.addContactToGroupButton.click(function() {
            self.addContactToGroupDialog.dialog("open");
        });
    },

    updateContactsView: function() {
        var self = this;

        $("#groupsLists select.groups-list").empty().append('<option value="0">All Groups</option>');
        $.each(this.contactsData.groups, function() {
            $("#groupsLists select.groups-list").append('<option value="' + this.id + '">' + this.name + '</option>');
        });

        if (!this.activeGroup) {
            this.activeGroup = $("#groupsLists select.groups-list").val();
        } else {
            $("#groupsLists select.groups-list").val(self.activeGroup);
        }

        $("#groupsLists .all-contacts-wrapper").empty();

        $.each(this.contactsData.groups, function() {
            if (self.activeGroup == 0 || this.id == self.activeGroup) {

                $.each(this.contacts, function() {
                    self.setContactAction(this.id, this.name);
                });
            }
        });
    },

    setContactAction: function(contactId, contactName) {
        var self = this;

        $("#groupsLists .all-contacts-wrapper").append('<div class="current-contact" id="contact_' + contactId + '"><a class="set-action"></a><span class="hover"></span>' + '<span class="name">' + contactName + '</span></div>');
        $("#groupsLists .all-contacts-wrapper .current-contact").last().find("a.set-action").click(function() {
            //event.stopPropagation();
            self.getContact(contactId);
        });
    },

    getContact: function(contactId) {
        var self = this;
        var params = {
            contactId: contactId
        };

        this.showLoader();

        $.ajax(
            {
                type: "POST",
                url: self.getContactPath,
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(responseData) {
                    if (responseData.message) {
                        self.hideLoader();
                        self.errorHandler(responseData.message);
                    } else {
                        self.currentId.val(contactId);
                        self.currentFullName.text(responseData.result.name);
                        self.currentEmail.attr("href", "mailto:" + responseData.result.email);
                        self.currentEmail.text(responseData.result.email);
                        self.currentPhone.attr("href", "callto:" + responseData.result.phone);
                        self.currentPhone.text(responseData.result.phone);
                        self.currentGroup.text(responseData.result.group);

                        self.hideLoader();

                        $("#groupsLists .contact-info-wrapper").show(500);
                        self.removeContactButton.removeAttr("disabled");
                    }
                }
            });
    },

    addContact: function(firstName, middleName, lastName, email, phone, contactGroupId) {
        var self = this;
        var params = {
            firstName: firstName,
            middleName: middleName,
            lastName: lastName,
            email: email,
            phone: phone,
            contactGroupId: contactGroupId
        };

        this.showLoader();

        $.ajax(
            {
                type: "POST",
                url: self.addContactPath,
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(responseData) {
                    if (responseData.message) {
                        self.hideLoader();
                        self.errorHandler(responseData.message);
                    } else {
                        self.contactsData = responseData.result;
                        self.addContactDialog.dialog("close");
                        self.updateContactsView();
                        self.hideLoader();
                    }
                }
            });
    },

    removeContact: function(contactId) {
        var self = this;
        var params = {
            contactId: contactId
        };

        this.showLoader();

        $.ajax(
            {
                type: "POST",
                url: self.removeContactPath,
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(responseData) {
                    if (responseData.message) {
                        self.hideLoader();
                        self.errorHandler(responseData.message);
                    } else {
                        $("#groupsLists .contact-info-wrapper").hide(500);
                        self.contactsData = responseData.result;
                        self.updateContactsView();
                        self.hideLoader();
                    }
                }
            });
    },

    removeContactFromGroup: function(contactId) {
        var self = this;
        var params = {
            contactId: contactId
        };

        this.showLoader();

        $.ajax(
            {
                type: "POST",
                url: self.removeContactFromGroupPath,
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(responseData) {
                    if (responseData.message) {
                        self.hideLoader();
                        self.errorHandler(responseData.message);
                    } else {
                        self.getContact(self.currentId.val());
                        self.contactsData = responseData.result;
                        self.updateContactsView();
                        self.hideLoader();
                    }
                }
            });
    },

    addContactToGroup: function(contactId, contactGroupId) {
        var self = this;
        var params = {
            contactId: contactId,
            contactGroupId: contactGroupId
        };

        this.showLoader();

        $.ajax(
            {
                type: "POST",
                url: self.addContactToGroupPath,
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(responseData) {
                    if (responseData.message) {
                        self.hideLoader();
                        self.errorHandler(responseData.message);
                    } else {
                        self.getContact(self.currentId.val());
                        self.contactsData = responseData.result;
                        self.updateContactsView();
                        self.hideLoader();
                    }
                }
            });
    },

    validateContact: function(contactDialog) {
        var isValid = true;

        if (!this.regexName.test(contactDialog.find("input#firstName").val())) {
            contactDialog.find(".first-name .error").text("The first name is invalid");
            isValid = false;
        } else {
            contactDialog.find(".first-name .error").text("");
        }

        if (contactDialog.find("input#middleName").val() != "") {
            if (!this.regexName.test(contactDialog.find("input#middleName").val())) {
                contactDialog.find(".middle-name .error").text("The middle name is invalid");
                isValid = false;
            }
        } else {
            contactDialog.find(".middle-name .error").text("");
        }


        if (!this.regexName.test(contactDialog.find("input#lastName").val())) {
            contactDialog.find(".last-name .error").text("The last name is invalid");
            isValid = false;
        } else {
            contactDialog.find(".last-name .error").text("");
        }

        if (!this.regexEmail.test(contactDialog.find("input#email").val())) {
            contactDialog.find(".email .error").text("The e-mail is invalid");
            isValid = false;
        } else {
            contactDialog.find(".email .error").text("");
        }

        if (!this.regexPhone.test(contactDialog.find("input#phone").val())) {
            contactDialog.find(".phone .error").text("The phone is invalid");
            isValid = false;
        } else {
            contactDialog.find(".phone .error").text("");
        }

        return isValid;
    },

    errorHandler: function(errorText) {
        alert(errorText);
        this.hideLoader();
    },

    showLoader: function() {
        $("#groupsLists .load-hover").show();
    },

    hideLoader: function() {
        $("#groupsLists .load-hover").hide();
    }
};