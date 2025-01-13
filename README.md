# Copilot Studio API

A .NET API for interacting with Microsoft Copilot Studio using direct-to-engine settings through Dataverse. This API provides a simple interface to communicate with your Copilot Studio bots.

## Features

- Built with .NET 8.0
- Direct integration with Microsoft Copilot Studio via Dataverse
- Azure AD OAuth authentication
- Configurable bot and environment settings
- Swagger UI for API documentation
- Error handling and logging
- Context support for conversations
- Health check endpoint
- CORS support for development

## Prerequisites

- .NET 8.0 SDK
- Azure AD application registration
- Microsoft Copilot Studio environment and bot
- Dataverse environment

## Configuration

Update `appsettings.json` with your Copilot Studio settings:

```json
{
  "DirectToEngineSettings": {
    "EnvironmentId": "your-environment-id",
    "BotIdentifier": "your-bot-identifier",
    "TenantId": "your-tenant-id",
    "AppClientId": "your-app-client-id"
  }
}
```

## API Endpoints

### POST /api/copilot/chat

Send a message to your Copilot Studio bot.

Request body:
```json
{
    "message": "Your message here",
    "context": {
        "key1": "value1",
        "key2": "value2"
    }
}
```

Response:
```json
{
    "message": "Bot response",
    "conversationId": "conversation-id"
}
```

### GET /health

Check the health status of the API.

Response:
```json
{
    "status": "Healthy",
    "timestamp": "2025-01-13T21:43:14Z"
}
```

## Getting Started

1. Clone the repository
2. Update the configuration in `appsettings.json`
3. Run the application:
   ```bash
   cd src/CopilotStudioApi
   dotnet run
   ```
4. Access Swagger UI at `/swagger` to test the API

## Authentication

The API uses Azure AD OAuth authentication with Dataverse. Make sure your application has the necessary Dataverse API permissions configured in Azure AD:
- Dynamics CRM.user_impersonation

## Development

In development mode, the API includes:
- Swagger UI for testing endpoints
- CORS enabled for all origins (configurable)
- Detailed error messages

## Error Handling

The API includes comprehensive error handling and logging. Errors are logged and appropriate HTTP status codes are returned.

## License

MIT