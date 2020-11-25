using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#if net35
// net35 でも leaveOpen を利用する
using BinaryReader = SimpleProps.Internal.ReaderWriterWrappers.BinaryReaderEx;
using BinaryWriter = SimpleProps.Internal.ReaderWriterWrappers.BinaryWriterEx;
#endif

#if netstd
using SimpleProps.BclWrappers.System;
using SimpleProps.BclWrappers.System.Text;
#endif

namespace SimpleProps.Internal
{
    internal class PropBinaryLoader
    {
        // 非公開フィールド
        private Encoding _encoding;


        // コンストラクタ

        /// <summary>
        /// <see cref="PropBinaryLoader"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="encoding"></param>
        public PropBinaryLoader(Encoding encoding)
        {
            this._encoding = encoding;
        }


        // 非公開メソッド

        /// <summary>
        /// バイト長付きバイト配列を読み取ります。
        /// </summary>
        /// <param name="bw"></param>
        /// <returns></returns>
        private byte[] _readByteArray(BinaryReader br)
        {
            // Int32.Max を超える長さのバッファ読み取り対策非対応
            // …そもそもインメモリでやる処理ではない
            var bufLen = br.ReadUInt64();
            return br.ReadBytes((int)bufLen);
        }

        /// <summary>
        /// バイト長付き文字列を読み取ります。
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private string _readString(BinaryReader br)
        {
            var valueBuf = this._readByteArray(br);
            return this._encoding.GetString(valueBuf);
        }

        /// <summary>
        /// バイト長付きバッファ反転文字列を読み取ります。
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private string _readInversedString(BinaryReader br)
        {
            var valueBuf = this._readByteArray(br);
            for (var i = 0; i < valueBuf.Length; i++)
                valueBuf[i] = (byte)(Byte.MaxValue - valueBuf[i]);
            return this._encoding.GetString(valueBuf);
        }

        /// <summary>
        /// <see cref="DateTime"/> を読み取ります。
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        private DateTime _readDateTime(BinaryReader br)
        {
            return new DateTime(
                br.ReadInt32(), // Year
                br.ReadInt32(), // Month
                br.ReadInt32(), // Day
                br.ReadInt32(), // Hour
                br.ReadInt32(), // Minute
                br.ReadInt32(), // Second
                br.ReadInt32());// Millisecond
        }

        private Int16[] _readInt16Array(BinaryReader br)
        {
            var length = (int)br.ReadUInt64();
            var values = new short[length];
            for (var i = 0; i < length; i++)
                values[i] = br.ReadInt16();
            return values;
        }

        private Double[] _readDoubleArray(BinaryReader br)
        {
            var length = (int)br.ReadUInt64();
            var values = new double[length];
            for (var i = 0; i < length; i++)
                values[i] = br.ReadDouble();
            return values;
        }


        // 公開メソッド

        /// <summary>
        /// セクション テーブルを読み込みます。
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public Dictionary<PropSection, ulong> LoadSectionTable(BinaryReader br)
        {
            // セクション数の取得
            var sectionCount = br.ReadUInt32();

            // セクション情報の読み取り
            var sectionTable = new Dictionary<PropSection, ulong>();
            for (var i = 0u; i < sectionCount; i++)
            {
                var name = this._readString(br);            // セクション名
                var itemTableStartOffset = br.ReadUInt64(); // アイテム テーブルの開始オフセット

                var section = new PropSection(name);
                sectionTable.Add(section, itemTableStartOffset);
            }

            return sectionTable;
        }

        /// <summary>
        /// アイテム バッファ テーブルを読み込みます。
        /// </summary>
        /// <param name="br"></param>
        /// <param name="completePropInfo">実際のアイテム バッファに付記されているタイプ情報などを読み取るかどうかを指定します。このパラメータはシークが可能な Stream でのみ使用が可能です。</param>
        /// <returns></returns>
        public Dictionary<PropItem, ulong> LoadItemBufferTable(BinaryReader br, bool completePropInfo)
        {
            var st = br.BaseStream;
            if (completePropInfo && st.CanSeek)
                throw new InvalidOperationException("Internal Error");

            // アイテム数の取得
            var itemCount = br.ReadUInt32();

            // アイテム バッファ情報の読み取り

#if DEBUG
            Console.WriteLine("== LOAD ITEM BUFER TABLE ==");
#endif

            var itemBufferTable = new Dictionary<PropItem, ulong>();
            for (var i = 0u; i < itemCount; i++)
            {
                var name = this._readString(br);
                var itemBufferStartOffset = br.ReadUInt64();

#if DEBUG
                Console.WriteLine("{0}={1}", name, itemBufferStartOffset);
#endif

                var item = new PropItem(name, PropType.Buffer, null);
                itemBufferTable.Add(item, itemBufferStartOffset);
            }

            if (!completePropInfo)
                return itemBufferTable;

            // 各アイテムの詳細情報を取得する
            var itemBufferSectionStartOffset = st.Position; // アイテム バッファ テーブルの終了位置を開始位置とする
            foreach (var elem in itemBufferTable)
            {
                var bufStartOffset = itemBufferSectionStartOffset + (long)elem.Value;
                st.Seek(bufStartOffset, SeekOrigin.Begin);

                // value データ含めて完全にロード
                this.LoadItemBuffer(br, elem.Key, false);
            }

            return itemBufferTable;
        }

        /// <summary>
        /// アイテム バッファ本体を読み取ります。
        /// </summary>
        /// <param name="br"></param>
        /// <param name="item"></param>
        /// <param name="skipValueData"></param>
        /// <returns></returns>
        public PropItem LoadItemBuffer(BinaryReader br, PropItem item, bool skipValueData)
        {
            var propType = (PropType)br.ReadUInt16();
            var bufferMode = (PropItemBufferMode)br.ReadByte();

            var result = new PropItem(item.Name, propType, null);
            if (bufferMode == PropItemBufferMode.Null)
                return result;

            switch (propType)
            {
                case PropType.Int16:
                    result.Value = br.ReadInt16();
                    break;
                case PropType.Int32:
                    result.Value = br.ReadInt32();
                    break;
                case PropType.Int64:
                    result.Value = br.ReadInt64();
                    break;
                case PropType.Double:
                    result.Value = br.ReadDouble();
                    break;
                case PropType.String:
                    result.Value = this._readString(br);
                    break;
                case PropType.InversedString:
                    result.Value = this._readInversedString(br);
                    break;
                case PropType.DateTime:
                    result.Value = this._readDateTime(br);
                    break;
                case PropType.Guid:
                    result.Value = new Guid(br.ReadBytes(Guid.Empty.ToByteArray().Length));
                    break;
                case PropType.Buffer:
                    result.Value = this._readByteArray(br);
                    break;
                case PropType.Int16Array:
                    result.Value = this._readInt16Array(br);
                    break;
                case PropType.DoubleArray:
                    result.Value = this._readDoubleArray(br);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }
    }
}
