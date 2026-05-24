.PHONY: docker-up migrate migrate-remove

COMPOSE := docker compose
STARTUP_PROJECT := Eventix.Api
MIGRATIONS_PROJECT := Eventix.Infrastructure

up:
	$(COMPOSE) up -d postgres

down:
	$(COMPOSE) down -v

migrate:
	dotnet ef database update --project $(MIGRATIONS_PROJECT) --startup-project $(STARTUP_PROJECT) --context PublicDbContext
	dotnet ef database update --project $(MIGRATIONS_PROJECT) --startup-project $(STARTUP_PROJECT) --context TenantDbContext

migrate-remove:
	dotnet ef migrations remove --project $(MIGRATIONS_PROJECT) --startup-project $(STARTUP_PROJECT) --context TenantDbContext
	dotnet ef migrations remove --project $(MIGRATIONS_PROJECT) --startup-project $(STARTUP_PROJECT) --context PublicDbContext
