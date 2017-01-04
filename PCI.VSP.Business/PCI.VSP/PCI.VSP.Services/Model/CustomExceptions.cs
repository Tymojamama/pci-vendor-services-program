using System;

namespace PCI.VSP.Services.Model.CustomExceptions
{
    public class VspServiceLayerException : System.ApplicationException
    {
        private const String _message = "An exception has occurred during a service request.";

        internal VspServiceLayerException()
            : base(_message)
        {
        }

        internal VspServiceLayerException(String message)
            : base(String.IsNullOrEmpty(message) ? _message : message)
        {
        }

        internal VspServiceLayerException(Exception innerException)
            : base(_message, innerException)
        {
        }

        internal VspServiceLayerException(String message, Exception innerException)
            : base(String.IsNullOrEmpty(message) ? _message : message, innerException)
        {
        }
    }

    public class InvalidCredentialsException : VspServiceLayerException
    {
        private const string _message = "The credentials presented to the VSP service are invalid.";
        internal InvalidCredentialsException()
            : base(_message)
        {
        }
    }


}