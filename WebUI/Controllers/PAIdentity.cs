using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using ProcessAccelerator.Data;
using ProcessAccelerator.Core.Model;

namespace ProcessAccelerator.WebUI.Controllers
{
    public class PAIdentity : MarshalByRefObject, System.Security.Principal.IIdentity
    {
        #region Private variables

        private readonly FormsAuthenticationTicket _ticket;
        private readonly int? _clientID;
        private int _role;
        private readonly string _friendlyName;
        private string _roleName;
        private readonly bool _IsAdmin;
        private readonly bool _IsGuest;
        private readonly bool _multipleRoles;
        private readonly string _mode;
        private readonly string _roleList;

        #endregion

        #region Constuctor

        /// <summary>
        /// Initialize the identity with the FormsAuthenticationTicket in order
        /// to read custom userdata.
        /// </summary>
        /// <param name="ticket"></param>
        public PAIdentity(FormsAuthenticationTicket ticket)
        {
            _ticket = ticket;

            var _roles = ticket.UserData.Split(',');    // Get a list of roles
            int clientID;
            if (int.TryParse(_roles[0],out clientID))
            {
                _clientID = clientID;
                _friendlyName = _roles[1];
                _IsAdmin = bool.Parse(_roles[2]);
                _IsGuest = bool.Parse(_roles[3]);
                _mode = _roles[4];
                _roleName = _roles[5];
                _role = int.Parse(_roles[6]);
                int roleid;
                int.TryParse(_roles[7], out roleid);
                if (roleid > 1) 
                { 
                    _multipleRoles = true;
                    _roleList = "0";
                    for (var i = 8; i < _roles.Count(); i++)
                    {
                        _roleList = _roleList + "," + _roles[i];
                    }
                }
                else { 
                    _multipleRoles = false;
                    _roleList = "";
                }

            }
            else {
                _clientID = null;  // Consider the first element as client id
                _friendlyName = "";
                _IsAdmin = false;
                _IsGuest = false;
                _mode = "X";
                _roleName = "";
                _role = 0;
                _multipleRoles = false;
            }
            
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the AuthenticationType being used.
        /// </summary>
        public string AuthenticationType
        {
            get { return "Forms"; }
        }

        /// <summary>
        /// Gets a flag indicating if the user is Authenticated or not.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the user name associated with the FormsAuthenticationTicket.
        /// </summary>
        public string Name
        {
            get { return _ticket.Name; }
        }

        public string roleList
        {
            get { return _roleList; }
        }
        /// <summary>
        /// Gets the FormsAuthenticationTicket associated with this User.
        /// </summary>
        public FormsAuthenticationTicket Ticket
        {
            get { return _ticket; }
        }

        /// <summary>
        /// Gets the user's unique identifier stored in the FormsAuthenticationTicket.
        /// </summary>
        public int? clientID
        {
            get
            {
                return _clientID;
            }
        }

        public string friendlyName
        {
            get
            {
                return _friendlyName;
            }
        }

        public bool IsAdmin()
        {
            return _IsAdmin;
        }

        public bool IsGuest()
        {
            return _IsGuest;
        }

        public string roleName
        {
            get
            {
                return _roleName;
            }
            set
            {
                _roleName = roleName;
            }
        }

        public int role
        {
            get
            {
                return _role;
            }
            set
            {
                _role = role;
            }
        }

        public bool multipleRoles
        {
            get
            {
                return _multipleRoles;
            }
        }

        public string mode()
        {
            switch (_mode)
            {
                case "S":  // Sys acess
                    return "Sys";
                case "O":  // Access configured through org level
                    return "Org";
                case "F":  // Access configured through org level
                    return "Admin";
                case "X":
                    return "NA";    // No roles configured
                default:
                    return "NA";
            }
        }

        #endregion
    }
}
