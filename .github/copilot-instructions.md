# Copilot instructions — GH-300 demo repository (.NET edition)

## Project context
This is a small demo repository used to showcase GitHub Copilot capabilities.
It contains a legacy SQLite-backed inventory module and a shopping-cart module,
both intentionally imperfect so that Copilot has real work to do. It is a .NET 8
(C#) port of the original JavaScript + Python demo kit.

## Coding standards
- C#: target .NET 8, enable nullable reference types, prefer `records` for value objects,
  raise specific exceptions rather than returning sentinel values.
- Use file-scoped namespaces and `var` only when the type is obvious.
- Every public type and method needs an XML doc comment (`///`) explaining parameters,
  return value and thrown exceptions.
- Currency is always handled in integer cents, never `double`/`float`.

## Testing standards
- Tests use `xUnit`, live under `tests/`, and are named `<Type>Tests.cs`.
- Every test file must cover: the happy path, at least two boundary values, and one failure path.
- Parametrise boundary values with `[Theory]`/`[InlineData]` rather than copy-pasting cases.
- Never write a test that asserts on a private helper.

## Security expectations
- Never build SQL or shell commands with string concatenation or `string.Format`;
  use parameterised commands (`SqliteParameter`).
- Validate and normalise all external input at the boundary.
- Never log secrets, tokens or full payment details.

## Review standards
When reviewing, comment only on correctness, security, and public-API clarity.
Do not comment on formatting — that is handled by the formatter / `dotnet format`.
