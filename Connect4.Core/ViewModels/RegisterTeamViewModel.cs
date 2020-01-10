using System.ComponentModel.DataAnnotations;

namespace Connect4.Core.ViewModels
{
    public class RegisterTeamViewModel
    {
        /// <summary>
        ///     The unique name you have picked for your team
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string TeamName { get; set; } = default!;

        /// <summary>
        ///     The secret password you have picked for your team.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = default!;
    }
}
