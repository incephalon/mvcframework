using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Web;
using MVCFramework.Business.Providers.NHibernateSession;
using MVCFramework.Business.Providers.Portal;
using MVCFramework.Business.Repository.Entities;

namespace MVCFramework.Business.Providers.Membership
{
    public class MultiTenantRoleProvider : System.Web.Security.RoleProvider
    {
        string _applicationName = null;
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
            set
            {
                _applicationName = value;
            }
        }

        public string ConnectionName { get; set; }
        
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "MultiTenantRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Multi-tenant role provider");
            }

            base.Initialize(name, config);

            string applicationName = GetConfigValue(config["applicationName"], null);

            if (!string.IsNullOrEmpty(applicationName))
                ApplicationName = applicationName;

            string connectionStringName = GetConfigValue(config["connectionStringName"], string.Empty);
            if (string.IsNullOrEmpty(connectionStringName))
                throw new ProviderException("The connection string name is missing or empty.");

            ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (ConnectionStringSettings == null || string.IsNullOrEmpty(ConnectionStringSettings.ConnectionString))
                throw new ProviderException("The connection string is missing or blank.");
        }

        #region Provider helpers

        //
        // Helper functions to retrieve config values from the configuration file.
        //

        private static string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        #endregion

        private RoleRepository RoleRepository
        {
            get { return new RoleRepository(NHibernateSessionProvider.Instance.CurrentSession); }

        }
        public override string[] GetRolesForUser(string username)
        {
            //TODO: we should cache user roles per portal once he logs in, and remove them once he logs out

            return RoleRepository.GetUserRoles(username, PortalProviderManager.Provider.GetCurrentPortal().Tenant.ID)
                .ToArray();
        }

        public override bool IsUserInRole(string username, string rolename)
        {
            return RoleRepository.IsUserInRole(username, PortalProviderManager.Provider.GetCurrentPortal().Tenant.ID, rolename);
        }

        #region not implemented

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}