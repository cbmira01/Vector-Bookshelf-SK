# Project TODO Checklist

This checklist is based on the project blueprint and outlines all the tasks needed to build the demonstrator and tutorial application.

## Phase 1: Multi-Project Solution & UI Skeleton

- [ ] **Create Multi-Project Solution**
  - [ ] Create new solution with the following projects:
    - [ ] **Project.API** (ASP.NET Core Web API with Razor Pages)
    - [ ] **Project.Services** (Class Library for Business Logic)
    - [ ] **Project.Data** (Class Library for Data Access/Models)
    - [ ] (Optional) **Project.Common** (Shared Utilities & Configuration)
  - [ ] Set up dependency injection in Project.API to reference Services and Data.

- [ ] **Implement Basic Home Page**
  - [ ] Create a Razor Pages home page in Project.API.
  - [ ] Add a prominent header with project title and brief description.
  - [ ] Add navigation buttons/links for:
    - [ ] Step Zero (Status and Reset)
    - [ ] Book Selection
    - [ ] Download Management
    - [ ] Indexing
    - [ ] Querying
  - [ ] Integrate Bootstrap for responsive layout.

- [ ] **Integrate Swagger and Hangfire**
  - [ ] Add Swashbuckle.AspNetCore for Swagger in Project.API.
  - [ ] Configure Swagger to display API endpoints.
  - [ ] Add Hangfire to Project.API and configure a basic dashboard endpoint.
  - [ ] Verify the application builds and runs with the home page, Swagger, and Hangfire dashboard visible.

## Phase 2: Step Zero & Book Selection Endpoints

- [ ] **Implement Step Zero Endpoints**
  - [ ] Create a controller (e.g., `StatusController`) in Project.API.
  - [ ] Implement `GET /status`:
    - [ ] Return JSON with placeholder project state (download count, indexing progress, CPU/memory/storage metrics).
  - [ ] Implement `POST /reset`:
    - [ ] Add safeguard (confirmation token/warning flag).
    - [ ] Return success/failure JSON response.
  - [ ] Create corresponding stub business logic in Project.Services.

- [ ] **Implement Book Selection Endpoints**
  - [ ] Create a controller (e.g., `BookSelectionController`) in Project.API.
  - [ ] Implement `POST /selection/update`:
    - [ ] Trigger generation/update of the selection index.
    - [ ] Return JSON with index-building status.
  - [ ] Implement `POST /selection/cancel`:
    - [ ] Cancel current index-building process.
    - [ ] Return success/failure message.
  - [ ] Implement `GET /selection/search`:
    - [ ] Accept query string (e.g., "au:Jeff ti:declare pu:1700").
    - [ ] Return a stubbed, paginated list of matching books (author, title, birth/death dates, synthesized publication date).
  - [ ] Implement `POST /selection/update-status`:
    - [ ] Accept item ID to update selection status.
    - [ ] Return success/failure JSON.
  - [ ] Implement `GET /selection/selected`:
    - [ ] Return stubbed list of currently selected books.
  - [ ] Implement `POST /selection/unselect-all`:
    - [ ] Clear all selections and return success/failure.
  - [ ] Create corresponding stub business logic in Project.Services.

## Phase 3: Download Management Endpoints

- [ ] **Implement Download Management Endpoints**
  - [ ] Create a controller (e.g., `DownloadController`) in Project.API.
  - [ ] Implement `POST /download/start`:
    - [ ] Initiate download for selected books/metadata.
    - [ ] Return JSON confirming start of download.
  - [ ] Implement `GET /download/progress`:
    - [ ] Return JSON list with each active download's details:
      - Download ID
      - Current percentage complete
      - Estimated time remaining
      - Error messages
      - Gutenberg server and connection type used
      - Cooldown timer status
  - [ ] Implement endpoints for:
    - [ ] `POST /download/pause`
    - [ ] `POST /download/resume`
    - [ ] `POST /download/cancel`
    - [ ] `POST /download/retry`
    - Each should accept a download identifier and return success/failure.
  - [ ] In Project.Services, add stubbed background tasks using Hangfire for download operations.

## Phase 4: Indexing and Chunking Endpoints

