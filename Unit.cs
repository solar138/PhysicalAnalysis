﻿using Number = double;

namespace PhysicalAnalysis
{
    /// <summary>
    ///     A standardized interval to measure a dimension with.
    /// </summary>
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
        public static readonly Unit AstronomicalUnit = new("Astronomical Unit", "au", Dimension.Length, 149597870700, 0.0);
        public static readonly Unit LightYear = new("Light Year", "ly", Dimension.Length, 9.4607304725808e15, 0.0);
        public static readonly Unit Parsec = new("Parsec", "pc", Dimension.Length, 3.09e16, 0.0);

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

        // Derived Units
        public static readonly DerivedUnit Newton = new ("Newton", "N", "kg m s^-2");
        public static readonly DerivedUnit Joule = new("Joule", "J", "kg m^2 s^-2");
        public static readonly DerivedUnit Coulomb = new("Coulomb", "C", "A s");
        public static readonly DerivedUnit Hertz = new("Hertz", "Hz", "s^-1");
        public static readonly DerivedUnit Pascal = new("Pascal", "Pa", "N m^-2");
        public static readonly DerivedUnit Watt = new("Watt", "W", "J s^-1");
        public static readonly DerivedUnit Volt = new("Volt", "V", "J C^-1");
        public static readonly DerivedUnit Farad = new("Farad", "F", "C V^-1");
        public static readonly DerivedUnit Ohm = new("Ohm", "Ω", "V A^-1");
        public static readonly DerivedUnit Siemens = new("Siemens", "S", "Ω^-1");
        public static readonly DerivedUnit Weber = new("Weber", "Wb", "V s");
        public static readonly DerivedUnit Tesla = new("Tesla", "T", "Wb m^-2");
        public static readonly DerivedUnit Henry = new("Henry", "H", "Wb A^-1");
        public static readonly DerivedUnit Gray = new("Gray", "Gy", "J kg^-1");
        public static readonly DerivedUnit Liter = new("Liter", "L", "0.001 m^3");

        // Physical constants
        public static readonly DerivedUnit SpeedOfLight = new("Speed Of Light", "c", "299792458 m s^-1");
        public static readonly DerivedUnit ReducedPlanck = new("Reduced Planck Constant", "ℏ", "1.054571817e-34 J s");
        public static readonly DerivedUnit Planck = new("Planck Constant", "h", "6.62607015e-34 J s");
        public static readonly DerivedUnit Gravitation = new("Gravitational Constant", "G", "6.6743015e-11 N m^2 kg^-2");


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
    public class DerivedUnit
    {
        public string name;
        public string symbol;
        public Quantity factor;
        public DerivedUnit(string name, string symbol, string dimensions)
        {
            if (!(dimensions[0] >= '0' && dimensions[0] <= '9'))
            {
                dimensions = "1 " + dimensions;
            }

            this.name = name;
            this.symbol = symbol;
            this.factor = new Quantity(dimensions);
            Dimension.compositeSymbols.Add(symbol, this);
        }
    }


    [Serializable]
    public class UnitNotFoundException : InvalidOperationException
    {
        public UnitNotFoundException() { }
        public UnitNotFoundException(string message) : base(message) { }
        public UnitNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}