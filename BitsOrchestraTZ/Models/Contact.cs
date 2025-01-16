using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BitsOrchestraTZ.Models
{
    public class Contact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int ContactID { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public bool Married { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        public string Phone { get; set; } = null!;
        public decimal Salary { get; set; }
    }
}
