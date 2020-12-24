using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.QueryModels
{
    public class SubscriptionParameters: QueryStringParameters
    {
        public Guid UserId { get; set; }
    }
}
