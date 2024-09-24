using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

using Number = double;

namespace PhysicalAnalysis
{
    /// <summary>
    ///     A set of units and powers representing a specific measured quantity.
    /// </summary>
    public struct QuantityDimension
    {
        private Dictionary<Dimension, (Unit, Number)> dimension = [];

        /// <summary>
        ///     Returns whether the dimension is specified.
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public readonly bool HasDimension(Dimension dimension) => this.dimension.ContainsKey(dimension);

        /// <summary>
        ///     Returns whether the dimension is specified.
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public readonly bool HasDimension(Dimension dimension, double power) => this.dimension.TryGetValue(dimension, out var value) && value.Item2 == power;
        /// <summary>
        ///     Returns whether the dimension is specified.
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public readonly (Unit?, Number) GetUnits(Dimension dimension) => this.dimension.GetValueOrDefault(dimension, (null, 0.0));

        public QuantityDimension()
        {
        }

        /// <summary>
        ///     Copies an existing QuantityDimension.
        /// </summary>
        /// <param name="template"> Template to copy. </param>
        public QuantityDimension(QuantityDimension template)
        {
            foreach (var (dimension, (unit, power)) in template.dimension)
            {
                this.dimension[dimension] = (unit, power);
            }
        }

        /// <summary>
        ///     Creates a QuantityDimension from a list of units.
        /// </summary>
        /// <param name="units"> Units to add. </param>
        /// <exception cref="UnitMismatchException"> A dimension can only have one unit. </exception>
        public QuantityDimension(params Unit[] units)
        {
            foreach (var unit in units)
            {
                if (dimension.TryGetValue(unit.Dimension, out (Unit, Number) value))
                {
                    if (dimension[unit.Dimension].Item1 != unit)
                    {
                        throw new UnitMismatchException("One dimension can only have one unit.");
                    }
                    dimension[unit.Dimension] = (unit, value.Item2 + 1.0);
                }
                else
                {
                    dimension[unit.Dimension] = (unit, 1.0);
                }
            }
        }

        /// <summary>
        ///     Creates a QuantityDimension from a string of units. Using "symbol^power" syntax and spaces separating dimensions.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="UnitNotFoundException"></exception>
        public static QuantityDimension Parse(string str)
        {
            var dimensions = new QuantityDimension();
            var values = str.Split(' ');

            for (var i = 0; i < values.Length; i++)
            {
                var parts = values[i].Split('^');

                if (Dimension.symbols.TryGetValue(parts[0], out var unit))
                {
                    dimensions.dimension[unit.Dimension] = (unit, parts.Length == 1 ? (Number)1.0 : Number.Parse(parts[1]));
                }
                else
                {
                    var symbol = new string(parts[0].Skip(1).ToArray());

                    if (Dimension.symbols.TryGetValue(symbol, out unit))
                    {
                        dimensions += (unit, parts.Length == 1 ? (Number)1.0 : Number.Parse(parts[1]));
                    }
                    else
                    {
                        throw new UnitNotFoundException("Unit " + parts[0] + " does not exist.");
                    }
                }
            }
            return dimensions;
        }

        /// <summary>
        ///     Converts the QuantityDimension to a string, following the MathOptions.consolidateUnits setting.
        /// </summary>
        /// <returns></returns>
        public override readonly string ToString()
        {
            if (MathOptions.consolidateUnits)
            {
                foreach (var (symbol, unit) in Dimension.compositeSymbols)
                {
                    var remainder = this - unit.factor.dimension;

                    if (remainder.dimension.Count <= this.dimension.Count)
                    {
                        return " " + unit.symbol + remainder.ToStringRaw();
                    }
                }
            }
            if (MathOptions.baseKilograms)
            {
                return dimension.Aggregate("", (current, kv) => current + " " + (kv.Value.Item1 == Unit.Gram ? "k" : "") + (kv.Value.Item2 == 1 ? kv.Value.Item1.Symbol : $"{kv.Value.Item1.Symbol}^{kv.Value.Item2}"));
            }
            return dimension.Aggregate("", (current, kv) => current + " " + (kv.Value.Item2 == 1 ? kv.Value.Item1.Symbol : $"{kv.Value.Item1.Symbol}^{kv.Value.Item2}"));
        }

