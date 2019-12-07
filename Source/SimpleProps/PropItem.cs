using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleProps
{
    public class PropItem
    {
        // 非公開フィールド
        private Object _value;


        // 公開プロパティ

        public string Name
        {
            get;
            set;
        }

        public PropType Type
        {
            get;
            private set;
        }

        public Object Value
        {
            get => this._value;
            set => this._setValue(value);
        }

        public bool IsNull
        {
            get => this.Value == null;
        }


        // コンストラクタ

        public PropItem(string name, PropType type, Object value)
        {
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }


        // 非公開メソッド

        private void _setValue(Object value)
        {
            if (PropTypeUtils.GetPropType(value) != this.Type)
                throw new ArgumentException();
            this._value = value;
        }
    }
}
