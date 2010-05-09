using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class SimplexNoiseGenerator : INoiseGenerator
    {
        // Taken from http://webstaff.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf

        static int[][] grad3 = {new int[] {1, 1, 0}, new int[] {-1, 1, 0}, new int[] {1, -1, 0}, new int[] {-1, -1, 0}, 
                                new int[] {1, 0, 1}, new int[] {-1, 0, 1}, new int[] {1, 0, -1}, new int[] {-1, 0, -1}, 
                                new int[] {0, 1, 1}, new int[] {0, -1, 1}, new int[] {0, 1, -1}, new int[] {0, -1, -1}};

        // TODO: generate from seed, and also maybe increase the length and range
        private static byte[] p = new byte[256];

        private static byte[] perm = new byte[512];

        static SimplexNoiseGenerator()
        {
            GeneratePermutationTable();

            // To remove the need for index wrapping, double the permutation table length
            for (int i = 0; i < 512; i++)
            {
                perm[i] = p[i & 255];
            }
        }

        static void GeneratePermutationTable()
        {
            for (int x = 0; x < p.Length; x++)
            {
                p[x] = (byte)x;
            }

            // TODO: configurable seed
            var random = new Random(0);
            for (int source = 0; source < p.Length; source++)
            {
                var destination = random.Next(p.Length);
                var temp = p[destination];
                p[destination] = p[source];
                p[source] = temp;
            }
        }

        // This method is a *lot* faster than using (int)Math.floor(x)
        static int fastfloor(double x)
        {
            return x > 0 ? (int) x : (int) x - 1;
        }

        static double dot(int[] g, double x, double y, double z)
        {
            return g[0] * x + g[1] * y + g[2] * z;
        }

        public double GetNoise(DoubleVector3 location)
        {
            double xin = location.X;
            double yin = location.Y;
            double zin = location.Z;

            double n0, n1, n2, n3; // Noise contributions from the four corners

            // Skew the input space to determine which simplex cell we're in
            const double F3 = 1.0 / 3.0;
            double s = (xin + yin + zin) * F3; // Very nice and simple skew factor for 3D
            int i = fastfloor(xin + s);
            int j = fastfloor(yin + s);
            int k = fastfloor(zin + s);

            const double G3 = 1.0 / 6.0; // Very nice and simple unskew factor, too
            double t = (i + j + k) * G3;
            double X0 = i - t; // Unskew the cell origin back to (x,y,z) space
            double Y0 = j - t;
            double Z0 = k - t;
            double x0 = xin - X0; // The x,y,z distances from the cell origin
            double y0 = yin - Y0;
            double z0 = zin - Z0;
            
            // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
            // Determine which simplex we are in.
            int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
            int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords
            
            if (x0 >= y0)
            {
                if (y0 >= z0)
                {
                    // X Y Z order
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
                else if (x0 >= z0)
                {
                    // X Z Y order
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
                else
                {
                    // Z X Y order
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                } 
            }
            else
            {
                // x0 < y0
                if (y0 < z0)
                {
                    // Z Y X order
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                } 
                else if (x0 < z0)
                {
                    // Y Z X order
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                } 
                else
                {
                    // Y X Z order
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                } 
            }

            // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
            // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
            // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
            // c = 1/6.
            
            double x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
            double y1 = y0 - j1 + G3;
            double z1 = z0 - k1 + G3;
            double x2 = x0 - i2 + 2.0 * G3; // Offsets for third corner in (x,y,z) coords
            double y2 = y0 - j2 + 2.0 * G3;
            double z2 = z0 - k2 + 2.0 * G3;
            double x3 = x0 - 1.0 + 3.0 * G3; // Offsets for last corner in (x,y,z) coords
            double y3 = y0 - 1.0 + 3.0 * G3;
            double z3 = z0 - 1.0 + 3.0 * G3;
            
            // Work out the hashed gradient indices of the four simplex corners
            int ii = i & 255;
            int jj = j & 255;
            int kk = k & 255;
            int gi0 = perm[ii + perm[jj + perm[kk]]] % 12;
            int gi1 = perm[ii + i1 + perm[jj + j1 + perm[kk + k1]]] % 12;
            int gi2 = perm[ii + i2 + perm[jj + j2 + perm[kk + k2]]] % 12;
            int gi3 = perm[ii + 1 + perm[jj + 1 + perm[kk + 1]]] % 12;
            
            // Calculate the contribution from the four corners
            double t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 < 0)
            {
                n0 = 0.0;
            }
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * dot(grad3[gi0], x0, y0, z0);
            }
            
            double t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 < 0)
            {
                n1 = 0.0;
            }
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * dot(grad3[gi1], x1, y1, z1);
            }
            
            double t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 < 0)
            {
                n2 = 0.0;
            }
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * dot(grad3[gi2], x2, y2, z2);
            }
            
            double t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 < 0)
            {
                n3 = 0.0;
            }
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * dot(grad3[gi3], x3, y3, z3);
            }
            
            // Add contributions from each corner to get the final noise value.
            // The result is scaled to stay just inside [-1,1]
            return 32.0 * (n0 + n1 + n2 + n3);
        }  
    }
}


