using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleProps.Internal
{
    internal class NullStream : Stream
    {
        // 非公開フィールド
        private long _length;
        private long _position;


        // 公開プロパティ

        public override bool CanRead
        {
            get => false;
        }

        public override bool CanSeek
        {
            get => true;
        }

        public override bool CanWrite
        {
            get => true;
        }

        public override long Length
        {
            get => this._length;
        }

        public override long Position
        {
            get => this._position;
            set => this._setPosition(value);
        }


        // コンストラクタ

        /// <summary>
        /// <see cref="NullStream"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public NullStream()
        {
            this._length = 0;
            this._position = 0;
        }


        // 非公開メソッド

        private void _setPosition(long value)
        {
            this._position = value;
            if (this._length < this._position)
                this._length = this._position;
        }


        // 公開メソッド

        public override void Flush()
        {
            // 実装なし
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
                case SeekOrigin.End:
                    this.Position = this.Length + offset;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return this.Position;
        }

        public override void SetLength(long value)
        {
            this._length = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.Position += count;
        }
    }
}
