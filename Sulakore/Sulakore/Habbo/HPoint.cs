using System.Drawing;

namespace Sulakore.Habbo
{
    public struct HPoint
    {
        #region Properties
        public static HPoint Empty;
        private readonly Point _Point;

        private readonly int _X;
        public int X { get { return _X; } }

        private readonly int _Y;
        public int Y { get { return _Y; } }

        private readonly string _Z;
        public string Z { get { return string.IsNullOrEmpty(_Z) ? "0.0" : _Z; } }
        #endregion

        public HPoint(int X, int Y)
            : this(X, Y, "0.0")
        { }
        public HPoint(int X, int Y, string Z)
        {
            _X = X;
            _Y = Y;
            _Z = Z;
            _Point = new Point(X, Y);
        }

        public Point ToPoint()
        {
            return _Point;
        }
        public override string ToString()
        {
            return string.Format("{{X={0},Y={1},Z={2}}}", X, Y, Z);
        }
    }
}