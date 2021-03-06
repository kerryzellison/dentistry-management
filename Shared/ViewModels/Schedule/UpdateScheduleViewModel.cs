using System;
using System.ComponentModel.DataAnnotations;

namespace DentistryManagement.Shared.ViewModels.Schedule
{
    public class UpdateScheduleViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
