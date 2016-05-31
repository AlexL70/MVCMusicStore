using System;
using System.ComponentModel.DataAnnotations;

namespace MVCMusicStore.Models
{
    public class Cart
    {
        public Cart() : base() { }
        public Cart(int recordId, string cartId, int albumId, int count, DateTime created)
        {
            RecordId = recordId;
            CartId = cartId;
            AlbumId = albumId;
            Count = count;
            DateCreated = created;
        }

        [Key]
        public int RecordId { get; set; }
        public string CartId { get; set; }
        public int AlbumId { get; set; }
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Album Album { get; set; }
    }
}
