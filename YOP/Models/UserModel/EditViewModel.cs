using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YOP.Models.UserModel
{
    public class EditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is incorrect")]
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public bool ShowBirthday { get; set; }
        public string Phone { get; set; }
        public bool ShowPhone { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsBanned { get; set; }
    }
}
