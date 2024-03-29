﻿using System.Configuration.Provider;

namespace MVCFramework.Business.Providers.Mail
{
    public class MailServiceProviderCollection : ProviderCollection
    {
        new public MailServiceProviderBase this[string name]
        {
            get { return (MailServiceProviderBase)base[name]; }
        }
    }
}
