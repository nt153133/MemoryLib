using System;
using System.Globalization;
using System.Runtime.InteropServices;

// ReSharper disable file NonReadonlyMemberInGetHashCode

namespace MemLib.Ffxiv.Structures {
    /// <summary>Represents a vector with three  single-precision floating-point values.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3 : IEquatable<Vector3> {
        /// <summary>The X component of the vector.</summary>
        public float X;

        /// <summary>The Y component of the vector.</summary>
        public float Y;

        /// <summary>The Z component of the vector.</summary>
        public float Z;

        /// <summary>Gets a vector whose 3 elements are equal to zero.</summary>
        /// <returns>A vector whose three elements are equal to zero (that is, it returns the vector (0,0,0).</returns>
        public static Vector3 Zero => new Vector3();

        /// <summary>Gets a vector whose 3 elements are equal to one.</summary>
        /// <returns>A vector whose three elements are equal to one (that is, it returns the vector (1,1,1).</returns>
        public static Vector3 One => new Vector3(1f);

        public Vector3 Normalized => Normalize(this);

        private const float Deg2Rad = (float)Math.PI * 2f / 360f;
        private const float Rad2Deg = 1f / Deg2Rad;

        /// <summary>Access the X, Y, Z components using [0], [1], [2] respectively.</summary>
        /// <param name="index">The index.</param>
        /// <returns>The component.</returns>
        public float this[int index] {
            get {
                switch (index) {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
            set {
                switch (index) {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        #region Constructors

        public Vector3(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(float value) {
            X = value;
            Y = value;
            Z = value;
        }

        public Vector3(string commaseperated) {
            var array = commaseperated.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            X = float.Parse(array[0], CultureInfo.InvariantCulture);
            Y = float.Parse(array[1], CultureInfo.InvariantCulture);
            Z = float.Parse(array[2], CultureInfo.InvariantCulture);
        }

        #endregion

        public Vector3 Add(float x, float y, float z) {
            return this + new Vector3(x, y, z);
        }

        public void Set(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }

        public static float Dot(Vector3 vector1, Vector3 vector2) {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        public static float Magnitude(Vector3 vector) {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        public static float SqrMagnitude(Vector3 vector) {
            return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
        }

        public static Vector3 Normalize(Vector3 value) {
            var mag = Magnitude(value);
            return mag > float.Epsilon ? value / mag : Zero;
        }

        private static float Clamp(float value, float min, float max) {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static float AngleBetween(Vector3 from, Vector3 to) {
            return (float)Math.Acos(Clamp(Dot(from.Normalized, to.Normalized), -1f, 1f));
        }

        public static float Angle(Vector3 from, Vector3 to) {
            var denominator = (float)Math.Sqrt(SqrMagnitude(from) * SqrMagnitude(to));
            if (denominator < Math.Sqrt(float.Epsilon))
                return 0f;
            var dot = Clamp(Dot(from, to) / denominator, -1f, 1f);
            return ((float)Math.Acos(dot)) * Rad2Deg;
        }

        public float Distance(Vector3 to) {
            return Distance(this, to);
        }

        public float DistanceSquared(Vector3 to) {
            return DistanceSquared(this, to);
        }

        public static float Distance2D(Vector3 from, Vector3 to) {
            var num = from.X - to.X;
            var num2 = from.Z - to.Z;
            return (float)Math.Sqrt(num * num + num2 * num2);
        }

        public float Distance2D(Vector3 to) {
            return Distance2D(this, to);
        }

        public static float Distance2DSqr(Vector3 from, Vector3 to) {
            var num = from.X - to.X;
            var num2 = from.Z - to.Z;
            return num * num + num2 * num2;
        }

        public float Distance2DSqr(Vector3 to) {
            return Distance2DSqr(this, to);
        }

        /// <summary>Returns the length of this vector object.</summary>
        /// <returns>The vector's length.</returns>
        public float Length() {
            return (float) Math.Sqrt(X * (double) X + Y * (double) Y + Z * (double) Z);
        }

        /// <summary>Returns the length of the vector squared.</summary>
        /// <returns>The vector's length squared.</returns>
        public float LengthSquared() {
            return (float) (X * (double) X + Y * (double) Y + Z * (double) Z);
        }

        /// <summary>Computes the Euclidean distance between the two given points.</summary>
        /// <param name="value1">The first point.</param>
        /// <param name="value2">The second point.</param>
        /// <returns>The distance.</returns>
        public static float Distance(Vector3 value1, Vector3 value2) {
            var num1 = value1.X - value2.X;
            var num2 = value1.Y - value2.Y;
            var num3 = value1.Z - value2.Z;
            return (float) Math.Sqrt(num1 * (double) num1 + num2 * (double) num2 + num3 * (double) num3);
        }

        /// <summary>Returns the Euclidean distance squared between two specified points.</summary>
        /// <param name="value1">The first point.</param>
        /// <param name="value2">The second point.</param>
        /// <returns>The distance squared.</returns>
        public static float DistanceSquared(Vector3 value1, Vector3 value2) {
            var num1 = value1.X - value2.X;
            var num2 = value1.Y - value2.Y;
            var num3 = value1.Z - value2.Z;
            return (float) (num1 * (double) num1 + num2 * (double) num2 + num3 * (double) num3);
        }

        /// <summary>Computes the cross product of two vectors.</summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The cross product.</returns>
        public static Vector3 Cross(Vector3 vector1, Vector3 vector2) {
            return new Vector3((float) (vector1.Y * (double) vector2.Z - vector1.Z * (double) vector2.Y),
                (float) (vector1.Z * (double) vector2.X - vector1.X * (double) vector2.Z),
                (float) (vector1.X * (double) vector2.Y - vector1.Y * (double) vector2.X));
        }

        /// <summary>Returns the reflection of a vector off a surface that has the specified normal.</summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="normal">The normal of the surface being reflected off.</param>
        /// <returns>The reflected vector.</returns>
        public static Vector3 Reflect(Vector3 vector, Vector3 normal) {
            var num1 = (float) (vector.X * (double) normal.X + vector.Y * (double) normal.Y +
                                vector.Z * (double) normal.Z);
            var num2 = (float) (normal.X * (double) num1 * 2.0);
            var num3 = (float) (normal.Y * (double) num1 * 2.0);
            var num4 = (float) (normal.Z * (double) num1 * 2.0);
            return new Vector3(vector.X - num2, vector.Y - num3, vector.Z - num4);
        }

        /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
        /// <param name="value1">The vector to restrict.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The restricted vector.</returns>
        public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max) {
            var x1 = value1.X;
            var num1 = (double) x1 > (double) max.X ? max.X : x1;
            var x2 = (double) num1 < (double) min.X ? min.X : num1;
            var y1 = value1.Y;
            var num2 = (double) y1 > (double) max.Y ? max.Y : y1;
            var y2 = (double) num2 < (double) min.Y ? min.Y : num2;
            var z1 = value1.Z;
            var num3 = (double) z1 > (double) max.Z ? max.Z : z1;
            var z2 = (double) num3 < (double) min.Z ? min.Z : num3;
            return new Vector3(x2, y2, z2);
        }

        /// <summary>Performs a linear interpolation between two vectors based on the given weighting.</summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="amount">A value between 0 and 1 that indicates the weight of <paramref name="value2" />.</param>
        /// <returns>The interpolated vector.</returns>
        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount) {
            return new Vector3(value1.X + (value2.X - value1.X) * amount, value1.Y + (value2.Y - value1.Y) * amount,
                value1.Z + (value2.Z - value1.Z) * amount);
        }
        /// <summary> Returns a vector that is made from the smallest components of two vectors. </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns>The vector.</returns>
        public static Vector3 Min(Vector3 vector1, Vector3 vector2) {
            return new Vector3(Math.Min(vector1.X, vector2.X), Math.Min(vector1.X, vector2.X), Math.Min(vector1.X, vector2.X));
        }

        /// <summary> Returns a vector that is made from the largest components of two vectors.</summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns>The vector.</returns>
        public static Vector3 Max(Vector3 vector1, Vector3 vector2) {
            return new Vector3(Math.Max(vector1.X, vector2.X), Math.Max(vector1.X, vector2.X), Math.Max(vector1.X, vector2.X));
        }

        #region Overrides of ValueType

        public override string ToString() {
            return $"<{X.ToString("G", CultureInfo.InvariantCulture)}," +
                   $" {Y.ToString("G", CultureInfo.InvariantCulture)}," +
                   $" {Z.ToString("G", CultureInfo.InvariantCulture)}>";
        }

        #endregion

        #region Equality members

        public bool Equals(Vector3 other) {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj) {
            return obj is Vector3 other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hash = ((X.GetHashCode() << 5) + X.GetHashCode()) ^ Y.GetHashCode();
                return ((hash << 5) + hash) ^ Z.GetHashCode();
            }
        }

        #endregion

        #region Operators

        public static Vector3 operator +(Vector3 left, Vector3 right) {
            return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vector3 operator -(Vector3 left, Vector3 right) {
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static bool operator ==(Vector3 left, Vector3 right) {
            return left.X.Equals(right.X) && left.Y.Equals(right.Y) && left.Z.Equals(right.Z);
        }

        public static bool operator !=(Vector3 left, Vector3 right) {
            return !left.X.Equals(right.X) || !left.Y.Equals(right.Y) || !left.Z.Equals(right.Z);
        }

        public static Vector3 operator /(Vector3 left, Vector3 right) {
            return new Vector3(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
        }

        public static Vector3 operator /(Vector3 value1, float value2) {
            var num = 1f / value2;
            return new Vector3(value1.X * num, value1.Y * num, value1.Z * num);
        }

        public static Vector3 operator *(Vector3 left, Vector3 right) {
            return new Vector3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        public static Vector3 operator *(Vector3 left, float right) {
            return left * new Vector3(right);
        }

        public static Vector3 operator *(float left, Vector3 right) {
            return new Vector3(left) * right;
        }

        #endregion
    }
}