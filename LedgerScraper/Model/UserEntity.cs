using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace LedgerScraper.Model
{
    class UserEntity : TableEntity
    {
        public int af_score { get; set; }
        public int af_rank { get; set; }
        public int gh_score { get; set; }
        public int gh_rank { get; set; }
        public int ma_score { get; set; }
        public int ma_rank { get; set; }
        public int os_score { get; set; }
        public int os_rank { get; set; }
        public int rb_score { get; set; }
        public int rb_rank { get; set; }
    }
}
