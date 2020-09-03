using AppleBananaCommon;
using MassTransit;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apple
{
    [Serializable]
    public class AppleBananaConsumer :IConsumer<AppleBananaEvent>
    {
        public static string received;

        public async Task Consume(ConsumeContext<AppleBananaEvent> context)
        {
            var receivedMessage = ((MassTransit.Context.ConsumeContextScope<AppleBananaEvent>)context).Message;
            JavaScriptSerializer js = new JavaScriptSerializer();
            received = js.Serialize(receivedMessage);
        }
    }
}
