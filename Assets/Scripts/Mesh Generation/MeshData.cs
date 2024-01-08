
public class MeshData
{
    public static readonly int[] quadTri = new int[]
    {
        0, 1, 3, 3, 1, 2,
    };

    public static readonly int[] cubeTri = new int[]
    {
        0, 2, 1, 3, 2, 0,
        6, 5, 2, 2, 5, 1,
        0, 4, 7, 7, 3, 0,
        6, 7, 5, 5, 7, 4,
        2, 3, 7, 7, 6, 2,
        5, 4, 1, 1, 4, 0,
    };

    public static readonly int[] projectedCubeTri = new int[]
    {
         5, 4, 0, 0, 1, 5, // Front
         2, 6, 7, 7, 3, 2, // Back
         4, 5, 7, 7, 6, 4, // Top
         3, 7, 5, 5, 1, 3, // Left
         0, 4, 6, 6, 2, 0, // Right
         1, 0, 2, 2, 3, 1, // Bottom
    };
}
