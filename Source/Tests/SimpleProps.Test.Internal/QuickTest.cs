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

            using (var fs = File.OpenWrite(".\\Test001-TestMethod1.txt"))
            using (var pr = new PropWriter(fs))
            {
                pr.Write(new Props(new PropSectionCollection()
                {
                    new PropSection("PropSection01", new PropItemCollection()
                    {
                        new PropItem("PropItem01-001", PropType.String, "hello"),
                        new PropItem("PropItem01-002", PropType.String, "world!!"),
                        new PropItem("PropItem01-003", PropType.String, "こんにちは"),
                        new PropItem("PropItem01-003", PropType.DateTime, DateTime.Parse("1970/01/01 00:00:00.000")),
                    }),

                    new PropSection("PropSection02", new PropItemCollection()
                    {
                        new PropItem("PropItem02-001", PropType.String, "りんご"),
                        new PropItem("PropItem02-002", PropType.String, "アップル"),
                        new PropItem("PropItem02-003", PropType.String, "ｴｲｯﾎﾟｩ"),
                    }),

                    new PropSection("PropSection03", new PropItemCollection()
                    {
                        new PropItem("PropItem03-001", PropType.InversedString, "りんご"),
                        new PropItem("PropItem03-002", PropType.InversedString, "林檎"),
                        new PropItem("PropItem03-003", PropType.InversedString, "Apple"),
                    })
                }));
            }
        }

        /// <summary>
        /// <see cref="Test001"/> で書き出したファイルからデータを読み取ります。
        /// </summary>
        public void Test002()
        {

        }
    }
}
