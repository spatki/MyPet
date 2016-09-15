using System.ComponentModel.DataAnnotations;

namespace ProcessAccelerator.WebUI.Dto
{
    public class StrLenAttribute : StringLengthAttribute
    {
        public StrLenAttribute(int maximumLength)
            : base(maximumLength)
        {
            ErrorMessage = "{0} may not be longer than {1} characters";
        }
    }
}