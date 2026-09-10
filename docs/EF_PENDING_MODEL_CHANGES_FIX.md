# EF Core pending model changes

The migration snapshot was incomplete after manually authored migrations. EF Core 10 correctly detects that the current model differs from `PersonalWealthDbContextModelSnapshot`.

Use EF tooling version 10.0.12 to keep the design-time tool aligned with the runtime. The repository CI validates the model and database integration tests against SQL Server.
