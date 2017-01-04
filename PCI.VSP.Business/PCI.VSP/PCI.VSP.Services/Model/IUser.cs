using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCI.VSP.Services.Model
{
    public enum RoleType
    {
        Vendor = 0,
        Client = 1,
        SystemUser = 2
    }

    public interface IUser
    {
        Guid Id { get; set; }
        String Username { get; set; }
        String Email { get; set; }
        String FirstName { get; set; }
        String LastName { get; set; }
        String SecurityQuestion { get; set; }
        DateTime LastLogin { get; set; }
        Boolean MustChangePassword { get; set; }
        Guid AccountId { get; set; }
        Boolean IsLocked { get; set; }
    }
}
