using System.ComponentModel.DataAnnotations;

namespace ndisforms.Data.Models
{
    public class TblEmail
    {
        [Key]
        public int Id { get; set; }
        public string MailType { get; set; }
        public string MailIdType { get; set; }
        public string MailId { get; set; }
        public string EmailName { get; set; }   
    }
}
