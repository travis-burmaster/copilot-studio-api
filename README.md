# Copilot Studio API

A .NET API for interacting with Microsoft Copilot Studio using direct-to-engine settings and Azure AD authentication. This API provides a simple interface to communicate with your Copilot Studio bots.

## Features

- Direct integration with Microsoft Copilot Studio
- Azure AD authentication using DefaultAzureCredential
- Configurable bot and environment settings
- Swagger UI for API documentation
- Error handling and logging
- Context support for conversations

## Prerequisites

- .NET 7.0 or later
- Azure AD application registration
- Microsoft Copilot Studio environment and bot

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

## Getting Started

1. Clone the repository
2. Update the configuration in `appsettings.json`
3. Run the application
4. Access Swagger UI at `/swagger` to test the API

## Authentication

The API uses Azure AD authentication with DefaultAzureCredential. Make sure your application has the necessary permissions configured in Azure AD.

## Error Handling

The API includes comprehensive error handling and logging. Errors are logged and appropriate HTTP status codes are returned.

## License

MIT