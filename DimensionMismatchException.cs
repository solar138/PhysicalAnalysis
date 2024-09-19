namespace PhysicalAnalysis
{
    [Serializable]
    public class DimensionMismatchException : InvalidOperationException
    {
        public DimensionMismatchException() { }
        public DimensionMismatchException(string message) : base(message) { }
        public DimensionMismatchException(string message, Exception inner) : base(message, inner) { }
    }
}