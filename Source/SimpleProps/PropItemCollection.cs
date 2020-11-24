using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleProps
{
    public class PropItemCollection : List<PropItem>
    {
        // 公開プロパティ

        public PropItem this[string itemName]
        {
            get => this.Where(i => i.Name == itemName).First();
        }
    }
}
