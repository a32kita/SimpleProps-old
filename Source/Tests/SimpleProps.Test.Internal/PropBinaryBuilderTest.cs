using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleProps.Internal;

namespace SimpleProps.Test.Internal
{
    /// <summary>
    /// PropBinaryBuilderTest の概要の説明
    /// </summary>
    [TestClass]
    public class PropBinaryBuilderTest
    {
        public PropBinaryBuilderTest()
        {
            //
            // TODO: コンストラクター ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Test001 [正常処理] : 空のセクション テーブルを生成する。
        /// </summary>
        [TestMethod]
        public void Test001()
        {
            // UTF-8 でビルダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);

            // セクションの定義
            var sections = new PropSectionCollection()
            {
                // 0 件
            };

            // 処理実行
            try
            {
                var binResult = binBuilder.CreateSectionTable(sections, new ulong[sections.Count]);
                this.TestContext.WriteLine("Added sections = {0}", sections.Count);
                this.TestContext.WriteLine("Created buffer = {0} bytes", binResult.Length);
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }

        /// <summary>
        /// Test002 [正常処理] : 1 件分 (アイテム 0 件) のセクション テーブルを生成する。
        /// </summary>
        [TestMethod]
        public void Test002()
        {
            // UTF-8 でビルダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);

            // セクションの定義
            var sections = new PropSectionCollection()
            {
                new PropSection("DEBUG01"),
            };

            // 処理実行
            try
            {
                var binResult = binBuilder.CreateSectionTable(sections, new ulong[sections.Count]);
                this.TestContext.WriteLine("Added sections = {0}", sections.Count);
                this.TestContext.WriteLine("Created buffer = {0} bytes", binResult.Length);
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }

        /// <summary>
        /// Test003 [正常処理] : 2 件分 (アイテム 0 件) のセクション テーブルを、異なるオフセット指定で 2 回生成し、バイト長を比較する。
        /// </summary>
        [TestMethod]
        public void Test003()
        {
            // UTF-8 でビルダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);

            // セクションの定義
            var sections = new PropSectionCollection()
            {
                new PropSection("DEBUG01"),
                new PropSection("DEBUG02"),
            };

            // 処理実行
            try
            {
                var bufResult1 = binBuilder.CreateSectionTable(sections, new ulong[] { 0, 0 });
                var bufResult2 = binBuilder.CreateSectionTable(sections, new ulong[] { 1000, 2000 });

                Assert.IsTrue(bufResult1.Length == bufResult2.Length);
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }

        /// <summary>
        /// Test051 [異常処理] : オフセットの指定件数 0 で、 1 件分 (アイテム 0 件) のセクション テーブルを生成する。
        /// </summary>
        [TestMethod]
        public void Test051()
        {
            // UTF-8 でビルダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);

            // セクションの定義
            var sections = new PropSectionCollection()
            {
                new PropSection("DEBUG01"),
            };

            // 処理実行
            Exception thrown = null;
            try
            {
                var bufResult = binBuilder.CreateSectionTable(sections, new ulong[0]);
            }
            catch (Exception ex)
            {
                thrown = ex;

                this.TestContext.WriteLine("Thrown: {0}", ex);
            }

            Assert.IsTrue(thrown is ArgumentException);
        }

        /// <summary>
        /// Test101 [正常処理] : 空のアイテム バッファ テーブルを生成する。
        /// </summary>
        [TestMethod]
        public void Test101()
        {
            // UTF-8 でビルダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);

            // アイテム コレクションの定義
            var items = new PropItemCollection()
            {
                // 0 件
            };

            // 処理実行
            try
            {
                var binResult = binBuilder.CreateItemBufferTable(items, new ulong[items.Count]);
                this.TestContext.WriteLine("Added props = {0}", items.Count);
                this.TestContext.WriteLine("Created buffer = {0} bytes", binResult.Length);
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }

        /// <summary>
        /// Test102 [正常処理] : すべての PropType を含むアイテム バッファ テーブルを生成する。
        /// </summary>
        [TestMethod]
        public void Test102()
        {
            // UTF-8 でビルダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);

            // アイテム コレクションの定義
            var items = new PropItemCollection();

            // すべて null 値で追加
            var types = Enum.GetNames(typeof(PropType));
            foreach (var type in types)
                items.Add(new PropItem(type + "Prop", (PropType)Enum.Parse(typeof(PropType), type), null));

            // 処理実行
            try
            {
                var binResult = binBuilder.CreateItemBufferTable(items, new ulong[items.Count]);
                this.TestContext.WriteLine("Added props = {0}", items.Count);
                this.TestContext.WriteLine("Created buffer = {0} bytes", binResult.Length);
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }


    }
}
