using System;

namespace Larnaca.Blueprints.src.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class EndpointAttribute : Attribute
    {
        public EGrpcMethodType? GrpcMethodType { get; private set; }
        public EHttpVerb? HttpVerb { get; private set; }
        public EndpointAttribute() { }
        public EndpointAttribute(EGrpcMethodType grpcMethodType)
        {
            GrpcMethodType = grpcMethodType;
        }
        public EndpointAttribute(EHttpVerb httpVerb)
        {
            HttpVerb = httpVerb;
        }
    }
}
