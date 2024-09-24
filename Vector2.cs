namespace PhysicalAnalysis
{
    /// <summary>
    ///     A 2D vector composed of an x and y quantity components.
    /// </summary>
    /// <param name="x"> X-component. </param>
    /// <param name="y"> Y-component. </param>
    public struct Vector2(Quantity x, Quantity y)
    {
        public Quantity x = x;
        public Quantity y = y;

        /// <summary>
        ///     Creates a new vector with the given x and y components.
        /// </summary>
        /// <param name="x"> X-component. </param>
        /// <param name="y"> Y-component. </param>
        public Vector2(double x, double y) : this(new Quantity(x), new Quantity(y))
        { }

        /// <summary>
        ///     Creates a new vector with the given x and y components.
        /// </summary>
        /// <param name="x"> String form of quantity x. </param>
        /// <param name="y"> String form of quantity y. </param>
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

        /// <summary>
        ///     Creates a vector from a variety of string forms:
        ///     * x, y components: (12.3 m, 4.56 m)
        ///     * compass coordinates(East = +X): (N, 123km)
        ///     * compass coordinates and an angle direction, this will convert the polar coordinates to cartesian coordinates: (NW, 45 deg, 123 km)
        /// </summary>
        /// <param name="str"> String to parse. </param>
        /// <returns> A vector equivalent to the string in cartesian coordinates. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Invalid string format. </exception>
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

        /// <summary>
        ///     Creates a unit vector from the angle CCW from the x-axis.
        /// </summary>
        /// <param name="angle"> Angle in radians or degrees depending on MathOptions.angleUnit. </param>
        /// <returns> The unit vector. </returns>
        public static Vector2 AngleVector(double angle)
        {
            return new Vector2(Math.Cos(MathOptions.ConvertAngle(angle)), Math.Sin(MathOptions.ConvertAngle(angle)));
        }

        /// <summary>
        ///     Creates a vector of length magnitude from the angle CCW from the x-axis.
        /// </summary>
        /// <param name="angle"> Angle in radians or degrees depending on MathOptions.angleUnit. </param>
        /// <param name="magnitude"> Magnitude of the vector. </param>
        /// <returns> The vector. </returns>
        public static Vector2 AngleMagnitudeVector(double angle, double magnitude)
        {
            return new Vector2(Math.Cos(MathOptions.ConvertAngle(angle)) * magnitude, Math.Sin(MathOptions.ConvertAngle(angle)) * magnitude);
        }

        /// <summary>
        ///     Creates a unit vector from the angle CCW from the x-axis.
        /// </summary>
        /// <param name="angle"> Angle of the vector CCW from the x-axis. </param>
        /// <returns> The unit vector. </returns>
        public static Vector2 AngleVector(Quantity angle)
        {
            return new Vector2(angle.Cos(), angle.Sin());
        }

        /// <summary>
        ///     Creates a vector of length magnitude from the angle CCW from the x-axis.
        /// </summary>
        /// <param name="angle"> Angle of the vector CCW from the x-axis. </param>
        /// <param name="magnitude"> Magnitude of the vector. </param>
        /// <returns></returns>
        public static Vector2 AngleMagnitudeVector(Quantity angle, Quantity magnitude)
        {
            return new Vector2(angle.Cos() * magnitude, angle.Sin() * magnitude);
        }
        
        /// <summary>
        ///     Adds two vectors, the dimensions of the two vectors must be the same. Units are converted if necessary.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns> Component-wise sum of both vectors. </returns>
        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        /// <summary>
        ///     Subtracts two vectors, the dimensions of the two vectors must be the same. Units are converted if necessary.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns> Component-wise difference of both vectors. </returns>
        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        /// <summary>
        ///     Scales the vector by the quantity. 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns> A new scaled vector with the dimensions multiplied. </returns>
        public static Vector2 operator *(Vector2 lhs, Quantity rhs)
        {
            return new Vector2(lhs.x * rhs, lhs.y * rhs);
        }

        /// <summary>
        ///     Scales the vector by the quantity.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns> A new scaled vector with the dimensions multiplied. </returns>
        public static Vector2 operator *(Quantity lhs, Vector2 rhs)
        {
            return new Vector2(rhs.x * lhs, rhs.y * lhs);
        }

        /// <summary>
        ///     Scales the vector by the reciprocal of the quantity.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns> A new scaled vector with the dimensions multiplied. </returns>
        public static Vector2 operator /(Vector2 lhs, Quantity rhs)
        {
            return new Vector2(lhs.x / rhs, lhs.y / rhs);
        }

        /// <summary>
        ///     Returns the dot product of two vectors.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
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
        /// <returns> Angle in radians or degrees depending on MathOptions.angleUnit. </returns>
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

        /// <summary>
        ///     Converts the vector to a string depending on the coordinate system specified in MathOptions.vectorSystem.
        /// </summary>
        /// <returns></returns>
        public override readonly string ToString()
        {
            return MathOptions.vectorSystem == MathOptions.VectorSystem.Cartesian ? $"({x}, {y})" : $"({~this} @ {!this})";
        }
    }
}