- [ ] **Implement Indexing and Chunking Endpoints**
  - [ ] Create a controller (e.g., `IndexingController`) in Project.API.
  - [ ] Implement `POST /index/start`:
    - [ ] Trigger indexing (chunking and vector embedding) for downloaded books.
    - [ ] Return JSON indicating that indexing has started.
  - [ ] Implement `GET /index/progress`:
    - [ ] Return JSON with indexing progress details:
      - Percentage complete
      - Current book being processed
      - Estimated time remaining
      - Error messages
  - [ ] Implement endpoints for:
    - [ ] `POST /index/pause`
    - [ ] `POST /index/resume`
    - [ ] `POST /index/cancel`
    - Each should control the indexing process and return success/failure.
  - [ ] Stub business logic in Project.Services using Hangfire for background indexing tasks.

## Phase 5: Natural Language Query and LLM Integration

- [ ] **Implement Natural Language Query Endpoint**
  - [ ] Create a controller (e.g., `QueryController`) in Project.API.
  - [ ] Implement `POST /query`:
    - [ ] Accept a natural language query.
    - [ ] Stub a call to Semantic Kernel to simulate LLM processing.
    - [ ] Return JSON with a stubbed completion that includes placeholder citations.
- [ ] **Set Up Basic Agentic Template Workflow (Stub)**
  - [ ] In Project.Services, create an LLM integration class.
  - [ ] Define a simple agentic template that:
    - Detects if a vector search is required.
    - Simulates outputting a structured command for external API calls.
  - [ ] Wire this simulated call into the `/query` endpoint response.

## Phase 6: Docker Deployment

- [ ] **Dockerize ASP.NET Core API (Project.API)**
  - [ ] Create a Dockerfile for Project.API.
  - [ ] Ensure the Dockerfile builds the API with Razor Pages, Swagger, and Hangfire.
  - [ ] Configure environment variables and volume mappings for configuration files.
- [ ] **Dockerize Fuseki**
  - [ ] Use the official Fuseki Docker image.
  - [ ] Map configuration files (.env, shiro.ini) via volumes.
- [ ] **Dockerize LLM Service**
  - [ ] Create or configure a Dockerfile for the LLM container (using Ollama with llama 3.2).
- [ ] **Create Docker Compose File**
  - [ ] Define services for:
    - ASP.NET Core API
    - Fuseki
    - LLM service
  - [ ] Configure named volumes for persistent storage.
  - [ ] Set up networking and environment variable mappings.

## Phase 7: Testing & Documentation

- [ ] **Testing**
  - [ ] Create unit tests for controllers in Project.API.
  - [ ] Create unit tests for business logic in Project.Services.
  - [ ] Create integration tests for API endpoints (e.g., /status, /selection/search).
  - [ ] Implement console or API-accessible sanity checks (e.g., dump RDF file contents, verify chunked items).

- [ ] **Documentation**
  - [ ] Create `README.md`:
    - Overview of the project.
    - Setup instructions.
    - Usage guidelines.
  - [ ] Create `THEORY.md` explaining the project’s operation.
  - [ ] Create `INSTALL.md` with detailed installation and Docker deployment instructions.
  - [ ] Create `USAGE.md` detailing how to use the API and UI.
  - [ ] Create `TESTING.md` with instructions for running tests and debugging.
  - [ ] Create `CONTRIBUTING.md` with guidelines for contributions.
  - [ ] Create `WORKSCONSULTED.md` listing references and resources.
  - [ ] Maintain `SPEC.md` and `TODO.md` for ongoing development updates.

---

# Final Integration & Review

- [ ] **Wire-Up All Components**
  - [ ] Ensure all API endpoints are integrated and accessible via Swagger.
  - [ ] Verify Hangfire dashboard is operational and background tasks are scheduled.
  - [ ] Test end-to-end flows starting from the home page navigation to each phase.
- [ ] **Conduct Code Reviews and Testing**
  - [ ] Review code for adherence to best practices.
  - [ ] Run all tests and fix any issues.
  - [ ] Ensure there are no orphaned or unintegrated pieces of code.

---

This checklist is designed to be thorough yet incremental, ensuring each step is small enough to be implemented safely and builds on the previous steps. As development progresses, revisit and update this checklist as needed.
