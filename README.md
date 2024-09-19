C# computation library for completing physics calculations. Keeps track of units and dimensions, along with common operations.

## Features
Implemented:
* Units/Dimensional analysis
* Library of common units
* 2D Vectors

Coming Soon:
* 3D Vectors
* Errors/uncertainties
* More units

Far Future:
* Compile/convert to hard-coded for performance
* Algebraic manipulation

## Usage
* **Dimension**: A physical thing that can be measured. Examples: Temperature, Mass, Length
* **Unit**: A specific standardized interval to measure the aforementioned physical thing. Examples: Kelvin, Kilograms, Meters

### Quantity
Simple physical values can be created using the `Quantity` struct. Quantities are immutable. To create a quantity, specify the value, then units after separated by spaces. Example: `new Quantity("123.456 kg m s^-2")`. Alternatively, separately specify the base and units using `new Quantity(double, params Unit[])`. Example: `new Quantity(123.456, Unit.Meter, Unit.Meter) = 123.456 m^2`
Adding, subtracting, multiplying, and dividing works as expected. Units are automatically converted with preference on the left-hand-side operand's units. Adding and subtracting requires the same *dimension* on both operands, otherwise a `DimensionMismatchException` will occur.

Quantities are always displayed with units included. Change the units of quantities using `Quantity.ConvertUnit`.

### Vector2
Currently, only 2D Vectors are supported in the `Vector2` struct. 2D Vectors are a pair of `Quantity` joined together. Vector2s are immutable.

Vectors can be created in multiple ways:
* Through x, y components (quantities, doubles, or strings of quantities) using `Vector2(double, double)`.
* Through an angle using `Vector2.AngleVector(double)`.
* Through an angle and a mangitude using `Vector2.AngleMagnitudeVector(double, double)`.
* Through x, y components using `Vector2.Parse(string)`. Example: `(12.3 m, 4.56 m)`
* Through compass coordinates (East = +X) using `Vector2.Parse(string)`. Example: `N, 123km`
* Through compass coordinates and an angle direction using `Vector2.Parse(string)`. Example: `(NW, 45 deg, 123 km)`

Parentheses around the string are optional, they are trimmed off immediately.
Unsupported formats will throw a `ArgumentOutOfRangeException`.

### MathOptions
Static class with configurable options.
* angleUnit (Radians, Degrees): Change the unit to display and input angles in.
* vectorSystem (Cartesian, Polar): Change the method of displaying vectors.
* baseKilograms (true, false): Whether grams should be shown as kilograms.

### Unit
Custom units can be created through the `new Unit(string, string, Dimension, baseFactor, baseOffset)` constructor.
