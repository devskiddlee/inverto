using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace inverto
{
    public class Utils
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Triangle
        {
            public Vector3 p1, p2, p3;

            public bool intersect(Vector3 ray_origin, Vector3 ray_end)
            {
                const float EPSILON = 0.0000001f;
                Vector3 edge1, edge2, h, s, q;
                float a, f, u, v, t;
                edge1 = p2 - p1;
                edge2 = p3 - p1;
                h = Vector3.Cross(ray_end - ray_origin, edge2);
                a = Vector3.Dot(edge1, h);

                if (a > -EPSILON && a < EPSILON)
                    return false;

                f = 1.0f / a;
                s = ray_origin - p1;
                u = f * Vector3.Dot(s, h);

                if (u < 0.0 || u > 1.0)
                    return false;

                q = Vector3.Cross(s, edge1);
                v = f * Vector3.Dot((ray_end - ray_origin), q);

                if (v < 0.0 || u + v > 1.0)
                    return false;

                t = f * Vector3.Dot(edge2, q);

                if (t > EPSILON && t < 1.0)
                    return true;

                return false;
            }
        };
    }
}
