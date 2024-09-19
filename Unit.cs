using Number = double;

namespace PhysicalAnalysis
{
    public class Unit
    {
        public string Name { get; }
        public string Symbol { get; }
        public Dimension Dimension { get; }
        public Number BaseFactor { get; }
        public Number BaseOffset { get; }

        // SI Base Units
        public static readonly Unit Meter = Dimension.Length.baseUnit;
        public static readonly Unit Gram = Dimension.Mass.baseUnit;
        public static readonly Unit Kelvin = Dimension.Temperature.baseUnit;
        public static readonly Unit Second = Dimension.Time.baseUnit;
        public static readonly Unit Ampere = Dimension.ElectricCurrent.baseUnit;
        public static readonly Unit Dollar = Dimension.Currency.baseUnit;
        public static readonly Unit Mole = Dimension.AmountOfSubstance.baseUnit;
        public static readonly Unit Radian = Dimension.Angle.baseUnit;

        // Other Mass Units
        public static readonly Unit Pound = new("Pound", "lb", Dimension.Mass, 453.59237, 0.0);
        public static readonly Unit Ounce = new("Ounce", "oz", Dimension.Mass, 28.3495231, 0.0);
        public static readonly Unit ShortTon = new("US Ton", "st", Dimension.Mass, 907184.74, 0.0);
        public static readonly Unit MetricTonne = new("Metric Tonne", "t", Dimension.Mass, 1e6, 0.0);
        public static readonly Unit LongTon = new("Imperial Ton", "lt", Dimension.Mass, 1016047.203454, 0.0);
        public static readonly Unit Slug = new("Slug", "slug", Dimension.Mass, 14593.9029, 0.0);
        public static readonly Unit Carat = new("Carat", "ct", Dimension.Mass, 0.2, 0.0);
        public static readonly Unit Grain = new("Grain", "gr", Dimension.Mass, 0.0647989, 0.0);
        public static readonly Unit AtomicMassUnit = new("Atomic Mass Unit", "amu", Dimension.Mass, 1.660538921e-24, 0.0);

        public static readonly ScaledUnit Kilogram = new(Gram, 1000.0);

        // Other Length Units
        public static readonly Unit Inch = new("Inch", "in", Dimension.Length, 0.0254, 0.0);
        public static readonly Unit Foot = new("Foot", "ft", Dimension.Length, 0.3048, 0.0);
        public static readonly Unit Yard = new("Yard", "yd", Dimension.Length, 0.9144, 0.0);
        public static readonly Unit Mile = new("Mile", "mi", Dimension.Length, 1609.34, 0.0);
        public static readonly Unit NauticalMile = new("Nautical Mile", "nmi", Dimension.Length, 1852.0, 0.0);

        // Other Temperature Units
        public static readonly Unit Celsius = new("Celsius", "°C", Dimension.Temperature, 1.0, 273.15);
        public static readonly Unit Fahrenheit = new("Fahrenheit", "°F", Dimension.Temperature, 5.0 / 9.0, 459.67);

        // Other Time Units
        public static readonly Unit Minute = new("Minute", "min", Dimension.Time, 60.0, 0.0);
        public static readonly Unit Hour = new("Hour", "h", Dimension.Time, 3600.0, 0.0);
        public static readonly Unit Day = new("Day", "d", Dimension.Time, 86400.0, 0.0);
        public static readonly Unit Week = new("Week", "w", Dimension.Time, 604800.0, 0.0);
        public static readonly Unit Year = new("Year", "y", Dimension.Time, 31557600.0, 0.0);
        public static readonly Unit Decade = new("Decade", "dec", Dimension.Time, 315576000.0, 0.0);
        public static readonly Unit Century = new("Century", "cent", Dimension.Time, 3155760000.0, 0.0);
        public static readonly Unit Millennium = new("Millennium", "mill", Dimension.Time, 31557600000.0, 0.0);

        // Angle Units
        public static readonly Unit Degree = new("Degree", "deg", Dimension.Angle, Math.PI / 180.0, 0.0);


        public Unit(string name, string symbol, Dimension dimension, Number baseFactor = 1.0, Number baseOffset = 0.0)
        {
            this.Symbol = symbol;
            this.Name = name;
            this.Dimension = dimension;
            this.BaseFactor = baseFactor;
            this.BaseOffset = baseOffset;

            if (this.Dimension.baseUnit == null)
            {
                this.Dimension.baseUnit = this;
                this.BaseFactor = 1.0;
            }

            if (GetType() == typeof(Unit))
                Dimension.symbols.Add(symbol, this);
        }

    }

    public class ScaledUnit(Unit unit, Number factor) : Unit(unit.Name, unit.Symbol, unit.Dimension, unit.BaseFactor, unit.BaseOffset)
    {
        public Number factor = factor;
        public Unit baseUnit = unit;
    }


    [Serializable]
    public class UnitNotFoundException : InvalidOperationException
    {
        public UnitNotFoundException() { }
        public UnitNotFoundException(string message) : base(message) { }
        public UnitNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}