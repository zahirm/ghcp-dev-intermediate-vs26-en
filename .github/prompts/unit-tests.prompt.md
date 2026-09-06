---
mode: agent
description: Generate a complete xUnit suite for the selected type.
---
Generate an `xUnit` test suite for the type I have open.

Follow the repository standards in `.github/copilot-instructions.md`.

Structure the output as:
1. A short list of the behaviours you identified, before any code.
2. The test file itself, ready to save under `tests/`.
3. A table of the edge cases you deliberately covered and the ones you decided were out of scope.

Requirements:
- One `[Fact]` per behaviour, named `<Behaviour>`.
- Parametrise boundary values with `[Theory]`/`[InlineData]` rather than copy-pasting cases.
- Include at least one test that asserts the exception type and message (`Assert.Throws`).
- Do not modify the type under test.
