using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleProps.Internal.ReaderWriterWrappers
{
#if net35
    internal class BinaryWriterEx : IDisposable
    {
        // 非公開フィールド
        private bool _leaveOpen;
        private BinaryWriter _binaryWriter;


        // コンストラクタ

        public BinaryWriterEx(Stream output)
            : this(output, new UTF8Encoding(false, true), false)
        {
        }

        public BinaryWriterEx(Stream output, Encoding encoding)
            : this(output, encoding, false)
        {
        }

        public BinaryWriterEx(Stream output, Encoding encoding, bool leaveOpen)
        {
            this._binaryWriter = new BinaryWriter(output, encoding);
            this._leaveOpen = leaveOpen;
        }


        // 公開メソッド

        public virtual void Flush()
            => this._binaryWriter.Flush();

        public virtual long Seek(int offset, SeekOrigin origin)
            => this._binaryWriter.Seek(offset, origin);

        public virtual void Write(double value)
            => this._binaryWriter.Write(value);

        public virtual void Write(string value)
            => this._binaryWriter.Write(value);

        public virtual void Write(float value)
            => this._binaryWriter.Write(value);

        public virtual void Write(ulong value)
            => this._binaryWriter.Write(value);

        public virtual void Write(long value)
            => this._binaryWriter.Write(value);

        public virtual void Write(uint value)
            => this._binaryWriter.Write(value);

        public virtual void Write(int value)
            => this._binaryWriter.Write(value);

        public virtual void Write(ushort value)
            => this._binaryWriter.Write(value);

        public virtual void Write(short value)
            => this._binaryWriter.Write(value);

        public virtual void Write(char ch)
            => this._binaryWriter.Write(ch);

        public virtual void Write(char[] chars, int index, int count)
            => this._binaryWriter.Write(chars, index, count);

        public virtual void Write(char[] chars)
            => this._binaryWriter.Write(chars);

        public virtual void Write(byte[] buffer, int index, int count)
            => this._binaryWriter.Write(buffer, index, count);

        public virtual void Write(byte[] buffer)
            => this._binaryWriter.Write(buffer);

        public virtual void Write(sbyte value)
            => this._binaryWriter.Write(value);

        public virtual void Write(byte value)
            => this._binaryWriter.Write(value);

        public virtual void Write(bool value)
            => this._binaryWriter.Write(value);

        public virtual void Write(decimal value)
            => this._binaryWriter.Write(value);


        public void Dispose()
        {
            this._binaryWriter.Flush();
            this._binaryWriter.BaseStream.Flush();
            if (this._leaveOpen == false)
                ((IDisposable)this._binaryWriter).Dispose();
        }
    }
#endif
}
