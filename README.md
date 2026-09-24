---

## Recurring Jobs (Hangfire)

Added 3 recurring background jobs that run automatically on schedules.

### Jobs

| Job ID | Description | Schedule (Cron) |
|---|---|---|
| `auto-close-stale-jobs` | Closes jobs that have been open for more than 30 days | `0 0 * * *` (Daily 12 AM) |
| `cleanup-cancelled-applications` | Deletes cancelled applications older than 60 days | `0 3 * * 0` (Sunday 3 AM) |
| `daily-jobs-report` | Logs a daily summary (new jobs, new applications, cancellations) | `0 9 * * *` (Daily 9 AM) |

### Files Added
JobApplication.Infrastructure/
└── BackgroundJobs/
├── CloseStaleJobsJob.cs
├── CleanupCancelledApplicationsJob.cs
├── DailyJobsReportJob.cs
└── RecurringJobsConfiguration.cs


### Changes

- Added `CreatedAt` property to `Job` entity (with migration).
- Registered the 3 jobs as scoped services in `Program.cs`.
- Registered `RecurringJobsConfiguration.RegisterRecurringJobs()` after Hangfire Dashboard.
- Hangfire Dashboard available at `/hangfire` → **Recurring Jobs** tab.
