using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleProps
{
    public class PropSectionCollection : List<PropSection>
    {
        // 公開プロパティ

        public PropSection this[string sectionName]
        {
            get => this.Where(s => s.Name == sectionName).First();
        }
    }
}
