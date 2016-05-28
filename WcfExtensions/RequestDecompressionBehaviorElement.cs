using System;
using System.ServiceModel.Configuration;

namespace WCFExtensions
{
    public class RequestDecompressionBehaviorElement : BehaviorExtensionElement
    {
        public override Type BehaviorType => typeof(RequestDecompressionEndpointBehavior);

        protected override object CreateBehavior()
        {
            return new RequestDecompressionEndpointBehavior();
        }

    
    }
}