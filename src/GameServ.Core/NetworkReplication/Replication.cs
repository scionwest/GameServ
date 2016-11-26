using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameServ.Core.NetworkReplication
{
    public class Replication<T> : Replication
    {
        private T value;

        public Replication(T initialValue, string propertyName)
        {
            this.Value = initialValue;
            this.Property = propertyName;
        }

        public string Property { get; set; }

        public T Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
            }
        }
    }

    public class Replication
    {
        public static Replication<TDataType> Bind<TOwner, TDataType>([CallerMemberName] string property = "")
        {
            return new Replication<TDataType>(default(TDataType), property);
        }
    }
}