        /// <summary>
        ///     Converts the QuantityDimension to a string without consolidating units, only showing the base units.
        /// </summary>
        /// <returns></returns>
        public readonly string ToStringRaw()
        {
            if (MathOptions.baseKilograms)
            {
                return dimension.Aggregate("", (current, kv) => kv.Value.Item2 == 0.0 ? "" : current + " " + (kv.Value.Item1 == Unit.Gram ? "k" : "") + (kv.Value.Item2 == 1 ? kv.Value.Item1.Symbol : $"{kv.Value.Item1.Symbol}^{kv.Value.Item2}"));
            }
            return dimension.Aggregate("", (current, kv) => kv.Value.Item2 == 0.0 ? "" : current + " " + (kv.Value.Item2 == 1 ? kv.Value.Item1.Symbol : $"{kv.Value.Item1.Symbol}^{kv.Value.Item2}"));
        }

        /// <summary>
        ///     Subtracts dimensions together. Equivalent to dividing quantities.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static QuantityDimension operator -(QuantityDimension lhs, QuantityDimension rhs)
        {
            var d = new QuantityDimension
            {
                dimension = new Dictionary<Dimension, (Unit, Number)>(lhs.dimension)
            };

            foreach (var (dimension, (unit, power)) in rhs.dimension)
            {
                if (rhs.dimension[dimension].Item1 != unit)
                {
                    throw new UnitMismatchException("Cannot subtract dimensions with different units.");
                }
                var newPower = d.dimension.GetValueOrDefault(dimension, (unit, 0.0)).Item2 - power;

                if (newPower == 0.0)
                {
                    d.dimension.Remove(dimension);
                }
                else
                {
                    d.dimension[dimension] = (unit, newPower);
                }
            }
            return d;
        }

        /// <summary>
        ///     Adds dimensions together. Equivalent to multiplying quantities.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static QuantityDimension operator +(QuantityDimension lhs, QuantityDimension rhs)
        {
            var d = new QuantityDimension
            {
                dimension = new Dictionary<Dimension, (Unit, Number)>(lhs.dimension)
            };

            foreach (var (dimension, (unit, power)) in rhs.dimension)
            {
                if (rhs.dimension[dimension].Item1 != unit)
                {
                    throw new UnitMismatchException("Cannot add dimensions with different units.");
                }

                var newPower = d.dimension.TryGetValue(dimension, out var v) ? v.Item2 + power : power;

                if (newPower == 0.0)
                {
                    d.dimension.Remove(dimension);
                }
                else
                {
                    d.dimension[dimension] = (unit, newPower);
                }
            }
            return d;
        }


        /// <summary>
        ///     Multiplies dimensions together. Equivalent to raising the power of quantities.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static QuantityDimension operator *(QuantityDimension lhs, Number rhs)
        {
            var d = new QuantityDimension
            {
                dimension = new Dictionary<Dimension, (Unit, Number)>(lhs.dimension)
            };

            if (rhs != 0)
            {
                foreach (var (dimension, (unit, power)) in lhs.dimension)
                {
                    d.dimension[dimension] = (unit, power * rhs);
                }
            }
            return d;
        }
        /// <summary>
        ///     Divides dimensions together. Equivalent to raising the inverse power of quantities.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static QuantityDimension operator /(QuantityDimension lhs, Number rhs)
        {
            var d = new QuantityDimension
            {
                dimension = new Dictionary<Dimension, (Unit, Number)>(lhs.dimension)
            };

            foreach (var (dimension, (unit, power)) in lhs.dimension)
            {
                d.dimension[dimension] = (unit, power / rhs);
            }
            return d;
        }

