Project Blueprint Overview
Project Structure and Goals

    Multi-Project Solution:
        Project.API: ASP.NET Core Web API with Razor pages for the UI, Swagger integration, and Hangfire dashboard.
        Project.Services: Business logic and asynchronous task orchestration (downloads, indexing, etc.).
        Project.Data: Data access layer for Fuseki, the selection index (relational DB), and eventual vector database integration.
        Project.Common (Optional): Shared utilities and configuration models.

    Key Functional Areas:
        UI Panels/Workflow: Progressive steps (Step Zero, Book Selection, Download Management, Indexing, Querying) navigable from a home page.
        RESTful API Endpoints: For status/reset, book selection, downloads, indexing, and queries.
        Data Persistence: Fuseki for RDF data, relational DB for selection index, and an in-memory vector store (with potential Redisearch integration later).
        LLM Integration: Use agentic templates with Semantic Kernel to allow the LLM to invoke vector search and external calls.
        Asynchronous Task Management: Managed by Hangfire (with a built-in dashboard), scheduling tasks like downloads and indexing.
        Deployment via Docker: Containers for API/UI, Fuseki, LLM (Ollama), and later services, orchestrated with Docker Compose.

    Configuration & Documentation:
        Use template files for sensitive configurations (e.g., .env-template).
        A TOML file for non-sensitive configuration (Gutenberg servers, rate limits, cooldown timers).
        Markdown files for project documentation (README, theory, installation, usage, testing, contributing, etc.).

Step-by-Step Blueprint
Phase 1: Establish the Multi-Project Solution and Initial UI Skeleton

Step 1.1: Set Up the Multi-Project Solution

    Create a new solution.
    Add the following projects:
        Project.API: ASP.NET Core Web API project (include Razor Pages).
        Project.Services: Class library for business logic.
        Project.Data: Class library for data models and repository interfaces.
        Optionally, Project.Common: For shared utilities.
    Wire up basic dependency injection in the API project to reference Services and Data.

Step 1.2: Implement a Basic Home Page

    In Project.API, create a Razor Pages home page that shows the project title and a list of buttons/links navigating to:
        Step Zero (Status and Reset)
        Book Selection
        Download Management
        Indexing
        Querying
    Use Bootstrap for layout and responsiveness.

Step 1.3: Add Swagger and Hangfire to Project.API

    Integrate Swashbuckle.AspNetCore for Swagger.
    Integrate Hangfire and add its dashboard endpoint.
    Verify that the API endpoints (even if stubbed) are visible in Swagger.

Phase 2: Build API Endpoints for Step Zero and Book Selection

Step 2.1: Implement Step Zero Endpoints

    GET /status: Return stubbed JSON with placeholder project status (e.g., number of books, performance metrics).
    POST /reset: Stub an endpoint that clears state (using a confirmation flag) and returns success.

Step 2.2: Implement Book Selection Endpoints

    POST /selection/update: Stub an endpoint to trigger generation/update of the selection index.

    POST /selection/cancel: Stub an endpoint to cancel the index-building process.

    GET /selection/search: Accept a query string parameter; return a stubbed list of books.

    POST /selection/update-status: Accept an item ID and update selection status.

    GET /selection/selected: Return stubbed list of selected books.

    POST /selection/unselect-all: Clear all selections.

    Ensure these endpoints are wired up with basic controllers in Project.API and business logic placeholders in Project.Services.

Phase 3: Develop Download Management Endpoints

Step 3.1: Implement Download Initiation and Progress Endpoints

    POST /download/start: Start download based on current selection.
    GET /download/progress: Return JSON with a list of active downloads (fields: download ID, percentage, estimated time, error messages, server info, cooldown status).

Step 3.2: Implement Download Control Endpoints

    POST /download/pause, /download/resume, /download/cancel, /download/retry: Each accepting a download identifier and returning success/failure.

    In Project.Services, add stubbed background tasks (using Hangfire) for downloads.

Phase 4: Develop Indexing and Chunking Endpoints

Step 4.1: Implement Indexing Endpoints

    POST /index/start: Trigger indexing (chunking and vector embedding).
    GET /index/progress: Return progress details (percentage, current book, estimated time, error messages).

