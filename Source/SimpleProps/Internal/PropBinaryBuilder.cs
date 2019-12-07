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


        public PropBinaryBuilder(Encoding encoding)
        {
            this._encoding = encoding;
        }

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

        public byte[] CreateSectionTable(PropSectionCollection sections, ulong[] startOffset)
        {
            if (sections.Count != startOffset.Length)
                throw new ArgumentException();

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                foreach (var sec in sections.Select((sct, idx) => new { sct = sct, startOffset = startOffset[idx] }))
                {
                    this._writeString(bw, sec.sct.Name);  // アイテム名 (バッファ長付き)
                    bw.Write((uint)sec.sct.Items.Count);  // アイテム数
                }

                bw.Flush();
                return ms.ToArray();
            }
        }
    }
}
