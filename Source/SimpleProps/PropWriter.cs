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
        private bool _isDisposed;
        private PropBinaryBuilder _binBuilder;


        // 公開静的フィールド

        public static readonly Encoding DefaultEncoding;


        // コンストラクタ

        static PropWriter()
        {
            DefaultEncoding = ReaderWriterConstants.DefaultEncoding;
        }

        public PropWriter(Stream stream)
            : this(stream, DefaultEncoding, false)
        {
            // 実装なし
        }

        public PropWriter(Stream stream, Encoding encoding, bool leaveOpen)
        {
            this._stream = stream;
            this._encoding = encoding;
            this._leaveOpen = leaveOpen;
            this._isDisposed = false;
            this._binBuilder = new PropBinaryBuilder(this._encoding);
        }


        // 非公開メソッド

        private ulong[][] _createItemBufferMap(PropBinaryBuilder binBuilder, PropSectionCollection sections)
        {
            var itemBufferMap = new ulong[sections.Count][];
            var currentPosition = 0uL;
            var ns = new NullStream();
            for (var i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                itemBufferMap[i] = new ulong[section.Items.Count];
                for (var j = 0; j < section.Items.Count; j++)
                {
                    var item = section.Items[j];
                    ns.Seek(0, SeekOrigin.Begin);

                    // Reference アルゴリズム未実装
                    var bufferMode = PropItemBufferMode.Buffered;
                    if (item.IsNull)
                        bufferMode = PropItemBufferMode.Null;
                    binBuilder.WriteItemBuffer(ns, item, bufferMode);

                    itemBufferMap[i][j] = currentPosition;
                    currentPosition += (ulong)ns.Position;
                }
            }

            return itemBufferMap;
        }


        // 公開メソッド

        public void Write(Props props)
        {
            if (this._isDisposed)
                throw new ObjectDisposedException(nameof(PropWriter));

            using (var bw = new BinaryWriter(this._stream, this._encoding, true))
            {
                // アイテム バッファの空書き込みを実行し、アイテム バッファのマップを作成する
                var itemBufferMap = this._createItemBufferMap(_binBuilder, props.Sections);

#if DEBUG
                Console.WriteLine("== ITEM BUFFER MAPPING ==");
                for (var i = 0; i < props.Sections.Count; i++)
                {
                    var sect = props.Sections[i];
                    Console.WriteLine("[{0}]", sect.Name);
                    for (var j = 0; j < sect.Items.Count; j++)
                    {
                        var item = sect.Items[j];
                        var itemName = item.Name;
                        var bufferPos = itemBufferMap[i][j];
                        Console.WriteLine("{0}={1}", itemName, bufferPos);
                    }
                }
#endif

                // アイテム バッファ テーブルを作成する
                var itemBufferTableBuffer = new byte[props.Sections.Count][];
                var itemBufferTableMap = new ulong[props.Sections.Count];
                var currentPosition = 0uL;
                for (var i = 0; i < props.Sections.Count; i++)
                {
                    itemBufferTableBuffer[i] = _binBuilder.CreateItemBufferTable(props.Sections[i].Items, itemBufferMap[i]);
                    itemBufferTableMap[i] = currentPosition;
                    currentPosition += (ulong)itemBufferTableBuffer[i].Length; //itemBufferTableMap[i];
                }

                // セクション テーブルを作成し、書き込む
                bw.Write(_binBuilder.CreateSectionTable(props.Sections, itemBufferTableMap));

                // アイテム バッファ テーブルを書き込む
                for (var i = 0; i < itemBufferTableBuffer.Length; i++)
                    bw.Write(itemBufferTableBuffer[i]);

                // バイナリライタを Flush してからアイテム バッファを書き込む
                bw.Flush();
                for (var i = 0; i < props.Sections.Count; i++)
                {
                    var section = props.Sections[i];
                    for (var j = 0; j < section.Items.Count; j++)
                    {
                        var item = section.Items[j];

                        // Reference アルゴリズム未実装
                        var bufferMode = PropItemBufferMode.Buffered;
                        if (item.IsNull)
                            bufferMode = PropItemBufferMode.Null;
                        _binBuilder.WriteItemBuffer(this._stream, section.Items[j], bufferMode);
                    }
                }
            }

            this._stream.Flush();
        }

        public void Dispose()
        {
            if (!this._leaveOpen)
                this._stream.Dispose();
            this._isDisposed = true;
        }
    }
}
