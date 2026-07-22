using Unity.VisualScripting;
using UnityEngine;

namespace Utility
{
    public enum CartesianAxes
    {
        DownRight,
        DownLeft,
        UpLeft,
        UpRight,
    }
    public static class ToskaExtensionMethods
    {
        public static CartesianAxes IncrementClockwise(this CartesianAxes axis)
        {
            switch (axis)
            {
                case (CartesianAxes.DownRight):
                    axis = CartesianAxes.DownLeft;
                    break;
                case (CartesianAxes.DownLeft):
                    axis = CartesianAxes.UpLeft;
                    break;
                case (CartesianAxes.UpLeft):
                    axis = CartesianAxes.UpRight;
                    break;
                case (CartesianAxes.UpRight):
                    axis = CartesianAxes.UpLeft;
                    break;
            }
            return axis;
        }

        public static CartesianAxes DecrementClockwise(this CartesianAxes axis)
        {
            switch (axis)
            {
                case (CartesianAxes.DownRight):
                    axis = CartesianAxes.UpRight;
                    break;
                case (CartesianAxes.UpRight):
                    axis = CartesianAxes.UpLeft;
                    break;
                case (CartesianAxes.UpLeft):
                    axis = CartesianAxes.DownLeft;
                    break;
                case (CartesianAxes.DownLeft):
                    axis = CartesianAxes.DownRight;
                    break;
            }
            return axis;
        }
        public static Vector3 GetReciprocal(this Vector3 vector3)
        {
            float x = vector3.x;
            float y = vector3.y;
            float z = vector3.z;
            return new Vector3(1 / x, 1 / y, 1 / z);
        }

        public static Vector3 MultiplyByVector(this Vector3 vector1, Vector3 vector2)
        {
            float x = vector1.x * vector2.x;
            float y = vector1.y * vector2.y;
            float z = vector1.z * vector2.z;
            return new Vector3(x, y, z);
        }

        public static Vector3 DivideByVector(this Vector3 vector1, Vector3 vector2)
        {
            // I know divisions on computers are slow but I cant be fuccccckkkeeed

            float x = vector1.x / vector2.x;
            float y = vector1.y / vector2.y;
            float z = vector1.z / vector2.z;
            return new Vector3(x, y, z);
        }

        public static Vector2 DivideByVector(this Vector2 vector1, Vector2 vector2)
        {
        
            float x = vector1.x / vector2.x;
            float y = vector1.y / vector2.y;
            return new Vector2(x, y);
        }

        public static Vector3 DivideByVector(this Vector3 vector1, Vector2 vector2)
        {
            float x = vector1.x / vector2.x;
            float y = vector1.y / vector2.y;
            float z = vector1.z / 1;
            return new Vector3(x, y, z);
        }

        public static Vector3 HalfVector(this Vector3 vector)
        {
            // writing this as an extension in case I want to make a slightly faster version using bit shifting later
            float x = vector.x / 2.0f;
            float y = vector.y / 2.0f;
            float z = vector.z / 2.0f;
            return new Vector3(x, y, z);
        }

        public static Vector2 HalfVector(this Vector2 vector)
        {
            float x = vector.x / 2.0f;
            float y = vector.y / 2.0f;
            return new Vector2(x, y);
        }

        public static Vector2 GetReciprocal(this Vector2 vector2)
        {
            float x = vector2.x;
            float y = vector2.y;
            return new Vector3(1 / x, 1 / y);
        }
    }
}
