    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    namespace BeautySpaceDomain.Model;

    public partial class Position : Entity
    {
        private string _name;

        [Required(ErrorMessage = "Введіть назву посади")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Назва посади не може містити у собі менше двох символів")]
        [Display(Name = "Посада")]
        public string Name
        {
            get => _name;
            set => _name = value.Substring(0, 1).ToUpperInvariant() + value.Substring(1);
        }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
