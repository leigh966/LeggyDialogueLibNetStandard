using System;

namespace LeggyDialogueLib
{
    public class BadTreeException : Exception
    {

        public BadTreeException() { }
        public BadTreeException(string message) : base(message) { }

        public BadTreeException(string message, Exception innerException) : base(message, innerException) { }


    }
}
