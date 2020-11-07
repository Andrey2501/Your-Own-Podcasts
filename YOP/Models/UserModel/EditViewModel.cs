using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetControlBackend.Models.UserModel
{
    public class EditViewModel
    {
        [Required]
        public Guid Id { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public bool ShowBirthday { get; set; }
        public string Phone { get; set; }
        public bool ShowPhone { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsBanned { get; set; }
    }
}
