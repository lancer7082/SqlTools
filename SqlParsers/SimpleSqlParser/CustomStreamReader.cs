using System.IO;

namespace SqlTools.SimpleSqlParser
{
    public class CustomStreamReader : StreamReader
    {
        public long Position { get; set; }

        public CustomStreamReader(Stream stream) : base(stream)
        {
            Position = 0;
        }

        public override int Read()
        {
            Position++;
            return base.Read();
        }

        public long Seek(long offset)
        {
            Position = offset;
            return this.BaseStream.Seek(offset, SeekOrigin.Begin);
        }
    }
}
