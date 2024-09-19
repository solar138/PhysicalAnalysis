
namespace PhysicalAnalysis
{
    [Serializable]
    public class UnitMismatchException : InvalidOperationException
    {
        public UnitMismatchException() { }
        public UnitMismatchException(string message) : base(message) { }
        public UnitMismatchException(string message, Exception inner) : base(message, inner) { }
    }
}