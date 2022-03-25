using System;
using System.Numerics;

namespace SoftwareCities.figures
{
    /// <summary>
    /// Rotation for Figures and Compsitions.
    /// </summary>
    public class Rotation
    {
        private Matrix4x4 transformation = Matrix4x4.Identity;

        public static Rotation NONE = new Rotation();

        public static Rotation VOR_90_GRAD = FromAngle(90, new Vector3(0, 0, -1));
        public static Rotation VOR_180_GRAD = FromAngle(180, new Vector3(0, 0, -1));
        public static Rotation VOR_270_GRAD = FromAngle(270, new Vector3(0, 0, -1));

        public static Rotation LINKS_90_GRAD = FromAngle(90, new Vector3(0, 1, 0));
        public static Rotation LINKS_180_GRAD = FromAngle(180, new Vector3(0, 1, 0));
        public static Rotation LINKS_270_GRAD = FromAngle(270, new Vector3(0, 1, 0));

        public static Rotation ROLLE_RECHTS_90_GRAD = FromAngle(90, new Vector3(1, 0, 0));
        public static Rotation ROLLE_RECHTS_180_GRAD = FromAngle(180, new Vector3(1, 0, 0));
        public static Rotation ROLLE_RECHTS_270_GRAD = FromAngle(270, new Vector3(1, 0, 0));

        public static Rotation ZURUECK_90_GRAD = FromAngle(90, new Vector3(0, 0, 1));
        public static Rotation ZURUECK_180_GRAD = FromAngle(180, new Vector3(0, 0, 1));
        public static Rotation ZURUECK_270_GRAD = FromAngle(270, new Vector3(0, 0, 1));

        public static Rotation RECHTS_90_GRAD = FromAngle(90, new Vector3(0, -1, 0));
        public static Rotation RECHTS_180_GRAD = FromAngle(180, new Vector3(0, -1, 0));
        public static Rotation RECHTS_270_GRAD = FromAngle(270, new Vector3(0, -1, 0));

        public static Rotation ROLLE_LINKS_90_GRAD = FromAngle(90, new Vector3(-1, 0, 0));
        public static Rotation ROLLE_LINKS_180_GRAD = FromAngle(180, new Vector3(-1, 0, 0));
        public static Rotation ROLLE_LINKS_270_GRAD = FromAngle(270, new Vector3(-1, 0, 0));
        
        public static Rotation FromAngle(double grad, Vector3 achse)
        {
            Rotation drehung = new Rotation();
            drehung.transformation = Matrix4x4.CreateFromAxisAngle(achse, (float) ToRadians(grad));
            return drehung;
        }

        public Position Apply(Position position)
        {
            Vector4 ziel = Vector4.Transform(new Vector4(position.GetX(), position.GetY(), position.GetZ(), 1),
                transformation);
            return Position.Xyz(ziel.X, ziel.Y, ziel.Z);
        }

        public Matrix4x4 GetTransform()
        {
            return transformation;
        }

        public static double ToRadians(double angdeg)
        {
            return angdeg / 180.0 * Math.PI;
        }
    }
}