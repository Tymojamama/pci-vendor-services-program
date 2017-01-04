using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using PCI.VSP.Services;
using System.Collections.Specialized;
using System.Security;
using System.Security.Cryptography;

namespace PCI.VSP.Web.Security
{
    public class VspMembershipProvider : System.Web.Security.MembershipProvider
    {
        private NameValueCollection _config = null;

        // these are defaults in case they aren't set inside the web.config file:
        private const Int32 _maxInvalidPasswordAttempts = 5;
        private const Boolean _enablePasswordReset = false;
        private const Boolean _enablePasswordRetrieval = false;
        private const Boolean _requiresQuestionAndAnswer = true;
        private const Boolean _requiresUniqueEmail = true;
        private const Int32 _minRequiredPasswordLength = 6;
        private const Int32 _minRequiredNonalphanumericCharacters = 1;
        private const Int32 _passwordAttemptWindow = 10;

        public override string ApplicationName
        {
            get
            {
                if (_config["applicationName"] != null)
                    return Convert.ToString(_config["applicationName"]);
                return System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            _config = config;
            base.Initialize(name, config);
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            Byte[] salt = GenerateSalt(username);
            ChangePasswordRequest cpr = new ChangePasswordRequest()
            {
                Username = username.Trim(),
                OldPassword = MakeSecureString(EncodeString(oldPassword, salt)),
                NewPassword = MakeSecureString(newPassword) // password is hashed by CRM plugin
            };
            return new VspService().ChangePassword(cpr);
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            Byte[] salt = GenerateSalt(username);
            ChangePasswordQuestionRequest cpqr = new ChangePasswordQuestionRequest()
            {
                Password = MakeSecureString(EncodeString(password, salt)),
                PasswordAnswer = MakeSecureString(newPasswordAnswer), // answer is hashed by CRM plugin
                PasswordQuestion = newPasswordQuestion.Trim(),
                Username = username.ToString()
            };
            return new VspService().ChangePasswordQuestionAndAnswer(cpqr);
        }

        private System.Security.SecureString MakeSecureString(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) { return null; }
            System.Security.SecureString secureString = new System.Security.SecureString();
            foreach (Char c in value.Trim().ToCharArray())
                secureString.AppendChar(c);
            secureString.MakeReadOnly();
            return secureString;
        }

        public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get 
            {
                if (_config["enablePasswordReset"] != null)
                    return Convert.ToBoolean(_config["enablePasswordReset"]);
                return _enablePasswordReset;
            }
        }

        public override bool EnablePasswordRetrieval
        {
            get
            {
                if (_config["enablePasswordRetrieval"] != null)
                    return Convert.ToBoolean(_config["enablePasswordRetrieval"]);
                return _enablePasswordRetrieval;
            }
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
        {
            VspService vspService = new VspService();
            Services.Model.IUser user = vspService.GetUser(username);
            if (user == null) { return null; }

            return new MembershipUser("VspMembershipProvider", user.Username, user.Id, user.Email, user.SecurityQuestion, null, false, false, new DateTime(),
                user.LastLogin, new DateTime(), new DateTime(), new DateTime());
        }

        public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                if (_config["maxInvalidPasswordAttempts"] != null)
                    return Convert.ToInt32(_config["maxInvalidPasswordAttempts"]);
                return _maxInvalidPasswordAttempts;
            }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get 
            {
                if (_config["minRequiredNonalphanumericCharacters"] != null)
                    return Convert.ToInt32(_config["minRequiredNonalphanumericCharacters"]);
                return _minRequiredNonalphanumericCharacters;
            }
        }

        public override int MinRequiredPasswordLength
        {
            get 
            {
                if (_config["minRequiredPasswordLength"] != null)
                    return Convert.ToInt32(_config["minRequiredPasswordLength"]);
                return _minRequiredPasswordLength;
            }
        }

        public override int PasswordAttemptWindow
        {
            get
            {
                if (_config["passwordAttemptWindow"] != null)
                    return Convert.ToInt32(_config["passwordAttemptWindow"]);
                return _passwordAttemptWindow;
            }
        }

        public override System.Web.Security.MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                if (_config["requiresQuestionAndAnswer"] != null)
                    return Convert.ToBoolean(_config["requiresQuestionAndAnswer"]);
                return _requiresQuestionAndAnswer;
            }
        }

        public override bool RequiresUniqueEmail
        {
            get
            {
                if (_config["requiresUniqueEmail"] != null)
                    return Convert.ToBoolean(_config["requiresUniqueEmail"]);
                return _requiresUniqueEmail;
            }
        }

        private string EncodeString(string pass, Byte[] salt)
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(pass);
            byte[] dst = new byte[salt.Length + bytes.Length];
            Buffer.BlockCopy(salt, 0, dst, 0, salt.Length);
            Buffer.BlockCopy(bytes, 0, dst, salt.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA256");
            byte[] inArray = algorithm.ComputeHash(dst);
            return Convert.ToBase64String(inArray);
        }

        private Byte[] GenerateSalt(string foundation)
        {
            Char[] inverse = foundation.ToCharArray();
            Array.Reverse(inverse);
            return System.Text.Encoding.ASCII.GetBytes("B4MEF!8Hbpa6@#" + new string(inverse) + "B4yhN$%HUY^1sd");
        }

        public override string ResetPassword(string username, string answer)
        {
            String newPassword = Membership.GeneratePassword(this.MinRequiredPasswordLength, this.MinRequiredNonAlphanumericCharacters);
            Byte[] salt = GenerateSalt(username);

            ResetPasswordRequest rpr = new ResetPasswordRequest()
            {
                Username = username.Trim(),
                SecurityAnswer = MakeSecureString(EncodeString(answer.Trim(), salt)),
                NewPassword = MakeSecureString(newPassword) // password is hashed by CRM plugin
            };
            
            if (!(new VspService().ResetUserPassword(rpr)))
                throw new MembershipPasswordException();
            return newPassword;
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            if (String.IsNullOrWhiteSpace(username))
                throw new System.Configuration.Provider.ProviderException("Username cannot be null.");

            AuthenticationRequest authRequest = new AuthenticationRequest()
            {
                Password = MakeSecureString(EncodeString(password, GenerateSalt(username.Trim()))),
                Username = username.Trim(),
            };

            var vspService = new VspService();
            var user = vspService.ValidateUser(authRequest);
            if (user != null && !user.IsLocked)
            {
                HttpContext.Current.Session[Constants.User] = user;
                FormsAuthentication.SetAuthCookie(username, true);
            }
            else if (HttpContext.Current.Session[Constants.User] != null)
                HttpContext.Current.Session.Remove(Constants.User);

            return (user != null && !user.IsLocked);
        }
    }
}