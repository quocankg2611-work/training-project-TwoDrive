# Copilot Instructions

## Project Guidelines
- Prefer built-in ASP.NET Core 10 endpoint validation via AddValidation over custom request validation helpers.
- Implement middleware as concrete middleware classes (constructor + InvokeAsync), similar to GlobalExceptionHandlingMiddleware, instead of using extension-method wrapper style for middleware behavior.