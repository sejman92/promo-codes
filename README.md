# SignalR Promo Code Server Application

This repository contains a C# SignalR server application that enables real-time communication between clients. 
The app uses SignalR for handling connections and communication.



## Table of Contents
- [About](#about)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [Testing](#testing)
- [Troubleshooting](#troubleshooting)
- [License](#license)

## About
Application contains one Hub: 
**DiscountHub** is a real-time SignalR server application designed to manage and interact with discount codes. It allows clients to generate and use discount codes in a secure and efficient way. The application leverages SignalR for real-time communication and ensures concurrency with a thread-safe mechanism to handle discount code storage.

### Features

- **Generate Discount Codes**: Clients can request the server to generate a specified number of discount codes of a given length. The server validates the request, generates random codes, and stores them for future use.
- **Use Discount Codes**: Clients can use a discount code, and the server validates the code's existence before removing it from the list of active codes.
- **Real-Time Communication**: The application uses SignalR to communicate with clients in real-time. Whenever a discount code is generated or used, the result is immediately communicated back to the client.
- **Persistent Storage**: The discount codes are stored persistently in a JSON file, ensuring that they are not lost between application restarts.

### Key Components

- **SignalR Hub (`DiscountHub`)**: The central component that handles real-time communication with clients. It provides methods to generate new discount codes and use existing ones.
- **Validation**: Both the generation and usage of discount codes are validated using FluentValidation to ensure that the requests meet the required criteria.
- **Concurrency Handling**: The discount codes are stored in a thread-safe `ConcurrentDictionary`. The file-based storage uses a `SemaphoreSlim` to ensure that concurrent read/write operations are managed safely.

### Workflow

1. **Generate Codes**:
   - Clients send a request **GenerateRequest** to generate a specific number of discount codes.
        | Field  | Note                        | Type   | Allowed Values    |
        |--------|-----------------------------|--------|-------------------|
        | Count  | The number of codes         | ushort | Min: 1, Max: 2000 |
        | Length | Length of the code          | byte   | Min: 7, Max: 8    |
        
   - The server validates the request.
   - If the request is valid, the server generates the codes, stores them, and responds with the result **GenerateResponse**.
        | Field   | Note                        | Type   | Possible values                                     |
        |---------|-----------------------------|--------|-----------------------------------------------------|
        | Result  | Request result              | bool   | **true** -when success, **false** -when failure     |

2. **Use Code**:
   - Clients can submit a discount code to be used with **UseCodeRequest** request
      | Field  | Note                        | Type          |
      |--------|-----------------------------|---------------|
      | Code   | Discount code               | **string**(8) |

   - The server checks if the code exists, removes it if valid, and responds with the success or failure result **UseCodeResponse**.
      | Field    | Note                        | Type          | Possible Values                                               |
      |----------|-----------------------------|---------------|---------------------------------------------------------------| 
      | Result   | Request result code         | **byte**      | **0** - Success, **1** - InvalidRequest, **2** - CodeNotFound |

3. **Persistent Storage**:
   - The discount codes are loaded from a JSON file at the server's startup and saved back whenever the codes are updated.

### Methods

- **OnConnectedAsync**: Triggered when a client connects to the server. The server loads existing discount codes from persistent storage.
- **OnDisconnectedAsync**: Triggered when a client disconnects.
- **GenerateCodes**: Generates a specified number of discount codes and returns the result to the client.
- **UseCode**: Validates and processes the use of a discount code.

### Dependencies

- **SignalR**: Provides real-time communication with clients.
- **FluentValidation**: Used for validating incoming requests to ensure correct input.
- **ILogger**: Custom logger used for logging messages and errors.

### File Storage

The application stores the discount codes in a file named `discount_codes.json`. This file is read and written to whenever discount codes are loaded or updated.

### Usage

Clients can interact with the `DiscountHub` by connecting to the SignalR server and calling the available methods for generating or using discount codes. They will receive real-time responses regarding the success or failure of their requests.

## Prerequisites

Before you can run the application, make sure you have the following installed:

- **.NET SDK**: The application is built using .NET. You will need to install the .NET SDK.
  - [Install .NET SDK](https://dotnet.microsoft.com/download/dotnet)
- **Visual Studio** (Optional, but recommended): Visual Studio or another C# IDE to open and edit the project.
  - [Install Visual Studio](https://visualstudio.microsoft.com/)

Additionally, you’ll need the following software:
- **Git**: For cloning the repository.
  - [Install Git](https://git-scm.com/)
- **SQL Server** (Optional): If your application requires database connectivity.

## Installation

1. Clone the repository:

    ```bash
    git clone https://github.com/your-username/your-repository-name.git
    cd your-repository-name
    ```

2. Open the solution file in Visual Studio or another IDE of your choice:

    ```bash
    open your-solution.sln
    ```

3. Restore the required NuGet packages:

    ```bash
    dotnet restore
    ```

    This will download all necessary dependencies listed in the project.

## Running the Application

Once the setup is complete, follow the steps below to run the SignalR server application:

1. **Build the project:**

    In your terminal (or IDE), run:

    ```bash
    dotnet build
    ```

2. **Run the server application:**

    To start the SignalR server application, use the following command:

    ```bash
    dotnet run
    ```

    This will start the server, and it will be available at `http://localhost:5144` (or another port depending on the settings in `launchSettings.json`).

    You can now connect clients to this SignalR server to send/receive real-time messages.

3. **Testing the Server (Optional):**

    You can test the SignalR hub using client applications (e.g., a web client, .NET client, or another server). For example, use the browser console for testing or build a client application to interact with the SignalR hub.

## Testing

If you want to run the unit tests associated with this SignalR application, follow these steps:

1. Run the unit tests using the following command:

    ```bash
    dotnet test
    ```

    This will execute all the tests defined in your project. Make sure your tests are set up correctly and that your application is running as expected.

## Troubleshooting

If you run into issues while running the application, here are some common troubleshooting steps:

1. **Port in Use:**
    - If you see an error like `Address already in use`, it means another process is using the default port (5144). You can either:
      - Stop the other process using the port, or
      - Change the port by updating the `appsettings.json` or `launchSettings.json` file with a different port.

2. **Missing Dependencies:**
    - Ensure all required dependencies are restored by running `dotnet restore`.

3. **SignalR Connection Issues:**
    - Verify that the client application is correctly connecting to the correct URL of the SignalR server (`http://localhost:5144` or the configured URL).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
