import { useState } from 'react'
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import './App.css';
import { CodesGeneratedResponseMessage, CodeUsedResponseMessage, GenerateResponse, UseCodeResponse, UseCodeResponseType } from './consts';


const App = () => {
  const [connection, setConnection] = useState<HubConnection | undefined>(undefined);
  const [count, setCount] = useState(1);
  const [length, setLength] = useState(7);
  const [code, setCode] = useState('');
  const [response, setResponse] = useState(null);
  const [serverUrl, setServerUrl] = useState("http://localhost:5144/discount");

  const connectToServer = async () => {
    const conn = new HubConnectionBuilder()
      .withUrl(serverUrl)
      .withAutomaticReconnect()
      .build();

    conn.on(CodesGeneratedResponseMessage, (response: GenerateResponse) => {
      if (response === undefined) {
        alert("GenerateResponse is undefined.");
      }

      if (response.result === true) {
        alert("Requested codes have been generated.");
      }
      else {
        alert("Failed to generate codes.");
      }
    });

    conn.on(CodeUsedResponseMessage, (response: UseCodeResponse) => {

      if (response === undefined) {
        alert("UseCode response is undefined.");
      }

      switch (response.result) {
        case UseCodeResponseType.Success:
          alert("Code used successfully.");
          break;
        case UseCodeResponseType.InvalidRequest:
          alert("Invalid code.");
          break;
        case UseCodeResponseType.CodeNotFound:
          alert("Code has expired.");
          break;
        default:
          alert("Failed to use code.");
          break;
      }
    });

    try {

      await conn.start();
      setConnection(conn);
      console.log("Connected to the server");
    } catch (error) {
      console.error("Connection failed", error);
    }
  };

  const generateCodes = async () => {
    if (!connection) {
      alert("Not connected to the server.");
      return;
    }

    try {
      const result = await connection.invoke("GenerateCodes", { count, length });
      setResponse(result);
    } catch (error) {
      console.error("Error generating codes", error);
    }
  };

  const useCode = async () => {
    if (!connection) {
      alert("Not connected to the server.");
      return;
    }

    try {
      const result = await connection.invoke("UseCode", { code });
      setResponse(result);
    } catch (error) {
      console.error("Error using code", error);
    }
  };

  return (
    <div style={{ padding: '20px' }}>
      <h1>Discount Code Client</h1>

      {!connection && (
        <div>
          <input style={{ width: '200px' }} type="text" value={serverUrl} onChange={(e) => setServerUrl(e.target.value)} />
          <br />
          <button onClick={connectToServer}>Connect to Server</button>
        </div>


      )}

      {connection && (
        <>
          <h2>Connected to server: {serverUrl}</h2>
          <h3>Connected as: {connection.connectionId}</h3>
        </>
      )}

      <div style={{ marginTop: '20px' }}>
        <h2>Generate Discount Codes</h2>
        <label>
          Count:
          <input
            type="number"
            value={count}
            onChange={(e) => setCount(Number(e.target.value))}
          />
        </label>
        <br />
        <label>
          Length:
          <input
            type="number"
            value={length}
            onChange={(e) => setLength(Number(e.target.value))}
          />
        </label>
        <br />
        <button onClick={generateCodes}>Generate</button>
      </div>

      <div style={{ marginTop: '20px' }}>
        <h2>Use Discount Code</h2>
        <label>
          Code:
          <input
            type="text"
            value={code}
            onChange={(e) => setCode(e.target.value)}
          />
        </label>
        <br />
        <button onClick={useCode}>Use Code</button>
      </div>

      {response && (
        <div style={{ marginTop: '20px' }}>
          <h2>Response</h2>
          <pre>{JSON.stringify(response, null, 2)}</pre>
        </div>
      )}
    </div>
  );
};

export default App;
