using UnityEngine;

namespace SmartwallPackage.SmartwallInput
{
    /// <summary>
    /// Container class for Blob data, location and sizes are normalized. Pivot of the blob is centered.
    /// </summary>
    public class Blob
    {
        public int Id;
        public Vector2 Position;
        public float Width;
        public float Height;

        public Blob(int id, float x, float y, float width, float height)
        {
            Id       = id;
            Position = new Vector2(x, y);
            Width    = width;
            Height   = height;
        }
    }
}
