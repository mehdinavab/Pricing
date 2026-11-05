# API language and region

The API translates validation and error responses with `ApiMessages.resx`, `ApiMessages.fa-IR.resx` and `ApiMessages.ar-SA.resx`. Clients send a standard `Accept-Language` value such as `fa-IR`, `ar-SA` or `en-US`; quality weights are respected. If that header is absent or has no supported language, `X-Region: IR`, `SA` or `US` selects Persian, Arabic or English. `Accept-Language` takes priority. English is the default for API messages. Responses include `Content-Language` and `Vary: Accept-Language, X-Region`.

The public catalog keeps its configured default language when neither header is supplied. Its product translations use `Accept-Language`, or `X-Region` when no language header is supplied. Its `Content-Language` reports the language of the product data, which may differ from the API message culture when a requested catalog translation is unavailable.

Application and Domain exceptions retain their existing English message keys and remain independent of ASP.NET Core and resource packages. The API translates those keys at its response boundary. Messages containing a record identifier or code use resource templates. Unknown internal messages are replaced with a localized generic title rather than sent untranslated. Audit entries, developer diagnostics, external provider responses and stored SMS text are not translated as API messages.
