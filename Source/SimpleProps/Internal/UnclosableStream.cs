using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleProps.Internal
{
    public class UnclosableStream : Stream, IDisposable
    {
        // 非公開フィールド

        private EventHandler _closing;


        // 公開プロパティ

        public Stream BaseStream
        {
            get;
            private set;
        }

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public override long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }


        // 公開イベント




        // コンストラクタ

        public UnclosableStream(Stream baseStream)
        {
            this.BaseStream = baseStream;
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }
    }
}
