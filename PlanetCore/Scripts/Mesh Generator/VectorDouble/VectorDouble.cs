using System;
using UnityEngine;

namespace VectorDoubles {
    public struct TransformMatrices {
        private static sbyte[,] none = new sbyte[,]{
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 }
        };
        private static sbyte[,] down = new sbyte[,]{
            { 1, 0, 0 },
            { 0, -1, 0 },
            { 0, 0, -1 }
        };
        private static sbyte[,] right = new sbyte[,]{
            { 1, 0, 0 },
            { 0, 0, -1 },
            { 0, 1, 0 }
        };
        private static sbyte[,] left = new sbyte[,]{
            { 1, 0, 0 },
            { 0, 0, 1 },
            { 0, -1, 0 }
        };
        private static sbyte[,] forward = new sbyte[,]{
            { 0, -1, 0 },
            { 1, 0, 0 },
            { 0, 0, 1 }
        };
        private static sbyte[,] backward = new sbyte[,]{
            { 0, 1, 0 },
            { -1, 0, 0 },
            { 0, 0, 1 }
        };

        public static sbyte[,] None {
            get { return none; }
        }
        public static sbyte[,] Down {
            get { return down; }
        }
        public static sbyte[,] Right {
            get { return right; }
        }
        public static sbyte[,] Left {
            get { return left; }
        }
        public static sbyte[,] Forward {
            get { return forward; }
        }
        public static sbyte[,] Backward {
            get { return backward; }
        }
    }

    public struct Vector2Double {
        public double x, y;

        public Vector2Double(double X, double Y) {
            x = X;
            y = Y;
        }

        public static Vector2Double operator +(Vector2Double a, Vector2Double b) {
            Vector2Double sum = new Vector2Double() {
                x = a.x + b.x,
                y = a.y + b.y
            };

            return sum;
        }
    }

    public struct Vector3Double {
        public double x, y, z;

        public Vector3Double(double X, double Y, double Z) {
            x = X;
            y = Y;
            z = Z;
        }
        public static Vector3Double operator -(Vector3Double a, Vector3 b) {
            Vector3Double difference = new Vector3Double() {
                x = a.x - (double)b.x,
                y = a.y - (double)b.y,
                z = a.z - (double)b.z
            };

            return difference;
        }
        public static Vector3Double operator +(Vector3Double a, Vector3 b) {
            Vector3Double sum = new Vector3Double() {
                x = a.x + (double)b.x,
                y = a.y + (double)b.y,
                z = a.z + (double)b.z
            };

            return sum;
        }
        public static Vector3Double operator +(Vector3 a, Vector3Double b) {
            Vector3Double sum = new Vector3Double() {
                x = (double)a.x + b.x,
                y = (double)a.y + b.y,
                z = (double)a.z + b.z
            };

            return sum;
        }
        public static Vector3Double operator +(Vector3Double a, Vector3Double b) {
            Vector3Double sum = new Vector3Double() {
                x = a.x + b.x,
                y = a.y + b.y,
                z = a.z + b.z
            };

            return sum;
        }
        public static Vector3Double operator -(Vector3Double a, Vector3Double b) {
            Vector3Double difference = new Vector3Double() {
                x = a.x - b.x,
                y = a.y - b.y,
                z = a.z - b.z
            };

            return difference;
        }
        public static Vector3Double operator *(double d, Vector3Double a) {
            Vector3Double product = new Vector3Double() {
                x = a.x * d,
                y = a.y * d,
                z = a.z * d
            };

            return product;
        }
        public static Vector3Double operator *(Vector3Double a, double d) {
            Vector3Double product = new Vector3Double() {
                x = a.x * d,
                y = a.y * d,
                z = a.z * d
            };

            return product;
        }
        public static Vector3Double operator /(Vector3Double a, double d) {
            Vector3Double quotient = new Vector3Double() {
                x = a.x / d,
                y = a.y / d,
                z = a.z / d
            };

            return quotient;
        }
        //rotates a vector by a rotation matrix
        public static Vector3Double operator *(sbyte[,] rotation, Vector3Double a) {
            a = a / Magnitude(a);

            Vector3Double vec = new Vector3Double() {
                x = rotation[0, 0] * a.x + rotation[0, 1] * a.y + rotation[0, 2] * a.z,
                y = rotation[1, 0] * a.x + rotation[1, 1] * a.y + rotation[1, 2] * a.z,
                z = rotation[2, 0] * a.x + rotation[2, 1] * a.y + rotation[2, 2] * a.z
            };

            return vec;
        }
        public static double Magnitude(Vector3Double a) {
            return Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
        }
    }
}