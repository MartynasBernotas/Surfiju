using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevKnowledgeBase.Domain.Entities
{
    public class TripMember
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public Trip Trip { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; }
    }
}
