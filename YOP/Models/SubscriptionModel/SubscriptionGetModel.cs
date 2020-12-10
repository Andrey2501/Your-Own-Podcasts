using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YOP.Models.SubscriptionModel
{
    public class SubscriptionGetModel: Subscription
    {
        public int CountLikes { get; set; } = 0;
        public string AuthorName { get; set; }
    }
}
