using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleProps.Internal;

namespace SimpleProps.Test.Internal
{
    /// <summary>
    /// ビルダとローダの仕様が噛み合っていることを確認する
    /// </summary>
    [TestClass]
    public class PropBinaryLoaderTest
    {
        public PropBinaryLoaderTest()
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
        /// Test001 [正常処理] : 空のセクション テーブルを読み取る。
        /// </summary>
        [TestMethod]
        public void Test001()
        {
            // UTF-8 でビルダ・ローダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);
            var binLoader = new PropBinaryLoader(encoding);

            // セクションの定義
            var sections = new PropSectionCollection()
            {
                // 0 件
            };

            // セクション テーブルを生成
            var ms = new MemoryStream(binBuilder.CreateSectionTable(sections, new ulong[sections.Count]));
            ms.Seek(0, SeekOrigin.Begin);

            // 処理実行
            try
            {
                using (var br = new BinaryReader(ms))
                {
                    var sectionTable = binLoader.LoadSectionTable(br);

                    // 0 件のセクション情報が格納されていることを確認
                    Assert.AreEqual(0, sectionTable.Count);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }

        /// <summary>
        /// Test002 [正常処理] : 5 件分のセクション テーブルを読み取る。
        /// </summary>
        [TestMethod]
        public void Test002()
        {
            // UTF-8 でビルダ・ローダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);
            var binLoader = new PropBinaryLoader(encoding);

            // セクションの定義
            var sectionNames = new string[]
            {
                "DEBUG_SECTION_01",
                "DEBUG_SECTION_02",
                "DEBUG_SECTION_03",
                "DEBUG_SECTION_04",
                "DEBUG_SECTION_05",
            };

            var sections = new PropSectionCollection()
            {
                new PropSection(sectionNames[0]),
                new PropSection(sectionNames[1]),
                new PropSection(sectionNames[2]),
                new PropSection(sectionNames[3]),
                new PropSection(sectionNames[4]),
            };

            // セクション テーブルを生成
            var ms = new MemoryStream(binBuilder.CreateSectionTable(sections, new ulong[sections.Count]));
            ms.Seek(0, SeekOrigin.Begin);

            // 処理実行
            try
            {
                using (var br = new BinaryReader(ms))
                {
                    var sectionTable = binLoader.LoadSectionTable(br);

                    // 5 件のセクション情報が格納されていることを確認
                    Assert.AreEqual(5, sectionTable.Count);

                    // 各セクションのセクション名が正しいことを確認
                    var loadedNames = sectionTable.Keys.Select(s => s.Name).ToArray();
                    for (var i = 0; i < 5; i++)
                    {
                        this.TestContext.WriteLine("expected: {0}, actual: {1}", sectionNames[i], loadedNames[i]);
                        Assert.AreEqual(sectionNames[i], loadedNames[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }
    }
}
