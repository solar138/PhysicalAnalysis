using Number = double;

namespace PhysicalAnalysis
{
    public static class MathOptions
    {
        public enum AngleUnit { Radian, Degree }
        public enum VectorSystem { Cartesian, Polar }

        public static AngleUnit angleUnit = AngleUnit.Radian;
        public static VectorSystem vectorSystem = VectorSystem.Cartesian;
        public static bool baseKilograms = true;

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