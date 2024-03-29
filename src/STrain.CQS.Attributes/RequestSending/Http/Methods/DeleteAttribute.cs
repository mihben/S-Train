﻿using STrain.CQS.Attributes.RequestSending.Http;

namespace STrain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DeleteAttribute : MethodAttribute
    {
        public DeleteAttribute()
            : this(string.Empty)
        {

        }
        public DeleteAttribute(string path)
            : base(path, HttpMethod.Delete)
        {
        }
    }
}
