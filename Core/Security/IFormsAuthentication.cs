﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessAccelerator.Core.Security
{
    public interface IFormsAuthentication
    {
        void SignIn(string userName, bool createPersistentCookie, IEnumerable<string> roles);
        void SignOut();
    }
}
