# 🧱 Project Context – __PropAgent__ – Property Management Platform (Baseline)

## 🎯 Objective
Build a **multi-tenant, modular Property Management Platform** composed of independent, loosely-coupled service apps (Property, Work Orders, Inventory, Accounting, etc.) unified under a single authentication and authorization system.

Each module can run **independently** yet shares:
- The same **OIDC-based AuthN/AuthZ**
- The same **tenant resolution model**
- Consistent **security and audit practices**

All components will be open-source, self-hostable, and fully scriptable (no commercial dependencies).

---

## ⚙️ Core Tech Stack
| Layer | Choice | Notes |
|--------|---------|-------|
| **Frontend** | **React + Next.js (App Router)** | Modern, SSR/ISR capable, flexible theming |
| **BFF (Backend-for-Frontend)** | **ASP.NET Core + YARP** | Handles login/logout, secure cookie sessions, proxies API calls |
| **Auth Server** | **ASP.NET Core + OpenIddict + ASP.NET Core Identity** | OIDC/OAuth 2.1 compliant; stores users in PostgreSQL |
| **Database** | **PostgreSQL 15+** | Primary store for Identity and tenants |
| **Migrations** | **Manual SQL scripts + DbUp runner** | No EF Core migrations; numbered SQL in `/ops/migrations` |
| **Multi-Tenancy** | **Finbuckle.MultiTenant** | Tenant resolution by subdomain or `/t/{slug}` path |
| **Object Storage** | **MinIO or SeaweedFS** | S3-compatible, presigned URLs for uploads/downloads |
| **Language** | **C# (.NET 8)** + **SQL (PostgreSQL dialect)** | Backend stack |
| **Infra** | **Docker Compose** | Local dev orchestration for Postgres / Auth / API / UI / MinIO |

---

## 🏗️ Architectural Principles
- **SQL-first design** — schema and DDL are hand-authored, versioned, and reviewed.  
- **Multi-tenant isolation** — every table includes `tenant_id`; global filters enforce it.  
- **Claims-based authorization** — policy-based via permissions (`invoice.read`, `wo.create`, etc.).  
- **Event-driven integration** — modules communicate through outbox-backed domain events.  
- **Security first** — short-lived access tokens, refresh rotation, MFA, secure cookies, HTTPS everywhere.  
- **Modular** — each domain (Property, WO, Inventory, Accounting) is a peer service with its own DB and API.  
- **OSS only** — no commercial licensing (Duende BFF, etc. excluded).

---

## 🧩 Phase 1 – Authentication & Multi-Tenant Architecture
### Goals
1. Establish the unified **Auth & Tenant foundation** every service will use.
2. Deliver a working **React + Next.js UI** that signs in via the **ASP.NET BFF** and displays tenant-scoped data.
3. Create a fully automated, SQL-based **migrations pipeline** for PostgreSQL.

### Deliverables
- **Auth Server**
  - OpenIddict (OIDC Provider) + ASP.NET Core Identity (PostgreSQL store)
  - Local accounts + Google OIDC federation (first external provider)
  - Tenants / UserTenants / Roles / Permissions schema
  - Claims factory emits `tenant_id` and `permissions`
- **BFF**
  - YARP reverse proxy (`/bff/login`, `/bff/logout`, `/bff/user`, `/bff/api/*`)
  - Cookie session (HTTP-only, SameSite=Lax/Strict)
  - CSRF protection + audit logging
- **UI**
  - Next.js route guards using `/bff/user`
  - Tenant-specific theme and branding
- **Migrations**
  - `/ops/migrations/pgsql/V0001__bootstrap.sql` (extensions + version table)
  - `/V0002__identity_core.sql` (Identity tables)
  - `/V0003__tenants.sql`, `/V0004__permissions.sql`, `/R__seed_basics.sql`
  - DbUp console runner in `/ops/migrator`
- **Dev Environment**
  - Docker-Compose: PostgreSQL + Auth + BFF + UI + MinIO
  - Makefile or script to build/start all components
  - TLS self-signed certs for local HTTPS

### Success Criteria
- User can sign up/login → session cookie set → `/bff/user` returns identity + tenant.  
- Authenticated API calls include tenant context and succeed/fail per policy.  
- Each migration runs cleanly via DbUp and records checksums in `schema_version`.  
- Google sign-in works; new federations pluggable by tenant.  
- Source repo structured for future modules (`/workorders`, `/accounting`, etc.).

---

## 🪜 Project Layout

/app
/ui-next/ # Next.js frontend
/bff/ # ASP.NET Core YARP proxy, cookie sessions
/auth/ # ASP.NET Core OpenIddict + Identity + Postgres
/api/ # Future domain APIs
/shared/ # DTOs, constants, permission lists
/ops/
/migrations/pgsql/
/migrator/ # DbUp runner
docker-compose.yml
Context.md # ← this file

---

## 🧠 Next Phases (after Phase 1)
**Phase 2 – Work Orders Service**  
Independent maintenance component; publishes/consumes events with other modules.  

**Phase 3 – Accounting Service**  
Double-entry ledger, posting rules, fiscal periods; integrates via events.  

**Phase 4 – Property & Inventory Modules**  
Shared asset base for all services, with consistent tenant scoping.

---

✅ **You are here:**  
Start **Phase 1 – Authentication & Multi-Tenant Architecture**.  
Next chat will focus on scaffolding the Auth Server, defining the first SQL migrations, and wiring tenant resolution.
