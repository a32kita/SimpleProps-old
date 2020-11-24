using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleProps.Internal
{
    internal class PropBinaryBuilder
    {
        // 非公開フィールド
        private Encoding _encoding;


        // コンストラクタ

        /// <summary>
        /// <see cref="PropBinaryBuilder"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="encoding"></param>
        public PropBinaryBuilder(Encoding encoding)
        {
            this._encoding = encoding;
        }


        // 非公開メソッド

        /// <summary>
        /// バイト長付きバイト配列を書き込みます。
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="value"></param>
        private void _writeByteArray(BinaryWriter bw, byte[] value)
        {
            bw.Write((ulong)value.Length);
            bw.Write(value);
        }

        /// <summary>
        /// バイト長付き文字列を書き込みます。
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="value"></param>
        private void _writeString(BinaryWriter bw, string value)
        {
            var valueBuf = this._encoding.GetBytes(value);
            this._writeByteArray(bw, valueBuf);
        }

        /// <summary>
        /// バイト長付きバッファ反転文字列を書き込みます。
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="value"></param>
        private void _writeInversedString(BinaryWriter bw, string value)
        {
            var valueBuf = this._encoding.GetBytes(value);
            this._writeByteArray(bw, valueBuf.Select(b => (byte)(Byte.MaxValue - b)).ToArray());
        }

        /// <summary>
        /// uint 型の配列を書き込みます。
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="values"></param>
        private void _writeInt16Array(BinaryWriter bw, Int16[] values)
        {
            bw.Write((ulong)values.Length);
            foreach (var v in values)
            {
                bw.Write(v);
            }
        }

        /// <summary>
        /// double 型の配列を書き込みます。
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="values"></param>
        private void _writeDoubleArray(BinaryWriter bw, Double[] values)
        {
            bw.Write((ulong)values.Length);
            foreach (var v in values)
            {
                bw.Write(v);
            }
        }

        /// <summary>
        /// <see cref="DateTime"/> 構造体を書き込みます。
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="value"></param>
        private void _writeDateTime(BinaryWriter bw, DateTime value)
        {
            // DateTime を構成する全要素は Int32
            bw.Write(value.Year);
            bw.Write(value.Month);
            bw.Write(value.Day);
            bw.Write(value.Hour);
            bw.Write(value.Minute);
            bw.Write(value.Second);
            bw.Write(value.Millisecond);
        }


        // 公開メソッド

        /// <summary>
        /// セクション テーブルのバッファを生成します。バッファは、セクションの数と名前が同一である場合、同じ長さになります。
        /// </summary>
        /// <param name="sections"></param>
        /// <param name="itemBufferTableStartOffset">アイテム バッファ テーブル セクション内でのアイテム バッファ テーブルの開始オフセット</param>
        /// <returns></returns>
        public byte[] CreateSectionTable(PropSectionCollection sections, ulong[] itemBufferTableStartOffset)
        {
            if (sections.Count != itemBufferTableStartOffset.Length)
                throw new ArgumentException("Internal Error", nameof(itemBufferTableStartOffset));

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write((uint)sections.Count); // セクション数

#if DEBUG
                Console.WriteLine("== CREATE SECTION TABLE ==");
                Console.WriteLine("SectionsCount: {0}", sections.Count);
#endif

                foreach (var elem in sections.Select((sec, idx) => new { sec, itemTableStartOffset = itemBufferTableStartOffset[idx] }))
                {
                    this._writeString(bw, elem.sec.Name); // セクション名 (バッファ長付き)
                    bw.Write(elem.itemTableStartOffset);  // アイテム テーブルの開始オフセット

#if DEBUG
                    Console.WriteLine("{0}={1}", elem.sec.Name, elem.itemTableStartOffset);
#endif
                }

                bw.Flush();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// アイテム バッファ テーブルのバッファを生成します。バッファは、アイテムの数と名前が同一である場合、同じ長さになります。
        /// </summary>
        /// <param name="items"></param>
        /// <param name="itemBufferStartOffset"></param>
        /// <returns></returns>
        public byte[] CreateItemBufferTable(PropItemCollection items, ulong[] itemBufferStartOffset)
        {
            if (items.Count != itemBufferStartOffset.Length)
                throw new ArgumentException("Internal Error", nameof(itemBufferStartOffset));

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write((uint)items.Count); // アイテム数

#if DEBUG
                Console.WriteLine("== CREATE ITEM BUFFER TABLE ==");
#endif
                foreach (var elem in items.Select((item, idx) => new { item, itemBufferOffset = itemBufferStartOffset[idx] }))
                {
#if DEBUG
                    Console.WriteLine("{0}={1}", elem.item.Name, elem.itemBufferOffset);
#endif

                    this._writeString(bw, elem.item.Name); // アイテム名 (バッファ長付き)
                    bw.Write(elem.itemBufferOffset);       // アイテム バッファの開始オフセット
                }

                bw.Flush();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 指定した <see cref="Stream"/> にアイテム バッファを書き込みます。
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="item"></param>
        /// <param name="bufferMode"></param>
        /// <param name="referenceOffset"></param>
        public void WriteItemBuffer(Stream outputStream, PropItem item, PropItemBufferMode bufferMode, ulong? referenceOffset = null)
        {
            if (bufferMode == PropItemBufferMode.Reference && !referenceOffset.HasValue)
                throw new ArgumentException("Internal Error", nameof(referenceOffset));
            if (bufferMode == PropItemBufferMode.Null && !item.IsNull ||
                bufferMode != PropItemBufferMode.Null && item.IsNull)
                throw new InvalidOperationException();
            
            using (var bw = new BinaryWriter(outputStream, this._encoding, true))
            {
                //var prePosition = (ulong)outputStream.Position;
                bw.Write((ushort)item.Type);
                bw.Write((byte)bufferMode);

                if (bufferMode == PropItemBufferMode.Null)
                {
                    // 何もしない
                }
                else if (bufferMode == PropItemBufferMode.Reference)
                    bw.Write(referenceOffset.Value);
                else if (bufferMode == PropItemBufferMode.Buffered)
                {
                    switch (item.Type)
                    {
                        case PropType.Int16:
                            bw.Write((Int16)item.Value);
                            break;
                        case PropType.Int32:
                            bw.Write((Int32)item.Value);
                            break;
                        case PropType.Int64:
                            bw.Write((Int64)item.Value);
                            break;
                        case PropType.Double:
                            bw.Write((Double)item.Value);
                            break;
                        case PropType.String:
                            this._writeString(bw, (string)item.Value);
                            break;
                        case PropType.InversedString:
                            this._writeInversedString(bw, (string)item.Value);
                            break;
                        case PropType.DateTime:
                            this._writeDateTime(bw, (DateTime)item.Value);
                            break;
                        case PropType.Guid:
                            bw.Write(((Guid)item.Value).ToByteArray());
                            break;
                        case PropType.Buffer:
                            this._writeByteArray(bw, (byte[])item.Value);
                            break;
                        case PropType.Int16Array:
                            this._writeInt16Array(bw, (Int16[])item.Value);
                            break;
                        case PropType.DoubleArray:
                            this._writeDoubleArray(bw, (Double[])item.Value);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                    throw new NotImplementedException();

                bw.Flush(); // using が抜けたら Flush が走るので不要？
            }
        }
    }
}
