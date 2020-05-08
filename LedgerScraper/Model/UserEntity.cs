using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace LedgerScraper.Model
{
    class UserEntity : TableEntity
    {
        public long af_score { get; set; }
        public long af_rank { get; set; }
        public long gh_score { get; set; }
        public long gh_rank { get; set; }
        public long ma_score { get; set; }
        public long ma_rank { get; set; }
        public long os_score { get; set; }
        public long os_rank { get; set; }
        public long rb_score { get; set; }
        public long rb_rank { get; set; }
    }
}
