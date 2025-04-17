import React, { useState } from 'react';
import axios from 'axios';
import './App.css';

function App() {
  const [message, setMessage] = useState('');
  const [response, setResponse] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const result = await axios.post('http://localhost:5000/gpt/chat', {
        message: message
      }, {
        headers: {
          'Content-Type': 'application/json'
        }
      });
      setResponse(result.data.response);
    } catch (error) {
      console.error('Error:', error);
      setResponse(`Error: ${error.message}`);
    }
    setLoading(false);
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>GPT Chat Interface</h1>
      </header>
      <main className="App-main">
        <form onSubmit={handleSubmit} className="chat-form">
          <textarea
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            placeholder="Type your message here..."
            className="message-input"
          />
          <button type="submit" disabled={loading} className="submit-button">
            {loading ? 'Sending...' : 'Send'}
          </button>
        </form>
        {response && (
          <div className="response-container">
            <h2>Response:</h2>
            <p className="response-text">{response}</p>
          </div>
        )}
      </main>
    </div>
  );
}

export default App; 