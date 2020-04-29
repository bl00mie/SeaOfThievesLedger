using Microsoft.Azure.Cosmos.Table;

namespace LedgerScraper.Model
{
    class LedgerEntity : TableEntity
    {
// *************************************************************
        public string af_0_hi_player { get; set; }
        public long af_0_hi_rank { get; set; }
        public long af_0_hi_score { get; set; }
        public string af_0_lo_player { get; set; }
        public long af_0_lo_rank { get; set; }
        public long af_0_lo_score { get; set; }

        public string af_1_hi_player { get; set; }
        public long af_1_hi_rank { get; set; }
        public long af_1_hi_score { get; set; }
        public string af_1_lo_player { get; set; }
        public long af_1_lo_rank { get; set; }
        public long af_1_lo_score { get; set; }

        public string af_2_hi_player { get; set; }
        public long af_2_hi_rank { get; set; }
        public long af_2_hi_score { get; set; }
        public string af_2_lo_player { get; set; }
        public long af_2_lo_rank { get; set; }
        public long af_2_lo_score { get; set; }

        public string af_3_hi_player { get; set; }
        public long af_3_hi_rank { get; set; }
        public long af_3_hi_score { get; set; }
        public string af_3_lo_player { get; set; }
        public long af_3_lo_rank { get; set; }
        public long af_3_lo_score { get; set; }

        // *************************************************************
        public string gh_0_hi_player { get; set; }
        public long gh_0_hi_rank { get; set; }
        public long gh_0_hi_score { get; set; }
        public string gh_0_lo_player { get; set; }
        public long gh_0_lo_rank { get; set; }
        public long gh_0_lo_score { get; set; }

        public string gh_1_hi_player { get; set; }
        public long gh_1_hi_rank { get; set; }
        public long gh_1_hi_score { get; set; }
        public string gh_1_lo_player { get; set; }
        public long gh_1_lo_rank { get; set; }
        public long gh_1_lo_score { get; set; }

        public string gh_2_hi_player { get; set; }
        public long gh_2_hi_rank { get; set; }
        public long gh_2_hi_score { get; set; }
        public string gh_2_lo_player { get; set; }
        public long gh_2_lo_rank { get; set; }
        public long gh_2_lo_score { get; set; }

        public string gh_3_hi_player { get; set; }
        public long gh_3_hi_rank { get; set; }
        public long gh_3_hi_score { get; set; }
        public string gh_3_lo_player { get; set; }
        public long gh_3_lo_rank { get; set; }
        public long gh_3_lo_score { get; set; }

        // *************************************************************
        public string ma_0_hi_player { get; set; }
        public long ma_0_hi_rank { get; set; }
        public long ma_0_hi_score { get; set; }
        public string ma_0_lo_player { get; set; }
        public long ma_0_lo_rank { get; set; }
        public long ma_0_lo_score { get; set; }

        public string ma_1_hi_player { get; set; }
        public long ma_1_hi_rank { get; set; }
        public long ma_1_hi_score { get; set; }
        public string ma_1_lo_player { get; set; }
        public long ma_1_lo_rank { get; set; }
        public long ma_1_lo_score { get; set; }

        public string ma_2_hi_player { get; set; }
        public long ma_2_hi_rank { get; set; }
        public long ma_2_hi_score { get; set; }
        public string ma_2_lo_player { get; set; }
        public long ma_2_lo_rank { get; set; }
        public long ma_2_lo_score { get; set; }

        public string ma_3_hi_player { get; set; }
        public long ma_3_hi_rank { get; set; }
        public long ma_3_hi_score { get; set; }
        public string ma_3_lo_player { get; set; }
        public long ma_3_lo_rank { get; set; }
        public long ma_3_lo_score { get; set; }

        // *************************************************************
        public string os_0_hi_player { get; set; }
        public long os_0_hi_rank { get; set; }
        public long os_0_hi_score { get; set; }
        public string os_0_lo_player { get; set; }
        public long os_0_lo_rank { get; set; }
        public long os_0_lo_score { get; set; }

        public string os_1_hi_player { get; set; }
        public long os_1_hi_rank { get; set; }
        public long os_1_hi_score { get; set; }
        public string os_1_lo_player { get; set; }
        public long os_1_lo_rank { get; set; }
        public long os_1_lo_score { get; set; }

        public string os_2_hi_player { get; set; }
        public long os_2_hi_rank { get; set; }
        public long os_2_hi_score { get; set; }
        public string os_2_lo_player { get; set; }
        public long os_2_lo_rank { get; set; }
        public long os_2_lo_score { get; set; }

        public string os_3_hi_player { get; set; }
        public long os_3_hi_rank { get; set; }
        public long os_3_hi_score { get; set; }
        public string os_3_lo_player { get; set; }
        public long os_3_lo_rank { get; set; }
        public long os_3_lo_score { get; set; }

        // *************************************************************
        public string rb_0_hi_player { get; set; }
        public long rb_0_hi_rank { get; set; }
        public long rb_0_hi_score { get; set; }
        public string rb_0_lo_player { get; set; }
        public long rb_0_lo_rank { get; set; }
        public long rb_0_lo_score { get; set; }

        public string rb_1_hi_player { get; set; }
        public long rb_1_hi_rank { get; set; }
        public long rb_1_hi_score { get; set; }
        public string rb_1_lo_player { get; set; }
        public long rb_1_lo_rank { get; set; }
        public long rb_1_lo_score { get; set; }

        public string rb_2_hi_player { get; set; }
        public long rb_2_hi_rank { get; set; }
        public long rb_2_hi_score { get; set; }
        public string rb_2_lo_player { get; set; }
        public long rb_2_lo_rank { get; set; }
        public long rb_2_lo_score { get; set; }

        public string rb_3_hi_player { get; set; }
        public long rb_3_hi_rank { get; set; }
        public long rb_3_hi_score { get; set; }
        public string rb_3_lo_player { get; set; }
        public long rb_3_lo_rank { get; set; }
        public long rb_3_lo_score { get; set; }

    }
}
