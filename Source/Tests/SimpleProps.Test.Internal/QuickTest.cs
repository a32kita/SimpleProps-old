using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleProps.Test.Internal
{
    /// <summary>
    /// Test001 の概要の説明
    /// </summary>
    [TestClass]
    public class QuickTest
    {
        public QuickTest()
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
        /// Test001-TestMethod1.txt にデータを書き出します。
        /// </summary>
        [TestMethod]
        public void Test001()
        {
            //
            // TODO: テスト ロジックをここに追加してください
            //

            using (var fs = File.OpenWrite(".\\QuickTest-Test001.txt"))
            using (var pw = new PropWriter(fs))
            {
                pw.Write(new Props(new PropSectionCollection()
                {
                    new PropSection("PropSection01", new PropItemCollection()
                    {
                        new PropItem("PropItem01-001", PropType.String, "hello"),
                        new PropItem("PropItem01-002", PropType.String, "world!!"),
                        new PropItem("PropItem01-003", PropType.String, "こんにちは"),
                        new PropItem("PropItem01-004", PropType.DateTime, DateTime.Parse("1970/01/01 00:00:00.000")),
                    }),

                    new PropSection("PropSection02", new PropItemCollection()
                    {
                        new PropItem("PropItem02-001", PropType.String, "りんご"),
                        new PropItem("PropItem02-002", PropType.String, "アップル"),
                        new PropItem("PropItem02-003", PropType.String, "ｴｲｯﾎﾟｩ"),
                        new PropItem("PropItem02-004", PropType.Int64, Int64.MaxValue),
                    }),

                    new PropSection("PropSection03", new PropItemCollection()
                    {
                        new PropItem("PropItem03-001", PropType.InversedString, "りんご"),
                        new PropItem("PropItem03-002", PropType.InversedString, "林檎"),
                        new PropItem("PropItem03-003", PropType.InversedString, "Apple"),
                        new PropItem("PropItem03-004", PropType.Guid, Guid.Parse("35B9E3E5-FDCC-4F47-8C64-35DE4D521103")),
                    }),

                    new PropSection("PropSection04", new PropItemCollection()
                    {
                        new PropItem("PropItem03-001", PropType.InversedString, "私（わたくし）はその人を常に先生と呼んでいた。だからここでもただ先生と書くだけで本名は打ち明けない。これは世間を憚（はば）かる遠慮というよりも、その方が私にとって自然だからである。私はその人の記憶を呼び起すごとに、すぐ「先生」といいたくなる。筆を執（と）っても心持は同じ事である。よそよそしい頭文字（かしらもじ）などはとても使う気にならない。"),
                    }),
                }));
            }
        }

        /// <summary>
        /// <see cref="Test001"/> で書き出したファイルからデータを読み取ります。
        /// </summary>
        [TestMethod]
        public void Test002()
        {
            using (var fs = File.OpenRead(".\\QuickTest-Test001.txt"))
            using (var pr = new PropReader(fs))
            {
                var props = pr.ReadAllProps();
                foreach (var sect in props.Sections)
                {
                    this.TestContext.WriteLine("[{0}]", sect.Name);
                    foreach (var propItem in sect.Items)
                    {
                        this.TestContext.WriteLine("{0}={1}:{2}", propItem.Name, propItem.Type, propItem.Value);
                    }
                }
            }
        }

        [TestMethod]
        public void Test003()
        {
            var rotateCount = 0;
            var rotateWord = new Func<string>(() =>
            {
                var wordList = new string[]
                {
                    "トヨタ自動車",
                    "日産自動車",
                    "本田技研",
                    "スバル自動車",

                    "東急電鉄",
                    "東武鉄道",
                    "京成電鉄",
                    "京浜急行電鉄",
                    "相模鉄道",
                    "小田急電鉄",
                    "京王電鉄",
                    "西武鉄道",

                    "日本テレビ",
                    "フジテレビ",
                    "TBS",
                    "テレビ朝日",
                    "テレビ東京",
                };
                rotateCount = (rotateCount + 1) % wordList.Length;

                return wordList[rotateCount];
            });

            var props = new Props();
            for (var i = 0; i < 100; i++)
            {
                var sect = new PropSection(String.Format("DEBUG_SECT_{0:000}", i));
                for (var j = 0; j < 500; j++)
                {
                    var item = new PropItem(
                        String.Format("DEBUG_{0:000}-{1:0000}", i, j),
                        PropType.String,
                        rotateWord());
                    sect.Items.Add(item);
                }
                props.Sections.Add(sect);
            }

            using (var fs = File.OpenWrite(".\\QuickTest-Test003.txt"))
            using (var pw = new PropWriter(fs))
            {
                pw.Write(props);
            }
        }

        /// <summary>
        /// <see cref="Test003"/> で書き出したファイルからデータを読み取ります。
        /// </summary>
        [TestMethod]
        public void Test004()
        {
            using (var fs = File.OpenRead(".\\QuickTest-Test003.txt"))
            using (var pr = new PropReader(fs))
            {
                var props = pr.ReadAllProps();
                foreach (var sect in props.Sections)
                {
                    this.TestContext.WriteLine("[{0}]", sect.Name);
                    foreach (var propItem in sect.Items)
                    {
                        this.TestContext.WriteLine("{0}={1}:{2}", propItem.Name, propItem.Type, propItem.Value);
                    }
                }
            }
        }
    }
}
