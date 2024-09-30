# Set your container name
CONTAINER_NAME=my_container

# Help command to display available commands
.PHONY: help
help:
	@echo "Available commands:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  %-15s %s\n", $$1, $$2}'

# build containers
.PHONY: build
build: ## build containers
	docker build . -t identity-api -f ./Identity.API/Dockerfile

# start containers
.PHONY: start
start: ## Start containers
	docker network create identity-network
	docker run --name auth-demo --network identity-network -e POSTGRES_USER=identity_user -e POSTGRES_PASSWORD=identity_pw -e POSTGRES_DB=identity -p 5452:5432 -d postgres
	docker run --name identity-api --network identity-network -p 8085:8080 -d identity-api

.PHONY: reset all
reset: ## Reset all
	docker kill identity-api
	docker rm identity-api
	docker kill auth-demo
	docker rm auth-demo
	docker network rm identity-network
	make build
	make start

# start .NET containers
.PHONY: start-api
start-api: ## Start containers
	docker kill identity-api
	docker rm identity-api
	docker run --name identity-api --network identity-network -p 8085:8080 -d identity-api

# Execute a command inside the container
.PHONY: exec
exec: ## Execute a command inside the running container
	docker exec -it $(CONTAINER_NAME) my_command
