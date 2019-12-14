using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            var itemBufferTable = new Dictionary<PropItem, ulong>();
            for (var i = 0u; i < itemCount; i++)
            {
                var name = this._readString(br);
                var itemBufferStartOffset = br.ReadUInt64();

                var item = new PropItem(name, PropType.Buffer, null);
                itemBufferTable.Add(item, itemBufferStartOffset);
            }

            if (!completePropInfo)
                return itemBufferTable;

            

            return itemBufferTable;
        }

        public void LoadItemBuffer(BinaryReader br, PropItem item, bool skipValueData)
        {
            var itemType = (PropType)br.ReadUInt16();
            var bufferMode = (PropItemBufferMode)br.ReadByte();

            // TODO: アイテムの詳細をバイナリから読み取る実装
            throw new NotImplementedException();
        }
    }
}
