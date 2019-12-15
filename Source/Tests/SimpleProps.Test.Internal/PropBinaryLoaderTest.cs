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

        /// <summary>
        /// Test101 [正常処理] : 空のアイテム バッファ テーブルを読み取る。
        /// </summary>
        [TestMethod]
        public void Test101()
        {
            // UTF-8 でビルダ・ローダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);
            var binLoader = new PropBinaryLoader(encoding);

            // セクションの定義
            var items = new PropItemCollection()
            {
                // 0 件
            };

            // アイテム バッファ テーブルを生成
            var ms = new MemoryStream(binBuilder.CreateItemBufferTable(items, new ulong[items.Count]));
            ms.Seek(0, SeekOrigin.Begin);

            // 処理実行
            try
            {
                using (var br = new BinaryReader(ms))
                {
                    // value データ本体は読み取らない
                    var itemBufferTable = binLoader.LoadItemBufferTable(br, false);

                    // 0 件のセクション情報が格納されていることを確認
                    Assert.AreEqual(0, itemBufferTable.Count);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }

        /// <summary>
        /// Test102 [正常処理] : 5 件のアイテム バッファ テーブルを読み取る。
        /// </summary>
        [TestMethod]
        public void Test102()
        {
            // UTF-8 でビルダ・ローダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);
            var binLoader = new PropBinaryLoader(encoding);

            // セクションの定義
            var propItemNames = new string[]
            {
                "DEBUG_PROP_01",
                "DEBUG_PROP_02",
                "DEBUG_PROP_03",
                "DEBUG_PROP_04",
                "DEBUG_PROP_05",
            };

            var propItems = new PropItemCollection()
            {
                new PropItem(propItemNames[0], PropType.String, null),
                new PropItem(propItemNames[1], PropType.String, null),
                new PropItem(propItemNames[2], PropType.String, null),
                new PropItem(propItemNames[3], PropType.String, null),
                new PropItem(propItemNames[4], PropType.String, null),
            };

            // アイテム バッファ テーブルを生成
            var ms = new MemoryStream(binBuilder.CreateItemBufferTable(propItems, new ulong[propItems.Count]));
            ms.Seek(0, SeekOrigin.Begin);

            // 処理実行
            try
            {
                using (var br = new BinaryReader(ms))
                {
                    var propItemBufferTable = binLoader.LoadItemBufferTable(br, false);

                    // 5 件のセクション情報が格納されていることを確認
                    Assert.AreEqual(5, propItemBufferTable.Count);

                    // 各セクションのセクション名が正しいことを確認
                    var loadedNames = propItemBufferTable.Keys.Select(s => s.Name).ToArray();
                    for (var i = 0; i < 5; i++)
                    {
                        this.TestContext.WriteLine("expected: {0}, actual: {1}", propItemNames[i], loadedNames[i]);
                        Assert.AreEqual(propItemNames[i], loadedNames[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }

        /// <summary>
        /// Test201 [正常処理] : 1 件のアイテム バッファ (null) を読み取る。
        /// </summary>
        [TestMethod]
        public void Test201()
        {
            // UTF-8 でビルダ・ローダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);
            var binLoader = new PropBinaryLoader(encoding);

            // アイテムの定義
            var propItemName = "DEBUG_ITEM";
            var propItemType = PropType.String;
            string propItemValue = null;
            var propItem = new PropItem(propItemName, propItemType, propItemValue);

            // アイテム バッファの生成
            var ms = new MemoryStream();
            binBuilder.WriteItemBuffer(ms, propItem, PropItemBufferMode.Null);
            ms.Seek(0, SeekOrigin.Begin);

            // 処理実行
            try
            {
                using (var br = new BinaryReader(ms))
                {
                    // アイテム バッファ テーブルから読み取ったと想定するアイテム
                    var loadedItemFromBufferTable = new PropItem(propItemName, PropType.Buffer, null);

                    // フル ロード
                    var loadedItem = binLoader.LoadItemBuffer(br, loadedItemFromBufferTable, false);

                    // アイテム名が正しいことの確認
                    this.TestContext.WriteLine("Name> expected: {0}, actual: {1}", propItemName, loadedItem.Name);
                    Assert.AreEqual(propItemName, loadedItem.Name);

                    // タイプが正しいことの確認
                    this.TestContext.WriteLine("Type> expected: {0}, actual: {1}", propItemType, loadedItem.Type);
                    Assert.AreEqual(propItemType, loadedItem.Type);

                    // 値の内容の確認
                    this.TestContext.WriteLine("Value> expected: {0}, actual: {1}", propItemValue, loadedItem.Value);
                    Assert.AreEqual(propItemValue, loadedItem.Value);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }

        /// <summary>
        /// Test202 [正常処理] : 1 件のアイテム バッファ (String) を読み取る。
        /// </summary>
        [TestMethod]
        public void Test202()
        {
            // UTF-8 でビルダ・ローダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);
            var binLoader = new PropBinaryLoader(encoding);

            // アイテムの定義
            var propItemName = "DEBUG_ITEM";
            var propItemType = PropType.String;
            var propItemValue = "DEBUG_ITEM_VALUE";
            var propItem = new PropItem(propItemName, propItemType, propItemValue);

            // アイテム バッファの生成
            var ms = new MemoryStream();
            binBuilder.WriteItemBuffer(ms, propItem, PropItemBufferMode.Buffered);
            ms.Seek(0, SeekOrigin.Begin);

            // 処理実行
            try
            {
                using (var br = new BinaryReader(ms))
                {
                    // アイテム バッファ テーブルから読み取ったと想定するアイテム
                    var loadedItemFromBufferTable = new PropItem(propItemName, PropType.Buffer, null);

                    // フル ロード
                    var loadedItem = binLoader.LoadItemBuffer(br, loadedItemFromBufferTable, false);

                    // アイテム名が正しいことの確認
                    this.TestContext.WriteLine("Name> expected: {0}, actual: {1}", propItemName, loadedItem.Name);
                    Assert.AreEqual(propItemName, loadedItem.Name);

                    // タイプが正しいことの確認
                    this.TestContext.WriteLine("Type> expected: {0}, actual: {1}", propItemType, loadedItem.Type);
                    Assert.AreEqual(propItemType, loadedItem.Type);

                    // 値の内容の確認
                    this.TestContext.WriteLine("Value> expected: {0}, actual: {1}", propItemValue, loadedItem.Value);
                    Assert.AreEqual(propItemValue, loadedItem.Value);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }

        /// <summary>
        /// Test203 [正常処理] : 1 件のアイテム バッファ (InversedString) を読み取る。
        /// </summary>
        [TestMethod]
        public void Test203()
        {
            // UTF-8 でビルダ・ローダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);
            var binLoader = new PropBinaryLoader(encoding);

            // アイテムの定義
            var propItemName = "DEBUG_ITEM";
            var propItemType = PropType.InversedString;
            var propItemValue = "DEBUG_ITEM_VALUE";
            var propItem = new PropItem(propItemName, propItemType, propItemValue);

            // アイテム バッファの生成
            var ms = new MemoryStream();
            binBuilder.WriteItemBuffer(ms, propItem, PropItemBufferMode.Buffered);
            ms.Seek(0, SeekOrigin.Begin);

            // 処理実行
            try
            {
                using (var br = new BinaryReader(ms))
                {
                    // アイテム バッファ テーブルから読み取ったと想定するアイテム
                    var loadedItemFromBufferTable = new PropItem(propItemName, PropType.Buffer, null);

                    // フル ロード
                    var loadedItem = binLoader.LoadItemBuffer(br, loadedItemFromBufferTable, false);

                    // アイテム名が正しいことの確認
                    this.TestContext.WriteLine("Name> expected: {0}, actual: {1}", propItemName, loadedItem.Name);
                    Assert.AreEqual(propItemName, loadedItem.Name);

                    // タイプが正しいことの確認
                    this.TestContext.WriteLine("Type> expected: {0}, actual: {1}", propItemType, loadedItem.Type);
                    Assert.AreEqual(propItemType, loadedItem.Type);

                    // 値の内容の確認
                    this.TestContext.WriteLine("Value> expected: {0}, actual: {1}", propItemValue, loadedItem.Value);
                    Assert.AreEqual(propItemValue, loadedItem.Value);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }

        /// <summary>
        /// Test204 [正常処理] : 1 件のアイテム バッファ (DateTime) を読み取る。
        /// </summary>
        [TestMethod]
        public void Test204()
        {
            // UTF-8 でビルダ・ローダを初期化
            var encoding = Encoding.UTF8;
            var binBuilder = new PropBinaryBuilder(encoding);
            var binLoader = new PropBinaryLoader(encoding);

            // アイテムの定義
            var propItemName = "DEBUG_ITEM";
            var propItemType = PropType.DateTime;
            var propItemValue = new DateTime(1970, 1, 1, 12, 0, 0, 500);
            var propItem = new PropItem(propItemName, propItemType, propItemValue);

            // アイテム バッファの生成
            var ms = new MemoryStream();
            binBuilder.WriteItemBuffer(ms, propItem, PropItemBufferMode.Buffered);
            ms.Seek(0, SeekOrigin.Begin);

            // 処理実行
            try
            {
                using (var br = new BinaryReader(ms))
                {
                    // アイテム バッファ テーブルから読み取ったと想定するアイテム
                    var loadedItemFromBufferTable = new PropItem(propItemName, PropType.Buffer, null);

                    // フル ロード
                    var loadedItem = binLoader.LoadItemBuffer(br, loadedItemFromBufferTable, false);

                    // アイテム名が正しいことの確認
                    this.TestContext.WriteLine("Name> expected: {0}, actual: {1}", propItemName, loadedItem.Name);
                    Assert.AreEqual(propItemName, loadedItem.Name);

                    // タイプが正しいことの確認
                    this.TestContext.WriteLine("Type> expected: {0}, actual: {1}", propItemType, loadedItem.Type);
                    Assert.AreEqual(propItemType, loadedItem.Type);

                    // 値の内容の確認
                    this.TestContext.WriteLine("Value> expected: {0}, actual: {1}", propItemValue, loadedItem.Value);
                    Assert.AreEqual(propItemValue, loadedItem.Value);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"予期せぬエラー: {ex}");
            }
        }
    }
}
