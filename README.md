# ChatbotGPT

A simple chat application that uses the Gemini API for generating responses.

## Features

- Modern React frontend with a clean UI
- .NET Core backend with CORS support
- Integration with Google's Gemini API
- Real-time chat interface
- Error handling and logging

## Prerequisites

- Node.js (v14 or higher)
- .NET Core SDK (v6.0 or higher)
- A Google Cloud account with Gemini API access
- Gemini API key

## Setup

1. Backend Setup:
```bash
cd backend
```

2. Configure the Gemini API key:
   - Open `appsettings.json`
   - Replace the `ApiKey` value in the `Gemini` section with your Gemini API key:
   ```json
   "Gemini": {
     "ApiKey": "your-api-key-here"
   }
   ```
   for OpenAI:
   - Replace the `ApiKey` value in the `OpenAI` section with your Gemini API key:
   ```json
   "OpenAI": {
     "ApiKey": "your-api-key-here"
   }
   ```

3. Start the backend server:
```bash
dotnet run
```
The backend will start on `http://localhost:5000`

4. Frontend Setup (in a new terminal):
```bash
cd frontend
npm install
npm start
```
The frontend will start on `http://localhost:3000`

## API Endpoints

### POST /gpt/chat
Sends a message to the Gemini API and returns the response.

Request body:
```json
{
  "message": "Your message here"
}
```

Response:
```json
{
  "response": "Gemini API response here"
}
```

## Error Handling

The application includes comprehensive error handling:
- Invalid API key
- Empty messages
- API request failures
- Response parsing errors

All errors are logged and returned with appropriate HTTP status codes.

## Logging


Logs are written to the console and can be configured in `appsettings.json`.

## Security

- API keys are stored in configuration files (not in source control)
- CORS is properly configured for development
- Input validation is performed on all requests
