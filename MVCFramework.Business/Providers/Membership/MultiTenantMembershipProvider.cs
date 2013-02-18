using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using MVCFramework.Business.Exceptions;
using MVCFramework.Business.Providers.Portal;
using MVCFramework.Business.Repository.Entities;
using MVCFramework.Model.Entities;
using MVCFramework.Business.Providers.NHibernateSession;

namespace MVCFramework.Business.Providers.Membership
{
    public sealed class MultiTenantMembershipProvider : MembershipProvider
    {
        private string _applicationName;
        public override string ApplicationName
        {
            get
            {
                if (!string.IsNullOrEmpty(_applicationName))
                    return _applicationName;

                if (HttpContext.Current != null)
                {
                    // get the request host
                    string host = HttpContext.Current.Request.Headers["Host"]
                        .Split(':')[0] // removes port from host
                        .Replace("www.", string.Empty); // removes www. from host

                    return host;
                }

                throw new ArgumentNullException("ApplicationName", "Could not determine the application name for the membership provider.");
            }
            set { _applicationName = value; }
        }

        public string CurrentUser
        {
            get;
            private set;
        }

        public string ConnectionName { get; set; }

        private MachineKeySection MachineKey { get; set; }

        private UserRepository UserRepository
        {
            get { return new UserRepository(NHibernateSessionProvider.Instance.CurrentSession); }
        }

        #region Provider settings

        private bool _enablePasswordReset = false;
        public override bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
        }

