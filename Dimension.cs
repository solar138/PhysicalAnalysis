namespace PhysicalAnalysis
{
    /// <summary>
    ///     A physical measured value, often expressed in multiple units.
    /// </summary>
    public class Dimension
    {
        /// <summary>
        ///     The name of the dimension.
        /// </summary>
        public string name;

        /// <summary>
        ///     The base unit set at "1" for the internal representation. 
        /// </summary>
        public Unit baseUnit;

        public List<Unit> units = [];

        public static readonly Dictionary<string, Unit> symbols = [];
        public static readonly Dictionary<string, DerivedUnit> compositeSymbols = [];

        public static readonly Dimension Length = new("Length", "meter", "m");
        public static readonly Dimension Mass = new("Mass", "gram", "g");
        public static readonly Dimension Temperature = new("Temperature", "kelvin", "K");
        public static readonly Dimension Time = new("Time", "second", "s");
        public static readonly Dimension ElectricCurrent = new("Electric Current", "ampere", "A");
        public static readonly Dimension Currency = new("Currency", "USD", "$");
        public static readonly Dimension AmountOfSubstance = new("Amount of Substance", "mole", "mol");
        public static readonly Dimension Angle = new("Angle", "radian", "rad");

        /// <summary>
        ///     Create a new dimension from a name as well as a base unit with a symbol and name.
        /// </summary>
        /// <param name="name"> Name of the dimension. </param>
        /// <param name="baseUnit"> Name of the new base unit. </param>
        /// <param name="baseUnitSymbol"> Symbol of the new base unit. </param>
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