# SMS module

## Persisted entities

| Entity | SQL table | Responsibility |
| --- | --- | --- |
| `SmsMessage` | `sms.Messages` | Recipient, content/template, schedule, status, provider reference, receipt and next retry time |
| `SmsLog` | `sms.Logs` | One delivery attempt: provider reference, attempt number, start/completion time, result, safe retry classification, receipt and cost |
| `SmsProviderConfiguration` | `sms.ProviderConfigurations` | Provider account name/type, enabled state, credential configuration reference, sender line, timeout and template mappings |
| `SmsModuleConfiguration` | `sms.ModuleConfigurations` | Module enabled state, active provider configuration, batch size and retry policy |

SQL Server owns module and provider settings. Queueing selects the active provider configuration and pins its ID on each message. Changing the active provider affects new messages. Existing messages retain their provider configuration. Provider configuration changes (line, templates or credential reference) apply on the next attempt. Disabling a provider pauses its pending queue; disabling the module pauses both queueing and processing. The migration creates SmsIr and Kavenegar configurations with Kavenegar active, matching the approved setup.

Domain methods validate settings and template mappings. Configuration repositories support reading and saving the entities with row-version concurrency. There are no public configuration administration endpoints in this change.

## Credentials

Provider entities store a configuration **key**, never an API key. The seeded references are:

- `Sms:Providers:Kavenegar:ApiKey` (environment variable `Sms__Providers__Kavenegar__ApiKey`)
- `Sms:Providers:SmsIr:ApiKey` (environment variable `Sms__Providers__SmsIr__ApiKey`)

Set credentials through user secrets or environment variables for both API and worker. Empty API-key values in appsettings are placeholders. Delivery fails explicitly when the referenced key is absent. The old appsettings provider-selection/retry/line/template fields no longer control delivery; update the persisted entities instead. For an existing customized installation, review and transfer those values before switching to this version. No legacy business-data cleanup runs during startup.

Kavenegar numeric application template IDs map to template names through the provider entity's `SetTemplateMappings`. `Code` maps to `token`; explicit `token`, `token2`, `token3`, `token10`, and `token20` parameters are supported. Supplying both `Code` and `token` is rejected. SMS.ir uses native numeric template IDs. Provider contract: https://kavenegar.com/rest.html

Kavenegar uses a dedicated reusable HTTP transport without HTTP client logging because the API key appears in its URL. Each request uses the persisted timeout. SMS.ir waits are also bounded by the configured timeout; its SDK cannot cancel the underlying network operation. An uncertain outcome is never automatically resent.

## Processing and logs

Pricing.Worker hosts Hangfire Server and schedules `sms-outbox` every two minutes. Configure ConnectionStrings:SmsDb or DefaultConnection for both hosts; HangfireDb falls back to SmsDb, then DefaultConnection. Windows Service lifetime integration uses service name `Kahrad Pricing Worker`.

Application orchestrates processing. Before dispatch, one transaction saves the message claim and its Started log. After dispatch, another transaction saves both the final attempt result and message status. RowVersion prevents competing processors from claiming the same message. Each recipient gets its own provider request and receipt. Logs contain no message body, phone number, OTP, credential or raw exception/provider-response payload. Historical attempts are not fabricated for legacy messages.

`MaxAttempts` includes the initial send. Retry delay doubles after failed attempts, capped by MaxRetryDelaySeconds, without extending expiration or changing the original scheduled time. Only confirmed safe rejections retry: currently Kavenegar status 409. SMS.ir failures remain terminal until their retry safety is established. Authentication, invalid configuration and validation failures do not retry. A log's CanRetry records provider classification; the module policy and expiration still determine whether another attempt is scheduled.

Timeouts, connection failures and malformed responses produce Unknown attempt outcomes and terminal message failures. Interruption after claiming can leave a Sending message and Started log. Reconcile these against provider records before an explicitly authorized resend. Hangfire retries re-read SQL state and do not automatically resend Sent, Failed or Sending messages.

Authenticated endpoint `GET /api/sms/{id}/logs?page=1&pageSize=20` returns attempt DTOs in attempt order. Page size is limited to 100. Responses: 200, 400 for invalid pagination, 401 when unauthenticated, 404 for a missing message. The route follows the module's existing API route convention.

## Migrations and validation

`20261105130003_AddSmsLogsAndConfigurationEntities` creates provider configurations, singleton module configuration and logs, and adds the required provider-configuration foreign key to messages. It explicitly backfills existing message provider references and aborts on unknown legacy provider names. Previous migrations remain unchanged. Review deployment SQL before applying it to any other environment. Rollback drops settings and attempt history introduced by this migration.

Run unit tests:

```powershell
dotnet test Tests/UnitTests/Kahrad.Pricing.Sms.Tests
```

For the SQL integration test, set SMS_TEST_CONNECTION to an explicitly selected local/test database, then run:

```powershell
dotnet test Tests/IntegrationTests/Kahrad.Pricing.Sms.IntegrationTests
```

Without that variable, the integration test is skipped. The test uses a transaction that is rolled back and never calls a live provider. A live delivery smoke test still requires provider credentials.
