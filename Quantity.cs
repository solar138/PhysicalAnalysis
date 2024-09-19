using System.Text.RegularExpressions;

using Number = double;

namespace PhysicalAnalysis
{
    public partial struct Quantity
    {
        public Number value;

        public QuantityDimension dimension = new();

        public Quantity(Number value, QuantityDimension dimension)
        {
            this.value = value;
            this.dimension = dimension;
        }
        public Quantity(string str)
        {
            var values = str.Split(' ');

            var dimensions = new QuantityDimension();
            var result = MyRegex().Match(values[0]);

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
        public static Quantity operator *(Quantity lhs, Quantity rhs)
        {
            return new Quantity(lhs.value * QuantityDimension.MatchUnits(lhs, rhs).value, lhs.dimension + rhs.dimension);
        }
        public static Quantity operator /(Quantity lhs, Quantity rhs)
        {
            return new Quantity(lhs.value / QuantityDimension.MatchUnits(lhs, rhs).value, lhs.dimension - rhs.dimension);
        }

        public override readonly string ToString()
        {
            if (!MathOptions.baseKilograms)
            {
                var units = dimension.ToString();
                return $"{this.value}{units}";
            }

            var (unit, power) = dimension.GetUnits(Dimension.Mass);

            var value = this.value;

            if (unit == Unit.Gram)
            {
                value *= Math.Pow(0.001, power);

                var units = dimension.ToString();
                return $"{value}{units}";
            }
            else
            {
                var units = dimension.ToString();
                return $"{value}{units}";
            }
        }

        public static bool CompareDimensions(Quantity lhs, Quantity rhs)
        {
            return lhs.dimension == rhs.dimension;
        }

        /// <summary>
        ///     Returns the cosine of this quantity.
        /// </summary>
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
            return new Quantity((value - oldUnit.BaseOffset) * Math.Pow(unit.BaseFactor / oldUnit.BaseFactor, power) + unit.BaseOffset, dimension);
        }

        public static Quantity Parse(string str)
        {
            return new Quantity(str);
        }

        [GeneratedRegex(@"([\d\.]+)([a-zA-Z]+(\^\-?\d+)?)")]
        private static partial Regex MyRegex();
    }

}