Step 4.2: Implement Indexing Controls

    POST /index/pause, /index/resume, /index/cancel: Control indexing process.

    Stub out business logic in Project.Services to simulate indexing tasks using Hangfire jobs.

Phase 5: Develop Natural Language Query Endpoint and LLM Integration

Step 5.1: Implement the Query Endpoint

    POST /query: Accept a query string and return a stubbed completion with citations.
    Ensure the endpoint is wired to use Semantic Kernel (even if it’s a basic call for now).

Step 5.2: Outline the Agentic Template Mechanism

    Define a basic agentic template that, upon receiving a query, formats a potential command for external tool invocation.
    In Project.Services or within a dedicated LLM integration class, simulate detecting this command and calling a stubbed vector search endpoint.
    Wire the results back to complete the response.

Phase 6: Dockerize the Application and Prepare the Deployment Stack

Step 6.1: Dockerize Each Service

    Create Dockerfiles for:
        ASP.NET Core API (with UI, Swagger, and Hangfire).
        Fuseki (using official images, mapping configuration files via volumes).
        LLM Container (using the Ollama Docker image for llama 3.2).
    Ensure your Dockerfiles follow best practices (minimal images, environment variables, etc.).

Step 6.2: Create a Docker Compose File

    Define services for each container.
    Set up named volumes for persistent storage (for Fuseki and later for vector data if needed).
    Define network dependencies and environment variable mapping.

Phase 7: Testing and Documentation

Step 7.1: Add Basic Unit and Integration Tests

    In each project (API, Services, Data), add tests for key functionalities and sanity check endpoints (e.g., dump RDF, verify chunking).
    Create a test project if needed.

Step 7.2: Write Documentation

    Create Markdown files in the repository covering:
        README (overview, goals, how to run)
        Theory of Operation
        Installation Instructions
        Usage Guidelines
        Testing/Debugging Procedures
        Contributing Guidelines
        Works Consulted
        Maintain SPEC.md and TODO.md for ongoing development notes

Iterative Breakdown Into Code Generation Prompts

Below are the prompts for a code-generation LLM, each in its own markdown code block.
Prompt 1: Set Up the Multi-Project Solution and UI Skeleton

Create a new .NET solution with three projects:
1. Project.API (an ASP.NET Core Web API project with Razor Pages support).
2. Project.Services (a class library for business logic).
3. Project.Data (a class library for data access and models).

In Project.API:
- Set up basic dependency injection to reference Project.Services and Project.Data.
- Create a Razor Pages home page with a header displaying the project title and brief description.
- Add navigation buttons/links for "Step Zero", "Book Selection", "Download Management", "Indexing", and "Querying".
- Integrate Bootstrap for layout.

Also, add Swagger support using Swashbuckle.AspNetCore and configure a basic Hangfire dashboard endpoint.
Ensure that the project builds and runs, displaying the home page with navigation links.

Prompt 2: Implement Step Zero API Endpoints

In the Project.API project, create a new controller (e.g., StatusController) to handle the "Step Zero" endpoints. Implement the following endpoints:

1. GET /status
   - Returns a JSON object with placeholder values for project state (number of downloaded books, indexing progress, and CPU/memory/storage metrics).

2. POST /reset
   - Accepts a request to reset the system state. Include a safeguard mechanism (like a confirmation token).
   - Returns a JSON response indicating success or failure.

Create corresponding stubbed business logic in Project.Services and, if necessary, data access placeholders in Project.Data.
Ensure that these endpoints are accessible via Swagger.

Prompt 3: Implement Book Selection API Endpoints

Develop the book selection endpoints in the Project.API project by creating a new controller (e.g., BookSelectionController). Implement the following endpoints with stubbed logic:

1. POST /selection/update
   - Triggers the generation/update of the selection index from the Gutenberg RDF master index.
   - Returns a JSON object indicating the status of the index building process.

2. POST /selection/cancel
   - Cancels the current index building process.
   - Returns a JSON success/failure message.

3. GET /selection/search
   - Accepts a query string parameter (e.g., "au:Jeff ti:declare pu:1700") and returns a paginated list of matching books (with author, title, birth/death dates, and a synthesized publication date).
   - Use stubbed data for now.