        private bool _enablePasswordRetrieval = false;
        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }

        private int _maxInvalidPasswordAttempts = 3;
        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }

        private int _minRequiredNonAlphanumericCharacters = 0;
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }

        private int _minRequiredPasswordLength = 5;
        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }

        private int _passwordAttemptWindow = 10;
        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }

        private MembershipPasswordFormat _passwordFormat = MembershipPasswordFormat.Hashed;
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
        }

        private string _passwordStrengthRegularExpression = string.Empty;
        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        private bool _requiresQuestionAndAnswer = false;
        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }

        private bool _requiresUniqueEmail = true;
        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }

        #endregion

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentException("config");

            if (string.IsNullOrEmpty(name))
                name = "MultiTenantMembershipProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Multi-tenant membership provider");
            }

            base.Initialize(name, config);

            string applicationName = GetConfigValue(config["applicationName"], null);

            if (!string.IsNullOrEmpty(applicationName))
                ApplicationName = applicationName;

            _maxInvalidPasswordAttempts = GetConfigValue(config["maxInvalidPasswordAttempts"], _maxInvalidPasswordAttempts);
            _passwordAttemptWindow = GetConfigValue(config["passwordAttemptWindow"], _passwordAttemptWindow);
            _minRequiredNonAlphanumericCharacters = GetConfigValue(config["minRequiredNonAlphanumericCharacters"], _minRequiredNonAlphanumericCharacters);
            _minRequiredPasswordLength = GetConfigValue(config["minRequiredPasswordLength"], _minRequiredPasswordLength);
            _passwordStrengthRegularExpression = GetConfigValue(config["passwordStrengthRegularExpression"], _passwordStrengthRegularExpression);
            _enablePasswordReset = GetConfigValue(config["enablePasswordReset"], _enablePasswordReset);
            _enablePasswordRetrieval = GetConfigValue(config["enablePasswordRetrieval"], _enablePasswordRetrieval);
            _requiresQuestionAndAnswer = GetConfigValue(config["requiresQuestionAndAnswer"], _requiresQuestionAndAnswer);
            _requiresUniqueEmail = GetConfigValue(config["requiresUniqueEmail"], _requiresUniqueEmail);

            string passwordFormat = GetConfigValue(config["passwordFormat"], null);

            if (!string.IsNullOrEmpty(passwordFormat))
                if (!Enum.TryParse(config["passwordFormat"], out _passwordFormat))
                    throw new ProviderException("Password format not supported.");

            string connectionStringName = GetConfigValue(config["connectionStringName"], string.Empty);
            if (string.IsNullOrEmpty(connectionStringName))
                throw new ProviderException("The connection string name is missing or empty.");

            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionStringSettings == null || string.IsNullOrEmpty(connectionStringSettings.ConnectionString))
                throw new ProviderException("The connection string is missing or blank.");

            ConnectionName = connectionStringSettings.ConnectionString;

            // Get encryption and decryption key information from the configuration.
            MachineKey = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");

            if (MachineKey.ValidationKey.ToLower().Contains("autogenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords are not supported with auto-generated keys.");
        }

        #region Provider helpers

        //
        // Helper functions to retrieve config values from the configuration file.
        //

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        private int GetConfigValue(string configValue, int defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return
                Convert.ToInt32(configValue);
        }

        private bool GetConfigValue(string configValue, bool defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return
                Convert.ToBoolean(configValue);
        }

        /// <summary>
        /// Compares password values based on the MembershipPasswordFormat.
        /// </summary>
        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
            }

            if (pass1 == pass2)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        /// </summary>
        public string EncodePassword(string password)
        {
            string encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    encodedPassword = password;
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:

                    if (MachineKey == null)
                        throw new ArgumentNullException("MachineKey", "Cannot hash passwords without a MachineKey. Either define a valid machine key or change the hashing algorithm.");

                    HMACSHA1 hash = new HMACSHA1() { Key = HexToByte(MachineKey.ValidationKey) };
                    encodedPassword =
                      Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }

        /// <summary>
        /// Decrypts or leaves the password clear based on the PasswordFormat.
        /// </summary>
        public string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        /// <summary>
        /// Converts a hexadecimal string to a byte array. Used to convert encryption
        /// key values from the configuration.
        /// </summary>
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        #endregion

        public override string GetUserNameByEmail(string email)
        {
            return UserRepository.GetUserNameByEmail(email);
        }

        public override bool ValidateUser(string username, string password)
        {

            bool isValid = false;

            var portal = PortalProviderManager.Provider.GetCurrentPortal();

            if (portal == null)
                return false;

            var user = UserRepository.GetUserByName(username, portal.Tenant.ID);

            if (user == null)
                return false;

            CurrentUser = user.UserName;

            if (CheckPassword(password, user.Hash))
            {
                if (user.Enabled)
                {
                    isValid = true;

                    //TODO: update last login date time for this user
                }
            }
            else
            {
                //TODO: cduta: log user login failure
            }

            return isValid;
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword))
                return false;

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

            var portal = PortalProviderManager.Provider.GetCurrentPortal();

            try
            {
                UserRepository.UpdatePassword(username, portal.Tenant.ID, EncodePassword(newPassword));
            }
            catch (Exception ex)
            {
                throw new BusinessException("Failed to update the user password.", ex);
            }

            return true;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var portal = PortalProviderManager.Provider.GetCurrentPortal();

            if (portal == null)
                throw new ProviderException("Failed to retrieve user. Unknown portal.");

            if (string.IsNullOrEmpty(username))
                username = CurrentUser;

            User u = UserRepository.GetUserByName(username, portal.Tenant.ID);

            return u == null ? null : new MembershipUser(
                Name, u.UserName, u.ID, u.Email,

                null, null,

                u.Enabled, !u.Enabled,

                DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now
                //TODO: need to read these from database at some point...
                );
        }


        public override MembershipUser CreateUser(string username, string password, string email,
                                                  string passwordQuestion, string passwordAnswer,
                                                  bool isApproved,
                                                  object providerUserKey, // user's unique provider key: usually a GUID
                                                  out MembershipCreateStatus status)
        {

            status = MembershipCreateStatus.ProviderError;

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (_requiresUniqueEmail && !string.IsNullOrEmpty(GetUserNameByEmail(email)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser m = GetUser(username, false);

            if (m == null)
            {
                if (providerUserKey != null && !(providerUserKey is Guid))
                {
                    status = MembershipCreateStatus.InvalidProviderUserKey;
                    return null;
                }

                Tenant t = PortalProviderManager.Provider.GetCurrentPortal().Tenant;
                User u = new User(t, username, email, EncodePassword(password)) { Enabled = isApproved };

                var userID = UserRepository.Insert(u);

                status = userID == Guid.Empty ? MembershipCreateStatus.UserRejected : MembershipCreateStatus.Success;
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }

            return null;
        }

        #region not implemented

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
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

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
