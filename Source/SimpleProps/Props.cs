using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleProps
{
    public class Props
    {
        // 公開プロパティ

        public PropSectionCollection Sections
        {
            get;
            private set;
        }


        // コンストラクタ

        public Props()
            : this(new PropSection[0])
        {
            // 実装なし
        }

        public Props(IEnumerable<PropSection> sections)
        {
            this.Sections = new PropSectionCollection();
            this.Sections.AddRange(sections);
        }
    }
}
