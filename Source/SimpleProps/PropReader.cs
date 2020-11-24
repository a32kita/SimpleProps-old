using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SimpleProps.Internal;

#if net35
// net35 でも leaveOpen を利用する
using BinaryReader = SimpleProps.Internal.ReaderWriterWrappers.BinaryReaderEx;
using BinaryWriter = SimpleProps.Internal.ReaderWriterWrappers.BinaryWriterEx;
#endif

namespace SimpleProps
{
    public class PropReader : IDisposable
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

                // アイテム バッファ テーブルを読み取る
                var abtStartPos = br.BaseStream.Position;
                var itemBufferTables = new List<Dictionary<PropItem, ulong>>();
                foreach (var kvp in sectionTable)
                {
                    // 読み取り開始位置の調整
                    var currentPosition = br.BaseStream.Position;
                    var actualStartPosition = abtStartPos + (long)kvp.Value;
                    if (currentPosition < actualStartPosition)
                    {
                        // 読み飛ばし処理: Seek を使わない
                        var distance = actualStartPosition - currentPosition;
                        for (var i = 0L; i < distance; i++)
                            br.ReadByte();
                    }
                    else if (currentPosition > actualStartPosition)
                    {
#if DEBUG
                        Console.WriteLine("== ITEM BUFFER TABLE LOADING ERROR ==");
                        Console.WriteLine("global:");
                        foreach (var elem in sectionTable)
                            Console.WriteLine("  {0}: {1}", elem.Key.Name, elem.Value);
                        Console.WriteLine();

                        Console.WriteLine("local:");
                        Console.WriteLine("  abtStartPos: {0}", abtStartPos);
                        Console.WriteLine("  kvp.Value: {0}", kvp.Value);
                        Console.WriteLine("  currentPosition: {0}", currentPosition);
                        Console.WriteLine("  actualStartPosition: {0}", actualStartPosition);
#endif

                        throw new IOException();
                    }

                    // 読み取る
                    // 非ランダム アクセス ストリームにも対応させるため、情報の完全化は無効にする
                    itemBufferTables.Add(this._binLoader.LoadItemBufferTable(br, false));
                }

                // アイテム バッファのロード
                for (var i = 0; i < itemBufferTables.Count; i++)
                {
                    var ibt = itemBufferTables[i];
                    var sect = sectionTable.Keys.ToArray()[i];

                    foreach (var kvp in ibt)
                    {
                        // フル ロードしてセクションに追加格納
                        var propItem = this._binLoader.LoadItemBuffer(br, kvp.Key, false);
                        sect.Items.Add(propItem);
                    }
                }

                return new Props(sectionTable.Keys);
            }
        }

        public void Dispose()
        {
            if (!this._leaveOpen)
                this._stream.Dispose();
            this._isDisposed = true;
        }
    }
}