4. POST /selection/update-status
   - Accepts an item ID to update a book's selection status (selected/unselected).
   - Returns a JSON response indicating success or failure.

5. GET /selection/selected
   - Returns a list of currently selected books.

6. POST /selection/unselect-all
   - Unselects all books.
   - Returns a JSON response indicating success.

Create basic service methods in Project.Services to support these endpoints.

Prompt 4: Implement Download Management API Endpoints

In the Project.API project, create a DownloadController to manage download operations. Implement the following endpoints with stubbed logic:

1. POST /download/start
   - Initiates the download of selected books/metadata based on the current selection.
   - Returns a JSON response indicating that the download has started.

2. GET /download/progress
   - Returns a JSON list of active downloads. Each download entry should include:
     - Download ID
     - Current percentage complete
     - Estimated time remaining
     - Any error messages
     - The specific Gutenberg server and connection type used
     - Cooldown timer status

3. POST /download/pause, /download/resume, /download/cancel, and /download/retry
   - Each endpoint accepts a download identifier and returns a JSON response indicating success or failure.

Use Hangfire to simulate background tasks for downloads, and add stubbed business logic in Project.Services.

Prompt 5: Implement Indexing and Chunking API Endpoints

In Project.API, create an IndexingController to handle indexing and chunking of downloaded books. Implement these endpoints with stubbed logic:

1. POST /index/start
   - Triggers the indexing process (chunking and vector embedding) for downloaded books.
   - Returns a JSON response indicating that indexing has started.

2. GET /index/progress
   - Returns a JSON object with progress details, including:
     - Percentage complete
     - The current book being processed
     - Estimated time remaining
     - Any error messages

3. POST /index/pause, /index/resume, and /index/cancel
   - Accepts commands to control the indexing process.
   - Returns JSON responses indicating success or failure.

Implement stubbed business logic in Project.Services using Hangfire for background processing.

Prompt 6: Implement Natural Language Query Endpoint and Agentic Template Stub

In Project.API, create a QueryController to handle natural language queries. Implement the following endpoint:

1. POST /query
   - Accepts a query string from the client.
   - Uses a stubbed call to Semantic Kernel to simulate LLM processing with agentic templates.
   - Returns a JSON object with a completion that includes placeholder citations.

In Project.Services, create a basic integration class that simulates the agentic template workflow:
   - Define a template that checks if the query needs external vector search.
   - If so, simulate an external API call (stubbed) and return a combined response.

Ensure that the endpoint is accessible via Swagger.

Prompt 7: Dockerize the Application

Create Dockerfiles for the main services:

1. For the ASP.NET Core API (Project.API):
   - Create a Dockerfile that builds and runs the API with Razor Pages, Swagger, and Hangfire.
   - Ensure environment variables and volume mappings are set up for configuration files.

2. For Fuseki:
   - Use the official Fuseki image.
   - Map configuration files (.env, shiro.ini) from host volumes.

3. For the LLM service:
   - Create a Dockerfile (or use an existing image) for running the Ollama container with the llama 3.2 model.

Create a Docker Compose file that defines these services, sets up named volumes for persistent storage, and defines networking between containers.

Prompt 8: Add Basic Testing and Documentation

Set up a basic testing framework:

1. Create unit tests for key controllers in Project.API and service methods in Project.Services.
2. Implement integration tests that call your API endpoints (e.g., /status, /selection/search) and verify stub responses.

Also, add Markdown documentation files in the repository:
- README.md: Overview of the project, setup instructions, and usage guidelines.
- THEORY.md: Explanation of the project’s theory of operation.
- INSTALL.md: Detailed installation and Docker deployment instructions.
- USAGE.md: How to use the API and UI.
- TESTING.md: Instructions on running tests and debugging.
- CONTRIBUTING.md: Guidelines for contributing.
- WORKSCONSULTED.md: References and resources used.

Ensure these documentation files are well organized and reflect the current state of the project.

Final Review

    Each prompt is designed to build on the previous steps, ensuring that no piece is left orphaned.
    The steps are small enough to be implemented safely and iteratively while advancing the overall project.
    Integration points (e.g., wiring endpoints to business logic, Docker Compose for orchestration) are clearly defined.
    The agentic LLM workflow is stubbed out initially with plans to iterate and refine later.
