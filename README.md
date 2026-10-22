\# 🤖 DotNetAIChatbot



A local AI chatbot built with \*\*ASP.NET Core Web API (.NET 8), Ollama, and SQL Server\*\*. The application allows users to chat with a locally running AI model and stores conversation history in a database.



\## 🚀 Features



\- AI-powered responses using a locally running Ollama model

\- ASP.NET Core Web API built with .NET 8

\- SQL Server database integration

\- Entity Framework Core for data access

\- Persistent conversation and message history

\- Create and switch between saved conversations

\- Restore chat history after refreshing the browser

\- Web-based chat interface using HTML, CSS, and JavaScript



\## 🛠️ Tech Stack



| Technology | Purpose |

|---|---|

| C# | Backend development |

| ASP.NET Core Web API (.NET 8) | REST API |

| Ollama | Local AI model runtime |

| Llama 3.2 (1B) | Language model |

| Entity Framework Core 8 | ORM and database operations |

| SQL Server | Persistent chat storage |

| HTML, CSS, JavaScript | Frontend |



\## 📋 Prerequisites



Install the following before running the project:



\- \[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

\- \[Ollama](https://ollama.com/)

\- \[SQL Server](https://www.microsoft.com/sql-server)

\- Visual Studio 2022 or another compatible .NET IDE



\## ⚙️ Setup and Installation



\### 1. Clone the repository



```bash

git clone https://github.com/ShivashktiYadav/AIChatbot.git

cd AIChatbot

```



\### 2. Download the AI model



Start Ollama and run:



```bash

ollama pull llama3.2:1b

```



Make sure Ollama is running before sending chat requests.



\### 3. Configure SQL Server



Update the `ChatbotConnection` connection string in your local configuration to match your SQL Server instance.



Example for SQL Server Express:



```json

{

&#x20; "ConnectionStrings": {

&#x20;   "ChatbotConnection": "Server=.\\\\SQLEXPRESS;Database=DotNetAIChatbotDB;Trusted\_Connection=True;TrustServerCertificate=True;"

&#x20; }

}

```



Use your actual SQL Server instance name. Never commit passwords, API keys, or other secrets.



\### 4. Apply database migrations



Open the project folder in Visual Studio and run the following in Package Manager Console:



```powershell

Update-Database

```



Alternatively, from the project directory, use:



```bash

dotnet ef database update

```



The second option requires the EF Core design-time tools to be installed.



\### 5. Run the application



From the project directory:



```bash

dotnet restore

dotnet run

```



Open the local application URL displayed in the terminal. If Swagger is enabled in the current environment, use its URL to test the API.



\## 🔌 API Endpoints



| Method | Endpoint | Description |

|---|---|---|

| POST | `/api/Chat` | Send a message and receive an AI response |

| GET | `/api/Chat/conversations` | Retrieve saved conversations |

| GET | `/api/Chat/{conversationId}/messages` | Retrieve messages for a conversation |



\## 🔐 Security



Authentication and user-level conversation authorization are planned/in progress. Do not expose this application publicly until authentication, authorization, and appropriate API protections are implemented and tested.



\## 🗺️ Future Enhancements



\- ASP.NET Core Identity authentication

\- JWT-based API authentication

\- User-specific conversation access

\- API rate limiting

\- Improved error handling and validation

\- Deployment and production configuration



\## 👨‍💻 Author



\*\*Shivshakti Yadav\*\*



GitHub: \[ShivashktiYadav](https://github.com/ShivashktiYadav)

