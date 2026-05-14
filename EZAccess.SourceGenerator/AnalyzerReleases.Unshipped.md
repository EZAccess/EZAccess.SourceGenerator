; Unshipped analyzer release
; https://github.com/dotnet/roslyn/blob/main/docs/Adding%20Optional%20Parameters%20in%20Public%20API.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
EZ001 | CodeGenerator | Error | Empty DatabaseDef.json file
EZ002 | CodeGenerator | Error | invalid JSON in DatabaseDef.json
EZ003 | CodeGenerator | Error | Missing required property
EZ004 | CodeGenerator | Warning | Empty List