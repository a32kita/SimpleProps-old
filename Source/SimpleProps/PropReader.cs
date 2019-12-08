using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SimpleProps.Internal;

namespace SimpleProps
{
    public class PropReader
    {
        // 非公開フィールド
        private Stream _stream;
        private Encoding _encoding;
        private bool _leaveOpen;
        private bool _isDisposed;


        // 公開静的フィールド

        public static readonly Encoding DefaultEncoding;


        // コンストラクタ

        static PropReader()
        {
            DefaultEncoding = ReaderWriterConstants.DefaultEncoding;
        }

        public PropReader(Stream stream)
            : this(stream, DefaultEncoding, false)
        {
            // 実装なし
        }

        public PropReader(Stream stream, Encoding encoding, bool leaveOpen)
        {
            this._stream = stream;
            this._encoding = encoding;
            this._leaveOpen = leaveOpen;
            this._isDisposed = false;
        }


        // 非公開メソッド

        private Dictionary<string, ulong> _loadSectionTable(BinaryReader binaryReader)
        {
            throw new NotImplementedException();
        }


        // 公開メソッド

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Props ReadAllProps()
        {
            if (this._isDisposed)
                throw new ObjectDisposedException(nameof(PropReader));

            using (var br = new BinaryReader(this._stream, this._encoding, false))
            {

            }

            throw new NotImplementedException();
        }
    }
}
