namespace Universal.Serialization
{
    [System.Serializable]
    public struct SimpleResolution
    {
        public int width;
        public int height;
        public override readonly string ToString()
        {
            return $"{width}x{height}";
        }
    }
}