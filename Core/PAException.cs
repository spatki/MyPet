using System;

namespace ProcessAccelerator.Core
{
    [Serializable]
    public class PAException : Exception
    {
        public PAException(string message)
            : base(message)
        {
        }
    }
}
