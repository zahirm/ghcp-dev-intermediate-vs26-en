---
applyTo: "**/*.cs"
---
# C#-specific instructions

- Use `record` or `record struct` for value objects instead of anonymous types or bare classes.
- Prefer explicit `ArgumentException` / `KeyNotFoundException` over returning `null` or sentinels.
- Money is `int` cents. Never `double`/`float`.
- Log with `Microsoft.Extensions.Logging`, never `Console.WriteLine`.
- Any method longer than 30 lines should be decomposed.
- Enable and honour nullable reference types; annotate nullability explicitly.
