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

        private void _writeByteArray(BinaryWriter bw, byte[] value)
        {
            bw.Write((ulong)value.Length);
            bw.Write(value);
        }

        private void _writeString(BinaryWriter bw, string value)
        {
            var valueBuf = this._encoding.GetBytes(value);
            this._writeByteArray(bw, valueBuf);
        }

        private void _writeInversedString(BinaryWriter bw, string value)
        {
            var valueBuf = this._encoding.GetBytes(value);
            this._writeByteArray(bw, valueBuf.Select(b => (byte)(Byte.MaxValue - b)).ToArray());
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

                foreach (var elem in sections.Select((sec, idx) => new { sec, itemTableStartOffset = itemBufferTableStartOffset[idx] }))
                {
                    this._writeString(bw, elem.sec.Name); // セクション名 (バッファ長付き)
                    bw.Write(elem.itemTableStartOffset);  // アイテム テーブルの開始オフセット
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

                foreach (var elem in items.Select((item, idx) => new { item, itemBufferOffset = itemBufferStartOffset[idx] }))
                {
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
                var currentPosition = outputStream.Position;
                bw.Write((ushort)item.Type);
                bw.Write((byte)bufferMode);

                if (bufferMode == PropItemBufferMode.Reference)
                    bw.Write(referenceOffset.Value);
                else
                {
                    switch (item.Type)
                    {
                        case PropType.String:
                            this._writeString(bw, (string)item.Value);
                            break;
                        case PropType.InversedString:
                            this._writeInversedString(bw, (string)item.Value);
                            break;
                        case PropType.Buffer:
                            this._writeByteArray(bw, (byte[])item.Value);
                            break;
                        case PropType.Int16:
                            bw.Write((Int16)item.Value);
                            break;
                    }
                }
            }
        }
    }
}
