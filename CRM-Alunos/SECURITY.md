# Security Policy

## Reporting Vulnerabilities

If you discover a security vulnerability in CRM Alunos, please report it responsibly.

**Do NOT open a public GitHub issue for security vulnerabilities.**

Instead, please email: [SECURITY_EMAIL_PLACEHOLDER]

## Scope

This is a desktop application for educational management. The following are considered in scope:

- SQL Injection (SQLite)
- File system vulnerabilities
- Authentication/authorization issues
- Remote code execution
- Data exposure

## Out of Scope

- Denial of service
- Social engineering
- Issues in third-party dependencies (report to upstream)

## Response Time

- Acknowledgment: within 48 hours
- Initial assessment: within 1 week
- Fix timeline: depends on severity

## Supported Versions

| Version | Supported |
|---------|-----------|
| Latest  | Yes |
| Older   | No |

## Security Best Practices

This application:
- Uses SQLite (file-based, no network exposure)
- Stores data locally on user machine
- No external API calls except for update checking
- No telemetry or data collection
- Open source code (auditable)

## Dependencies

All dependencies are scanned automatically via:
- **Dependabot** - Automated dependency updates
- **CodeQL** - Static code analysis
- **Secret Scanning** - Hardcoded credential detection

## License

This project is licensed under MIT License.
