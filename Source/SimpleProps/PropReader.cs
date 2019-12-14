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
        private PropBinaryLoader _binLoader;


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
            this._binLoader = new PropBinaryLoader(this._encoding);
        }


        // 非公開メソッド


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
                // セクション テーブルを読み取る
                var sectionTable = this._binLoader.LoadSectionTable(br);
            }

            throw new NotImplementedException();
        }
    }
}
