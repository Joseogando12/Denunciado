﻿using Denounces.Domain.Helpers;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Denounces.Domain.Entities
{
    public class Vote : AuditEntity, IBaseEntity
    {   
        public bool IsFavorVote { get; set; }

        public bool IsNegativeVote { get; set; }

        public bool IsNullVote { get; set; }

        public long ProposalId { get; set; }
        public Proposal Proposal { get; set; }
    }
}
