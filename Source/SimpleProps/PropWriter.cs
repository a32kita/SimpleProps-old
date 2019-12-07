using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using SimpleProps.Internal;

namespace SimpleProps
{
    public class PropWriter : IDisposable
    {
        // 非公開フィールド
        private Stream _stream;
        private Encoding _encoding;
        private bool _leaveOpen;


        // 公開静的フィールド

        public static readonly Encoding DefaultEncoding;


        // コンストラクタ

        static PropWriter()
        {
            DefaultEncoding = Encoding.UTF8;
        }

        public PropWriter(Stream stream, Encoding encoding, bool leaveOpen)
        {
            this._stream = stream;
            this._encoding = encoding;
            this._leaveOpen = leaveOpen;
        }


        // 公開メソッド

        public void Write(Props props)
        {
            var binBuilder = new PropBinaryBuilder(this._encoding);
            using (var bw = new BinaryWriter(this._stream, this._encoding, false))
            {

            }
        }
    }
}
