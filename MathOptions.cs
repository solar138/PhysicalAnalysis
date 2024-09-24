using Number = double;

namespace PhysicalAnalysis
{
    /// <summary>
    ///     Static class with configurable display and unit options.
    /// </summary>
    public static class MathOptions
    {
        public enum AngleUnit { Radian, Degree }
        public enum VectorSystem { Cartesian, Polar }

        public static AngleUnit angleUnit = AngleUnit.Radian;
        public static VectorSystem vectorSystem = VectorSystem.Cartesian;

        /// <summary>
        ///      Whether to display grams as kilograms. This only affects .ToString(), the underlying base unit for calculations is still grams.
        /// </summary>
        public static bool baseKilograms = true;

        /// <summary>
        ///     Whether to show common composite units such as Newtons or Joules as a single unit.
        /// </summary>
        public static bool consolidateUnits = true;

        /// <summary>
        ///     The priority order for which units are consolidated first, if multiple are available.
        /// </summary>
        public static List<CompositeUnit> consolidatePriority = [];

        /// <summary>
        ///     Converts an angle to radians if useRadians is true.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Number ConvertAngle(Number angle)
        {
            return angleUnit == AngleUnit.Radian ? angle : angle * Math.PI / 180;
        }

        /// <summary>
        ///     Converts an angle to degrees if useRadians is false.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Number ReverseConvertAngle(Number angle)
        {
            return angleUnit == AngleUnit.Radian ? angle : angle * 180 / Math.PI;
        }
    }
}