using System.ComponentModel.DataAnnotations;

namespace ndisforms.Data.Models
{
    public class IR_Header
    {
        [Key]
        public int Id { get; set; }
        public Guid Report_guid { get; set; }
        public string? Report_number { get; set; }
        public bool Toi_reportable_incident { get; set; }
        public bool Toi_ndis { get; set; }
        public bool Toi_other_authority { get; set; }
        public string? Toi_other_authority_text { get; set; }
        public string? Nosmpr_name_of_witness { get; set; }
        public bool Nosmpr_rpt_rltd_hazard { get; set; }
        public bool Nosmpr_rpt_rltd_nearmiss { get; set; }
        public bool Nosmpr_rpt_rltd_incident { get; set; }
        public bool Nosmpr_rpt_rltd_concernchange { get; set; }
        public DateTime? Nosmpr_rpt_dtm { get; set; }
        public string? Nosmpr_rpt_location { get; set; }
        public string? Nosmpr_rpt_rltd_nameofclient { get; set; }
        public string? Doibr_text { get; set; }
        public DateTime RC_CR_DTM { get; set; }
    }
}
