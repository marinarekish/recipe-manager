# Result Convention

Every service method that can fail for a well-known reason returns
`Result` (void) or `Result<T>` instead of throwing exceptions or returning
`null`. Controllers map these to HTTP responses via `ToActionResult()`.

## Result → HTTP mapping

`ResultExtensions.ToActionResult()` in `Api/Extensions/ResultExtensions.cs`
defines the mapping. Every controller uses it; no per-exception `try/catch`
blocks.

| `ResultStatus`     | HTTP status          | Response body                                        |
| ------------------ | -------------------- | ---------------------------------------------------- |
| `Ok`               | 200                  | `{ value }` for `Result<T>`, bare 200 for `Result`   |
| `NotFound`         | 404                  | `{ message }` if set, bare 404 otherwise             |
| `NoContent`        | 204                  | empty body                                           |
| `ValidationError`  | 400                  | `{ message, errors[] }`                              |
| `Conflict`         | 409                  | `{ message }`                                        |
| `Forbidden`        | 403                  | no body (standard ASP.NET Core `ForbidResult`)       |
| `Unauthorized`     | 401                  | no body (standard ASP.NET Core `UnauthorizedResult`) |

`CreatedAtRoute` (201) is returned explicitly by the controller when
`result.IsSuccess` — it is not a `ResultStatus` value because it carries
controller-specific logic (Location header).

## Factory methods

```csharp
Result.Ok()
Result.NotFound("optional message")
Result.ValidationError("message")
Result.ValidationError(IEnumerable<string> errors)
Result.Conflict("optional message")
Result.Forbidden("optional message")
Result.Unauthorized("optional message")

Result<T>.Ok(value)
Result<T>.NotFound("optional message")
Result<T>.ValidationError("message")
Result<T>.Conflict("optional message")
Result<T>.Forbidden("optional message")
Result<T>.Unauthorized("optional message")
```

## Controller convention

```csharp
[HttpPost]
public async Task<IActionResult> Create(...)
{
    var result = await service.CreateAsync(...);

    if (!result.IsSuccess)
        return result.ToActionResult();

    // 201 with Location header
    return CreatedAtRoute("GetById", new { id = result.Value!.Id }, result.Value);
}

[HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(int id, ...)
{
    var result = await service.DeleteAsync(id, ...);
    return result.IsSuccess ? NoContent() : result.ToActionResult();
}

[HttpGet("{id:int}")]
public async Task<IActionResult> GetById(int id, ...)
{
    var result = await service.GetByIdAsync(id, ...);
    return result.ToActionResult();
}
```

Key rules:

- Action return type is `IActionResult` when using `ToActionResult()`.
- List endpoints that always succeed (`GetAllAsync`) stay as
  `Task<ActionResult<List<T>>>` — no Result wrapper.
- `CreatedAtRoute` / `NoContent` are returned explicitly on success;
  errors go through `ToActionResult()`.

## When to use each status

| Situation                          | Status          |
| ---------------------------------- | --------------- |
| Resource does not exist            | `NotFound`      |
| Input fails validation             | `ValidationError` |
| Duplicate / already exists         | `Conflict`      |
| Caller lacks required permission   | `Forbidden`     |
| Caller is not authenticated        | `Unauthorized`  |
| Unexpected server error            | throw (mapped to 500 by framework) |
