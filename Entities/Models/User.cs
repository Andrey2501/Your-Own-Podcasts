using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    [Table("user")]
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime? Birthday { get; set; }
        public bool ShowBirthday { get; set; }
        public string Phone { get; set; }
        public bool ShowPhone { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsBanned { get; set; }

    }
}
