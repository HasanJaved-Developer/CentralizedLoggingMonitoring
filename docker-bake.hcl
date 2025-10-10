// docker-bake.hcl
// Builds 3 services (multi-arch) from local Dockerfiles and pushes to GHCR.
// Image names must be LOWERCASE.

variable "REGISTRY_GHCR" { default = "ghcr.io" }
variable "OWNER"         { default = "hasanjaved-developer" }   // overridden by workflow
variable "REPO_SLUG"     { default = "centralized-logging" }     // your namespace/group
variable "DOCKERHUB_NAMESPACE" { default = "" }                  // optional

group "default" {
  targets = ["userapi", "api", "web"]
}

/* ---------------------- userapi (UserManagementApi) ---------------------- */
target "userapi" {
  // Your project folder contains UserManagementApi.csproj + Dockerfile
  context    = "."
  dockerfile = "./UserManagementApi/Dockerfile"

  // Default tag; workflow should append latest/semver etc. via --set
  tags = [
    "${REGISTRY_GHCR}/${OWNER}/${REPO_SLUG}/userapi:edge"
  ]

  platforms = ["linux/amd64", "linux/arm64"]
  labels = {
    "org.opencontainers.image.source" = "https://github.com/${OWNER}/CentralizedLoggingMonitoring"
    "org.opencontainers.image.title"  = "userapi"
  }
}

/* ---------------------- api (CentralizedLoggingApi) ---------------------- */
target "api" {
  context    = "."
  dockerfile = "./CentralizedLoggingApi/Dockerfile"

  tags = [
    "${REGISTRY_GHCR}/${OWNER}/${REPO_SLUG}/api:edge"
  ]

  platforms = ["linux/amd64", "linux/arm64"]
  labels = {
    "org.opencontainers.image.source" = "https://github.com/${OWNER}/CentralizedLoggingMonitoring"
    "org.opencontainers.image.title"  = "api"
  }
}

/* ---------------------- web (IntegrationPortal) ---------------------- */
target "web" {
  context    = "."
  dockerfile = "./ApiIntegrationMvc/Dockerfile"

  tags = [
    "${REGISTRY_GHCR}/${OWNER}/${REPO_SLUG}/web:edge"
  ]

  platforms = ["linux/amd64", "linux/arm64"]
  labels = {
    "org.opencontainers.image.source" = "https://github.com/${OWNER}/CentralizedLoggingMonitoring"
    "org.opencontainers.image.title"  = "web"
  }
}