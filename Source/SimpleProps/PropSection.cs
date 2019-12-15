using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleProps
{
    public class PropSection
    {
        // 公開プロパティ

        public string Name
        {
            get;
            set;
        }

        public PropItemCollection Items
        {
            get;
            private set;
        }

        public PropItem this[string itemName]
        {
            get => this.Items.Single(item => item.Name == itemName);
        }


        // コンストラクタ

        /// <summary>
        /// <see cref="PropSection"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        public PropSection(string name)
            : this(name, new PropItem[0])
        {
            // 実装なし
        }

        /// <summary>
        /// <see cref="PropItem"/> のコレクションを指定して、 <see cref="PropSection"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="items"></param>
        public PropSection(string name, IEnumerable<PropItem> items)
        {
            this.Name = name;
            this.Items = new PropItemCollection();
            this.Items.AddRange(items);
        }
    }
}
