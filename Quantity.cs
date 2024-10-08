using System.Text.RegularExpressions;

using Number = double;

namespace PhysicalAnalysis
{
    public partial struct Quantity
    {
        public Number value;

        public QuantityDimension dimension = new();

        /// <summary>
        ///     Creates a Quantity from a number and a dimension.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dimension"></param>
        public Quantity(Number value, QuantityDimension dimension)
        {
            this.value = value;
            this.dimension = dimension;
        }
        /// <summary>
        ///     Creates a Quantity from a string of units. Using "symbol^power" syntax and spaces separating dimensions.
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="UnitNotFoundException"> A unit was not found. </exception>
        public Quantity(string str)
        {
            var values = str.Split(' ');

            var dimensions = new QuantityDimension();
            var result = QuantityLookupRegex().Match(values[0]);

            var number = result.Groups[1].Value;
            var letter = result.Groups[2].Value;

            if (!string.IsNullOrEmpty(letter))
            {
                values[0] = letter;
                values = values.Prepend(number).ToArray();
            }

            value = Number.Parse(values[0]);

            for (var i = 1; i < values.Length; i++)
            {
                var parts = values[i].Split('^');

                if (Dimension.symbols.TryGetValue(parts[0], out var unit))
                {
                    dimensions += (unit, parts.Length == 1 ? (Number)1.0 : Number.Parse(parts[1]));
                }
                else if (Dimension.compositeSymbols.TryGetValue(parts[0], out var s))
                {
                    value *= s.factor.value;
                    dimensions += s.factor.dimension;
                } else
                {
                    var symbol = new string(parts[0].Skip(1).ToArray());
                    var prefix = parts[0][0];

                    value *= QuantityDimension.ReadSIPrefix(prefix);

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

            this.dimension = dimensions;
        }
        /// <summary>
        ///     Creates a quantity from a number and a list of units.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units"></param>
        public Quantity(Number value, params Unit[] units)
        {
            foreach (var unit in units)
            {
                var u = unit;
                if (unit is ScaledUnit scaledUnit)
                {
                    u = scaledUnit.baseUnit;
                    value *= scaledUnit.factor;
                }
                dimension += (u, 1.0);
            }
            this.value = value;
        }

        /// <summary>
        ///     Adds two quantities. Both quantities must have the same dimension.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        /// <exception cref="DimensionMismatchException"></exception>
        public static Quantity operator +(Quantity lhs, Quantity rhs)
        {
            if (!CompareDimensions(lhs, rhs))
            {
                throw new DimensionMismatchException("Cannot add quantities with different dimensions");
            }

            rhs = QuantityDimension.MatchUnits(lhs, rhs);
            rhs.value += lhs.value;

            return rhs;
        }

        /// <summary>
        ///     Subtracts two quantities. Both quantities must have the same dimension.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        /// <exception cref="DimensionMismatchException"></exception>
        public static Quantity operator -(Quantity lhs, Quantity rhs)
        {
            if (!CompareDimensions(lhs, rhs))
            {
                throw new DimensionMismatchException("Cannot subtract quantities with different dimensions");
            }

            rhs = QuantityDimension.MatchUnits(lhs, rhs);
            rhs.value = lhs.value - rhs.value;

            return rhs;
        }

        /// <summary>
        ///     Multiplies two quantities.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        /// <exception cref="DimensionMismatchException"></exception>
        public static Quantity operator *(Quantity lhs, Quantity rhs)
        {
            return new Quantity(lhs.value * QuantityDimension.MatchUnits(lhs, rhs).value, lhs.dimension + rhs.dimension);
        }

        /// <summary>
        ///     Multiplies two quantities.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        /// <exception cref="DimensionMismatchException"></exception>
        public static Quantity operator /(Quantity lhs, Quantity rhs)
        {
            return new Quantity(lhs.value / QuantityDimension.MatchUnits(lhs, rhs).value, lhs.dimension - rhs.dimension);
        }

        /// <summary>
        ///     Returns the string representation of the quantity.
        /// </summary>
        /// <returns></returns>
        public override readonly string ToString()
        {
            var value = this.value;
            if (!MathOptions.baseKilograms)
            {
                var units = dimension.ToStringQuantified(ref value);
                return $"{value}{units}";
            }

            var (unit, power) = dimension.GetUnits(Dimension.Mass);

            if (unit == Unit.Gram)
            {
                value *= Math.Pow(0.001, power);

                var units = dimension.ToStringQuantified(ref value);
                return $"{value}{units}";
            }
            else
            {
                var units = dimension.ToStringQuantified(ref value);
                return $"{value}{units}";
            }
        }

        public readonly string ToStringRaw()
        {
            var value = this.value;
            if (!MathOptions.baseKilograms)
            {
                var units = dimension.ToStringRaw();
                return $"{value}{units}";
            }

            var (unit, power) = dimension.GetUnits(Dimension.Mass);

            if (unit == Unit.Gram)
            {
                value *= Math.Pow(0.001, power);

                var units = dimension.ToStringRaw();
                return $"{value}{units}";
            }
            else
            {
                var units = dimension.ToStringRaw();
                return $"{value}{units}";
            }
        }

        /// <summary>
        ///     Returns the string representation of the quantity's units with respect to the scale of the dimension in consolidating units.
        /// </summary>
        /// <returns></returns>
        public readonly string ToStringDimensions()
        {
            var value = this.value;

            return value + dimension.ToStringQuantified(ref value);
        }

        /// <summary>
        ///     Returns true if both quantities have the same dimension.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool CompareDimensions(Quantity lhs, Quantity rhs)
        {
            return lhs.dimension == rhs.dimension;
        }

        /// <summary>
        ///     Returns the cosine of this quantity.
        /// </summary>
        /// <exception cref="DimensionMismatchException"> Cosine only works on angles. </exception>
        /// <returns> The cosine. </returns>
        public readonly Quantity Cos()
        {
            if (!dimension.HasDimension(Dimension.Angle, 1.0))
            {
                throw new DimensionMismatchException("Cannot take the cosine of a quantity without angle^1.");
            }
            return new Quantity(Math.Cos(QuantityDimension.ApplyUnit(this, Unit.Radian)));
        }

        /// <summary>
        ///     Returns the sine of this quantity.
        /// </summary>
        /// <exception cref="DimensionMismatchException"> Sine only works on angles. </exception>
        /// <returns> The sine. </returns>
        public readonly Quantity Sin()
        {
            if (!dimension.HasDimension(Dimension.Angle, 1.0))
            {
                throw new DimensionMismatchException("Cannot take the sine of a quantity without angle^1.");
            }
            return new Quantity(Math.Sin(QuantityDimension.ApplyUnit(this, Unit.Radian)));
        }

        /// <summary>
        ///     Returns the tangent of this quantity.
        /// </summary>
        /// <exception cref="DimensionMismatchException"> Tangent only works on angles. </exception>
        /// <returns> The tangent. </returns>
        public readonly Quantity Tan()
        {
            if (!dimension.HasDimension(Dimension.Angle, 1.0))
            {
                throw new DimensionMismatchException("Cannot take the sine of a quantity without angle^1.");
            }
            return new Quantity(Math.Tan(QuantityDimension.ApplyUnit(this, Unit.Radian)));
        }

        /// <summary>
        ///     Returns the square root of this quantity.
        /// </summary>
        /// <returns> The square root. </returns>
        public readonly Quantity Sqrt()
        {
            return new Quantity(Math.Sqrt(value), dimension / 2);
        }

        /// <summary>
        ///     Converts units to the target unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns> The modified quantity. </returns>
        public readonly Quantity ConvertUnit(Unit unit)
        {
            var (oldUnit, power) = this.dimension.GetUnits(unit.Dimension);

            if (oldUnit == null)
            {
                return this;
            }

            var dimension = new QuantityDimension(this.dimension).ReplaceUnit(unit, power);
            return new Quantity((value - oldUnit.BaseOffset) * Math.Pow(oldUnit.BaseFactor / unit.BaseFactor, power) + unit.BaseOffset, dimension);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Quantity Parse(string str)
        {
            return new Quantity(str);
        }

        [GeneratedRegex(@"([\d\.e\-]+)([a-zA-Z]+(\^\-?\d+)?)?")]
        private static partial Regex QuantityLookupRegex();
    }

}