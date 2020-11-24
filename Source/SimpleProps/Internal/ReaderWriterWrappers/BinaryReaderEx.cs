using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleProps.Internal.ReaderWriterWrappers
{
#if net35
    internal class BinaryReaderEx : IDisposable
    {
        // 非公開フィールド
        private bool _leaveOpen;
        private BinaryReader _binaryReader;


        // 公開プロパティ

        public Stream BaseStream
        {
            get => this._binaryReader?.BaseStream;
        }


        // コンストラクタ

        public BinaryReaderEx(Stream input)
            : this(input, new UTF8Encoding(), false)
        {
        }

        public BinaryReaderEx(Stream input, Encoding encoding)
            : this(input, encoding, false)
        {
        }

        public BinaryReaderEx(Stream input, Encoding encoding, bool leaveOpen)
        {
            this._binaryReader = new BinaryReader(input, encoding);
            this._leaveOpen = leaveOpen;
        }


        // 公開メソッド

        public virtual int PeekChar()
            => this._binaryReader.PeekChar();

        public virtual int Read()
            => this._binaryReader.Read();

        public virtual int Read(char[] buffer, int index, int count)
            => this._binaryReader.Read(buffer, index, count);

        public virtual int Read(byte[] buffer, int index, int count)
            => this._binaryReader.Read(buffer, index, count);

        public virtual bool ReadBoolean()
            => this._binaryReader.ReadBoolean();

        public virtual byte ReadByte()
            => this._binaryReader.ReadByte();

        public virtual byte[] ReadBytes(int count)
            => this._binaryReader.ReadBytes(count);

        public virtual char ReadChar()
            => this._binaryReader.ReadChar();

        public virtual char[] ReadChars(int count)
            => this._binaryReader.ReadChars(count);

        public virtual decimal ReadDecimal()
            => this._binaryReader.ReadDecimal();

        public virtual double ReadDouble()
            => this._binaryReader.ReadDouble();

        public virtual short ReadInt16()
            => this._binaryReader.ReadInt16();

        public virtual int ReadInt32()
            => this._binaryReader.ReadInt32();

        public virtual long ReadInt64()
            => this._binaryReader.ReadInt64();

        public virtual sbyte ReadSByte()
            => this._binaryReader.ReadSByte();

        public virtual float ReadSingle()
            => this._binaryReader.ReadSingle();

        public virtual string ReadString()
            => this._binaryReader.ReadString();

        public virtual ushort ReadUInt16()
            => this._binaryReader.ReadUInt16();

        public virtual uint ReadUInt32()
            => this._binaryReader.ReadUInt32();

        public virtual ulong ReadUInt64()
            => this._binaryReader.ReadUInt64();

        public void Dispose()
        {
            if (this._leaveOpen == false)
                ((IDisposable)this._binaryReader).Dispose();
        }
    }
#endif
}
