using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCI.VSP.Data.CRM.DataLogic;

namespace PCI.VSP.Test
{
    [TestClass]
    public class SecurityTest
    {
        [TestMethod]
        public void AuthenticateUserTest()
        {
            AuthenticateUser();
        }

        public PCI.VSP.Services.Model.IUser AuthenticateUser()
        {
            Globals.InitVspService();

            PCI.VSP.Services.VspService vspService = new PCI.VSP.Services.VspService();
            PCI.VSP.Services.AuthenticationRequest authRequest = new PCI.VSP.Services.AuthenticationRequest()
            {
                DomainName = String.Empty,
                Password = Globals.MakeSecureString("Oyfw3QXG8nidvM4UXMkBgE1bntyMijecfq8E3/la8n8="),
                Username = "rsmartin"
            };

            PCI.VSP.Services.Model.IUser user = vspService.ValidateUser(authRequest);
            return user;
        }

        [TestMethod]
        public void GetContactByUsername()
        {
            Globals.InitVspService();

            PCI.VSP.Data.CRM.DataLogic.ContactDataLogic cdl = new ContactDataLogic(Globals.GetGenericAuthRequest());
            PCI.VSP.Data.CRM.Model.Contact c = cdl.Retrieve("");
        }


        //[TestMethod]
        //public void ChangePassword()
        //{
        //    InitVspService();

        //    PCI.VSP.Services.ChangePasswordRequest cpr = new Services.ChangePasswordRequest()
        //    {
        //        Username = "rsmartin",
        //        OldPassword = MakeSecureString("whatever"),
        //        NewPassword = MakeSecureString("changedPassword")
        //    };

        //    PCI.VSP.Services.VspService vspService = new PCI.VSP.Services.VspService();
        //    Boolean result = vspService.ChangePassword(cpr);

        //}

        [TestMethod]
        public void ChangePasswordQuestion()
        {
            Globals.InitVspService();

            PCI.VSP.Services.ChangePasswordQuestionRequest cpqr = new Services.ChangePasswordQuestionRequest()
            {
                Username = "rsmartin",
                Password = Globals.MakeSecureString("whatever"),
                PasswordQuestion = "What is your favorite color?",
                PasswordAnswer = Globals.MakeSecureString("Blue.  No yel--  Auuuuuuuugh!")
            };

            PCI.VSP.Services.VspService vspService = new PCI.VSP.Services.VspService();
            Boolean result = vspService.ChangePasswordQuestionAndAnswer(cpqr);
        }
    }
}
