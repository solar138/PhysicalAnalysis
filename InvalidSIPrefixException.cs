namespace PhysicalAnalysis
{
    [Serializable]
    public class InvalidSIPrefixException : InvalidOperationException
    {
        public InvalidSIPrefixException() { }
        public InvalidSIPrefixException(string message) : base(message) { }
        public InvalidSIPrefixException(string message, Exception inner) : base(message, inner) { }
    }
}