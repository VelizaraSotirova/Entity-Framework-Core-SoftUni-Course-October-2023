using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicHub.Data.Models
{
    public class SongPerformer
    {
        [ForeignKey(nameof(SongId))]
        public int SongId { get; set; }

        [Required]
        public virtual Song Song { get; set; }

        [ForeignKey(nameof(PerformerId))]
        public int PerformerId { get; set;}

        [Required]
        public virtual Performer Performer { get; set;}
    }
}