        /// <summary>
        ///     Adds a dimension to the <see cref="QuantityDimension"/>.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static QuantityDimension operator +(QuantityDimension lhs, (Unit, Number) rhs)
        {
            lhs.dimension[rhs.Item1.Dimension] = (rhs.Item1, rhs.Item2 + lhs.dimension.GetValueOrDefault(rhs.Item1.Dimension, (rhs.Item1, 0.0)).Item2);

            if (lhs.dimension[rhs.Item1.Dimension].Item2 == 0.0)
            {
                lhs.dimension.Remove(rhs.Item1.Dimension);
            }

            return lhs;
        }
        /// <summary>
        ///     Adds a dimension to the <see cref="QuantityDimension"/>.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static QuantityDimension operator +(QuantityDimension lhs, Unit rhs)
        {
            lhs.dimension[rhs.Dimension] = (rhs, 1.0 + lhs.dimension.GetValueOrDefault(rhs.Dimension, (rhs, 0.0)).Item2);

            if (lhs.dimension[rhs.Dimension].Item2 == 0.0)
            {
                lhs.dimension.Remove(rhs.Dimension);
            }

            return lhs;
        }

        /// <summary>
        ///     Checks if two values have the exact same dimensions and powers. Will return true even if units are different.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(QuantityDimension lhs, QuantityDimension rhs)
        {
            if (lhs.dimension.Count != rhs.dimension.Count)
            {
                return false;
            }
            foreach (var (dimension, (unit, power)) in rhs.dimension)
            {
                if (!lhs.dimension.TryGetValue(dimension, out var value) || value.Item1.Dimension != unit.Dimension || value.Item2 != power)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        ///     Checks if two values do not have the exact same dimensions and powers.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(QuantityDimension lhs, QuantityDimension rhs)
        {
            if (lhs.dimension.Count != rhs.dimension.Count)
            {
                return true;
            }
            foreach (var (dimension, (unit, power)) in rhs.dimension)
            {
                if (!lhs.dimension.TryGetValue(dimension, out var value) || value.Item1.Dimension != unit.Dimension || value.Item2 != power)
                {
                    return true;
                }
            }
            return false;
        }
        public static readonly Dictionary<char, int> prefixToExp = new() { { 'k', 3 }, { 'M', 6 }, { 'G', 9 }, { 'T', 12 }, { 'P', 15 }, { 'E', 18 }, { 'Z', 21 }, { 'Y', 24 }, { 'a', -18 }, { 'f', -15 }, { 'p', -12 }, { 'n', -9 }, { 'u', -6 }, { 'm', -3 }, { 'c', -2 }, { 'd', -1 } };
        public static readonly Dictionary<int, char> expToPrefix = new() { { 3, 'k' }, { 6, 'M' }, { 9, 'G' }, { 12, 'T' }, { 15, 'P' }, { 18, 'E' }, { 21, 'Z' }, { 24, 'Y' }, { -18, 'a' }, { -15, 'f' }, { -12, 'p' }, { -9, 'n' }, { -6, 'u' }, { -3, 'm' }, { -2, 'c' }, { -1, 'd' } };

        /// <summary>
        ///     Reads a string with an SI prefix such as "kg" or "mm" and returns the factor.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidSIPrefixException"></exception>
        public static Number ReadSIPrefix(string str)
        {
            if (str.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(str), "SI prefix cannot be empty.");
            }
            if (prefixToExp.TryGetValue(str[0], out int value))
                return Math.Pow(10, value);
            throw new InvalidSIPrefixException("Invalid SI prefix: " + str);
        }

        /// <summary>
        ///     Reads an SI prefix such as k-ilo or m-illi and returns the factor.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="InvalidSIPrefixException"></exception>
        public static Number ReadSIPrefix(char str)
        {
            if (prefixToExp.TryGetValue(str, out int value))
                return Math.Pow(10, value);
            throw new InvalidSIPrefixException("Invalid SI prefix: " + str);
        }

        /// <summary>
        ///     Gets the closest SI prefix for the given value up to 3 decimal places.
        ///     Ex: 1234 -> 1.234k, 123456 -> 123.456k, 1234567 -> 1.234M
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string WriteSIPrefix(Number value)
        {
            var exponent = 0.3010299956639812 * ((Unsafe.BitCast<Number, long>(value) >> 52 & 0x7ffL) - 1023);
            var index = (int)exponent;

            if (exponent > 3 || exponent < -3)
            {
                index = (int)(exponent / 3.0) * 3;
            }

            return (value / Math.Pow(10, index)).ToString("000.000") + " " + expToPrefix[index];
        }

        /// <summary>
        ///     Converts the units in target to match the template if the dimension is present in both.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Quantity MatchUnits(Quantity template, Quantity target)
        {
            var factor = 1.0;
            var dimensions = new QuantityDimension();

            foreach (var (unit, power) in target.dimension.dimension.Values)
            {
                if (template.dimension.dimension.TryGetValue(unit.Dimension, out var value))
                {
                    factor *= Math.Pow(unit.BaseFactor, power) / Math.Pow(value.Item1.BaseFactor, value.Item2);
                    dimensions += value;
                }
                else
                {

                    dimensions += unit;
                }
            }

            return new Quantity(target.value * factor, dimensions);
        }

        /// <summary>
        ///     Converts the units in target to match the array.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        /// <exception cref="UnitMismatchException"></exception>
        public static Quantity MatchUnits(Quantity target, params Unit[] units)
        {
            var factor = 1.0;
            var dimensions = new QuantityDimension();
            var template = new Dictionary<Dimension, (Unit, Number)>();

            foreach (var unit in units)
            {
                if (template.TryGetValue(unit.Dimension, out (Unit, Number) value))
                {
                    if (template[unit.Dimension].Item1 != unit)
                    {
                        throw new UnitMismatchException("One dimension can only have one unit.");
                    }
                    template[unit.Dimension] = (unit, value.Item2 + 1.0);
                }
                else
                {
                    template[unit.Dimension] = (unit, 1.0);
                }
            }
            foreach (var (unit, power) in target.dimension.dimension.Values)
            {
                if (template.TryGetValue(unit.Dimension, out var value))
                {
                    factor *= Math.Pow(unit.BaseFactor, power) / Math.Pow(value.Item1.BaseFactor, value.Item2);
                    dimensions += value;
                }
                else
                {

                    dimensions += unit;
                }
            }

            return new Quantity(target.value * factor, dimensions);
        }

        /// <summary>
        ///     Converts one unit of a quantity to another unit.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <returns> Returns a dimensioned value. Use ApplyUnit instead if you need a dimensionless value.</returns>
        public static Quantity ConvertUnit(Quantity target, Unit unit)
        {
            var oldUnit = target.dimension.GetUnits(unit.Dimension).Item1;
            if (oldUnit == null || oldUnit == unit)
            {
                return target;
            }
            return new Quantity(target.value * (oldUnit.BaseFactor / unit.BaseFactor), new QuantityDimension(target.dimension).ReplaceUnit(unit));
        }

        /// <summary>
        ///     Converts one unit of a quantity to another unit.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="unit"></param>
        /// <returns> Returns a dimensionless value. Use ConvertUnit instead if you need a dimensioned value.</returns>
        public static double ApplyUnit(Quantity target, Unit unit)
        {
            var oldUnit = target.dimension.GetUnits(unit.Dimension).Item1;
            if (oldUnit == null || oldUnit == unit)
            {
                return target.value;
            }
            return target.value * (oldUnit.BaseFactor / unit.BaseFactor);
        }

        /// <summary>
        ///     Replaces an existing unit of the same dimension with another.
        /// </summary>
        /// <param name="unit"></param>
        public readonly QuantityDimension ReplaceUnit(Unit unit, double power = 1.0)
        {
            dimension[unit.Dimension] = (unit, power);

            return this;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is QuantityDimension dimension &&
                   EqualityComparer<Dictionary<Dimension, (Unit, double)>>.Default.Equals(this.dimension, dimension.dimension);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(dimension);
        }
    }
}