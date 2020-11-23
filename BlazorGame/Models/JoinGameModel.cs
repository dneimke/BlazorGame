using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorGame.Models
{
    public enum JoinMode { CreateNew, JoinExisting }

    public class JoinGameModel
    {
        [Required, EnumDataType(typeof(JoinMode))]
        public JoinMode? Mode { get; set; } = null;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(maximumLength:10, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 10 characters")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "A PIN is required")]
        [Range(1000, 9999, ErrorMessage = "A valid PIN code must be 4 digits")]
        public int PINCode { get; set; }
    }
}
