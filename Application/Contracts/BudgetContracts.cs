using System;
using System.Collections.Generic;

namespace QuizGameServer.Application.Contracts
{
    public class BudgetMonthsResponse
    {
        public List<BudgetMonthSummaryDto> Months { get; set; } = new();
    }

    public class BudgetMonthSummaryDto
    {
        public string Month { get; set; }
        public string Label { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BudgetStateResponse
    {
        public string Month { get; set; }
        public int Version { get; set; }
        public DateTime UpdatedAt { get; set; }
        public BudgetStateDto State { get; set; }
    }

    public class BudgetStateUpdateRequest
    {
        public int Version { get; set; }
        public BudgetStateDto State { get; set; }
    }

    public class BudgetStateDto
    {
        public List<BudgetMoneySourceDto> MoneySources { get; set; } = new();
        public List<BudgetTemplateDto> Templates { get; set; } = new();
        public List<BudgetHistoryEntryDto> History { get; set; } = new();
        public List<BudgetLogEntryDto> BudgetLog { get; set; } = new();
        public BudgetLogSnapshotDto? BudgetLogSnapshot { get; set; }
        public Dictionary<string, bool> BudgetLogBalanceLocks { get; set; } = new();
        public DateTime CurrentMonth { get; set; }
        public string MonthDescription { get; set; }
    }

    public class BudgetMoneySourceDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public decimal Spent { get; set; }
        public decimal Balance { get; set; }
        public DateTime? LastBalanceUpdate { get; set; }
    }

    public class BudgetTemplateDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, decimal> Changes { get; set; } = new();
    }

    public class BudgetHistoryEntryDto
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class BudgetLogEntryDto
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public Dictionary<string, decimal> Changes { get; set; } = new();
        public bool IsInitial { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BudgetLogSnapshotDto
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int EntryCount { get; set; }
        public List<BudgetLogEntryDto> Entries { get; set; } = new();
    }

    public class BudgetStartNextMonthRequest
    {
        public string FromMonth { get; set; }
    }

    public class BudgetSnapshotRequest
    {
        public string Month { get; set; }
        public BudgetLogSnapshotDto Snapshot { get; set; }
    }

    public class BudgetSnapshotResponse
    {
        public string Month { get; set; }
        public BudgetLogSnapshotDto Snapshot { get; set; }
    }
}
