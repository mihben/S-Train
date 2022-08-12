﻿using STrain.CQS.Attributes.RequestSending.Http;

namespace STrain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PatchAttribute : MethodAttribute
    {
        public PatchAttribute(string path)
            : base(path, HttpMethod.Patch)
        {
        }
    }
}
