using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetControlBackend.Models.UserModel
{
    public class UserViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "FirstName is required")]
        [StringLength(60, ErrorMessage = "FirstName can't be longer than 60 characters")]
        public string FirstName { get; set; }

        [StringLength(60, ErrorMessage = "LastName can't be longer than 60 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is incorrect")]
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public bool ShowBirthday { get; set; }

        [Phone(ErrorMessage = "Phone is incorrect")]
        public string Phone { get; set; }
        public bool ShowPhone { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsBanned { get; set; }
    }
}
