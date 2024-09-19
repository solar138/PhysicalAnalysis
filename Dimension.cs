namespace PhysicalAnalysis
{
    public class Dimension
    {
        public string name;
        public Unit baseUnit;

        public List<Unit> units = [];

        public static readonly Dictionary<string, Unit> symbols = [];

        public static readonly Dimension Length = new("Length", "meter", "m");
        public static readonly Dimension Mass = new("Mass", "gram", "g");
        public static readonly Dimension Temperature = new("Temperature", "kelvin", "K");
        public static readonly Dimension Time = new("Time", "second", "s");
        public static readonly Dimension ElectricCurrent = new("Electric Current", "ampere", "A");
        public static readonly Dimension Currency = new("Currency", "USD", "$");
        public static readonly Dimension AmountOfSubstance = new("Amount of Substance", "mole", "mol");
        public static readonly Dimension Angle = new("Angle", "radian", "rad");

        public Dimension(string name, string baseUnit, string baseUnitSymbol)
        {
            this.name = name;
            this.baseUnit = new Unit(baseUnit, baseUnitSymbol, this);
        }

        static Dimension()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Unit).TypeHandle);
        }
    }
}