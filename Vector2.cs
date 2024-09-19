namespace PhysicalAnalysis
{
    public struct Vector2(Quantity x, Quantity y)
    {
        public Quantity x = x;
        public Quantity y = y;

        public Vector2(double x, double y) : this(new Quantity(x), new Quantity(y))
        { }

        public Vector2(string x, string y) : this(new Quantity(x), new Quantity(y))
        { }

        public static readonly Dictionary<char, Vector2> compass = new() { { 'N', new Vector2(0, 1) }, { 'E', new Vector2(1, 0) }, { 'S', new Vector2(0, -1) }, { 'W', new Vector2(-1, 0) } };
        public static readonly Dictionary<string, (double, double)> compassDirections = new() {
        { "NE",  (0.0, 1.0)},
        { "SE",  (0.0, -1.0)},
        { "NW",  (Math.PI, -1.0)},
        { "SW",  (Math.PI, 1.0)},
        { "EN",  (Math.PI * 0.5, -1.0)},
        { "WN",  (Math.PI * 0.5, 1.0)},
        { "ES",  (Math.PI * 1.5, 1.0)},
        { "WS",  (Math.PI * 1.5, -1.0)},
    };

        public static Vector2 Parse(string str)
        {
            str = str.Trim('(', ')');
            var parts = str.Split(", ");

            if (parts.Length == 2)
            {
                if (compass.TryGetValue(parts[0][0], out var direction))
                {
                    var quantity = new Quantity(parts[1]);

                    return direction * quantity;
                }
                else
                {
                    var x = new Quantity(parts[0]);
                    var y = new Quantity(parts[1]);

                    return new Vector2(x, y);
                }
            }
            else if (parts.Length == 3)
            {
                if (parts[0].Length > 1)
                {
                    if (compassDirections.TryGetValue(parts[0], out var value))
                    {
                        var angle = new Quantity(parts[1]);
                        var magnitude = new Quantity(parts[2]);

                        if (magnitude.dimension.HasDimension(Dimension.Angle))
                        {
                            (angle, magnitude) = (magnitude, angle);
                        }
                        return AngleMagnitudeVector(angle * new Quantity(value.Item2) + new Quantity(value.Item1, Unit.Radian), magnitude);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Unsupported vector string format: " + str);
                    }
                }
            }

            throw new ArgumentOutOfRangeException("Unsupported vector string format: " + str);
        }

        public static Vector2 AngleVector(double angle)
        {
            return new Vector2(Math.Cos(MathOptions.ConvertAngle(angle)), Math.Sin(MathOptions.ConvertAngle(angle)));
        }

        public static Vector2 AngleMagnitudeVector(double angle, double magnitude)
        {
            return new Vector2(Math.Cos(MathOptions.ConvertAngle(angle)) * magnitude, Math.Sin(MathOptions.ConvertAngle(angle)) * magnitude);
        }

        public static Vector2 AngleVector(Quantity angle)
        {
            return new Vector2(angle.Cos(), angle.Sin());
        }

        public static Vector2 AngleMagnitudeVector(Quantity angle, Quantity magnitude)
        {
            return new Vector2(angle.Cos() * magnitude, angle.Sin() * magnitude);
        }

        // add subtract scale dot product length
        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static Vector2 operator *(Vector2 lhs, Quantity rhs)
        {
            return new Vector2(lhs.x * rhs, lhs.y * rhs);
        }

        public static Vector2 operator *(Quantity lhs, Vector2 rhs)
        {
            return new Vector2(rhs.x * lhs, rhs.y * lhs);
        }

        public static Vector2 operator /(Vector2 lhs, Quantity rhs)
        {
            return new Vector2(lhs.x / rhs, lhs.y / rhs);
        }

        public static Quantity operator *(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        /// <summary>
        ///     Returns the length of the vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Quantity operator ~(Vector2 vector)
        {
            return (vector.x * vector.x + vector.y * vector.y).Sqrt();
        }

        /// <summary>
        ///     Returns the argument(angle) of the vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Quantity operator !(Vector2 vector)
        {
            if (vector.x.value == 0.0)
            {
                return MathOptions.angleUnit == MathOptions.AngleUnit.Radian ? new Quantity(Math.PI / 2, new QuantityDimension(Unit.Radian)) : new Quantity(90, new QuantityDimension(Unit.Degree));
            }
            var angle = Math.Atan((vector.y / vector.x).value);

            if (angle < 0)
            {
                angle += 2 * Math.PI;
            }

            if (vector.x.value < 0 && vector.y.value < 0)
            {
                angle += Math.PI;
            }

            return MathOptions.angleUnit == MathOptions.AngleUnit.Radian ? new Quantity(angle, new QuantityDimension(Unit.Radian)) : new Quantity(angle * 180 / Math.PI, new QuantityDimension(Unit.Degree));
        }

        public override readonly string ToString()
        {
            return MathOptions.vectorSystem == MathOptions.VectorSystem.Cartesian ? $"({x}, {y})" : $"({~this} @ {!this})";
        }
    }
}