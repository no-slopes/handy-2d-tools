namespace H2DT.Management.Graphics
{
    public struct ScreenResolution
    {
        private int _width;
        private int _height;

        public int width => _width;
        public int height => _height;

        public ScreenResolution(int width, int height)
        {
            _width = width;
            _height = height;
        }
    }
}
