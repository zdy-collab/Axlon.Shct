# Woodpecker production deployment

This configuration runs the Woodpecker server and a Docker-backed production agent on the same host.

## Start Woodpecker

1. Rotate the GitHub OAuth secret that was previously exposed.
2. Generate a new agent secret with `openssl rand -hex 32`.
3. Copy `.env.example` to `.env` and replace every placeholder.
4. Protect the file with `chmod 600 .env`.
5. Start Woodpecker with `docker compose up -d --force-recreate`.

Port `9000` is only exposed to the internal Compose network. GitHub and users access Woodpecker through port `6607`.

## Configure the repository

Activate `zdy-collab/Axlon.Shct`, mark it as trusted, and add the following repository secrets:

- `registry_username`
- `registry_password`
- `dashboard_otlp_api_key`
- `mysql_password`
- `redis_password`
- `rabbitmq_user`
- `rabbitmq_password`

Add `49.233.152.22:8082` under repository registries so Woodpecker can pull the private CI image.

## Publish the CI image once

From the repository root on a machine authenticated to the private registry:

```powershell
docker build -f ci/woodpecker/Dockerfile -t 49.233.152.22:8082/axlon-ci:13.4.6 ci/woodpecker
docker push 49.233.152.22:8082/axlon-ci:13.4.6
```

After that, every push to `master` runs `.woodpecker/deploy.yaml`. The pipeline validates the solution and then uses `aspire deploy` to build and push application images and update the production Compose deployment.

## Replace an older Woodpecker deployment

The server and agent must run the same pinned version. From the repository root on the Woodpecker host, replace the old Compose file instead of continuing to start a previous `next` configuration:

```bash
cp deploy/woodpecker/docker-compose.yaml /www/docker/woodpecker/docker-compose.yml
cd /www/docker/woodpecker
docker compose pull
docker compose up -d --force-recreate
docker compose images
docker compose logs --tail 100 woodpecker-server woodpecker-agent
```

Both images reported by `docker compose images` must be `v3.17.0`. The agent log must no longer contain `could not persist agent config`, and the Woodpecker administration page must show `production-agent` online with the `deployment=production` label before restarting a pending workflow.

## First handoff from the old deployment

Before the first CI-owned deployment, stop the old Compose project once from its existing deployment directory:

```bash
docker compose --env-file .env.Production -f docker-compose.yaml down --remove-orphans
```

Do not add `-v`; named database and message-broker volumes must be preserved.
