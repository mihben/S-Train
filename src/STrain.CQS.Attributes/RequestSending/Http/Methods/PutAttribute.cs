﻿using STrain.CQS.Attributes.RequestSending.Http;

namespace STrain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PutAttribute : MethodAttribute
    {
        public PutAttribute()
            : this(string.Empty)
        {

        }
        public PutAttribute(string path)
            : base(path, HttpMethod.Put)
        {
        }
    }
}
