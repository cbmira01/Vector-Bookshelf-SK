Project Overview

The project is a demonstrator and tutorial application that uses a small curated collection (around 100 books) from Project Gutenberg as the basis for natural language queries. The system will allow users to select books, download them along with their metadata, process and index their contents (chunking and vector embeddings), and finally run natural language queries that return citable completions.

Key Goals:

    User Interface (UI): A web-based UI with a progressive workflow (Step Zero, Book Selection, Download Management, Indexing, and Querying) built using ASP.NET Core with Razor pages and a Bootstrap-based layout.
    APIs: A set of RESTful endpoints to manage status, book selection, downloads, indexing, and querying.
    Data Persistence:
        Graph Database: Fuseki for processing full RDF metadata (used in indexing/chunking).
        Selection Index: A lightweight relational database (or similar) to store a distilled index (author, title, birth/death dates, synthesized publication date).
        Vector Data: Initially, Semantic Kernel’s in-memory vector store will be used for prototyping vector search; with plans to upgrade to a persistent provider (e.g., Redis with RediSearch) later.
    LLM Integration:
        Using locally-hosted LLMs (e.g., Ollama with llama 3.2) via agentic templates managed within Semantic Kernel.
        Agentic templates will instruct the LLM to call external tools (like vector search endpoints) when generating query completions.
    Asynchronous Task Management: Hangfire is integrated to handle background tasks such as downloads and indexing, with a dashboard for monitoring job status.
    Configuration:
        Sensitive credentials and environment-specific configuration will be managed via template files (e.g., .env-template) with Docker volume mapping.
        A TOML configuration file will define Gutenberg servers, rate limits, and cooldown timers.
    Deployment: The entire application is containerized using Docker and orchestrated via Docker Compose. The stack includes containers for:
        The ASP.NET Core API & Razor UI (with Swagger enabled for API testing).
        Fuseki (with volume-mapped configuration files).
        Additional services (e.g., Semantic Kernel processing and a locally hosted LLM container).

Solution Architecture
Multi-Project Solution Structure

    Project.API
        Hosts ASP.NET Core controllers and Razor pages.
        Integrates Swagger (via Swashbuckle.AspNetCore) for API testing.
        Runs the Hangfire dashboard for background task management.

    Project.Services
        Contains business logic and service classes that orchestrate between the API and data layers.
        Implements asynchronous operations, integrating Hangfire jobs for downloads and indexing.

    Project.Data
        Manages data access for Fuseki, the selection index (relational DB), and any future vector database integration.
        Defines data models and repository interfaces.

    Project.Common (optional)
        Contains shared utilities, configuration models, and helper functions.

API Endpoints Overview
1. Step Zero (Status and Reset)

    GET /status:
    Returns a JSON object containing project state (e.g., number of downloaded books, indexing progress, CPU/memory/storage metrics).

    POST /reset:
    Initiates a full system reset (clears databases and repositories) and returns a success/error JSON response.
    Includes safeguard measures (confirmation token or warning flag) before execution.

2. Book Selection Panel

    POST /selection/update:
    Triggers the generation or update of the selection index (extracted from Gutenberg RDF master index) with no parameters. Returns status of the index-building process.

    POST /selection/cancel:
    Cancels the current index-building process. Returns success/failure.

    GET /selection/search:
    Accepts a search string (e.g., "au:Jeff ti:declare pu:1700") and returns a paginated list of matching books (author, title, birth/death dates, synthesized publication date).

    POST /selection/update-status:
    Updates a book's selection status based on an item ID provided by the client; returns success/failure.

    GET /selection/selected:
    Retrieves a list of all currently selected books.

    POST /selection/unselect-all:
    Unselects all books; returns success/failure.

3. Download Management

    POST /download/start:
    Initiates download of selected books/metadata (using the current selection).

    GET /download/progress:
    Returns a list of active downloads with details:
        Download ID
        Current percentage complete
        Estimated time remaining
        Error messages
        Specific Gutenberg server and connection type used
        Cooldown timer status (ensuring adherence to cooldown rules)

    POST /download/pause:
    Pauses a download based on a provided identifier.

    POST /download/resume:
    Resumes a paused download.

    POST /download/cancel:
    Cancels a download.

    POST /download/retry:
    Retries a failed download.

4. Indexing and Chunking

    POST /index/start:
    Initiates indexing (including chunking and vector embedding) of downloaded books.

    GET /index/progress:
    Returns progress details (percentage complete, current book processing, estimated time, error messages).

    POST /index/pause:
    Pauses the indexing process.

    POST /index/resume:
    Resumes indexing if paused.

    POST /index/cancel:
    Cancels the indexing process.

5. Natural Language Query

    POST /query:
    Accepts a query string and returns completions that include citations to book passages.
    (This endpoint uses agentic templates in Semantic Kernel to trigger external tool calls like vector search as needed.)

Agentic LLM Workflow

    Template Design:
    Create prompt templates that instruct the LLM to detect when to invoke external tools. For example, if a query requires document lookup, the LLM outputs a structured command (e.g., a JSON object) indicating an action such as "vectorSearch" with necessary parameters.

    Processing Flow:
        The LLM processes the input query using the agentic template.
        It determines if external data (vector search results) is needed.
        If required, the LLM outputs a command for the application to call the corresponding API endpoint.
        The application makes the API call, retrieves results, and then feeds those back to the LLM for generating a final, citable response.

    LLM Integration:
    Initially use Ollama with a local model (e.g., llama 3.2) and Semantic Kernel’s built-in vector search. Plan to extend this later as more experience is gained in the area.

Deployment (Docker Stack)

    Containers:
        ASP.NET Core API Container: Runs the API and Razor-based web UI (with Swagger and Hangfire integrated).
        Fuseki Container: Serves as the graph database; configuration files (e.g., .env, shiro.ini) are volume-mapped.
        LLM Container: Runs the locally-hosted LLM (via Ollama).
        Additional Containers: As needed for any future services (e.g., a persistent vector database using Redis/Redisearch if the built-in Semantic Kernel store is replaced later).

    Orchestration:
    Use Docker Compose to define service dependencies, named volumes for persistent storage, and network configurations.

    Configuration:
        Use mapped volumes for sensitive configuration files (with .env and template files kept out of version control).
        A TOML configuration file will manage Gutenberg servers, download cooldowns, and rate limits.

Testing and Documentation

    Testing:
    Include unit and integration tests for key components. Provide console-based or API-accessible sanity checks (e.g., dump RDF file contents, verify individual chunked items).

    Documentation:
    Use Markdown files in the repository for:
        A basic README.
        Theory of operation.
        Installation instructions.
        Usage guidelines.
        Testing and debugging procedures.
        Contributing guidelines.
        Works consulted.
        Maintain LLM-generated SPEC.md and TODO.md files during development.

Asynchronous Task Management

    Hangfire Integration:
    Use Hangfire to manage background tasks (downloads, indexing).
        Run the Hangfire dashboard within the API project.
        Error handling and retries are basic: errors are reported, and user intervention is required for retries—no fully automatic retry mechanism.

Next Steps

    Phase-Based UI Development:
    Start with a provisional home page that navigates to various phases (Step Zero, Book Selection, Download Management, Indexing, and Querying).

    Refinement:
    The system is designed with sensible defaults. As you learn more (especially about vector search and LLM agentic templates), revisit and refine those components.